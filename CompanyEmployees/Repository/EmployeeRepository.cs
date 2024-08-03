using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanyEmployees.Repository
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId, bool trackChanges)
        {
            var employees = await FindByCondition(e => e.CompanyId == companyId, trackChanges).OrderBy(c => c.Name).ToListAsync();
            return employees;
        }

        public async Task<Employee> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges)
        {
            var employee = await FindByCondition(e => e.CompanyId == companyId && e.Id == id, trackChanges).SingleOrDefaultAsync();
            return employee;
        }

        public void CreateEmployeeForCompany(Employee employee)
        {
            Create(employee);
        }

        public void DeleteEmployee(Employee employee)
        {
            Delete(employee);
        }
    }
}
