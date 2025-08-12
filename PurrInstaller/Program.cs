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
        public static string version = "v1.0.0";
        public static string buildDate = "2025-08-10";
        public static string runtime = ".NET 8.0";
        // ANSI colors
        public static string purple = "\u001b[38;5;135m"; // Deep purple
        public static string lilac = "\u001b[38;5;183m";  // Soft lilac
        public static string reset = "\u001b[0m";

        public static string splash = $@"
  {purple}/\_/\{reset}   {lilac}Purr {version}{reset}
 {purple}( o.o ){reset}  {lilac}The Finite Package Manager{reset}
  {purple}> ^ <{reset}   {lilac}purrfectly fetching packages{reset}

{lilac}Build:{reset}   {buildDate}
{lilac}Runtime:{reset} {runtime}
";



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
            // download the latest version of Purr
            var latestVersion = await GetLatestVersionAsync();
            // some compLex shit to make it look nice XD
            double charsPerSecond = 110.0;
            double delay = 100.0 / charsPerSecond;
            TypeText(splash, delay);

            Console.WriteLine($"Latest Purr version: {latestVersion}");
            Console.WriteLine();
            Console.WriteLine("where do you want to install purr to?");
        }
    }
}
