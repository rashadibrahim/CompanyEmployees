using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanyEmployees.Repository
{
    public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
    {
        public CompanyRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<Company>> GetAllCompaniesAsync(bool trackChanges)
        {
            var companies =  await FindAll(trackChanges).OrderBy(c => c.Name).ToListAsync();
            return companies;
        }
        public async Task<Company> GetCompanyAsync(Guid companyId, bool trackChanges)
        {
            var company = await FindByCondition(c => c.Id == companyId, trackChanges).SingleOrDefaultAsync();
            return company;
        }

        public void CreateCompany(Company company)
        {
            Create(company);
        }

        public async Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges)
        {
            var companies = await FindByCondition(c => ids.Contains(c.Id), trackChanges).ToListAsync();
            return companies;
        }

        public void DeleteCompany(Company company)
        {
            Delete(company);
        }

    }
}
