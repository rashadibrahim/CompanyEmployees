using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;
using System.ComponentModel.Design;

namespace Service
{
    internal sealed class CompanyService : ICompanyService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        public CompanyService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync(bool trackChanges)
        {
            var companies = await _repository.Company.GetAllCompaniesAsync(trackChanges);
            var companiesDto = companies.Select(c =>
            new CompanyDto(c.Id, c.Name ?? "", String.Join(' ', c.Address, c.Country)))
                .ToList();
            return companiesDto;
        }
        public async Task<CompanyDto> GetCompanyAsync(Guid id, bool trackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(id, trackChanges);
            if (company is null)
            {
                throw new CompanyNotFoundException(id);
            }
            var companyDto = new CompanyDto(company.Id, company.Name ?? "", String.Join(' ', company.Address, company.Country));
            return companyDto;
        }

        public async Task<CompanyDto> CreateCompanyAsync(CompanyForCreationDto company)
        {
            var newCompany = new Company()
            {
                Name = company.Name,
                Address = company.Address,
                Country = company.Country
            };
            _repository.Company.CreateCompany(newCompany);
            await _repository.SaveAsync();
            var companyDto = new CompanyDto(newCompany.Id, newCompany.Name ?? "", String.Join(' ', newCompany.Address, newCompany.Country));
            return companyDto;
        }

        public async Task<IEnumerable<CompanyDto>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges)
        {
            
            if (ids is null)
                throw new IdParametersBadRequestException();
            var companies = await _repository.Company.GetByIdsAsync(ids, trackChanges);
            if (ids.Count() != companies.Count())
                throw new CollectionByIdsBadRequestException();

            var companyDtos = companies.Select(c => new CompanyDto(c.Id, c.Name ?? "", String.Join(' ', c.Address, c.Country)));
            return companyDtos;
        }

        public async Task<IEnumerable<CompanyDto>> CreateCompanyCollectionAsync(IEnumerable<CompanyForCreationDto> companyCollection)
        {
            if (companyCollection is null)
                throw new CompanyCollectionBadRequest();

            var newCompanies = companyCollection.Select(c => new Company()
            {
                Name = c.Name,
                Country = c.Country,
                Address = c.Address
            });
            var companyCollectionToReturn = new List<CompanyDto>();
            foreach (var company in newCompanies)
            {
                
                _repository.Company.CreateCompany(company);
                await _repository.SaveAsync();
                companyCollectionToReturn.Add(new CompanyDto(company.Id, company.Name ?? "", String.Join(' ', company.Address, company.Country)));

            }
            return companyCollectionToReturn;
        }

        public async Task DeleteCompanyAsync(Guid companyId, bool trackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);
            if (company is null)
            {
                throw new CompanyNotFoundException(companyId);
            }
            _repository.Company.DeleteCompany(company);
            await _repository.SaveAsync();
        }

        public async Task UpdateCompanyAsync(Guid companyId, CompanyForUpdateDto companyForUpdate, bool trackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);
            if (company is null)
            {
                throw new CompanyNotFoundException(companyId);
            }
            company.Name = companyForUpdate.Name;
            company.Address = companyForUpdate.Address;
            company.Country = companyForUpdate.Country;
            await _repository.SaveAsync();
        }

    }
}