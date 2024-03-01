using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Product.Data.Configurations;
using Product.Data.DataAccess;
using Product.Services.Core;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<AppDatabaseSetting>(
    builder.Configuration.GetSection("MongoDB"));

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();

builder.Services.AddSingleton<AppDbContext>();

builder.Services.AddStackExchangeRedisCache(redisOption =>
{
    redisOption.Configuration = builder.Configuration.GetValue<string>("Redis:ConnectionString");
});


var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();

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