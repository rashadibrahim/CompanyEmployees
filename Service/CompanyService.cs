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

        public IEnumerable<CompanyDto> GetAllCompanies(bool trackChanges)
        {
            var companies = _repository.Company.GetAllCompanies(trackChanges);
            var companiesDto = companies.Select(c =>
            new CompanyDto(c.Id, c.Name ?? "", String.Join(' ', c.Address, c.Country)))
                .ToList();
            return companiesDto;
        }
        public CompanyDto GetCompany(Guid companyId, bool trackChanges)
        {
            var company = _repository.Company.GetCompany(companyId, trackChanges);
            if (company is null)
            {
                throw new CompanyNotFoundException(companyId);
            }
            var companyDto = new CompanyDto(company.Id, company.Name ?? "", String.Join(' ', company.Address, company.Country));
            return companyDto;
        }

        public CompanyDto CreateCompany(CompanyForCreationDto company)
        {
            var newCompany = new Company()
            {
                Name = company.Name,
                Address = company.Address,
                Country = company.Country
            };
            _repository.Company.CreateCompany(newCompany);
            _repository.Save();
            var companyDto = new CompanyDto(newCompany.Id, newCompany.Name ?? "", String.Join(' ', newCompany.Address, newCompany.Country));
            return companyDto;
        }

        public IEnumerable<CompanyDto> GetByIds(IEnumerable<Guid> ids, bool trackChanges)
        {
            
            if (ids is null)
                throw new IdParametersBadRequestException();
            var companies = _repository.Company.GetByIds(ids, trackChanges);
            if (ids.Count() != companies.Count())
                throw new CollectionByIdsBadRequestException();

            var companyDtos = companies.Select(c => new CompanyDto(c.Id, c.Name ?? "", String.Join(' ', c.Address, c.Country)));
            return companyDtos;
        }

        public IEnumerable<CompanyDto> CreateCompanyCollection(IEnumerable<CompanyForCreationDto> companyCollection)
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
                _repository.Save();
                companyCollectionToReturn.Add(new CompanyDto(company.Id, company.Name ?? "", String.Join(' ', company.Address, company.Country)));

            }
            return companyCollectionToReturn;
        }

        public void DeleteCompany(Guid companyId, bool trackChanges)
        {
            var company = _repository.Company.GetCompany(companyId, trackChanges);
            if (company is null)
            {
                throw new CompanyNotFoundException(companyId);
            }
            _repository.Company.DeleteCompany(company);
            _repository.Save();
        }

        public void UpdateCompany(Guid companyId, CompanyForUpdateDto companyForUpdate, bool trackChanges)
        {
            var company = _repository.Company.GetCompany(companyId, trackChanges);
            if (company is null)
            {
                throw new CompanyNotFoundException(companyId);
            }
            company.Name = companyForUpdate.Name;
            company.Address = companyForUpdate.Address;
            company.Country = companyForUpdate.Country;
            _repository.Save();
        }

    }
}