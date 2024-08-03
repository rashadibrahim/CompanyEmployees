using Contracts;
using Entities.Models;

namespace CompanyEmployees.Repository
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public IEnumerable<Employee> GetEmployees(Guid companyId, bool trackChanges)
        {
            var employees = FindByCondition(e => e.CompanyId == companyId, trackChanges).OrderBy(c => c.Name).ToList();
            return employees;
        }

        public Employee GetEmployee(Guid companyId, Guid id, bool trackChanges)
        {
            var employee = FindByCondition(e => e.CompanyId == companyId && e.Id == id, trackChanges).SingleOrDefault();
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
