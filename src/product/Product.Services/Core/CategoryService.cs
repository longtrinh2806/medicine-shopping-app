using Mapster;
using MongoDB.Driver;
using Product.Data.DataAccess;
using Product.Data.Dtos;
using Product.Data.ViewModels;
using ProductData.MongoCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Product.Services.Core
{
    public interface ICategoryService
    {
        Task<ResponseModel> GetAll(int pageNumber, int pageSize);
        Task<ResponseModel> GetById(Guid id);
        Task<ResponseModel> Create(CategoryDto request);
        Task<ResponseModel> UpdateById(Guid id, CategoryDto request);
        Task<ResponseModel> DeleteById(Guid id);
    }
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _dbContext;
        public CategoryService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ResponseModel> GetAll(int pageNumber, int pageSize)
        {
            try
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
            catch (Exception ex)
            {
                return new ResponseModel { Succeeded = false, Message = ex.Message };
            }
        }

        public async Task<ResponseModel> GetById(Guid id)
        {
            try
            {
                var result = await _dbContext.Categories.Find(p => p.Id == id).FirstOrDefaultAsync();
                if (result == null)
                    throw new Exception("Invalid Id");

                return new ResponseModel { Succeeded = true, Data = result, Message = "Got Successfully" };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Succeeded = false, Message = ex.Message };
            }
            
        }

        public async Task<ResponseModel> Create(CategoryDto request)
        {
            try
            {
                var category = request.Adapt<Category>();
                await _dbContext.Categories.InsertOneAsync(category);

                return new ResponseModel { Succeeded = true, Data = category, Message = "Created Successfully" };
            }
            catch (Exception ex)
            {
                return new ResponseModel{ Succeeded = false, Message = ex.Message };
            }
        }
        public async Task<ResponseModel> UpdateById(Guid id, CategoryDto request)
        {
            try
            {
                var filter = Builders<Category>.Filter.Eq(c => c.Id, id);
                var update = Builders<Category>.Update
                    .Set(c => c.Name, request.Name)
                    .Set(c => c.ParentId, request.ParentId);

                var result = await _dbContext.Categories.UpdateOneAsync(filter, update);

                if (result.ModifiedCount < 1) throw new Exception("Invalid id");

                return new ResponseModel { Data = request, Succeeded = true, Message = "Updated Successfully" };
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
                var result = await _dbContext.Categories.DeleteOneAsync(c => c.Id == id);
                if (result.DeletedCount == 0)
                    throw new Exception("Invalid Id");
                return new ResponseModel { Succeeded = true, Message = "Deleted Successfully" };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Succeeded = false, Message = ex.Message };
            }
        }
    }
}
