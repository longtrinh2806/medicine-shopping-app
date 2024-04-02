using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orders.Data.Dtos;
using Orders.Data.Models;
using Orders.Service.Core;
using System.Security.Cryptography;

namespace Orders.Api.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("get-cart-by-customerid/{customerId}")]
        public IActionResult GetCartByCustomerId(Guid customerId)
        {
            var result = _cartService.GetCartByCustomerId(customerId);

            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result.Message);
        }

        [HttpGet("get-cart-item/{cartId}")]
        public IActionResult GetCartItemByCartId(Guid cartId)
        {
            var result = _cartService.GetCartItemByCartId(cartId);

            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result.Message);
        }

        [HttpPost("add-item-to-cart/{cartId}")]
        public IActionResult AddItemToCart(Guid cartId, CartItemDto detailDto)
        {
            var result = _cartService.AddItemToCart(cartId, detailDto);

            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result.Message);
        }

        [HttpPut("update-item/{cartItemId}")]
        public IActionResult UpdateItem(Guid cartItemId, UpdatedCartItemDto updatedCartItemDto)
        {
            var result = _cartService.UpdateItem(cartItemId, updatedCartItemDto);

            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result.Message);
        }

        [HttpDelete("delete-item/{cartItemId}")]
        public IActionResult DeleteItem(Guid cartItemId)
        {
            var result = _cartService.DeleteItem(cartItemId);

            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result.Message);
        }
    }
}
