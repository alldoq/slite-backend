using SliteBackend.Models;

namespace SliteBackend.Services;

public class MockServiceService : IServiceService
{
    private static readonly List<Service> _services = new()
    {
        new Service { Id = 1, Name = "Web Development", CompanyId = 1 },
        new Service { Id = 2, Name = "Mobile Apps", CompanyId = 1 },
        new Service { Id = 3, Name = "Consulting", CompanyId = 2 }
    };

    public async Task<IEnumerable<Service>> GetAllServicesAsync()
    {
        await Task.Delay(10);
        return _services;
    }

    public async Task<Service?> GetServiceByIdAsync(int id)
    {
        await Task.Delay(10);
        return _services.FirstOrDefault(s => s.Id == id);
    }

    public async Task<IEnumerable<Service>> GetServicesByCompanyIdAsync(int companyId)
    {
        await Task.Delay(10);
        return _services.Where(s => s.CompanyId == companyId);
    }

    public async Task<Service> CreateServiceAsync(Service service)
    {
        await Task.Delay(10);
        service.Id = _services.Count + 1;
        _services.Add(service);
        return service;
    }

    public async Task<Service?> UpdateServiceAsync(int id, Service service)
    {
        await Task.Delay(10);
        var existingService = _services.FirstOrDefault(s => s.Id == id);
        if (existingService != null)
        {
            existingService.Name = service.Name;
            existingService.CompanyId = service.CompanyId;
        }
        return existingService;
    }

    public async Task<bool> DeleteServiceAsync(int id)
    {
        await Task.Delay(10);
        var service = _services.FirstOrDefault(s => s.Id == id);
        if (service != null)
        {
            _services.Remove(service);
            return true;
        }
        return false;
    }

    public async Task<bool> ServiceExistsAsync(int id)
    {
        await Task.Delay(10);
        return _services.Any(s => s.Id == id);
    }
}