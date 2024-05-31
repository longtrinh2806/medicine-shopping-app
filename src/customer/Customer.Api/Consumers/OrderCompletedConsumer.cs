using Customer.Data.DataAccess;
using customer_order_contract;
using MassTransit;

namespace Customer.Api.Consumers
{
    public class OrderCompletedConsumer : IConsumer<OrderCompleted>
    {
        private readonly AppDbContext _dbContext;

        public OrderCompletedConsumer(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<OrderCompleted> context)
        {
            var existingMembership = _dbContext.Memberships.FirstOrDefault(x => x.CustomerId.Equals(context.Message.CustomerId));

            if (existingMembership == null)
                throw new Exception("CustomerId not exist");

            existingMembership.DiemTichLuy += context.Message.DiemTichLuy;

            _dbContext.Memberships.Update(existingMembership);
            await _dbContext.SaveChangesAsync();
        }
    }
}
