using customer_order_contract;
using Mapster;
using MassTransit;
using Orders.Data.DataAccess;
using Orders.Data.Dtos;
using Orders.Data.Filters;
using Orders.Data.Models;
using Orders.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Service.Core
{
    public interface IAdminOrderService
    {
        ResponseModel CancelledOrder(Guid orderId);
        ResponseModel CompletedOrder(Guid orderId);
        ResponseModel ConfirmedOrder(Guid orderId);
        ResponseModel GetAllOrder(PaginationFilter pagination);
        ResponseModel GetCancelledOrder(PaginationFilter paginationFilter);
        ResponseModel GetCompletedOrder(PaginationFilter paginationFilter);
        ResponseModel GetConfirmedOrder(PaginationFilter pagination);
        ResponseModel GetOrderFromDateToDate(PaginationFilter paginationFilter, DateFilter dateFilter);
        ResponseModel GetPendingOrder(PaginationFilter pagination);
    }
    public class AdminOrderService : IAdminOrderService
    {
        private readonly AppDbContext _appDbContext;
        private IPublishEndpoint _publishEndpoint;

        public AdminOrderService(AppDbContext appDbContext, IPublishEndpoint publishEndpoint)
        {
            _appDbContext = appDbContext;
            _publishEndpoint = publishEndpoint;
        }

        public ResponseModel ConfirmedOrder(Guid orderId)
        {
            try
            {
                var order = _appDbContext.Orders.FirstOrDefault(order => order.Id == orderId);

                if (order == null)
                    throw new Exception("Order not found");

                if (order.Status == EnumStatus.COMPLETE)
                    throw new Exception("Order is completed. Can not change order status");

                order.Status = EnumStatus.CONFIRMED;
                order.UpdatedAt = DateTime.Now;

                _appDbContext.Orders.Update(order);
                _appDbContext.SaveChanges();

                return new ResponseModel { Message = "Confirmed Order Successfully", Succeeded = true };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new ResponseModel { Message = ex.Message, Succeeded = false };
            }
        }

        public ResponseModel CancelledOrder(Guid orderId)
        {
            try
            {
                var order = _appDbContext.Orders.FirstOrDefault(order => order.Id == orderId);

                if (order == null)
                    throw new Exception("Order not found");
                if (order.Status == EnumStatus.COMPLETE)
                    throw new Exception("Order is completed. Can not change order status");

                order.Status = EnumStatus.CANCELLED;
                order.UpdatedAt = DateTime.Now;

                _appDbContext.Orders.Update(order);
                _appDbContext.SaveChanges();

                return new ResponseModel { Message = "CANCELLED Order Successfully", Succeeded = true };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new ResponseModel { Message = ex.Message, Succeeded = false };
            }
        }

        public ResponseModel CompletedOrder(Guid orderId)
        {
            var transaction = _appDbContext.Database.BeginTransaction();
            try
            {
                var order = _appDbContext.Orders.FirstOrDefault(order => order.Id == orderId);

                if (order == null)
                    throw new Exception("Order not found");

                if (order.Status == EnumStatus.COMPLETE)
                    throw new Exception("Order is completed. Can not change order status");

                order.Status = EnumStatus.COMPLETE;
                order.UpdatedAt = DateTime.Now;

                _appDbContext.Orders.Update(order);
                _appDbContext.SaveChanges();

                Receipt receipt = new()
                {
                    OrderId = orderId,
                };

                _appDbContext.Receipts.Add(receipt);
                _appDbContext.SaveChanges();

                OrderCompleted orderCompleted = new()
                {
                    CustomerId = order.CustomerId,
                    DiemTichLuy = ((int)(order.TotalPrice / 1000))
                };

                _publishEndpoint.Publish(orderCompleted);

                transaction.Commit();

                return new ResponseModel { Message = "COMPLETE Order Successfully", Succeeded = true };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine(ex);
                return new ResponseModel { Message = ex.Message, Succeeded = false };
            }
        }

        public ResponseModel GetAllOrder(PaginationFilter pagination)
        {
            try
            {
                var orders = _appDbContext.Orders
                    .OrderBy(order => order.Id)
                    .Skip((pagination.PageIndex - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .ToList();

                if (orders.Count == 0)
                    throw new Exception("No Data");
                var result = orders.Adapt<List<OrderDto>>();

                return new ResponseModel { Data = result, Message = "Got Successfully", Succeeded = true };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new ResponseModel { Succeeded = false, Message = ex.Message };
            }
        }

        public ResponseModel GetPendingOrder(PaginationFilter pagination)
        {
            try
            {
                var pendingOrders = _appDbContext.Orders
                    .Where(o => o.Status.Equals(EnumStatus.PENDING))
                    .OrderBy(order => order.CreatedAt)
                    .ThenBy(order => order.Id)
                    .Skip((pagination.PageIndex - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .ToList();

                if (pendingOrders.Count == 0)
                    throw new Exception("No Data");

                var result = pendingOrders.Adapt<List<OrderDto>>();

                return new ResponseModel { Data = result, Message = "Got Successfully", Succeeded = true };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new ResponseModel { Succeeded = false, Message = ex.Message };
            }
        }

        public ResponseModel GetConfirmedOrder(PaginationFilter pagination)
        {
            try
            {
                var pendingOrders = _appDbContext.Orders
                    .Where(o => o.Status.Equals(EnumStatus.CONFIRMED))
                    .OrderBy(order => order.CreatedAt)
                    .ThenBy(order => order.Id)
                    .Skip((pagination.PageIndex - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .ToList();

                if (pendingOrders.Count == 0)
                    throw new Exception("No Data");

                var result = pendingOrders.Adapt<List<OrderDto>>();

                return new ResponseModel { Data = result, Message = "Got Successfully", Succeeded = true };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new ResponseModel { Succeeded = false, Message = ex.Message };
            }
        }

        public ResponseModel GetCancelledOrder(PaginationFilter pagination)
        {
            try
            {
                var pendingOrders = _appDbContext.Orders
                    .Where(o => o.Status.Equals(EnumStatus.CANCELLED))
                    .OrderBy(order => order.CreatedAt)
                    .ThenBy(order => order.Id)
                    .Skip((pagination.PageIndex - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .ToList();

                if (pendingOrders.Count == 0)
                    throw new Exception("No Data");

                var result = pendingOrders.Adapt<List<OrderDto>>();

                return new ResponseModel { Data = result, Message = "Got Successfully", Succeeded = true };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new ResponseModel { Succeeded = false, Message = ex.Message };
            }
        }

        public ResponseModel GetCompletedOrder(PaginationFilter pagination)
        {
            try
            {
                var pendingOrders = _appDbContext.Orders
                    .Where(o => o.Status.Equals(EnumStatus.COMPLETE))
                    .OrderBy(order => order.CreatedAt)
                    .ThenBy(order => order.Id)
                    .Skip((pagination.PageIndex - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .ToList();

                if (pendingOrders.Count == 0)
                    throw new Exception("No Data");

                var result = pendingOrders.Adapt<List<OrderDto>>();

                return new ResponseModel { Data = result, Message = "Got Successfully", Succeeded = true };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new ResponseModel { Succeeded = false, Message = ex.Message };
            }
        }

        public ResponseModel GetOrderFromDateToDate(PaginationFilter paginationFilter, DateFilter dateFilter)
        {
            try
            {
                if (dateFilter == null)
                    throw new Exception("Invalid Date");

                var orders = _appDbContext.Orders
                    .Where(x => x.CreatedAt >= dateFilter.From && x.CreatedAt <= dateFilter.To)
                    .OrderBy(order => order.CreatedAt)
                    .ThenBy(order => order.Id)
                    .Skip((paginationFilter.PageIndex - 1) * paginationFilter.PageSize)
                    .Take(paginationFilter.PageSize)
                    .ToList();

                var result = orders.Adapt<List<OrderDto>>();
                return new ResponseModel { Data = result, Message = "Got Successfully", Succeeded = true };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new ResponseModel { Succeeded = false, Message = ex.Message };
            }
        }
    }
}
