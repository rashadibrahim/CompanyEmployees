using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service
{
    internal sealed class EmployeeService : IEmployeeService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        public EmployeeService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
        }
        
        public IEnumerable<EmployeeDto> GetEmployees(Guid companyId, bool trackChanges)
        {
            var company = _repository.Company.GetCompany(companyId, false);
            if (company is null)
            {
                throw new CompanyNotFoundException(companyId);
            }
            var employees = _repository.Employee.GetEmployees(companyId, trackChanges);
            var employeesDto = employees.Select(e => new EmployeeDto(e.Id, e.Name, e.Age, e.Position));
            return employeesDto;
        }

        public EmployeeDto GetEmployee(Guid companyId, Guid id, bool trackChanges)
        {
            var company = _repository.Company.GetCompany(companyId, false);
            if (company is null)
            {
                throw new CompanyNotFoundException(companyId);
            }
            var employee = _repository.Employee.GetEmployee(companyId, id, trackChanges);
            if (employee is null)
            {
                throw new EmployeeNotFoundException(id);
            }
            var employeeDto = new EmployeeDto(employee.Id, employee.Name, employee.Age, employee.Position);
            return employeeDto;
        }

        public EmployeeDto CreateEmployeeForCompany(Guid companyId, EmployeeForCreationDto employeeForCreation, bool trackChanges)
        {
            var company = _repository.Company.GetCompany(companyId, false);
            if (company is null)
            {
                throw new CompanyNotFoundException(companyId);
            }
            var newEmployee = new Employee()
            {
                CompanyId = companyId,
                Name = employeeForCreation.Name,
                Age = employeeForCreation.Age,
                Position = employeeForCreation.Position
            };
            _repository.Employee.CreateEmployeeForCompany(newEmployee);
            _repository.Save();
            return new EmployeeDto(newEmployee.Id, newEmployee.Name, newEmployee.Age, newEmployee.Position);
        }
    }
}