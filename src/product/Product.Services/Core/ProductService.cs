using Amazon.Runtime.Internal;
using Mapster;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Driver;
using Product.Data.DataAccess;
using Product.Data.Dtos;
using Product.Data.MongoCollections;
using Product.Data.ViewModels;
using ProductData.MongoCollections;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Product.Services.Core
{
    public interface IProductService
    {
        //Task<ResponseModel> GetAll(int pageNumber, int pageSize);
        Task<ResponseModel> GetById(Guid id);
        Task<ResponseModel> Create(ProductDto request);
        Task<ResponseModel> UpdateById(Guid id, ProductUpdatedDto request);
        Task<ResponseModel> DeleteById(Guid id);
        ResponseModel GetByKeyWords(string searchTerms, int pageNumber, int pageSize);
        ResponseModel GetByCategoryId(Guid categoryId);
    }
    public class ProductService : IProductService
    {
        private readonly AppDbContext _dbContext;
        private readonly IDistributedCache _distributedCache;
        private readonly ICategoryService _categoryService;
        private readonly IInventoryService _inventoryService;

        public ProductService(AppDbContext dbContext, IDistributedCache distributedCache, ICategoryService categoryService, IInventoryService inventoryService)
        {
            _dbContext = dbContext;
            _distributedCache = distributedCache;
            _categoryService = categoryService;
            _inventoryService = inventoryService;
        }



        //public async Task<ResponseModel> GetAll(int pageNumber, int pageSize)
        //{
        //    string key = $"ProductPage_{pageNumber}";
        //    string? cachedProducts = _distributedCache.GetString(key);
        //    List<Products>? products;

        //    if (string.IsNullOrEmpty(cachedProducts))
        //    {
        //        products = await _dbContext.Products
        //                    .Find(_ => true)
        //                    .Skip((pageNumber - 1) * pageSize)
        //                    .Limit(pageSize)
        //                    .ToListAsync();
        //        if (products.Count < 1)
        //            throw new Exception("No Result");

        //            var cacheOption = new DistributedCacheEntryOptions()
        //                .SetSlidingExpiration(TimeSpan.FromMinutes(10));

        //            _distributedCache.SetString(key, JsonSerializer.Serialize(products), cacheOption);
        //        return new ResponseModel
        //        {
        //            Succeeded = true,
        //            Data = products,
        //            Message = "Got Successfully"
        //        };
        //    }

        //    products = JsonSerializer.Deserialize<List<Products>>(cachedProducts);
        //    return new ResponseModel
        //    {
        //        Succeeded = true,
        //        Data = products,
        //        Message = "Got Successfully"
        //    };
        //}

        public async Task<ResponseModel> GetById(Guid id)
        {
            string key = $"Product_{id}";
            string? cachedProduct = _distributedCache.GetString(key);

            if (string.IsNullOrEmpty(cachedProduct))
            {
                var result = await _dbContext.Products.Find(p => p.Id.Equals(id)).FirstOrDefaultAsync();

                if (result == null) throw new Exception("Invalid Id");

                var cacheOption = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                _distributedCache.SetString(key, JsonSerializer.Serialize<Products>(result), cacheOption);

                return new ResponseModel
                {
                    Succeeded = true,
                    Data = result,
                    Message = "Got Successfully"
                };
            }

            var product = JsonSerializer.Deserialize<Products>(cachedProduct);
            return new ResponseModel
            {
                Succeeded = true,
                Data = product,
                Message = "Got Successfully"
            };
        }

        public async Task<ResponseModel> Create(ProductDto request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var checkCategoryId = _categoryService.GetById(request.CategoryId);
            if (!checkCategoryId.Succeeded)
            {
                throw new Exception(checkCategoryId.Message);
            }

            string key = $"Product_CategoryID:{checkCategoryId}";
            string? cacheProduct = _distributedCache.GetString(key);

            if (!string.IsNullOrEmpty(cacheProduct))
            {
                _distributedCache.Remove(key);
            }

            var products = request.Adapt<Products>();
            await _dbContext.Products.InsertOneAsync(products);

            Inventory inventory = new()
            {
                ProductId = products.Id,
                QuantityInStock = request.SoLuong,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
            };

            await _dbContext.Inventories.InsertOneAsync(inventory);

            return new ResponseModel
            {
                Succeeded = true,
                Data = products,
                Message = "Created Successfully"
            };
            
        }

        public async Task<ResponseModel> UpdateById(Guid id, ProductUpdatedDto request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var filter = Builders<Products>.Filter
                .Eq(product => product.Id, id);

            var update = Builders<Products>.Update
                .Set(p => p.Price.GiaTienGoc, request.GiaTien)
                .Set(p => p.Price.GiaTienHienTai, request.GiaTienDaGiam)
                .Set(p => p.Price.PhanTramGiamGia, request.PhanTramGiamGia)
                .Set(p => p.CategoryId, request.CategoryId)
                .Set(p => p.LastUpdatedAt, DateTime.UtcNow);

            var result = await _dbContext.Products.UpdateOneAsync(filter, update);

            if (result.ModifiedCount < 1) throw new Exception("Invalid Id");

            string key = $"Product_CategoryID:{request.CategoryId}";
            string? cacheProduct = _distributedCache.GetString(key);

            if (!string.IsNullOrEmpty(cacheProduct))
                _distributedCache.Remove(key);

            return new ResponseModel
            {
                Succeeded = true,
                Data = result,
                Message = "Updated Successfully"
            };
        }

        public async Task<ResponseModel> DeleteById(Guid id)
        {
            var deletedProduct = _dbContext.Products.Find(p => p.Id.Equals(id)).FirstOrDefault();
            var result = await _dbContext.Products.DeleteOneAsync(p => p.Id == id);

            if (result.DeletedCount == 0)
                throw new Exception("Invalid Id");

            // Delete Cache Redis
            string key = $"Product_CategoryID:{deletedProduct.CategoryId}";
            string? cacheProduct = _distributedCache.GetString(key);

            if (!string.IsNullOrEmpty(cacheProduct))
                _distributedCache.Remove(key);

            // Delete Inventory
            _inventoryService.DeleteByProductId(deletedProduct.Id);

            return new ResponseModel { Succeeded = true, Message = "Deleted Successfully" };
        }

        public ResponseModel GetByKeyWords(string searchTerms, int pageNumber, int pageSize)
        {
            var filter = Builders<Products>.Filter.Text(searchTerms);
            var result = _dbContext.Products
                .Find(filter)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .Sort(Builders<Products>.Sort.MetaTextScore("textScore"))
                .ToList();

            if (result.Count < 1) throw new Exception("No Data");

            return new ResponseModel { Succeeded = true, Data = result, Message = "Got Successfully" };
        }

        public ResponseModel GetByCategoryId(Guid categoryId)
        {
            string key = $"Product_CategoryID:{categoryId}";
            string? cacheProducts = _distributedCache.GetString(key);

            if (string.IsNullOrEmpty(cacheProducts))
            {
                var categoryExisted = _categoryService.GetById(categoryId);

                if (!categoryExisted.Succeeded) throw new Exception(categoryExisted.Message);

                var products = _dbContext.Products
                    .Find(p => p.CategoryId.Equals(categoryId))
                    .ToList();
                if (!products.Any()) throw new Exception("No Result");

                var cacheOption = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                _distributedCache.SetString(key, JsonSerializer.Serialize(products), cacheOption);

                return new ResponseModel { Data = products, Message = "Got Successfully", Succeeded = true };
            }

            var results = JsonSerializer.Deserialize<List<Products>>(cacheProducts);
            return new ResponseModel { Data = results, Message = "Got Successfully", Succeeded = true };
        }
    }
}
