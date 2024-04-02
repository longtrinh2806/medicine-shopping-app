using Customer.Data.DataAccess;
using Customer.Data.Models;
using Customer.Data.Request;
using Customer.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Services.Core
{
    public interface IAddressService
    {
        ResponseModel GetAddressByCustomerId(Guid customerId);
    }
    public class AddressService : IAddressService
    {
        private readonly AppDbContext _appDbContext;

        public AddressService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public ResponseModel GetAddressByCustomerId(Guid customerId)
        {
            var result = _appDbContext.Addresses.FirstOrDefault(x => x.CustomerId.Equals(customerId));
            if (result == null)
                throw new Exception("Address not existed!!");
            return new ResponseModel { Data = result, Succeeded = true, Message = "Got Successfully" };
        }
    }
}
