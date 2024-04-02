using MassTransit;
using MongoDB.Driver;
using Product.Data.DataAccess;
using Product.Data.MongoCollections;
using product_order_contract;
using static MassTransit.Monitoring.Performance.BuiltInCounters;

namespace Product.Api.Consumers
{
    public class CheckProductInventoryConsumer : IConsumer<CheckProductInventory>
    {
        private readonly AppDbContext _appDbContext;

        public CheckProductInventoryConsumer(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task Consume(ConsumeContext<CheckProductInventory> context)
        {
            using (var session = _appDbContext.StartSession())
            {
                session.StartTransaction();
                try
                {
                    var inventory = await _appDbContext.Inventories.Find(inventory =>
                        inventory.ProductId.Equals(context.Message.ProductId)).FirstOrDefaultAsync();

                    if (inventory == null)
                        throw new InvalidOperationException("Inventory not found");

                    if (inventory.QuantityInStock < context.Message.Quantity)
                        throw new Exception($"Not enough product, product in stock: {inventory.QuantityInStock}");

                    var filter = Builders<Inventory>.Filter.Eq(inventory => inventory.ProductId, context.Message.ProductId);
                    var update = Builders<Inventory>.Update.Set(inventory => inventory.QuantityInStock, inventory.QuantityInStock -= context.Message.Quantity);
                    await _appDbContext.Inventories.UpdateOneAsync(filter, update);

                    await context.RespondAsync<ProductInventoryResult>(
                        new { Success = true, Message = $"Quantity after order: {inventory.QuantityInStock}" });

                    session.CommitTransaction();

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in CheckProductInventoryConsumer: " + ex);
                    session.AbortTransaction();

                    await context.RespondAsync<ProductInventoryResult>(
                        new { Success = false, Message = $"Error in CheckProductInventoryConsumer: {ex.Message}" });
                }
            }
        }
    }
}
