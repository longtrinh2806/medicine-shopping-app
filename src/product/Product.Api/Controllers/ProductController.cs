using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Product.Data.Dtos;
using Product.Data.Filters;
using Product.Services.Core;

namespace Product.Api.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAll([FromQuery] PaginationFilter filter)
        //{
        //    try
        //    {
        //        var paginationFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

        //        var result = await _productService.GetAll(paginationFilter.PageNumber, paginationFilter.PageSize);

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }

        //}

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var result = await _productService.GetById(id);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        
        [HttpGet("categoryId")]
        public IActionResult GetByCategoryId([FromQuery]Guid categoryId)
        {
            try
            {
                var result = _productService.GetByCategoryId(categoryId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("searchTerms")]
        public IActionResult GetByKeyWords([FromQuery] string searchTerms, [FromQuery] PaginationFilter filter)
        {
            try
            {
                var paginationFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
                var result = _productService.GetByKeyWords(searchTerms, paginationFilter.PageNumber, paginationFilter.PageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        [HttpPost]
        public async Task<IActionResult> Post(ProductDto request)
        {
            try
            {
                var result = await _productService.Create(request);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateById(Guid id, ProductUpdatedDto request)
        {
            try
            {
                var result = await _productService.UpdateById(id, request);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(Guid id)
        {
            try
            {
                var result = await _productService.DeleteById(id);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
