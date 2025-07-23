using furnet.Models;

namespace furnet.Services
{
    public interface ILocalPackageService
    {
        Task<List<FurConfig>> GetAllPackagesAsync();
        Task<FurConfig?> GetPackageAsync(string packageName, string? version = null);
        Task<bool> SavePackageAsync(FurConfig furConfig);
        Task<bool> DeletePackageAsync(string packageName);
        Task<PackageListResponse> GetPackageListAsync(string? sort = null, string? search = null, bool includeDetails = false);
        Task InitializeAsync();
    }
}
