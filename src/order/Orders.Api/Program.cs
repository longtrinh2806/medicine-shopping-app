using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Orders.Api.Configurations;
using Orders.Api.Consumers;
using Orders.Data.DataAccess;
using Orders.Service.Core;
using product_order_contract;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    });
});

builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseCors();

app.Run();
