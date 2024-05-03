using BespokeBooks.DataAccess.Data;
using BespokeBooks.Models;
using BespokeBooks.DataAccess.Repository.IRepository;


namespace BespokeBooks.DataAccess.Repository
{
    public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductImageRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }


        public void Update(ProductImage productImage)
        {
            _dbContext.ProductImages.Update(productImage);
        }
    }
}
