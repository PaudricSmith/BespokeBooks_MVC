using Bespoke.DataAccess.Data;
using Bespoke.DataAccess.Repository.IRepository;
using Bespoke.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bespoke.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }


        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public void Update(Category category)
        {
            _dbContext.Categories.Update(category);
        }
    }
}
