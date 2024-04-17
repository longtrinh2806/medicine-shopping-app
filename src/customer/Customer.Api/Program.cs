using auth_customer_contract;
using Customer.Api.Configurations;
using Customer.Api.Consumers;
using Customer.Data.DataAccess;
using Customer.Services.Core;
using MassTransit;
using MassTransit.Transports.Fabric;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net.Mime;
using System.Security.Claims;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

var hmac = new HMACSHA256(Convert.FromBase64String(config["JwtSettings:Key"]!));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                    .CreateLogger(nameof(JwtBearerEvents));
                logger.LogError(context.Exception, "Authentication failed.");
                return Task.CompletedTask;
            }
        };
        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(hmac.Key),
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        "ADMIN",
        p => p.RequireClaim(ClaimTypes.Role, "admin"));
});

// CORS configuration
builder.Services.AddCors(option =>
{
    option.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfig>();

// CORS configuration
builder.Services.AddCors(option =>
{
    option.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.Configure<RabbitMQConfiguration>(builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetValue<string>("MSSSQL:ConnectionStrings"));
});

builder.Services.AddStackExchangeRedisCache(redisOption =>
{
    redisOption.Configuration = builder.Configuration.GetValue<string>("Redis:ConnectionStrings");
});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<OrderCompletedConsumer>();
    x.AddConsumer<UserCreatedConsumer>(); ;

    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("customer", false));

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMQConfig = context.GetRequiredService<IOptions<RabbitMQConfiguration>>().Value;
        cfg.Host(rabbitMQConfig.ConnectionString);

        cfg.ReceiveEndpoint("user-created", endpoint =>
        {
            endpoint.DefaultContentType = new ContentType("application/json");
            endpoint.UseRawJsonSerializer();
            endpoint.ConfigureConsumeTopology = false;

            endpoint.ConfigureConsumer<UserCreatedConsumer>(context);
        });

        cfg.ConfigureEndpoints(context);
        cfg.UseRawJsonSerializer();
    });
});

builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IAddressService, AddressService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();

app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors();
app.MapControllers();

app.Run();
