using Microsoft.EntityFrameworkCore;
using ShoppingCart.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DataAccess.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private DbSet<T> _dbSet;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entity)
        {
            _dbSet.RemoveRange(entity);
        }

        public IEnumerable<T> GetAll(string? includePropertirs = null)
        {
            IQueryable<T> query = _dbSet;
            if (includePropertirs != null)
                foreach (var item in includePropertirs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    query = query.Include(item);
            return query.ToList();
        }

        public T GetT(System.Linq.Expressions.Expression<Func<T, bool>> predicate, string? includePropertirs = null)
        {
            IQueryable<T> query = _dbSet;
            query = query.Where(predicate);
            if (includePropertirs != null)
                foreach (var item in includePropertirs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    query = query.Include(item);
            return query.FirstOrDefault();
        }
    }
}
