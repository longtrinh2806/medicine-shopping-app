using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Product.Data.Dtos;
using Product.Services.Core;

namespace Product.Api.Controllers
{
    [Route("api/inventory")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet("{productId}")]
        public IActionResult GetByProductId(Guid productId)
        {
            try
            {
                var result = _inventoryService.GetByProductId(productId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{productId}")]
        public IActionResult UpdateByProductId(Guid productId, [FromBody] InventoryDto request)
        {
            try
            {
                var result = _inventoryService.UpdateByProductId(productId, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpDelete("{productId}")]
        public IActionResult DeleteByProductId(Guid productId)
        {
            try
            {
                var result = _inventoryService.DeleteByProductId(productId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
