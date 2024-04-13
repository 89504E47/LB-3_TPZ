using ShoppingCart.DataAccess.Data;
using ShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Repositories
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        private ApplicationDbContext _context;
        public CartRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public void Update(Cart cart)
        {
            var cartDb = _context.Carts.FirstOrDefault(x => x.Id == cart.Id);
            if (cartDb != null)
            {
                cartDb.Product = cart.Product;
                cartDb.ApplicationUserId = cart.ApplicationUserId;
                cartDb.ApplicationUser = cart.ApplicationUser;
                cartDb.Count = cart.Count;
            }
        }
        public void DecrementCartItem(Cart cart, int amount)
        {
            var cartDb = _context.Carts.FirstOrDefault(c => c.Id == cart.Id);
            if (cartDb != null)
                cartDb.Count -= amount;
        }

        public void IncrementCartItem(Cart cart, int count)
        {
            var cartDb = _context.Carts.FirstOrDefault(c => c.Id == cart.Id);
            if (cartDb != null)
                cartDb.Count += count;
        }
    }
}
