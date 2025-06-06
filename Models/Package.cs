using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace furnet.Models
{
    public class Package
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string Version { get; set; } = string.Empty;
        
        [Required]
        public List<string> Authors { get; set; } = new();
        
        public List<string> SupportedPlatforms { get; set; } = new();
        
        public string Description { get; set; } = string.Empty;
        
        public string LongDescription { get; set; } = string.Empty;
        
        public string License { get; set; } = string.Empty;
        
        public string LicenseUrl { get; set; } = string.Empty;
        
        public List<string> Keywords { get; set; } = new();
        
        public List<string> Tags { get; set; } = new();
        
        [Required]
        public string Homepage { get; set; } = string.Empty;
        
        [Required]
        public string IssueTracker { get; set; } = string.Empty;
        
        [Required]
        public string Git { get; set; } = string.Empty;
        
        [Required]
        public string Installer { get; set; } = string.Empty;
        
        [Required]
        public string InstallCommand { get; set; } = string.Empty;
        
        [Required]
        public List<string> Dependencies { get; set; } = new();
        
        // Additional metadata for the website
        public int Downloads { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class FurConfig
    {
        [JsonPropertyName("name")]
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [JsonPropertyName("version")]
        [Required]
        public string Version { get; set; } = string.Empty;
        
        [JsonPropertyName("authors")]
        [Required]
        public List<string> Authors { get; set; } = new();
        
        [JsonPropertyName("Supported_Platforms")]
        public List<string> SupportedPlatforms { get; set; } = new();
        
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
        
        [JsonPropertyName("long_description")]
        public string LongDescription { get; set; } = string.Empty;
        
        [JsonPropertyName("license")]
        public string License { get; set; } = string.Empty;
        
        [JsonPropertyName("license_url")]
        public string LicenseUrl { get; set; } = string.Empty;
        
        [JsonPropertyName("keywords")]
        public List<string> Keywords { get; set; } = new();
        
        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; } = new();
        
        [JsonPropertyName("homepage")]
        [Required]
        public string Homepage { get; set; } = string.Empty;
        
        [JsonPropertyName("issue_tracker")]
        [Required]
        public string IssueTracker { get; set; } = string.Empty;
        
        [JsonPropertyName("git")]
        [Required]
        public string Git { get; set; } = string.Empty;
        
        [JsonPropertyName("installer")]
        [Required]
        public string Installer { get; set; } = string.Empty;
        
        [JsonPropertyName("dependencies")]
        [Required]
        public List<string> Dependencies { get; set; } = new();
    }

    public class PackageListResponse
    {
        [JsonPropertyName("package_count")]
        public int PackageCount { get; set; }
        
        [JsonPropertyName("packages")]
        public List<string> Packages { get; set; } = new();
        
        [JsonPropertyName("package_details")]
        public List<FurConfig>? PackageDetails { get; set; }
    }
}
