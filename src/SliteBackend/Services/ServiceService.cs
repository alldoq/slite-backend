using Microsoft.EntityFrameworkCore;
using SliteBackend.Data;
using SliteBackend.Models;

namespace SliteBackend.Services;

public class ServiceService : IServiceService
{
    private readonly ApplicationDbContext _context;

    public ServiceService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Service>> GetAllServicesAsync()
    {
        return await _context.Services
            .Include(s => s.Company)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<Service?> GetServiceByIdAsync(int id)
    {
        return await _context.Services
            .Include(s => s.Company)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Service>> GetServicesByCompanyIdAsync(int companyId)
    {
        return await _context.Services
            .Include(s => s.Company)
            .Where(s => s.CompanyId == companyId)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<Service> CreateServiceAsync(Service service)
    {
        _context.Services.Add(service);
        await _context.SaveChangesAsync();
        return service;
    }

    public async Task<Service?> UpdateServiceAsync(int id, Service service)
    {
        var existingService = await _context.Services.FindAsync(id);
        if (existingService == null)
            return null;

        existingService.Name = service.Name;
        existingService.Description = service.Description;
        existingService.Price = service.Price;
        existingService.CompanyId = service.CompanyId;
        existingService.DurationHours = service.DurationHours;
        existingService.Category = service.Category;
        existingService.IsActive = service.IsActive;
        existingService.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existingService;
    }

    public async Task<bool> DeleteServiceAsync(int id)
    {
        var service = await _context.Services.FindAsync(id);
        if (service == null)
            return false;

        _context.Services.Remove(service);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ServiceExistsAsync(int id)
    {
        return await _context.Services.AnyAsync(s => s.Id == id);
    }
}