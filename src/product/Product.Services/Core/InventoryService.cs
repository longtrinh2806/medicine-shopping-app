using MongoDB.Driver;
using Product.Data.DataAccess;
using Product.Data.Dtos;
using Product.Data.MongoCollections;
using Product.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Product.Services.Core
{
    public interface IInventoryService
    {
        ResponseModel DeleteByProductId(Guid productId);
        ResponseModel GetByProductId(Guid productId);
        ResponseModel UpdateByProductId(Guid productId, InventoryDto request);
    }
    public class InventoryService : IInventoryService
    {
        private readonly AppDbContext _appDbContext;

        public InventoryService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public ResponseModel GetByProductId(Guid productId)
        {
            var result = _appDbContext.Inventories.Find(x => x.ProductId == productId).FirstOrDefault();
            if (result == null)
                throw new Exception("Invalid ProductID");
            return new ResponseModel { Data = result, Message = "Got Successfully", Succeeded = true };
        }

        public ResponseModel UpdateByProductId(Guid productId, InventoryDto request)
        {
            var filter = Builders<Inventory>.Filter.Eq(i => i.ProductId, productId);
            var update = Builders<Inventory>.Update
                .Set(i => i.QuantityInStock, request.QuantityInStock);

            var result = _appDbContext.Inventories.UpdateOne(filter, update);

            if (result.ModifiedCount == 0) throw new Exception("Invalid Id");
            return new ResponseModel
            {
                Succeeded = true,
                Data = result,
                Message = "Updated Successfully"
            };
        }
        public ResponseModel DeleteByProductId(Guid productId)
        {
            var result = _appDbContext.Inventories.DeleteOne(i => i.ProductId == productId);
            if (result.DeletedCount < 1)
                throw new Exception("Invalid Id");
            return new ResponseModel { Succeeded = true, Message = "Deleted Successfully" };
        }
    }
}
