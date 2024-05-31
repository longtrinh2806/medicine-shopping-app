using Customer.Data.Dtos;
using Customer.Data.Request;
using Customer.Services.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Api.Controllers
{
    [Route("api/customer")]
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public IActionResult GetByEmailorPhone([FromQuery] EmailOrPhone request)
        {
            try
            {
                var result = _customerService.GetByEmailorPhone(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("customerId")]
        public IActionResult GetById([FromQuery] Guid customerId)
        {
            try
            {
                var result = _customerService.GetById(customerId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[HttpPost]
        //public IActionResult CreateNewCustomer([FromBody] CustomerDto request)
        //{
        //    var result = _customerService.CreateNewCus(request);
        //    if (!result.Succeeded)
        //        return BadRequest(result);
        //    return Ok(result);
        //}

        [HttpPut("info/{customerId}")]
        public IActionResult UpdateCustomerInfo([FromBody] CustomerUpdatedDto request, Guid customerId)
        {
            var result = _customerService.UpdateCustomerInfo(request, customerId);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("address/{customerId}")]
        public IActionResult UpdateCustomerAddress([FromBody] AddressDto request, Guid customerId)
        {
            var result = _customerService.UpdateCustomerAddress(request, customerId);
            if (!result.Succeeded)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
