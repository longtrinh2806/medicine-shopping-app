using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Product.Data.Dtos;
using Product.Data.Filters;
using Product.Services.Core;
using ProductData.MongoCollections;

namespace Product.Api.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationFilter filter)
        {
            try
            {
                var paginationFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
                var result = await _categoryService.GetAll(paginationFilter.PageNumber, paginationFilter.PageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            try
            {
                var result = _categoryService.GetById(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("parentid/{parentId}")]
        public async Task<IActionResult> GetByParentId(Guid parentId)
        {
            try
            {
                var result = await _categoryService.GetByParentId(parentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = "ADMIN")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CategoryDto request)
        {
            try
            {
                var result = await _categoryService.Create(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = "ADMIN")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateById(Guid id,[FromBody] CategoryDto request)
        {
            try
            {
                var result = await _categoryService.UpdateById(id, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = "ADMIN")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(Guid id)
        {
            try
            {
                var result = await _categoryService.DeleteById(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
