using SliteBackend.Models;

namespace SliteBackend.Services;

public interface ICompanyService
{
    Task<IEnumerable<Company>> GetAllCompaniesAsync();
    Task<Company?> GetCompanyByIdAsync(int id);
    Task<Company?> GetCompanyByEmailAsync(string email);
    Task<Company> CreateCompanyAsync(Company company);
    Task<Company?> UpdateCompanyAsync(int id, Company company);
    Task<bool> DeleteCompanyAsync(int id);
    Task<bool> CompanyExistsAsync(int id);
    Task<IEnumerable<Company>> GetActiveCompaniesAsync();
}