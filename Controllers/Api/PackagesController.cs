using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Purrnet.Models;
using Purrnet.Services;

namespace Purrnet.Controllers.Api
{
    [ApiController]
    [Route("api/v1/packages")]
    public class PackagesController : ControllerBase
    {
        private readonly ILogger<PackagesController> _logger;
        private readonly IPackageService _packageService;
        private readonly TestingModeService _testingModeService;

        public PackagesController(ILogger<PackagesController> logger, IPackageService packageService, TestingModeService testingModeService)
        {
            _logger = logger;
            _packageService = packageService;
            _testingModeService = testingModeService;
        }

        [HttpGet]
        public async Task<ActionResult<PackageListResponse>> GetPackagesAsync(
            [FromQuery] string? sort = null, 
            [FromQuery] string? search = null,
            [FromQuery] bool details = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                if (_testingModeService.IsTestingMode)
                {
                    Response.Headers.Add("X-Testing-Mode", "true");
                }

                var searchResult = await _packageService.SearchPackagesAsync(search, sort, page, pageSize);
                
                var response = new PackageListResponse
                {
                    PackageCount = searchResult.TotalCount,
                    Packages = searchResult.Packages.Select(p => p.Name).ToList()
                };

                if (details)
                {
                    response.PackageDetails = searchResult.Packages.Select(p => new PurrConfig
                    {
                        Name = p.Name,
                        Version = p.Version,
                        Authors = p.Authors,
                        SupportedPlatforms = p.SupportedPlatforms,
                        Description = p.Description,
                        ReadmeUrl = p.ReadmeUrl,
                        License = p.License,
                        LicenseUrl = p.LicenseUrl,
                        Keywords = p.Keywords,
                        Categories = p.Categories,
                        Homepage = p.Homepage,
                        IssueTracker = p.IssueTracker,
                        Git = p.Git,
                        Installer = p.Installer,
                        Dependencies = p.Dependencies
                    }).ToList();
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting packages");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{packageName}")]
        [HttpGet("{packageName}/{version}")]
        public async Task<ActionResult<PurrConfig>> GetPackageAsync(string packageName, string? version = null)
        {
            try
            {
                if (_testingModeService.IsTestingMode)
                {
                    Response.Headers.Add("X-Testing-Mode", "true");
                }

                var package = await _packageService.GetPackageAsync(packageName, version);
                
                if (package == null)
                    return NotFound($"Package '{packageName}' not found");

                // Increment view count
                await _packageService.IncrementViewCountAsync(package.Id);

                var PurrConfig = new PurrConfig
                {
                    Name = package.Name,
                    Version = package.Version,
                    Authors = package.Authors,
                    SupportedPlatforms = package.SupportedPlatforms,
                    Description = package.Description,
                    ReadmeUrl = package.ReadmeUrl,
                    License = package.License,
                    LicenseUrl = package.LicenseUrl,
                    Keywords = package.Keywords,
                    Categories = package.Categories,
                    Homepage = package.Homepage,
                    IssueTracker = package.IssueTracker,
                    Git = package.Git,
                    Installer = package.Installer,
                    Dependencies = package.Dependencies
                };

                return Ok(PurrConfig);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting package {PackageName}", packageName);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PurrConfig>> UploadPackageAsync([FromBody] PurrConfig PurrConfig)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (_testingModeService.IsTestingMode)
                {
                    Response.Headers.Add("X-Testing-Mode", "true");
                }

                // Get user info from authentication
                var userIdClaim = User.FindFirst("UserId");
                int? userId = null;
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var parsedUserId))
                {
                    userId = parsedUserId;
                }

                var userName = User.Identity?.Name ?? "api-user";
                var success = await _packageService.SavePackageAsync(PurrConfig, userName, userId);
                
                if (!success)
                    return Conflict("Package already exists or failed to upload");

                return CreatedAtAction(nameof(GetPackageAsync), new { packageName = PurrConfig.Name }, PurrConfig);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading package {PackageName}", PurrConfig.Name);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{packageName}/download")]
        public async Task<ActionResult> IncrementDownloadAsync(string packageName)
        {
            try
            {
                var package = await _packageService.GetPackageAsync(packageName);
                if (package == null)
                    return NotFound();

                await _packageService.IncrementDownloadCountAsync(package.Id);
                return Ok(new { message = "Download count incremented" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error incrementing download count for {PackageName}", packageName);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("statistics")]
        public async Task<ActionResult<PackageStatistics>> GetStatisticsAsync()
        {
            try
            {
                if (_testingModeService.IsTestingMode)
                {
                    Response.Headers.Add("X-Testing-Mode", "true");
                }

                var stats = await _packageService.GetStatisticsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting statistics");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("tags")]
        public async Task<ActionResult<List<string>>> GetPopularTagsAsync([FromQuery] int limit = 10)
        {
            try
            {
                var tags = await _packageService.GetPopularTagsAsync(limit);
                return Ok(tags);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting popular tags");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("authors")]
        public async Task<ActionResult<List<string>>> GetPopularAuthorsAsync([FromQuery] int limit = 10)
        {
            try
            {
                var authors = await _packageService.GetPopularAuthorsAsync(limit);
                return Ok(authors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting popular authors");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("categories")]
        public async Task<ActionResult<List<string>>> GetPopularCategoriesAsync([FromQuery] int limit = 10)
        {
            try
            {
                var categories = await _packageService.GetPopularCategoriesAsync(limit);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting popular categories");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("tags/{tag}")]
        public async Task<ActionResult<List<Package>>> GetPackagesByTagAsync(string tag)
        {
            try
            {
                var packages = await _packageService.GetPackagesByTagAsync(tag);
                return Ok(packages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting packages by tag {Tag}", tag);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("authors/{author}")]
        public async Task<ActionResult<List<Package>>> GetPackagesByAuthorAsync(string author)
        {
            try
            {
                var packages = await _packageService.GetPackagesByAuthorAsync(author);
                return Ok(packages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting packages by author {Author}", author);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("categories/{category}")]
        public async Task<ActionResult<List<Package>>> GetPackagesByCategoryAsync(string category)
        {
            try
            {
                var packages = await _packageService.GetPackagesByCategoryAsync(category);
                return Ok(packages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting packages by category {Category}", category);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("cache")]
        public ActionResult ClearCache()
        {
            // For compatibility with package managers expecting this endpoint
            return Ok(new { message = "Cache cleared (using database storage)" });
        }
    }
}
