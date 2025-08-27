using Microsoft.EntityFrameworkCore;
using SliteBackend.Data;
using SliteBackend.Models;

namespace SliteBackend.Services;

public class CompanyService : ICompanyService
{
    private readonly ApplicationDbContext _context;

    public CompanyService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Company>> GetAllCompaniesAsync()
    {
        return await _context.Companies
            .Include(c => c.Services)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Company?> GetCompanyByIdAsync(int id)
    {
        return await _context.Companies
            .Include(c => c.Services)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Company?> GetCompanyByEmailAsync(string email)
    {
        return await _context.Companies
            .Include(c => c.Services)
            .FirstOrDefaultAsync(c => c.Email == email);
    }

    public async Task<Company> CreateCompanyAsync(Company company)
    {
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();
        return company;
    }

    public async Task<Company?> UpdateCompanyAsync(int id, Company company)
    {
        var existingCompany = await _context.Companies.FindAsync(id);
        if (existingCompany == null)
            return null;

        existingCompany.Name = company.Name;
        existingCompany.Description = company.Description;
        existingCompany.Email = company.Email;
        existingCompany.Phone = company.Phone;
        existingCompany.Website = company.Website;
        existingCompany.Address = company.Address;
        existingCompany.City = company.City;
        existingCompany.Country = company.Country;
        existingCompany.IsActive = company.IsActive;
        existingCompany.FoundedYear = company.FoundedYear;
        existingCompany.EmployeeCount = company.EmployeeCount;
        existingCompany.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existingCompany;
    }

    public async Task<bool> DeleteCompanyAsync(int id)
    {
        var company = await _context.Companies.FindAsync(id);
        if (company == null)
            return false;

        _context.Companies.Remove(company);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CompanyExistsAsync(int id)
    {
        return await _context.Companies.AnyAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Company>> GetActiveCompaniesAsync()
    {
        return await _context.Companies
            .Include(c => c.Services)
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }
}