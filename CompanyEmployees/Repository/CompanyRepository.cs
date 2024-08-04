using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Shared.RequestFeatures;

namespace CompanyEmployees.Repository
{
    public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
    {
        public CompanyRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<PagedList<Company>> GetAllCompaniesAsync(bool trackChanges, CompanyParameters companyParameters)
        {
            var companies =  await FindAll(trackChanges)
                .OrderBy(c => c.Name)
                .Skip(companyParameters.PageSize * companyParameters.PageNumber)
                .Take(companyParameters.PageSize)
                .ToListAsync();
            var count = await FindAll(trackChanges).CountAsync();
            return new PagedList<Company>(companies, count, companyParameters.PageNumber, companyParameters.PageSize);
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
