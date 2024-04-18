

namespace BespokeBooks.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository CategoryRepo { get; }
        IProductRepository ProductRepo { get; }
        ICompanyRepository CompanyRepo { get; }
        IShoppingCartRepository ShoppingCartRepo { get; }

        void Save();
    }
}
