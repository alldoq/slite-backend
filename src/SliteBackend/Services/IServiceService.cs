using SliteBackend.Models;

namespace SliteBackend.Services;

public interface IServiceService
{
    Task<IEnumerable<Service>> GetAllServicesAsync();
    Task<Service?> GetServiceByIdAsync(int id);
    Task<IEnumerable<Service>> GetServicesByCompanyIdAsync(int companyId);
    Task<Service> CreateServiceAsync(Service service);
    Task<Service?> UpdateServiceAsync(int id, Service service);
    Task<bool> DeleteServiceAsync(int id);
    Task<bool> ServiceExistsAsync(int id);
}