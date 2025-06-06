using Microsoft.AspNetCore.Mvc;
using furnet.Models;
using furnet.Services;

namespace furnet.Controllers.Api
{
    [ApiController]
    [Route("api/v1/packages")]
    public class PackagesController : ControllerBase
    {
        private readonly ILogger<PackagesController> _logger;
        private readonly IFurApiService _furApiService;

        public PackagesController(ILogger<PackagesController> logger, IFurApiService furApiService)
        {
            _logger = logger;
            _furApiService = furApiService;
        }

        [HttpGet("{packageName}/{version?}")]
        public async Task<ActionResult<FurConfig>> GetPackageAsync(string packageName, string? version = null)
        {
            try
            {
                var furConfig = await _furApiService.GetPackageAsync(packageName, version);
                
                if (furConfig == null)
                    return NotFound($"Package '{packageName}' not found");

                return Ok(furConfig);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting package {PackageName}", packageName);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public async Task<ActionResult<PackageListResponse>> GetPackagesAsync([FromQuery] string? sort = null, [FromQuery] string? search = null)
        {
            try
            {
                var response = await _furApiService.GetPackagesAsync(sort, search);
                
                if (response == null)
                    return StatusCode(500, "Failed to retrieve packages");

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting packages");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<FurConfig>> UploadPackageAsync([FromBody] FurConfig furConfig)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var success = await _furApiService.UploadPackageAsync(furConfig);
                
                if (!success)
                    return StatusCode(500, "Failed to upload package");

                return CreatedAtAction(nameof(GetPackageAsync), new { packageName = furConfig.Name }, furConfig);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading package {PackageName}", furConfig.Name);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("health")]
        public async Task<ActionResult> HealthCheckAsync()
        {
            try
            {
                var isHealthy = await _furApiService.IsApiHealthyAsync();
                return Ok(new { status = isHealthy ? "healthy" : "unhealthy" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed");
                return StatusCode(500, "Health check failed");
            }
        }

    }
}
