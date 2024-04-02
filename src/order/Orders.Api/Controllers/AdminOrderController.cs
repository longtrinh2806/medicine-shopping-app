using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orders.Data.Filters;
using Orders.Service.Core;

namespace Orders.Api.Controllers
{
    [Route("api/admin-order")]
    [ApiController]
    public class AdminOrderController : ControllerBase
    {
        private readonly IAdminOrderService _adminOrderService;

        public AdminOrderController(IAdminOrderService adminOrderService)
        {
            _adminOrderService = adminOrderService;
        }

        [HttpGet("/get-all-order")]
        public IActionResult GetAllOrder([FromQuery]PaginationFilter pagination)
        {
            PaginationFilter paginationFilter = new(pagination.PageIndex, pagination.PageSize);

            var result = _adminOrderService.GetAllOrder(paginationFilter);
            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result.Message);
        }

        [HttpGet("/get-order-from-date-to-date")]
        public IActionResult GetOrderFromDateToDate([FromQuery] PaginationFilter pagination, [FromQuery] DateFilter dateFilter)
        {
            PaginationFilter paginationFilter = new(pagination.PageIndex, pagination.PageSize);

            var result = _adminOrderService.GetOrderFromDateToDate(paginationFilter, dateFilter);
            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result.Message);
        }

        [HttpGet("/get-pending-order")]
        public IActionResult GetPendingOrder([FromQuery] PaginationFilter pagination)
        {
            PaginationFilter paginationFilter = new(pagination.PageIndex, pagination.PageSize);
            var result = _adminOrderService.GetPendingOrder(paginationFilter);
            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result.Message);
        }

        [HttpGet("/get-confirmed-order")]
        public IActionResult GetConfirmedOrder([FromQuery] PaginationFilter pagination)
        {
            PaginationFilter paginationFilter = new(pagination.PageIndex, pagination.PageSize);
            var result = _adminOrderService.GetConfirmedOrder(paginationFilter);

            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result.Message);
        }

        [HttpGet("/get-completed-order")]
        public IActionResult GetCompletedOrder([FromQuery] PaginationFilter pagination)
        {
            PaginationFilter paginationFilter = new(pagination.PageIndex, pagination.PageSize);
            var result = _adminOrderService.GetCompletedOrder(paginationFilter);

            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result.Message);
        }

        [HttpGet("/get-cancelled-order")]
        public IActionResult GetCancelledOrder([FromQuery] PaginationFilter pagination)
        {
            PaginationFilter paginationFilter = new(pagination.PageIndex, pagination.PageSize);
            var result = _adminOrderService.GetCancelledOrder(paginationFilter);

            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result.Message);
        }

        [HttpPut("update-order/{orderId}/confirmed")]
        public IActionResult ConfirmedOrder(Guid orderId)
        {
            var result = _adminOrderService.ConfirmedOrder(orderId);
            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result.Message);
        }

        [HttpPut("update-order/{orderId}/completed")]
        public IActionResult CompletedOrder(Guid orderId)
        {
            var result = _adminOrderService.CompletedOrder(orderId);
            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result.Message);
        }

        [HttpPut("update-order/{orderId}/cancelled")]
        public IActionResult CancelledOrder(Guid orderId)
        {
            var result = _adminOrderService.CancelledOrder(orderId);
            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result.Message);
        }
    }
}
