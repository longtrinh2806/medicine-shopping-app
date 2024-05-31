using auth_customer_contract;
using Customer.Data.DataAccess;
using Customer.Data.Models;
using customer_order_contract;
using MassTransit;

namespace Customer.Api.Consumers
{
    public class UserCreatedConsumer : IConsumer<UserCreated>
    {
        private readonly AppDbContext _dbContext;
        private IPublishEndpoint _publishEndpoint;

        public UserCreatedConsumer(AppDbContext dbContext, IPublishEndpoint publishEndpoint)
        {
            _dbContext = dbContext;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<UserCreated> context)
        {
            Customers customer = new()
            {
                UserId = context.Message.UserId,
                Email = context.Message.Email
            };
            _dbContext.Customers.Add(customer);
            await _dbContext.SaveChangesAsync();

            Membership newMembership = new()
            {
                CustomerId = customer.Id,
                CapDo = 0,
                DiemTichLuy = 0
            };
            customer.Membership = newMembership;

            _dbContext.Customers.Update(customer);
            await _dbContext.SaveChangesAsync();

            PublishToOrder(customer.Id);
        }

        private void PublishToOrder(Guid customerId)
        {
            CustomerCreated customerCreated = new()
            {
                CustomerId = customerId
            };

            _publishEndpoint.Publish(customerCreated);
        }
    }
}
