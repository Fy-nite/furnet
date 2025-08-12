using Microsoft.AspNetCore.Mvc;
using Purrnet.Services;

namespace Purrnet.Controllers.Api
{
    [ApiController]
    [Route("api/v1")]
    public class HealthController : ControllerBase
    {
        private readonly IPackageService _packageService;
        private readonly TestingModeService _testingModeService;

        public HealthController(IPackageService packageService, TestingModeService testingModeService)
        {
            _packageService = packageService;
            _testingModeService = testingModeService;
        }

        [HttpGet("health")]
        public async Task<ActionResult> GetHealthAsync()
        {
            try
            {
                // Test database connectivity
                var packageCount = await _packageService.GetPackageCountAsync();
                
                var health = new
                {
                    status = "healthy",
                    timestamp = DateTime.UtcNow,
                    version = "1.0.0",
                    packageCount = packageCount,
                    testingMode = _testingModeService.IsTestingMode,
                    database = "sqlite",
                    uptime = Environment.TickCount64
                };

                return Ok(health);
            }
            catch (Exception ex)
            {
                var health = new
                {
                    status = "unhealthy",
                    timestamp = DateTime.UtcNow,
                    error = ex.Message,
                    testingMode = _testingModeService.IsTestingMode
                };

                return StatusCode(503, health);
            }
        }

        [HttpGet("info")]
        public ActionResult GetApiInfo()
        {
            var info = new
            {
                name = "PurrNet Package Repository API",
                version = "1.0.0",
                description = "API for Purr package manager",
                endpoints = new
                {
                    packages = "/api/v1/packages",
                    health = "/api/v1/health",
                    statistics = "/api/v1/packages/statistics",
                    tags = "/api/v1/packages/tags",
                    authors = "/api/v1/packages/authors"
                },
                testingMode = _testingModeService.IsTestingMode,
                documentation = "See /api.md for detailed API documentation"
            };

            return Ok(info);
        }
    }
}
