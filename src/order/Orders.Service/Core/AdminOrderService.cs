using Mapster;
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

        public AdminOrderService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public ResponseModel ConfirmedOrder(Guid orderId)
        {
            try
            {
                var order = _appDbContext.Orders.FirstOrDefault(order => order.Id == orderId);

                if (order == null)
                    throw new Exception("Order not found");
                order.Status = EnumStatus.CONFIRMED;

                _appDbContext.Orders.Update(order);
                _appDbContext.SaveChanges();

                return new ResponseModel { Message = "Confirmed Order Successfully", Succeeded = true };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new ResponseModel { Message = "Confirm Failed", Succeeded = false };
            }
        }

        public ResponseModel CancelledOrder(Guid orderId)
        {
            try
            {
                var order = _appDbContext.Orders.FirstOrDefault(order => order.Id == orderId);

                if (order == null)
                    throw new Exception("Order not found");
                order.Status = EnumStatus.CANCELLED;

                _appDbContext.Orders.Update(order);
                _appDbContext.SaveChanges();

                return new ResponseModel { Message = "CANCELLED Order Successfully", Succeeded = true };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new ResponseModel { Message = "CANCELLED Failed", Succeeded = false };
            }
        }

        public ResponseModel CompletedOrder(Guid orderId)
        {
            try
            {
                var order = _appDbContext.Orders.FirstOrDefault(order => order.Id == orderId);

                if (order == null)
                    throw new Exception("Order not found");
                order.Status = EnumStatus.COMPLETE;

                _appDbContext.Orders.Update(order);
                _appDbContext.SaveChanges();

                return new ResponseModel { Message = "COMPLETE Order Successfully", Succeeded = true };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new ResponseModel { Message = "COMPLETE Failed", Succeeded = false };
            }
        }

        public ResponseModel GetAllOrder(PaginationFilter pagination)
        {
            try
            {
                var orders = _appDbContext.Orders
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
