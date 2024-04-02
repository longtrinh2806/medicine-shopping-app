using Customer.Data.Request;
using Customer.Services.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet]
        public IActionResult GetAddressByCustomerId(Guid customerId)
        {
            var result = _addressService.GetAddressByCustomerId(customerId);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }


    }
}
