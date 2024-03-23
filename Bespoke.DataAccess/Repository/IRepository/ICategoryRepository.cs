using BespokeBooks.Models;

namespace BespokeBooks.DataAccess.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        void Update(Category category);
        void Save();
    }
}
