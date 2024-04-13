using ShoppingCart.DataAccess.Data;
using ShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Repositories
{
    public class OrderDetailRepository: Repository<OrderDetail>, IOrderDetailRepository
    {
        private ApplicationDbContext _context;
        public OrderDetailRepository (ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public void Update (OrderDetail orderDetail)
        {
            _context.OrderDetails.Update(orderDetail);
        }
    }
}
