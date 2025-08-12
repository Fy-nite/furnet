using Purrnet.Models;

namespace Purrnet.Services
{
    public interface ILocalPackageService
    {
        Task<List<PurrConfig>> GetAllPackagesAsync();
        Task<PurrConfig?> GetPackageAsync(string packageName, string? version = null);
        Task<bool> SavePackageAsync(PurrConfig PurrConfig);
        Task<bool> DeletePackageAsync(string packageName);
        Task<PackageListResponse> GetPackageListAsync(string? sort = null, string? search = null, bool includeDetails = false);
        Task InitializeAsync();
    }
}
