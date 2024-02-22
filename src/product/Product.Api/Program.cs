using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Product.Data.DataAccess;
using Product.Services.Core;
using ProductData.DataAccess;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<AppDatabaseSetting>(
    builder.Configuration.GetSection("ApplicationDatabase"));

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddSingleton<AppDbContext>();

builder.Services.AddStackExchangeRedisCache(redisOption =>
{
    redisOption.Configuration = builder.Configuration.GetConnectionString("RedisConnectionString");
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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

app.Run();