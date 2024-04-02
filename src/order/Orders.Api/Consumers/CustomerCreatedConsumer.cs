using customer_order_contract;
using MassTransit;
using Orders.Data.DataAccess;
using Orders.Data.Models;

namespace Orders.Api.Consumers
{
    public class CustomerCreatedConsumer : IConsumer<CustomerCreated>
    {
        private readonly AppDbContext _dbContext;

        public CustomerCreatedConsumer(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<CustomerCreated> context)
        {
            Cart cart = new()
            {
                CustomerId = context.Message.CustomerId
            };
            _dbContext.Carts.Add(cart);
            await _dbContext.SaveChangesAsync();
        }
    }
}
