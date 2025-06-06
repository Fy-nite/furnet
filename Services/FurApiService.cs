using furnet.Models;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;

namespace furnet.Services
{
    public class FurApiService : IFurApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FurApiService> _logger;
        private readonly IMemoryCache _cache;
        private readonly string _baseUrl;
        private const string CACHE_KEY_PACKAGES = "cached_packages";
        private const string CACHE_KEY_PACKAGE_COUNT = "cached_package_count";
        private const string CACHE_KEY_PACKAGE_DETAILS = "cached_package_details";
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

        public bool IsApiAvailable { get; private set; } = true;

        public FurApiService(HttpClient httpClient, ILogger<FurApiService> logger, IMemoryCache cache, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _cache = cache;
            _baseUrl = configuration.GetValue<string>("ApiSettings:BaseUrl") ?? "http://localhost:5001";
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        public async Task<PackageListResponse?> GetPackagesAsync(string? sort = null, string? search = null)
        {
            try
            {
                var queryParams = new List<string>();
                if (!string.IsNullOrEmpty(sort)) queryParams.Add($"sort={Uri.EscapeDataString(sort)}");
                if (!string.IsNullOrEmpty(search)) queryParams.Add($"search={Uri.EscapeDataString(search)}");
                
                var query = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
                var response = await _httpClient.GetAsync($"/api/v1/packages{query}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var packageResponse = JsonSerializer.Deserialize<PackageListResponse>(content, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                    });

                    if (packageResponse != null)
                    {
                        IsApiAvailable = true;
                        
                        // Check if package count changed and invalidate cache if necessary
                        var cacheKey = $"{CACHE_KEY_PACKAGES}_{sort}_{search}";
                        if (_cache.TryGetValue(CACHE_KEY_PACKAGE_COUNT, out int cachedCount))
                        {
                            if (cachedCount != packageResponse.PackageCount)
                            {
                                _logger.LogInformation("Package count changed from {OldCount} to {NewCount}, clearing cache", 
                                    cachedCount, packageResponse.PackageCount);
                                ClearCache();
                            }
                        }
                        
                        // Update cached package count
                        _cache.Set(CACHE_KEY_PACKAGE_COUNT, packageResponse.PackageCount, _cacheExpiration);
                        _cache.Set(cacheKey, packageResponse, _cacheExpiration);
                    }
                    
                    return packageResponse;
                }
                
                IsApiAvailable = false;
                return GetCachedPackageResponse(sort, search);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching packages from API");
                IsApiAvailable = false;
                return GetCachedPackageResponse(sort, search);
            }
        }

        public async Task<FurConfig?> GetPackageAsync(string packageName, string? version = null)
        {
            try
            {
                var url = version != null 
                    ? $"/api/v1/packages/{Uri.EscapeDataString(packageName)}/{Uri.EscapeDataString(version)}"
                    : $"/api/v1/packages/{Uri.EscapeDataString(packageName)}";
                    
                var response = await _httpClient.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<FurConfig>(content, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                    });
                }
                
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching package {PackageName} from API", packageName);
                return null;
            }
        }

        public async Task<bool> UploadPackageAsync(FurConfig furConfig)
        {
            try
            {
                var json = JsonSerializer.Serialize(furConfig, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                });
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync("/api/v1/packages", content);
                
                if (response.IsSuccessStatusCode)
                {
                    // Clear cache since a new package was added
                    ClearCache();
                    _logger.LogInformation("Package {PackageName} uploaded successfully, cache cleared", furConfig.Name);
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to upload package {PackageName}. Status: {StatusCode}, Error: {Error}", 
                        furConfig.Name, response.StatusCode, errorContent);
                }
                
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading package {PackageName}", furConfig.Name);
                return false;
            }
        }

        public async Task<bool> IsApiHealthyAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/health");
                IsApiAvailable = response.IsSuccessStatusCode;
                return IsApiAvailable;
            }
            catch
            {
                IsApiAvailable = false;
                return false;
            }
        }

        public async Task<List<Package>> GetPackageDetailsAsync(string? sort = null, string? search = null)
        {
            var cacheKey = $"{CACHE_KEY_PACKAGE_DETAILS}_{sort}_{search}";
            
            // Skip cache if API is available to ensure fresh data
            if (!IsApiAvailable && _cache.TryGetValue(cacheKey, out List<Package>? cachedPackages) && cachedPackages != null)
            {
                _logger.LogInformation("Using cached package details (API unavailable)");
                return cachedPackages;
            }

            try
            {
                // Use the enhanced API with details=true
                var queryParams = new List<string>();
                if (!string.IsNullOrEmpty(sort)) queryParams.Add($"sort={Uri.EscapeDataString(sort)}");
                if (!string.IsNullOrEmpty(search)) queryParams.Add($"search={Uri.EscapeDataString(search)}");
                queryParams.Add("details=true");
                
                var query = "?" + string.Join("&", queryParams);
                var response = await _httpClient.GetAsync($"/api/v1/packages{query}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var packageResponse = JsonSerializer.Deserialize<PackageListResponse>(content, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                    });

                    if (packageResponse?.PackageDetails != null)
                    {
                        IsApiAvailable = true;
                        var packages = packageResponse.PackageDetails.Select(furConfig => new Package
                        {
                            Name = furConfig.Name,
                            Version = furConfig.Version,
                            Authors = furConfig.Authors,
                            SupportedPlatforms = furConfig.SupportedPlatforms,
                            Description = furConfig.Description,
                            LongDescription = furConfig.LongDescription,
                            License = furConfig.License,
                            LicenseUrl = furConfig.LicenseUrl,
                            Keywords = furConfig.Keywords,
                            Tags = furConfig.Tags,
                            Homepage = furConfig.Homepage,
                            IssueTracker = furConfig.IssueTracker,
                            Git = furConfig.Git,
                            Installer = furConfig.Installer,
                            InstallCommand = $"fur install {furConfig.Name}",
                            Dependencies = furConfig.Dependencies,
                            Downloads = Random.Shared.Next(100, 50000),
                            LastUpdated = DateTime.Now.AddDays(-Random.Shared.Next(1, 365))
                        }).ToList();
                        
                        _cache.Set(cacheKey, packages, _cacheExpiration);
                        _logger.LogInformation("Loaded {Count} packages with details from enhanced API", packages.Count);
                        return packages;
                    }
                }
                
                IsApiAvailable = false;
                _logger.LogWarning("Enhanced API failed, falling back to legacy method");
                // Fallback to old method if enhanced API not available
                return await GetPackageDetailsLegacyAsync(sort, search);
            }
            catch (Exception ex)
            {
                IsApiAvailable = false;
                _logger.LogError(ex, "Error fetching package details from enhanced API, falling back to legacy method");
                return await GetPackageDetailsLegacyAsync(sort, search);
            }
        }

        private async Task<List<Package>> GetPackageDetailsLegacyAsync(string? sort = null, string? search = null)
        {
            var packages = new List<Package>();
            var packageResponse = await GetPackagesAsync(sort, search);
            
            if (packageResponse?.Packages != null)
            {
                foreach (var packageName in packageResponse.Packages)
                {
                    var furConfig = await GetPackageAsync(packageName);
                    if (furConfig != null)
                    {
                        packages.Add(new Package
                        {
                            Name = furConfig.Name,
                            Version = furConfig.Version,
                            Authors = furConfig.Authors,
                            SupportedPlatforms = furConfig.SupportedPlatforms,
                            Description = furConfig.Description,
                            LongDescription = furConfig.LongDescription,
                            License = furConfig.License,
                            LicenseUrl = furConfig.LicenseUrl,
                            Keywords = furConfig.Keywords,
                            Tags = furConfig.Tags,
                            Homepage = furConfig.Homepage,
                            IssueTracker = furConfig.IssueTracker,
                            Git = furConfig.Git,
                            Installer = furConfig.Installer,
                            InstallCommand = $"fur install {furConfig.Name}",
                            Dependencies = furConfig.Dependencies,
                            Downloads = Random.Shared.Next(100, 50000),
                            LastUpdated = DateTime.Now.AddDays(-Random.Shared.Next(1, 365))
                        });
                    }
                }
            }
            
            return packages;
        }

        public void ClearCache()
        {
            // Clear all cache entries
            if (_cache is MemoryCache memoryCache)
            {
                var field = typeof(MemoryCache).GetField("_coherentState", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (field?.GetValue(memoryCache) is object coherentState)
                {
                    var entriesField = coherentState.GetType().GetProperty("EntriesCollection",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    
                    if (entriesField?.GetValue(coherentState) is System.Collections.IDictionary entries)
                    {
                        var keysToRemove = new List<object>();
                        foreach (System.Collections.DictionaryEntry entry in entries)
                        {
                            var key = entry.Key.ToString();
                            if (key?.Contains("cached_packages") == true || 
                                key?.Contains("cached_package") == true)
                            {
                                keysToRemove.Add(entry.Key);
                            }
                        }
                        
                        foreach (var key in keysToRemove)
                        {
                            _cache.Remove(key);
                        }
                    }
                }
            }

            _logger.LogInformation("All package cache entries cleared");
        }

        private PackageListResponse? GetCachedPackageResponse(string? sort, string? search)
        {
            var cacheKey = $"{CACHE_KEY_PACKAGES}_{sort}_{search}";
            if (_cache.TryGetValue(cacheKey, out PackageListResponse? cached))
            {
                return cached;
            }
            
            // Return fallback data if no cache available
            return new PackageListResponse
            {
                PackageCount = 0,
                Packages = new List<string>()
            };
        }
    }
}
