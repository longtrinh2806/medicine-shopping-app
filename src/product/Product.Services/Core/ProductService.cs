using Mapster;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Driver;
using Product.Data.DataAccess;
using Product.Data.Dtos;
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
        Task<ResponseModel> GetAll(int pageNumber, int pageSize);
        Task<ResponseModel> GetById(Guid id);
        Task<ResponseModel> Create(ProductDto request);
        Task<ResponseModel> UpdateById(Guid id, ProductDto request);
        Task<ResponseModel> DeleteById(Guid id);
    }
    public class ProductService : IProductService
    {
        private readonly AppDbContext _dbContext;
        private readonly IDistributedCache _distributedCache;

        public ProductService(AppDbContext dbContext, IDistributedCache distributedCache)
        {
            _dbContext = dbContext;
            _distributedCache = distributedCache;
        }

        public async Task<ResponseModel> GetAll(int pageNumber, int pageSize)
        {
            try
            {
                string key = $"ProductPage_{pageNumber}";
                string? cachedProducts = _distributedCache.GetString(key);
                List<Products> products;

                if (string.IsNullOrEmpty(cachedProducts))
                {
                    products = await _dbContext.Products
                                .Find(_ => true)
                                .Skip((pageNumber - 1) * pageSize)
                                .Limit(pageSize)
                                .ToListAsync();
                    if (products.Count < 1)
                        throw new Exception("No Result");

                    var cacheOption = new DistributedCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                    _distributedCache.SetString(key, JsonSerializer.Serialize(products), cacheOption);
                    return new ResponseModel
                    {
                        Succeeded = true,
                        Data = products,
                        Message = "Got Successfully"
                    };
                }

                products = JsonSerializer.Deserialize<List<Products>>(cachedProducts);
                return new ResponseModel
                {
                    Succeeded = true,
                    Data = products,
                    Message = "Got Successfully"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new ResponseModel { Succeeded = false, Message = ex.Message };
            }
        }

        public async Task<ResponseModel> GetById(Guid id)
        {
            try
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
            catch (Exception ex) 
            {
                return new ResponseModel {Succeeded = false, Message = ex.Message };
            }
        }

        public async Task<ResponseModel> Create(ProductDto request)
        {
            try
            {
                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                var products = request.Adapt<Products>();
                await _dbContext.Products.InsertOneAsync(products);

                return new ResponseModel
                {
                    Succeeded = true,
                    Data = products,
                    Message = "Created Successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Succeeded = false, Message = ex.Message };
            }
        }

        public async Task<ResponseModel> UpdateById(Guid id, ProductDto request)
        {
            try
            {
                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                var filter = Builders<Products>.Filter
                    .Eq(product => product.Id, id);

                var update = Builders<Products>.Update
                    .Set(p => p.TenSanPham, request.TenSanPham)
                    .Set(p => p.DonViTinh, request.DonViTinh)
                    .Set(p => p.QuyCach, request.QuyCach)
                    .Set(p => p.XuatXu, request.XuatXu)
                    .Set(p => p.MoTaNgan, request.MoTaNgan)
                    .Set(p => p.NhaSanXuat, request.NhaSanXuat)
                    .Set(p => p.GiaTien, request.GiaTien)
                    .Set(p => p.GiaTienDaGiam, request.GiaTienDaGiam)
                    .Set(p => p.PhanTramGiamGia, request.PhanTramGiamGia)
                    .Set(p => p.SoLuong, request.SoLuong)
                    .Set(p => p.ImageUrl, request.ImageUrl)
                    .Set(p => p.CategoryId, request.CategoryId)
                    .Set(p => p.DetailedDescription, request.DetailedDescription);

                var result = await _dbContext.Products.UpdateOneAsync(filter, update);

                if (result.ModifiedCount < 1) throw new Exception("Invalid Id");

                return new ResponseModel
                {
                    Succeeded = true,
                    Data = result,
                    Message = "Updated Successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Succeeded = false, Message = ex.Message };
            }
        }

        public async Task<ResponseModel> DeleteById(Guid id)
        {
            try
            {
                var result = await _dbContext.Products.DeleteOneAsync(p => p.Id == id);

                if (result.DeletedCount == 0)
                {
                    throw new Exception("Invalid Id");
                }
                return new ResponseModel { Succeeded = true, Message = "Deleted Successfully" };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Succeeded = true, Message = ex.Message };
            }
        }
    }
}
