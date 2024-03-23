using BespokeBooks.DataAccess.Data;
using BespokeBooks.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BespokeBooks.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _dbContext;
        public ICategoryRepository CategoryRepo { get; private set; }
        public IProductRepository ProductRepo { get; private set; }

        public UnitOfWork(ApplicationDbContext dbContext) 
        {
            _dbContext = dbContext;
            CategoryRepo = new CategoryRepository(_dbContext);
            ProductRepo = new ProductRepository(_dbContext);
        }


        public void Save()
        {
            _dbContext.SaveChanges();
        }
    }
}
