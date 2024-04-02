using Amazon.Runtime.Internal;
using Mapster;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Driver;
using Product.Data.DataAccess;
using Product.Data.Dtos;
using Product.Data.ViewModels;
using ProductData.MongoCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Product.Services.Core
{
    public interface ICategoryService
    {
        Task<ResponseModel> GetAll(int pageNumber, int pageSize);
        ResponseModel GetById(Guid id);
        Task<ResponseModel> Create(CategoryDto request);
        Task<ResponseModel> UpdateById(Guid id, CategoryDto request);
        Task<ResponseModel> DeleteById(Guid id);
        Task<ResponseModel> GetByParentId(Guid parentId);
    }
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _dbContext;
        private readonly IDistributedCache _distributedCache;
        public CategoryService(AppDbContext dbContext, IDistributedCache distributedCache)
        {
            _dbContext = dbContext;
            _distributedCache = distributedCache;
        }

        public async Task<ResponseModel> GetAll(int pageNumber, int pageSize)
        {
                var result = await _dbContext.Categories
                            .Find(_ => true)
                            .Skip((pageNumber - 1) * pageSize)
                            .Limit(pageSize)
                            .ToListAsync();
                if (result.Count < 1)
                    throw new Exception("No Result");

                return new ResponseModel { Succeeded = true, Data = result, Message = "Got successfully" };
        }

        public ResponseModel GetById(Guid id)
        {
            var result = _dbContext.Categories.Find(p => p.Id == id).FirstOrDefault();
            if (result == null)
                throw new Exception("Invalid CategoryID");
            return new ResponseModel { Succeeded = true, Data = result, Message = "Got Successfully" };
        }

        public async Task<ResponseModel> Create(CategoryDto request)
        {
            var categoryExisted = await _dbContext.Categories.Find(c => c.Id.Equals(request.ParentId)).FirstOrDefaultAsync();
            if (categoryExisted == null)
                throw new Exception("Invaid Parent ID");
            var category = request.Adapt<Category>();
            await _dbContext.Categories.InsertOneAsync(category);

            CheckKey(request.ParentId);

            return new ResponseModel { Succeeded = true, Data = category, Message = "Created Successfully" };
        }
        public async Task<ResponseModel> UpdateById(Guid id, CategoryDto request)
        {
            var filter = Builders<Category>.Filter.Eq(c => c.Id, id);
            var update = Builders<Category>.Update
                .Set(c => c.Name, request.Name)
                .Set(c => c.ParentId, request.ParentId);

            var result = await _dbContext.Categories.UpdateOneAsync(filter, update);

            if (result.ModifiedCount < 1) throw new Exception("Invalid CategoryID");

            CheckKey(request.ParentId);

            return new ResponseModel { Data = request, Succeeded = true, Message = "Updated Successfully" };
        }

        public async Task<ResponseModel> DeleteById(Guid id)
        {
            var deletedCategory = _dbContext.Categories.Find(c => c.Id == id).FirstOrDefault();
            if (deletedCategory == null)
                throw new Exception("Invalid CategoryID");

            CheckKey(deletedCategory.ParentId);

            var result = await _dbContext.Categories.DeleteOneAsync(c => c.Id == id);
            return new ResponseModel { Succeeded = true, Message = "Deleted Successfully" };
        }

        public async Task<ResponseModel> GetByParentId(Guid parentId)
        {
            string key = $"Category_ParentID:{parentId}";
            string? cachedCategory = _distributedCache.GetString(key);

                if (cachedCategory == null)
                {
                var result = await _dbContext.Categories.Find(p => p.ParentId == parentId).ToListAsync();
                if (result.Count < 1)
                    throw new Exception("Invalid ParentId");

                _distributedCache.SetString(key, JsonSerializer.Serialize(result));

                return new ResponseModel { Succeeded = true, Data = result, Message = "Got Successfully" };
                }
            var categoryResult = JsonSerializer.Deserialize<List<Category>>(cachedCategory);
            return new ResponseModel { Succeeded = true, Data = categoryResult, Message = "Got Successfully" };
        }

        private void CheckKey(Guid parentId)
        {
            string key = $"Category_ParentID:{parentId}";
            string? cachedCategory = _distributedCache.GetString(key);

            if (cachedCategory != null)
            {
                _distributedCache.Remove(key);
            }
        }
    }
}
