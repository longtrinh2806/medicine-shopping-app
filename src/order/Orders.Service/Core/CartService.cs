using Mapster;
using Orders.Data.DataAccess;
using Orders.Data.Dtos;
using Orders.Data.Models;
using Orders.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Service.Core
{
    public interface ICartService
    {
        ResponseModel AddItemToCart(Guid cartId, CartItemDto detailDto);
        ResponseModel GetCartByCustomerId(Guid customerId);
        ResponseModel GetCartItemByCartId(Guid cartId);
        ResponseModel UpdateItem(Guid cartItemId, UpdatedCartItemDto updatedCartItemDto);
        ResponseModel DeleteItem(Guid cartItemId);
    }
    public class CartService : ICartService
    {
        private readonly AppDbContext _dbContext;

        public CartService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ResponseModel GetCartByCustomerId(Guid customerId)
        {
            try
            {
                var cart = _dbContext.Carts.FirstOrDefault(c => c.CustomerId == customerId);
                if (cart == null)
                    throw new Exception("Cart not found. Invalid CustomerID or have error with RabbitMQ!!");

                return new ResponseModel { Data = cart, Succeeded = true, Message = "Got Successfully" };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new ResponseModel { Succeeded = false, Message = ex.Message };
            }
        }

        public ResponseModel AddItemToCart(Guid cartId, CartItemDto cartItemDto)
        {
            var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                var cart = _dbContext.Carts.FirstOrDefault(c => c.Id.Equals(cartId));
                if (cart == null)
                    throw new Exception("Cart not found. Invalid CartID or have error with RabbitMQ!!");

                var cartItem = cartItemDto.Adapt<CartItem>();

                if (cart.CartItems == null)
                {
                    cart.CartItems = new List<CartItem>();
                }

                cart.CartItems.Add(cartItem);

                _dbContext.Carts.Update(cart);
                _dbContext.SaveChanges();

                transaction.Commit();

                return new ResponseModel { Succeeded = true, Message = "Add Successfully" };
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                Console.WriteLine(ex);
                return new ResponseModel { Succeeded = false, Message = ex.Message };
            }
        }

        public ResponseModel GetCartItemByCartId(Guid cartId)
        {
            try
            {
                var result = _dbContext.CartItems.Where(cartItem => cartItem.CartId.Equals(cartId)).ToList();
                if (!result.Any())
                    throw new Exception("Doesn't have any Cart Item or Invalid CartId");

                return new ResponseModel { Data = result, Succeeded = true, Message = "Got Successfully" };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new ResponseModel { Succeeded = false, Message = ex.Message };
            }
        }

        public ResponseModel UpdateItem(Guid cartItemId, UpdatedCartItemDto cartItemDto)
        {
            var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                var result = _dbContext.CartItems.FirstOrDefault(cartItem => cartItem.Id.Equals(cartItemId));
                if (result == null)
                    throw new Exception("Invalid Cart Item ID!");
                cartItemDto.Adapt(result);

                _dbContext.CartItems.Update(result);
                _dbContext.SaveChanges();

                transaction.Commit();
                return new ResponseModel { Data = result, Succeeded = true, Message = "Updated Successfully" };
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                Console.WriteLine($"Error: {ex}");
                return new ResponseModel { Succeeded = false, Message = $"Error: {ex.Message}" };
            }
        }

        public ResponseModel DeleteItem(Guid cartItemId)
        {
            var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                var deletedItem = _dbContext.CartItems.FirstOrDefault(cartItem => cartItem.Id.Equals(cartItemId));
                if (deletedItem == null)
                    throw new Exception("Cart Item not existed!");
                _dbContext.CartItems.Remove(deletedItem);

                _dbContext.SaveChanges();
                transaction.Commit();

                return new ResponseModel { Succeeded = true, Message = "Deleted Successfully" };
            }
            catch (Exception ex)
            {
                transaction.Rollback();

                Console.WriteLine($"Error: {ex}");
                return new ResponseModel { Succeeded = false, Message = $"Error: {ex.Message}" };
            }
        }
    }
}
