using Purrnet.Models;

namespace Purrnet.Services
{
    public interface IPurrApiService
    {
        Task<PackageListResponse?> GetPackagesAsync(string? sort = null, string? search = null);
        Task<PurrConfig?> GetPackageAsync(string packageName, string? version = null);
        Task<bool> UploadPackageAsync(PurrConfig PurrConfig);
        Task<bool> IsApiHealthyAsync();
        Task<List<Package>> GetPackageDetailsAsync(string? sort = null, string? search = null);
        void ClearCache();
        bool IsApiAvailable { get; }
        bool UseLocalStorage { get; }
    }
}
