using Dapper;
using DapperEmployeeManagement.Context;
using DapperEmployeeManagement.Contracts;
using DapperEmployeeManagement.Dto;
using DapperEmployeeManagement.Entities;
using System.Data;

namespace DapperEmployeeManagement.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private DapperContext _context;
        public CompanyRepository(DapperContext dapperContext)
        {
            _context = dapperContext;
        }

        public async Task<IEnumerable<Company>> GetCompanies()
        {
            var query = "SELECT * FROM Companies";
            var paramters = new DynamicParameters();

            using (var connection = _context.CreateConnection())
            {
                var companies = await connection.QueryAsync<Company>(query);
                return companies.ToList();
            }
        }

        public async Task<Company> GetCompany(int id)
        {
            var query = "SELECT * FROM Companies WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                var company = await connection.QuerySingleOrDefaultAsync<Company>(query, new { id });

                return company;
            }
        }

        public async Task<Company> CreateCompany(CompanyForCreationDTO companyForCreationDTO)
        {
            var query = "INSERT INTO Companies (Name, Address, Country) VALUES (@Name, @Address, @Country)" + "SELECT CAST(SCOPE_IDENTITY() as int)";
            
            //add paramater 
            var paramters = new DynamicParameters();
            paramters.Add("Name", companyForCreationDTO.Name, DbType.String);
            paramters.Add("Address", companyForCreationDTO.Address, DbType.String);
            paramters.Add("Country", companyForCreationDTO.Country, DbType.String);
            using (var connection = _context.CreateConnection())
            {
                // execute 2 rows as a time.
                // query single expects the query result to be 1 row
                // this method returns the identity value of last inserted row
                var id = await connection.QuerySingleAsync<int>(query, paramters);

                var createdCompany = new Company
                {
                    Id = id,
                    Name = companyForCreationDTO.Name,
                    Address = companyForCreationDTO.Address,
                    Country = companyForCreationDTO.Country,    
                };
                return createdCompany;
            }
        }

        public async Task UdpateCompany(int id, CompanyForUpdateDTO companyForUpdateDTO)
        {
            var query = "UPDATE Companies SET Name = @Name, Address = @Address, Country = @Country WHERE Id = @Id";
            var paramters = new DynamicParameters();
            paramters.Add("Name", companyForUpdateDTO.Name, DbType.String);
            paramters.Add("Address", companyForUpdateDTO.Address, DbType.String);
            paramters.Add("Country", companyForUpdateDTO.Country, DbType.String);
            paramters.Add("Id", id, DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, paramters);
            }

        }

        public async Task DeleteCompany(int id)
        {
            var query = "DELETE FROM Companies WHERE Id = @Id";
            var paramters = new DynamicParameters();
            paramters.Add("Id", id, DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, paramters);
            }
        }

        public async Task<Company> GetCompanyByEmployeeId(int id)
        {
            var procedureName = "ShowCompanyForProvidedEmployeeId";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Int32, ParameterDirection.Input);
            using (var connection = _context.CreateConnection())
            {
                var company = await connection.QueryFirstOrDefaultAsync<Company>(procedureName, parameters, commandType: CommandType.StoredProcedure);

                return company;
            }
        }

        public async Task<Company> GetCompanyEmployeesMultipleResults(int id)
        {
            var query = "SELECT * FROM Companies WHERE Id = @Id;" +
                        "SELECT * FROM Employees WHERE CompanyId = @Id";
            using(var connection = _context.CreateConnection())
            using(var multi = await connection.QueryMultipleAsync(query,new {id }))
            {
                // query first statement
                var company = await multi.ReadSingleOrDefaultAsync<Company>();
                   
                // company is found, execute second select statements
                if (company != null)
                {
                    // update list of employees of company entity
                    company.Employees = (await multi.ReadAsync<Employee>()).ToList();
                }

                return company;

            }
        }

        public async Task<List<Company>> GetCompaniesEmployeesMultipleMapping()
        {
            var query = "SELECT * FROM Companies c JOIN Employees e ON c.Id = e.CompanyId";

            using (var connection = _context.CreateConnection())
            {
                var companyDict = new Dictionary<int, Company>();
                
                // query returns virtual table that contains records 
                // from both companies and employees tables
                var companies = await connection.QueryAsync<Company, Employee, Company>(query, (company, employee) =>
                {
                    if (!companyDict.TryGetValue(company.Id, out var currentCompany))
                    {
                        currentCompany = company;
                        companyDict.Add(company.Id, currentCompany);
                    }

                    // add found employee to employees list of company entity
                    currentCompany.Employees.Add(employee);
                    return currentCompany;
                });
                return companies.Distinct().ToList();
            }
        }

        public async Task CreateMultipleCompanies(List<CompanyForCreationDTO> companies)
        {
            var query = "INSERT INTO Companies (Name, Address, Country) VALUES (@Name, @Address, @Country)";

            using (var connection = _context.CreateConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    foreach (var company in companies)
                    {
                        var paramters = new DynamicParameters();
                        paramters.Add("Name", company.Name, DbType.String);
                        paramters.Add("Address", company.Address, DbType.String);
                        paramters.Add("Country", company.Country, DbType.String);

                        await connection.ExecuteAsync(query, paramters, transaction: transaction);
                    }

                    transaction.Commit();
                }
            }
        }
    }
}
