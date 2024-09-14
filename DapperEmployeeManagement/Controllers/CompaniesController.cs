using DapperEmployeeManagement.Contracts;
using DapperEmployeeManagement.Dto;
using Microsoft.AspNetCore.Mvc;

namespace DapperEmployeeManagement.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;

        public CompaniesController(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            try
            {
                var companies = await _companyRepository.GetCompanies();
                return Ok(companies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}", Name = "CompanyById")]

        public async Task<IActionResult> GetCompany(int id)
        {
            try
            {
                var company = await _companyRepository.GetCompany(id);
                if (company == null)
                {
                    return NotFound();
                }
                return Ok(company);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompany(CompanyForCreationDTO company)
        {
            try
            {
                var createdCompany = await _companyRepository.CreateCompany(company);
                return CreatedAtRoute("CompanyById", new { id = createdCompany.Id }, createdCompany);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompany(int id, CompanyForUpdateDTO companyForUpdateDTO)
        {
            try
            {
                var company = await _companyRepository.GetCompany(id);
                if (company == null)
                {
                    return NotFound();
                }
                await _companyRepository.UdpateCompany(id, companyForUpdateDTO);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            try
            {
                var company = await _companyRepository.GetCompany(id);
                if (company == null)
                {
                    return NotFound();
                }
                await _companyRepository.DeleteCompany(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("ByEmployeeId/{id}")]
        public async Task<IActionResult> GetCompanyForEmployee(int id)
        {
            try
            {
                var company = await _companyRepository.GetCompanyByEmployeeId(id);
                if (company == null)
                {
                    return NotFound();
                }
                return Ok(company);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}/MultipleResult")]
        public async Task<IActionResult> GetCompanyEmployeesMultipleResults(int id)
        {
            try
            {
                var company = await _companyRepository.GetCompanyEmployeesMultipleResults(id);
                if (company == null)
                {
                    return NotFound();
                }
                return Ok(company);
            }
            catch (Exception ex) {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("MultipleMapping")]
        public async Task<ActionResult> GetCompaniesEmployeesMultipleMapping()
        {
            try
            {
                var companies = await _companyRepository.GetCompaniesEmployeesMultipleMapping();
                return Ok(companies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("MultipleCompaniesCreation")]
        public async Task<ActionResult> CreateMultipleCompanies(List<CompanyForCreationDTO> companies)
        {
            try
            {
                await _companyRepository.CreateMultipleCompanies(companies);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
