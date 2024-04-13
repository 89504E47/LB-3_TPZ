using ShoppingCart.DataAccess.Data;
using ShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Repositories
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUser
    {
        private ApplicationDbContext _context;
        public ApplicationUserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public void Update(ApplicationUser applicationUser)
        {
            var applicationUserDb = _context.ApplicationUsers.FirstOrDefault(x => x.Id == applicationUser.Id);
            if (applicationUserDb != null)
            {
                applicationUserDb.Name = applicationUser.Name;
                applicationUserDb.Address = applicationUser.Address;
                applicationUserDb.City = applicationUser.City;
                applicationUserDb.State = applicationUser.State;
                applicationUserDb.PinCode = applicationUser.PinCode;
            }
        }
    }
}
