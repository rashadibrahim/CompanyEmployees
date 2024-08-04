using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

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

        public async Task<(IEnumerable<EmployeeDto> employees, MetaData metaData)> GetEmployeesAsync(Guid companyId, bool trackChanges, EmployeeParameters employeeParameters)
        {
            await CheckIfCompanyExists(companyId, trackChanges);
            if (!employeeParameters.ValidAgeRange)
            {
                throw new MaxAgeRangeBadRequestException();
            }
            var employeesWithMetaData = await _repository.Employee.GetEmployeesAsync(companyId, trackChanges, employeeParameters);
            
            var employeesDto = employeesWithMetaData.Select(e => new EmployeeDto(e.Id, e.Name, e.Age, e.Position));
            return (employeesDto, employeesWithMetaData.MetaData);
        }

        public async Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges)
        {
            await CheckIfCompanyExists(companyId, trackChanges);
            var employee = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id, trackChanges);
            
            var employeeDto = new EmployeeDto(employee.Id, employee.Name, employee.Age, employee.Position);
            return employeeDto;
        }

        public async Task<EmployeeDto> CreateEmployeeForCompanyAsync(Guid companyId, EmployeeForCreationDto employeeForCreation, bool trackChanges)
        {
            await CheckIfCompanyExists(companyId, trackChanges);
            var newEmployee = new Employee()
            {
                CompanyId = companyId,
                Name = employeeForCreation.Name,
                Age = employeeForCreation.Age,
                Position = employeeForCreation.Position
            };
            _repository.Employee.CreateEmployeeForCompany(newEmployee);
            await _repository.SaveAsync();
            return new EmployeeDto(newEmployee.Id, newEmployee.Name, newEmployee.Age, newEmployee.Position);
        }

        public async Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid id, bool trackChanges)
        {
            await CheckIfCompanyExists(companyId, trackChanges);

            var employee = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id, trackChanges);

            _repository.Employee.DeleteEmployee(employee);
            await _repository.SaveAsync();
        }

        public async Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid id, EmployeeForUpdateDto employeeForUpdate, bool compTrackChanges, bool empTrackChanges)
        {
            await CheckIfCompanyExists(companyId, compTrackChanges);

            var employee = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id, empTrackChanges);
            employee.Name = employeeForUpdate.Name;
            employee.Age = employeeForUpdate.Age;
            employee.Position = employeeForUpdate.Position;
            await _repository.SaveAsync();
        }

        private async Task CheckIfCompanyExists(Guid companyId, bool trackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);
            if (company is null)
                throw new CompanyNotFoundException(companyId);
        }
        private async Task<Employee> GetEmployeeForCompanyAndCheckIfItExists(Guid companyId, Guid id, bool trackChanges)
        {
            var employeeDb = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges);
            if (employeeDb is null)
                throw new EmployeeNotFoundException(id);
            return employeeDb;
        }

    }
}