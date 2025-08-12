using Purrnet.Models;
using Purrnet.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Purrnet.Services
{
    public class PackageService : IPackageService
    {
        private readonly PurrDbContext _context;
        private readonly ILogger<PackageService> _logger;

        public PackageService(PurrDbContext context, ILogger<PackageService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Package>> GetAllPackagesAsync()
        {
            return await _context.Packages
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Package?> GetPackageAsync(string packageName, string? version = null)
        {
            var query = _context.Packages.Where(p => p.Name == packageName && p.IsActive);
            
            if (!string.IsNullOrEmpty(version))
            {
                query = query.Where(p => p.Version == version);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<Package?> GetPackageByIdAsync(int id)
        {
            return await _context.Packages.FindAsync(id);
        }

        public async Task<bool> SavePackageAsync(PurrConfig PurrConfig, string createdBy, int? ownerId = null)
        {
            try
            {
                // Check if package already exists
                var existingPackage = await _context.Packages
                    .FirstOrDefaultAsync(p => p.Name == PurrConfig.Name);

                if (existingPackage != null)
                {
                    _logger.LogWarning("Package {PackageName} already exists", PurrConfig.Name);
                    return false;
                }

                var package = new Package
                {
                    Name = PurrConfig.Name,
                    Version = PurrConfig.Version,
                    Authors = PurrConfig.Authors ?? new List<string>(),
                    SupportedPlatforms = PurrConfig.SupportedPlatforms ?? new List<string>(),
                    Description = PurrConfig.Description ?? string.Empty,
                    ReadmeUrl = PurrConfig.ReadmeUrl ?? string.Empty,
                    License = PurrConfig.License ?? string.Empty,
                    LicenseUrl = PurrConfig.LicenseUrl ?? string.Empty,
                    Keywords = PurrConfig.Keywords ?? new List<string>(),
                    Homepage = PurrConfig.Homepage ?? string.Empty,
                    IssueTracker = PurrConfig.IssueTracker ?? string.Empty,
                    Git = PurrConfig.Git,
                    Installer = PurrConfig.Installer ?? string.Empty,
                    Dependencies = PurrConfig.Dependencies ?? new List<string>(),
                    InstallCommand = $"Purr install {PurrConfig.Name}",
                    Downloads = 0,
                    ViewCount = 0,
                    CreatedAt = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    OwnerId = ownerId,
                    IsActive = true,
                    ApprovalStatus = "Pending"
                };

                _context.Packages.Add(package);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Package {PackageName} saved successfully by {CreatedBy} (Owner ID: {OwnerId})", 
                    PurrConfig.Name, createdBy, ownerId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving package {PackageName}", PurrConfig.Name);
                return false;
            }
        }

        // Overload for backward compatibility
        public async Task<bool> SavePackageAsync(PurrConfig PurrConfig, string createdBy)
        {
            return await SavePackageAsync(PurrConfig, createdBy, null);
        }

        public async Task<bool> UpdatePackageAsync(int id, PurrConfig PurrConfig, string? updatedBy = null)
        {
            try
            {
                var package = await _context.Packages.FindAsync(id);
                if (package == null)
                {
                    return false;
                }

                package.Version = PurrConfig.Version;
                package.Authors = PurrConfig.Authors ?? new List<string>();
                package.SupportedPlatforms = PurrConfig.SupportedPlatforms ?? new List<string>();
                package.Description = PurrConfig.Description ?? string.Empty;
                package.ReadmeUrl = PurrConfig.ReadmeUrl ?? string.Empty;
                package.License = PurrConfig.License ?? string.Empty;
                package.LicenseUrl = PurrConfig.LicenseUrl ?? string.Empty;
                package.Keywords = PurrConfig.Keywords ?? new List<string>();
                package.Homepage = PurrConfig.Homepage ?? string.Empty;
                package.IssueTracker = PurrConfig.IssueTracker ?? string.Empty;
                package.Git = PurrConfig.Git;
                package.Installer = PurrConfig.Installer ?? string.Empty;
                package.Dependencies = PurrConfig.Dependencies ?? new List<string>();
                package.LastUpdated = DateTime.UtcNow;
                package.UpdatedBy = updatedBy;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating package {PackageId}", id);
                return false;
            }
        }

        public async Task<bool> DeletePackageAsync(int id)
        {
            try
            {
                var package = await _context.Packages.FindAsync(id);
                if (package == null)
                {
                    return false;
                }

                _context.Packages.Remove(package);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting package {PackageId}", id);
                return false;
            }
        }

        public async Task<bool> TogglePackageStatusAsync(int id)
        {
            try
            {
                var package = await _context.Packages.FindAsync(id);
                if (package == null)
                {
                    return false;
                }

                package.IsActive = !package.IsActive;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling package status {PackageId}", id);
                return false;
            }
        }

        public async Task<SearchResult> SearchPackagesAsync(string? query = null, string? sort = null, int page = 1, int pageSize = 20)
        {
            var queryable = _context.Packages.Where(p => p.IsActive);

            List<Package> filteredPackages;

            // Apply search filter (client-side for case-insensitive search)
            if (!string.IsNullOrEmpty(query))
            {
                var searchTerm = query;
                filteredPackages = await queryable.ToListAsync();
                filteredPackages = filteredPackages.Where(p =>
                    (!string.IsNullOrEmpty(p.Name) && p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(p.Description) && p.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (p.Authors != null && p.Authors.Any(a => !string.IsNullOrEmpty(a) && a.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))) ||
                    (p.Keywords != null && p.Keywords.Any(k => !string.IsNullOrEmpty(k) && k.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))) ||
                    (p.Categories != null && p.Categories.Any(c => !string.IsNullOrEmpty(c) && c.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
                ).ToList();
            }
            else
            {
                filteredPackages = await queryable.ToListAsync();
            }

            // Apply sorting (client-side)
            IEnumerable<Package> sortedPackages = sort?.ToLower() switch
            {
                "mostdownloads" => filteredPackages.OrderByDescending(p => p.Downloads),
                "leastdownloads" => filteredPackages.OrderBy(p => p.Downloads),
                "recentlyupdated" => filteredPackages.OrderByDescending(p => p.LastUpdated),
                "recentlyuploaded" => filteredPackages.OrderByDescending(p => p.CreatedAt),
                "oldestupdated" => filteredPackages.OrderBy(p => p.LastUpdated),
                "oldestuploaded" => filteredPackages.OrderBy(p => p.CreatedAt),
                "mostviewed" => filteredPackages.OrderByDescending(p => p.ViewCount),
                "toprated" => filteredPackages.OrderByDescending(p => p.Rating),
                _ => filteredPackages.OrderBy(p => p.Name)
            };

            var totalCount = sortedPackages.Count();
            var packages = sortedPackages
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new SearchResult
            {
                Packages = packages,
                TotalCount = totalCount,
                Query = query ?? string.Empty
            };
        }

        public async Task<PackageListResponse> GetPackageListAsync(string? sort = null, string? search = null, bool includeDetails = false)
        {
            var searchResult = await SearchPackagesAsync(search, sort, 1, 1000);

            var response = new PackageListResponse
            {
                PackageCount = searchResult.TotalCount,
                Packages = searchResult.Packages.Select(p => p.Name).ToList()
            };

            if (includeDetails)
            {
                response.PackageDetails = searchResult.Packages.Select(p => new PurrConfig
                {
                    Name = p.Name,
                    Version = p.Version,
                    Authors = p.Authors,
                    Description = p.Description,
                    Keywords = p.Keywords,
                    Homepage = p.Homepage,
                    IssueTracker = p.IssueTracker,
                    Git = p.Git,
                    Installer = p.Installer,
                    Dependencies = p.Dependencies
                }).ToList();
            }

            return response;
        }

        public async Task<PackageStatistics> GetStatisticsAsync()
        {
            var packages = await _context.Packages.ToListAsync();
            var activePackages = packages.Where(p => p.IsActive).ToList();

            return new PackageStatistics
            {
                TotalPackages = packages.Count,
                ActivePackages = activePackages.Count,
                TotalDownloads = activePackages.Sum(p => p.Downloads),
                TotalViews = activePackages.Sum(p => p.ViewCount),
                PopularAuthors = packages.Where(p => p.Authors != null && p.Authors.Any())
                    .SelectMany(p => p.Authors.Select(a => a.Trim()))
                    .GroupBy(author => author)
                    .OrderByDescending(g => g.Count())
                    .Take(20) 
                    .Select(g => g.Key)
                    .ToList(),
                MostDownloaded = activePackages.OrderByDescending(p => p.Downloads).Take(5).ToList(),
                RecentlyAdded = activePackages.OrderByDescending(p => p.CreatedAt).Take(5).ToList(),
                LastUpdated = DateTime.UtcNow
            };
        }

        public async Task<bool> IncrementDownloadCountAsync(int packageId)
        {
            try
            {
                var package = await _context.Packages.FindAsync(packageId);
                if (package != null)
                {
                    package.Downloads++;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error incrementing download count for package {PackageId}", packageId);
                return false;
            }
        }

        public async Task<bool> IncrementViewCountAsync(int packageId)
        {
            try
            {
                var package = await _context.Packages.FindAsync(packageId);
                if (package != null)
                {
                    package.ViewCount++;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error incrementing view count for package {PackageId}", packageId);
                return false;
            }
        }

        public async Task<bool> InitializeDatabaseAsync()
        {
            try
            {
                await _context.Database.EnsureCreatedAsync();
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing database");
                return false;
            }
        }

        

        public async Task<List<Package>> GetPackagesByTagAsync(string tag)
        {
            return await _context.Packages
                .Where(p => p.IsActive && p.Keywords.Contains(tag))
                .OrderByDescending(p => p.Downloads)
                .ToListAsync();
        }

        public async Task<List<Package>> GetPackagesByAuthorAsync(string author)
        {
            return await _context.Packages
                .Where(p => p.IsActive && p.Authors.Contains(author))
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<string>> GetPopularTagsAsync(int limit = 10)
        {
            var packages = await _context.Packages.Where(p => p.IsActive).ToListAsync();
            return packages
                .SelectMany(p => p.Keywords)
                .GroupBy(t => t)
                .OrderByDescending(g => g.Count())
                .Take(limit)
                .Select(g => g.Key)
                .ToList();
        }

        public async Task<List<string>> GetPopularAuthorsAsync(int limit = 10)
        {
            var packages = await _context.Packages.Where(p => p.IsActive).ToListAsync();
            return packages
                .SelectMany(p => p.Authors)
                .GroupBy(a => a)
                .OrderByDescending(g => g.Count())
                .Take(limit)
                .Select(g => g.Key)
                .ToList();
        }

        public async Task<bool> ClearAllDataAsync()
        {
            try
            {
                _context.Packages.RemoveRange(_context.Packages);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing all data");
                return false;
            }
        }

        public async Task<bool> ImportPackagesFromJsonAsync(string jsonFilePath)
        {
            try
            {
                if (!File.Exists(jsonFilePath))
                {
                    return false;
                }

                var json = await File.ReadAllTextAsync(jsonFilePath);
                var packages = JsonSerializer.Deserialize<List<PurrConfig>>(json);

                if (packages != null)
                {
                    foreach (var package in packages)
                    {
                        await SavePackageAsync(package, "import");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing packages from JSON");
                return false;
            }
        }

        public async Task<bool> ExportPackagesToJsonAsync(string jsonFilePath)
        {
            try
            {
                var packages = await GetAllPackagesAsync();
                var PurrConfigs = packages.Select(p => new PurrConfig
                {
                    Name = p.Name,
                    Version = p.Version,
                    Authors = p.Authors,
                    Description = p.Description,
                    Keywords = p.Keywords,
                    Categories = p.Categories,
                    Homepage = p.Homepage,
                    IssueTracker = p.IssueTracker,
                    Git = p.Git,
                    Installer = p.Installer,
                    Dependencies = p.Dependencies
                }).ToList();

                var json = JsonSerializer.Serialize(PurrConfigs, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(jsonFilePath, json);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting packages to JSON");
                return false;
            }
        }

        public async Task<int> GetPackageCountAsync()
        {
            return await _context.Packages.CountAsync(p => p.IsActive);
        }

        public async Task<bool> ClearAllDataAsync()
        {
            try
            {
                var packages = await _context.Packages.ToListAsync();
                _context.Packages.RemoveRange(packages);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Cleared all package data");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing all package data");
                return false;
            }
        }

        public async Task<bool> ImportPackagesFromJsonAsync(string jsonFilePath)
        {
            try
            {
                if (!File.Exists(jsonFilePath))
                {
                    _logger.LogError("JSON file not found: {FilePath}", jsonFilePath);
                    return false;
                }

                var json = await File.ReadAllTextAsync(jsonFilePath);
                var furConfigs = JsonSerializer.Deserialize<List<FurConfig>>(json);

                if (furConfigs == null || !furConfigs.Any())
                {
                    _logger.LogWarning("No packages found in JSON file: {FilePath}", jsonFilePath);
                    return true;
                }

                int importedCount = 0;
                foreach (var furConfig in furConfigs)
                {
                    if (await SavePackageAsync(furConfig, "import"))
                    {
                        importedCount++;
                    }
                }

                _logger.LogInformation("Imported {ImportedCount} out of {TotalCount} packages from {FilePath}", 
                    importedCount, furConfigs.Count, jsonFilePath);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing packages from JSON file: {FilePath}", jsonFilePath);
                return false;
            }
        }
    }
}