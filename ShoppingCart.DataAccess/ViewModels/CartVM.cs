// Под вопросом??

using ShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.ViewModels
{
    public class CartVM
    {
        public Cart Cart { get; set; } = new Cart();
        public IEnumerable<Cart> ListOfCart { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
