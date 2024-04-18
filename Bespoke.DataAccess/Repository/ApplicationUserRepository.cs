using BespokeBooks.DataAccess.Data;
using BespokeBooks.Models;
using BespokeBooks.DataAccess.Repository.IRepository;


namespace BespokeBooks.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ApplicationUserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
