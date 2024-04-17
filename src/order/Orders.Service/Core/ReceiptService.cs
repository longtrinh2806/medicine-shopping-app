using Orders.Data.DataAccess;
using Orders.Data.Models;
using Orders.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MassTransit.ValidationResultExtensions;

namespace Orders.Service.Core
{
    public interface IReceiptService
    {
        ResponseModel GetByOrderId(Guid orderId);
        ResponseModel GetById(Guid receiptId);
    }
    public class ReceiptService : IReceiptService
    {
        private readonly AppDbContext _dbContext;

        public ReceiptService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ResponseModel GetByOrderId(Guid orderId)
        {
            try
            {
                var result = _dbContext.Receipts.FirstOrDefault(x => x.OrderId == orderId);

                if (result == null)
                    throw new Exception("Receipt not existed!");
                return new ResponseModel { Data = result, Succeeded = true, Message = "Got Successfully" };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Succeeded = false, Message = ex.Message };
            }
        }

        public ResponseModel GetById(Guid receiptId)
        {
            try
            {
                var result = _dbContext.Receipts.FirstOrDefault(x => x.Id == receiptId);

                if (result == null)
                    throw new Exception("Receipt not existed!");
                return new ResponseModel { Data = result, Succeeded = true, Message = "Got Successfully" };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Succeeded = false, Message = ex.Message };
            }
        }
    }
}
