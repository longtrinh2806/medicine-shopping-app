using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orders.Data.Dtos;
using Orders.Service.Core;

namespace Orders.Api.Controllers
{
    [Route("api/order")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("don-hang-cua-toi/{customerId}")]
        public IActionResult GetOrderByCustomerId(Guid customerId)
        {
            var result = _orderService.GetOrderByCustomerId(customerId);
            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result.Message);
        }

        [HttpGet("chi-tiet-don-hang/{orderId}")]
        public IActionResult GetOrderdetail(Guid orderId)
        {
            var result = _orderService.GetOrderdetail(orderId);
            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result.Message);
        }

        [HttpPost("create-order/{customerId}")]
        public async Task<IActionResult> CreateNewOrder(Guid customerId, List<CartItemDto> createOrderDtos)
        {
            var result = await _orderService.CreateNewOrder(customerId, createOrderDtos);
            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result.Message);
        }
    }
}
