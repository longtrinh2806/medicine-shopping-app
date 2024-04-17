using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Orders.Api.Configurations;
using Orders.Api.Consumers;
using Orders.Data.DataAccess;
using Orders.Service.Core;
using product_order_contract;
using Swashbuckle.AspNetCore.SwaggerGen;
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

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetValue<string>("MSSSQL:ConnectionStrings"));
});

builder.Services.Configure<RabbitMQConfiguration>(builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IAdminOrderService, AdminOrderService>();
builder.Services.AddScoped<IReceiptService, ReceiptService>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<CustomerCreatedConsumer>();

    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("order", false));

    x.AddRequestClient<CheckProductInventory>(new Uri("exchange:check-product-inventory"));

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMQConfig = context.GetRequiredService<IOptions<RabbitMQConfiguration>>().Value;
        cfg.Host(rabbitMQConfig.ConnectionString);
        cfg.ConfigureEndpoints(context);
        //cfg.DefaultContentType = new ContentType("application/json");
        cfg.UseRawJsonSerializer();
    });
});

builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseCors();

app.Run();
