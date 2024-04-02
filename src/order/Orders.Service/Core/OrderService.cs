using MassTransit;
using Orders.Data.DataAccess;
using Orders.Data.Dtos;
using Orders.Data.Models;
using Orders.Data.ViewModels;
using product_order_contract;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Service.Core
{
    public interface IOrderService
    {
        Task<ResponseModel> CreateNewOrder(Guid customerId, List<CartItemDto> createOrderDtos);
        ResponseModel GetOrderByCustomerId(Guid customerId);
    }
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IRequestClient<CheckProductInventory> _requestClient;

        public OrderService(AppDbContext appDbContext, IRequestClient<CheckProductInventory> requestClient)
        {
            _appDbContext = appDbContext;
            _requestClient = requestClient;
        }

        public ResponseModel GetOrderByCustomerId(Guid customerId)
        {
            var orders = _appDbContext.Orders
                .Where(o => o.CustomerId.Equals(customerId))
                .ToList();

            if (!orders.Any())
                throw new Exception("No Result");

            return new ResponseModel { Data = orders, Message = "Got Successfully", Succeeded = true };
        }

        public async Task<ResponseModel> CreateNewOrder(Guid customerId, List<CartItemDto> createOrderDtos)
        {
            var transaction = _appDbContext.Database.BeginTransaction();
            try
            {
                if (createOrderDtos == null || !createOrderDtos.Any())
                    throw new ArgumentException("Empty List!", nameof(createOrderDtos));

                if (createOrderDtos.Any(dto => dto.Quantity < 1))
                    throw new ArgumentException("Invalid Quantity", nameof(createOrderDtos));

                long totalPrice = 0;
                var orderDetails = new List<OrderDetail>();

                Order order = new()
                {
                    CustomerId = customerId,
                    Status = EnumStatus.PENDING
                };

                _appDbContext.Orders.Add(order);
                _appDbContext.SaveChanges();

                foreach (var item in createOrderDtos)
                {
                    var response = await _requestClient.GetResponse<ProductInventoryResult>(new { item.ProductId, item.Quantity });
                    if (!response.Message.Success)
                        throw new Exception(response.Message.Message);

                    totalPrice += item.Price * item.Quantity;

                    _appDbContext.OrderDetails.Add(new OrderDetail
                    {
                        ProductId = item.ProductId,
                        OrderId = order.Id,
                        Quantity = item.Quantity,
                        Price = item.Price
                    });
                }

                order.TotalPrice = totalPrice;

                _appDbContext.Orders.Update(order);
                _appDbContext.SaveChanges();

                transaction.Commit();

                return new ResponseModel { Message = "Create Order Successfully", Succeeded = true };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine(ex);
                return new ResponseModel { Succeeded = false, Message = ex.Message };
            }
        }


    }
}
