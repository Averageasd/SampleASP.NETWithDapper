using DapperEmployeeManagement.Dto;
using DapperEmployeeManagement.Entities;

namespace DapperEmployeeManagement.Contracts
{
    public interface ICompanyRepository
    {
        public Task<IEnumerable<Company>> GetCompanies();
        public Task<Company> GetCompany(int id);
        public Task<Company> CreateCompany(CompanyForCreationDTO companyForCreationDTO);
        public Task UdpateCompany(int id, CompanyForUpdateDTO companyForUpdateDTO);
        public Task DeleteCompany(int id);
        public Task<Company> GetCompanyByEmployeeId(int id);
        public Task<Company> GetCompanyEmployeesMultipleResults(int id);
        public Task<List<Company>> GetCompaniesEmployeesMultipleMapping();
        public Task CreateMultipleCompanies(List<CompanyForCreationDTO> companies);

    }
}
