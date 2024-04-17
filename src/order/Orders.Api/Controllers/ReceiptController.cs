using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orders.Service.Core;

namespace Orders.Api.Controllers
{
    [Route("api/receipt")]
    [ApiController]
    [Authorize]
    public class ReceiptController : ControllerBase
    {
        private readonly IReceiptService _receiptService;

        public ReceiptController(IReceiptService receiptService)
        {
            _receiptService = receiptService;
        }

        [HttpGet("{receiptId}")]
        public IActionResult GetById(Guid receiptId) 
        {
            var result = _receiptService.GetById(receiptId);

            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpGet("order/{orderId}")]
        public IActionResult GetByOrderId(Guid orderId)
        {
            var result = _receiptService.GetByOrderId(orderId);

            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
