using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.IO.Compression;

namespace PurrInstaller
{ 
    public class Program
    {
        private static readonly string PurrApiUrl = "https://purr.finite.ovh/Latest";
        private static readonly string RepoOwner = "finite"; 
        private static readonly string RepoName = "PurrNet";   
        public static string version = "v1.0.0";
        public static string buildDate = "2025-08-10";
        public static string runtime = ".NET 8.0";
        // ANSI colors
        public static string purple = "\u001b[38;5;135m"; // Deep purple
        public static string lilac = "\u001b[38;5;183m";  // Soft lilac
        public static string reset = "\u001b[0m";
        public static string latestVersion = "";
        public static string splash = $@"
  {purple}/\_/\{reset}   {lilac}Purr {version}{reset}
 {purple}( o.o ){reset}  {lilac}The Finite Package Manager{reset}
  {purple}> ^ <{reset}   {lilac}purrfectly fetching packages{reset}

{lilac}Build:{reset}   {buildDate}
{lilac}Runtime:{reset} {runtime}
";
        public static Dictionary<string, string> options = new Dictionary<string, string>
        {
            { "1", "Install Purr" },
            { "2", "Uninstall Purr" },
            { "3", "Check for updates" },
            { "4", "Exit" }
        };

        public static async Task<int> RunCommandAsync(string command, string args, string? workingDir = null)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = workingDir ?? Environment.CurrentDirectory
                }
            };
            process.Start();
            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();
            process.WaitForExit();
            Console.WriteLine(output);
            if (!string.IsNullOrEmpty(error))
                Console.WriteLine(error);
            return process.ExitCode;
        }

        public static async Task InstallPurrAsync()
        {
            Console.WriteLine("Installing Purr from source...");
            Console.WriteLine("Fetching latest version...");
            Console.WriteLine($"Latest version: {latestVersion}");

            string downloadUrl = $"https://github.com/{RepoOwner}/{RepoName}/archive/refs/tags/{latestVersion}.zip";
            string tempDir = Path.Combine(Path.GetTempPath(), $"purr_{latestVersion}_{Guid.NewGuid()}");
            string zipPath = Path.Combine(tempDir, $"{latestVersion}.zip");
            Directory.CreateDirectory(tempDir);

            try
            {
                // Download zip
                using (var httpClient = new HttpClient())
                {
                    Console.WriteLine($"Downloading source from {downloadUrl}");
                    var data = await httpClient.GetByteArrayAsync(downloadUrl);
                    await File.WriteAllBytesAsync(zipPath, data);
                }

                // Extract zip
                Console.WriteLine("Extracting source...");
                ZipFile.ExtractToDirectory(zipPath, tempDir);

                // Find the extracted folder
                string extractedDir = Directory.GetDirectories(tempDir).First();
                string csproj = Directory.GetFiles(extractedDir, "*.csproj", SearchOption.AllDirectories).FirstOrDefault();
                if (csproj == null)
                {
                    Console.WriteLine("Could not find a .csproj file in the extracted source.");
                    return;
                }

                // Build
                Console.WriteLine("Building Purr...");
                int buildExit = await RunCommandAsync("dotnet", $"build \"{csproj}\" -c Release", extractedDir);
                if (buildExit == 0)
                {
                    Console.WriteLine("Build succeeded!");
                    // Optionally, copy the built executable to a location in PATH
                    string binDir = Path.Combine(Path.GetDirectoryName(csproj)!, "bin", "Release");
                    string exePath = Directory.GetFiles(binDir, "purr*.dll", SearchOption.AllDirectories).FirstOrDefault();
                    if (exePath != null)
                    {
                        string targetDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".purr");
                        Directory.CreateDirectory(targetDir);
                        string targetPath = Path.Combine(targetDir, Path.GetFileName(exePath));
                        File.Copy(exePath, targetPath, true);
                        Console.WriteLine($"Purr installed to {targetPath}");
                        Console.WriteLine("Add this directory to your PATH to use 'purr' globally.");
                    }
                    else
                    {
                        Console.WriteLine("Could not find built Purr executable.");
                    }
                }
                else
                {
                    Console.WriteLine("Build failed!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during install: {ex.Message}");
            }
            finally
            {
                // Clean up temp files if desired
                // Directory.Delete(tempDir, true);
            }
        }

        public static async Task UninstallPurrAsync()
        {
            Console.WriteLine("Uninstalling Purr...");
            string targetDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".purr");
            if (Directory.Exists(targetDir))
            {
                foreach (var file in Directory.GetFiles(targetDir, "purr*.dll"))
                {
                    File.Delete(file);
                }
                Console.WriteLine("Purr uninstalled from user directory.");
            }
            else
            {
                Console.WriteLine("Purr is not installed in the user directory.");
            }
        }

        public static async Task UpdatePurrAsync()
        {
            Console.WriteLine("Updating Purr...");
            await UninstallPurrAsync();
            await InstallPurrAsync();
        }

        static void TypeText(string text, double delayMs)
        {
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep((int)delayMs);
            }
            Console.WriteLine();
        }

        public static async Task<string> GetLatestVersionAsync()
        {
            try
            {
                using var httpClient = new HttpClient();
                var response = await httpClient.GetStringAsync(PurrApiUrl);
                return response.Trim();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching latest version: {ex.Message}");
                return "Unknown";
            }
        }

        public static async Task Main(string[] args)
        {
            latestVersion = await GetLatestVersionAsync();
            double charsPerSecond = 110.0;
            double delay = 100.0 / charsPerSecond;
            TypeText(splash, delay);

            Console.WriteLine($"Latest Purr version: {latestVersion}");
            Console.WriteLine();

            while (true)
            {
                Console.WriteLine("Choose an option:");
                foreach (var kvp in options)
                {
                    Console.WriteLine($"{kvp.Key}. {kvp.Value}");
                }
                Console.Write("\nEnter your choice (1-4): ");
                var choice = Console.ReadLine()?.Trim();
                Console.WriteLine();
                switch (choice)
                {
                    case "1":
                        await InstallPurrAsync();
                        break;
                    case "2":
                        await UninstallPurrAsync();
                        break;
                    case "3":
                        await UpdatePurrAsync();
                        break;
                    case "4":
                        Console.WriteLine("Exiting installer. Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.\n");
                        break;
                }
                Console.WriteLine();
            }
        }
    }
}
