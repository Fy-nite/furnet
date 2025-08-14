using System.Threading.Tasks;
using System.Net.Http;

namespace purrnet.api.latest;

// var api = new BaseAPI(app);
// app.MapGet("/Latest", () => api.Latest());
class BaseAPI
{
    public WebApplication App { get; private set; }
    public BaseAPI(WebApplication app = null)
    {
        App = app; // set app if provided, else null
        if (App != null)
        {
            throw new System.Exception("BaseAPI.App cannot be null");
        }
        App.MapGet("/Latest", () => Latest());
    }
    public async Task<string> Latest()
    {
        // Return latest version from GitHub releases
        try
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("PurrInstaller/1.0");
            var response = await httpClient.GetStringAsync("https://api.github.com/repos/Fy-nite/Purr/releases/latest");
            var json = System.Text.Json.JsonDocument.Parse(response);
            var latestVersion = json.RootElement.GetProperty("tag_name").GetString();
            return latestVersion ?? "Unknown";
        }
        catch (System.Exception ex)
        {
            return "v1.0.0"; // Fallback version
        }
    }
}