using BespokeBooks.DataAccess.Data;
using BespokeBooks.Models;
using BespokeBooks.DataAccess.Repository.IRepository;


namespace BespokeBooks.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }


        public void Update(Product product)
        {
            var productFromDatabase = _dbContext.Products.FirstOrDefault(p => p.Id == product.Id);

            if (productFromDatabase != null) 
            {
                productFromDatabase.Title = product.Title;
                productFromDatabase.ISBN = product.ISBN;
                productFromDatabase.Price = product.Price;
                productFromDatabase.Price50 = product.Price50;
                productFromDatabase.ListPrice = product.ListPrice;
                productFromDatabase.Price100 = product.Price100;
                productFromDatabase.Description = product.Description;
                productFromDatabase.CategoryId = product.CategoryId;
                productFromDatabase.Author = product.Author;
                productFromDatabase.ProductImages = product.ProductImages;
            }
        }
    }
}
