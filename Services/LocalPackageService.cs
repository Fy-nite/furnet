using furnet.Models;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace furnet.Services
{
    public class LocalPackageService : ILocalPackageService
    {
        private readonly ILogger<LocalPackageService> _logger;
        private readonly string _packagesDirectory;
        private readonly JsonSerializerOptions _jsonOptions;

        public LocalPackageService(ILogger<LocalPackageService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _packagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "LocalPackages");
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                WriteIndented = true
            };
        }

        public async Task InitializeAsync()
        {
            try
            {
                if (!Directory.Exists(_packagesDirectory))
                {
                    Directory.CreateDirectory(_packagesDirectory);
                    await CreateDefaultPackagesAsync();
                }
                _logger.LogInformation("Local package service initialized at {Directory}", _packagesDirectory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize local package service");
            }
        }

        public async Task<List<FurConfig>> GetAllPackagesAsync()
        {
            var packages = new List<FurConfig>();

            try
            {
                if (!Directory.Exists(_packagesDirectory))
                    return packages;

                var packageDirs = Directory.GetDirectories(_packagesDirectory);
                
                foreach (var packageDir in packageDirs)
                {
                    var configPath = Path.Combine(packageDir, "furconfig.json");
                    if (File.Exists(configPath))
                    {
                        var jsonContent = await File.ReadAllTextAsync(configPath);
                        var package = JsonSerializer.Deserialize<FurConfig>(jsonContent, _jsonOptions);
                        if (package != null)
                        {
                            packages.Add(package);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading packages from local storage");
            }

            return packages;
        }

        public async Task<FurConfig?> GetPackageAsync(string packageName, string? version = null)
        {
            try
            {
                var sanitizedName = SanitizePackageName(packageName);
                var packageDir = Path.Combine(_packagesDirectory, sanitizedName);
                var configPath = Path.Combine(packageDir, "furconfig.json");

                if (!File.Exists(configPath))
                    return null;

                var jsonContent = await File.ReadAllTextAsync(configPath);
                var package = JsonSerializer.Deserialize<FurConfig>(jsonContent, _jsonOptions);

                if (package != null && (version == null || package.Version == version))
                {
                    return package;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading package {PackageName} from local storage", packageName);
            }

            return null;
        }

        public async Task<bool> SavePackageAsync(FurConfig furConfig)
        {
            try
            {
                var sanitizedName = SanitizePackageName(furConfig.Name);
                var packageDir = Path.Combine(_packagesDirectory, sanitizedName);
                
                if (!Directory.Exists(packageDir))
                {
                    Directory.CreateDirectory(packageDir);
                }

                var configPath = Path.Combine(packageDir, "furconfig.json");
                var jsonContent = JsonSerializer.Serialize(furConfig, _jsonOptions);
                
                await File.WriteAllTextAsync(configPath, jsonContent);
                
                _logger.LogInformation("Package {PackageName} saved to local storage", furConfig.Name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving package {PackageName} to local storage", furConfig.Name);
                return false;
            }
        }

        public async Task<bool> DeletePackageAsync(string packageName)
        {
            try
            {
                var sanitizedName = SanitizePackageName(packageName);
                var packageDir = Path.Combine(_packagesDirectory, sanitizedName);
                
                if (Directory.Exists(packageDir))
                {
                    Directory.Delete(packageDir, true);
                    _logger.LogInformation("Package {PackageName} deleted from local storage", packageName);
                    return true;
                }
                
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting package {PackageName} from local storage", packageName);
                return false;
            }
        }

        public async Task<PackageListResponse> GetPackageListAsync(string? sort = null, string? search = null, bool includeDetails = false)
        {
            var allPackages = await GetAllPackagesAsync();
            var filteredPackages = allPackages.AsEnumerable();

            // Apply search filter
            if (!string.IsNullOrEmpty(search))
            {
                var searchLower = search.ToLower();
                filteredPackages = filteredPackages.Where(p =>
                    p.Name.ToLower().Contains(searchLower) ||
                    p.Description.ToLower().Contains(searchLower) ||
                    p.Authors.Any(a => a.ToLower().Contains(searchLower)) ||
                    p.Keywords.Any(k => k.ToLower().Contains(searchLower))
                );
            }

            // Apply sorting
            filteredPackages = sort?.ToLower() switch
            {
                "mostdownloads" => filteredPackages.OrderByDescending(p => p.Name), // Placeholder
                "leastdownloads" => filteredPackages.OrderBy(p => p.Name), // Placeholder
                "recentlyupdated" => filteredPackages.OrderByDescending(p => p.Version),
                "recentlyuploaded" => filteredPackages.OrderByDescending(p => p.Name),
                "oldestupdated" => filteredPackages.OrderBy(p => p.Version),
                "oldestuploaded" => filteredPackages.OrderBy(p => p.Name),
                _ => filteredPackages.OrderBy(p => p.Name)
            };

            var resultList = filteredPackages.ToList();

            var response = new PackageListResponse
            {
                PackageCount = resultList.Count,
                Packages = resultList.Select(p => p.Name).ToList()
            };

            if (includeDetails)
            {
                response.PackageDetails = resultList;
            }

            return response;
        }

        public Task<bool> IncrementViewCountAsync(int packageId)
        {
            // Since we're using local file storage, we can't track view counts
            // Return true for compatibility
            return Task.FromResult(true);
        }

        private async Task CreateDefaultPackagesAsync()
        {
            var defaultPackages = new List<FurConfig>
            {
                new FurConfig
                {
                    Name = "web-framework",
                    Version = "2.1.0",
                    Authors = new List<string> { "devuser1", "contributor2" },
                    SupportedPlatforms = new List<string> { "linux", "macos", "windows" },
                    Description = "Lightweight web framework",
                    License = "MIT",
                    LicenseUrl = "https://opensource.org/license/mit/",
                    Keywords = new List<string> { "web", "framework", "http" },
                    Homepage = "https://example.com/web-framework",
                    IssueTracker = "https://github.com/devuser1/web-framework/issues",
                    Git = "https://github.com/devuser1/web-framework.git",
                    Installer = "install.sh",
                    Dependencies = new List<string> { "make@latest", "gcc@9.0.0" }
                },
                new FurConfig
                {
                    Name = "json-parser",
                    Version = "1.5.3",
                    Authors = new List<string> { "jsondev" },
                    SupportedPlatforms = new List<string> { "linux", "macos" },
                    Description = "Fast JSON parsing library",
                    License = "Apache-2.0",
                    LicenseUrl = "https://www.apache.org/licenses/LICENSE-2.0",
                    Keywords = new List<string> { "json", "parser", "streaming" },
                    Homepage = "https://example.com/json-parser",
                    IssueTracker = "https://github.com/jsondev/json-parser/issues",
                    Git = "https://github.com/jsondev/json-parser.git",
                    Installer = "setup.py",
                    Dependencies = new List<string> { "make@latest" }
                }
            };

            foreach (var package in defaultPackages)
            {
                await SavePackageAsync(package);
            }

            _logger.LogInformation("Created {Count} default packages", defaultPackages.Count);
        }

        private static string SanitizePackageName(string packageName)
        {
            var sanitized = Regex.Replace(packageName, @"[<>:""/\\|?*]", "_");
            sanitized = Regex.Replace(sanitized, @"_+", "_");
            return sanitized.Trim('_');
        }
    }
}
