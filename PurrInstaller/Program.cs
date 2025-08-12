using System;
using System
    .IO;
using System
    .Linq;
using System
    .Reflection;
using System
    .Threading
    .Tasks;

namespace PurrInstaller
{ 
    public class Program
    {
        private static readonly string PurrApiUrl = "https://purr.finite.ovh/Latest";
        public static async Task<string> GetLatestVersionAsync()
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(PurrApiUrl);
            return response.Trim();
        }

        public static async Task Main(string[] args)
        {
            // download the latest version of Purr
            var latestVersion = await GetLatestVersionAsync();
        }
    }
}
