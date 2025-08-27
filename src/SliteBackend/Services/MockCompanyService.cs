using SliteBackend.Models;

namespace SliteBackend.Services;

public class MockCompanyService : ICompanyService
{
    private static readonly List<Company> _companies = new()
    {
        new Company { Id = 1, Name = "Tech Corp", Email = "contact@techcorp.com", Phone = "555-0001" },
        new Company { Id = 2, Name = "Innovate LLC", Email = "info@innovate.com", Phone = "555-0002" }
    };

    public async Task<IEnumerable<Company>> GetAllCompaniesAsync()
    {
        await Task.Delay(10);
        return _companies;
    }

    public async Task<Company?> GetCompanyByIdAsync(int id)
    {
        await Task.Delay(10);
        return _companies.FirstOrDefault(c => c.Id == id);
    }

    public async Task<Company?> GetCompanyByEmailAsync(string email)
    {
        await Task.Delay(10);
        return _companies.FirstOrDefault(c => c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<Company> CreateCompanyAsync(Company company)
    {
        await Task.Delay(10);
        company.Id = _companies.Count + 1;
        _companies.Add(company);
        return company;
    }

    public async Task<Company?> UpdateCompanyAsync(int id, Company company)
    {
        await Task.Delay(10);
        var existingCompany = _companies.FirstOrDefault(c => c.Id == id);
        if (existingCompany != null)
        {
            existingCompany.Name = company.Name;
            existingCompany.Email = company.Email;
            existingCompany.Phone = company.Phone;
        }
        return existingCompany;
    }

    public async Task<bool> DeleteCompanyAsync(int id)
    {
        await Task.Delay(10);
        var company = _companies.FirstOrDefault(c => c.Id == id);
        if (company != null)
        {
            _companies.Remove(company);
            return true;
        }
        return false;
    }

    public async Task<bool> CompanyExistsAsync(int id)
    {
        await Task.Delay(10);
        return _companies.Any(c => c.Id == id);
    }

    public async Task<IEnumerable<Company>> GetActiveCompaniesAsync()
    {
        await Task.Delay(10);
        return _companies; // For mock, all companies are active
    }
}