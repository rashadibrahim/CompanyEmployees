using Contracts;
using Entities.Models;

namespace CompanyEmployees.Repository
{
    public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
    {
        public CompanyRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public IEnumerable<Company> GetAllCompanies(bool trackChanges)
        {
            var companies =  FindAll(trackChanges).OrderBy(c => c.Name).ToList();
            return companies;
        }
        public Company GetCompany(Guid companyId, bool trackChanges)
        {
            var company = FindByCondition(c => c.Id == companyId, trackChanges).SingleOrDefault();
            return company;
        }
    }
}
