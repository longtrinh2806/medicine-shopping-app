using Customer.Data.DataAccess;
using Customer.Data.Dtos;
using Customer.Data.Models;
using Customer.Data.Request;
using Customer.Data.ViewModels;
using customer_order_contract;
using Mapster;
using MassTransit;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Customer.Services.Core
{
    public interface ICustomerService
    {
        ResponseModel GetByEmailorPhone(EmailOrPhone request);
        ResponseModel UpdateCustomerInfo(CustomerUpdatedDto request, Guid customerId);
        ResponseModel CreateNewCus(CustomerDto request);
        ResponseModel UpdateCustomerAddress(AddressDto request, Guid customerId);
    }
    public class CustomerService : ICustomerService
    {
        private readonly AppDbContext _appDbContext;
        private IPublishEndpoint _publishEndpoint;

        public CustomerService(AppDbContext appDbContext, IPublishEndpoint publishEndpoint)
        {
            _appDbContext = appDbContext;
            _publishEndpoint = publishEndpoint;
        }

        public ResponseModel GetByEmailorPhone(EmailOrPhone request)
        {
            Customers? result;

            if (string.IsNullOrEmpty(request.Email) && string.IsNullOrEmpty(request.Phone))
                throw new Exception("Input phone or email please!!");
            else if (!string.IsNullOrEmpty(request.Email) && !string.IsNullOrEmpty(request.Phone))
                throw new Exception("Just input 1");

            else if (string.IsNullOrEmpty(request.Phone))
            {
                result = _appDbContext.Customers.FirstOrDefault(y => y.Email == request.Email);
                if (result == null)
                    throw new Exception($"No Result with email: {request.Email}");
                return new ResponseModel { Data = result, Message = "Got Successfully", Succeeded = true };
            }

            else
            {
                result = _appDbContext.Customers.FirstOrDefault(c => c.PhoneNumber == request.Phone);
                if (result == null)
                    throw new Exception($"No Result with phone: {request.Phone}");
                return new ResponseModel { Data = result, Message = "Got Successfully", Succeeded = true };
            }
        }

        public ResponseModel CreateNewCus(CustomerDto request)
        {
            using (var transaction = _appDbContext.Database.BeginTransaction())
            {
                try
                {
                    var emailExisted = _appDbContext.Customers.FirstOrDefault(x => x.Email.Equals(request.Email));
                    if (emailExisted != null)
                        throw new Exception("Email is existed!!");

                    var phoneExisted = _appDbContext.Customers.FirstOrDefault(x => x.PhoneNumber.Equals(request.PhoneNumber));
                    if (phoneExisted != null)
                        throw new Exception("Phone is existed!!");

                    var newCustomer = request.Adapt<Customers>();

                    Membership newMembership = new()
                    {
                        CustomerId = newCustomer.Id,
                        CapDo = 0,
                        DiemTichLuy = 0
                    };
                    newCustomer.Membership = newMembership;

                    _appDbContext.Customers.Add(newCustomer);
                    _appDbContext.SaveChanges();

                    transaction.Commit();

                    CustomerCreated customerCreated = new()
                    {
                        CustomerId = newCustomer.Id,
                    };

                    _publishEndpoint.Publish(customerCreated);

                    return new ResponseModel { Message = "Created Successfully", Succeeded = true };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine(ex.Message);
                    return new ResponseModel { Message = ex.Message, Succeeded = false };
                }
            };
        }

        public ResponseModel UpdateCustomerInfo(CustomerUpdatedDto request, Guid customerId)
        {
            using (var transaction = _appDbContext.Database.BeginTransaction())
            {
                try
                {
                    var existedCustomer = _appDbContext.Customers.FirstOrDefault(x => x.Id == customerId);
                    if (existedCustomer == null)
                        throw new Exception("Invalid Id!!!");

                    existedCustomer.FirstName = request.FirstName;
                    existedCustomer.LastName = request.LastName;
                    existedCustomer.Gender = request.Gender;
                    existedCustomer.BirthDay = request.BirthDay;
                    existedCustomer.LastUpdatedAt = DateTime.UtcNow.AddHours(7);

                    _appDbContext.Customers.Update(existedCustomer);
                    _appDbContext.SaveChanges();

                    transaction.Commit();

                    return new ResponseModel { Data = existedCustomer, Message = "Updated Successfully", Succeeded = true };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine(ex.Message);
                    return new ResponseModel { Succeeded = false, Message = ex.Message };
                }
            }

        }

        public ResponseModel UpdateCustomerAddress(AddressDto request, Guid customerId)
        {
            using (var transaction = _appDbContext.Database.BeginTransaction())
            {
                try
                {
                    var existedCustomer = _appDbContext.Customers.FirstOrDefault(x => x.Id == customerId);
                    if (existedCustomer == null) throw new Exception("Customer not existed!!");

                    var existedAddress = _appDbContext.Addresses.FirstOrDefault(x => x.CustomerId == customerId);
                    if (existedAddress == null)
                    {
                        var newAddress = request.Adapt<Address>();
                        newAddress.CustomerId = customerId;

                        _appDbContext.Addresses.Add(newAddress);
                        _appDbContext.SaveChanges();
                        transaction.Commit();

                        return new ResponseModel { Succeeded = true, Message = "Add New Address Successfully" };
                    }

                    request.Adapt(existedAddress);
                    _appDbContext.Addresses.Update(existedAddress);
                    _appDbContext.SaveChanges();
                    transaction.Commit();

                    return new ResponseModel { Succeeded = true, Message = "Updated Successfully" };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new ResponseModel { Succeeded = false, Message = ex.Message };
                }

            }

        }

    }
}
