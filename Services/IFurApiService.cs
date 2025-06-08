using furnet.Models;

namespace furnet.Services
{
    public interface IFurApiService
    {
        Task<PackageListResponse?> GetPackagesAsync(string? sort = null, string? search = null);
        Task<FurConfig?> GetPackageAsync(string packageName, string? version = null);
        Task<bool> UploadPackageAsync(FurConfig furConfig);
        Task<bool> IsApiHealthyAsync();
        Task<List<Package>> GetPackageDetailsAsync(string? sort = null, string? search = null);
        void ClearCache();
        bool IsApiAvailable { get; }
        bool UseLocalStorage { get; }
    }
}
