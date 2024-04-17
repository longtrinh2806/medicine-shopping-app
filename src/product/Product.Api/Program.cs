using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Product.Api.Consumers;
using Product.Api.Swagger;
using Product.Data.Configurations;
using Product.Data.DataAccess;
using Product.Services.Core;
using product_order_contract;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

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
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigSwagger>();

builder.Services.Configure<AppDatabaseSetting>(
    builder.Configuration.GetSection("MongoDB"));

builder.Services.Configure<RabbitMQConfiguration>(builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();

builder.Services.AddSingleton<AppDbContext>();

builder.Services.AddStackExchangeRedisCache(redisOption =>
{
    redisOption.Configuration = builder.Configuration.GetValue<string>("Redis:ConnectionString");
});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<CheckProductInventoryConsumer>().Endpoint(e => e.Name = "check-product-inventory");
    x.AddRequestClient<CheckProductInventory>(new Uri("exchange:check-product-inventory"));

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMQConfig = context.GetRequiredService<IOptions<RabbitMQConfiguration>>().Value;
        cfg.Host(rabbitMQConfig.ConnectionString);
        cfg.ConfigureEndpoints(context);

        cfg.UseRawJsonSerializer();
    });
});


var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors();

app.MapControllers();

try
{
    var dbContext = app.Services.GetRequiredService<AppDbContext>();
    dbContext.CreateCollectionsIfNotExisted();
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
    throw;
}

MapperConfig.Configure();

app.Run();