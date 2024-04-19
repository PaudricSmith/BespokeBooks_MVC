using BespokeBooks.DataAccess.Data;
using BespokeBooks.Models;
using BespokeBooks.DataAccess.Repository.IRepository;


namespace BespokeBooks.DataAccess.Repository
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderDetailRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }


        public void Update(OrderDetail orderDetail)
        {
            _dbContext.OrderDetails.Update(orderDetail);
        }
    }
}
