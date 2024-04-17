using BespokeBooks.DataAccess.Data;
using BespokeBooks.DataAccess.Repository.IRepository;
using BespokeBooks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BespokeBooks.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CompanyRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Update(Company company)
        {
            var companyFromDatabase = _dbContext.Companies.FirstOrDefault(c => c.Id == company.Id);

            if (companyFromDatabase != null)
            {
                companyFromDatabase.Name = company.Name;
                companyFromDatabase.StreetAddress = company.StreetAddress;
                companyFromDatabase.City = company.City;
                companyFromDatabase.State = company.State;
                companyFromDatabase.PostalCode = company.PostalCode;
                companyFromDatabase.PhoneNumber = company.PhoneNumber;
            }
        }
    }
}
