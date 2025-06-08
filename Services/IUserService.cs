using furnet.Models;

namespace furnet.Services
{
    public interface IUserService
    {
        Task<User?> GetUserByGitHubIdAsync(string gitHubId);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User> CreateUserAsync(string gitHubId, string username, string email, string avatarUrl);
        Task<User> UpdateUserAsync(User user);
        Task<bool> IsAdminAsync(int userId);
        Task<List<Package>> GetUserPackagesAsync(int userId);
        Task<List<Package>> GetUserMaintainedPackagesAsync(int userId);
        Task<List<User>> GetAllUsersAsync();
        Task<bool> PromoteToAdminAsync(int userId);
        Task<bool> RevokeAdminAsync(int userId);
        Task<bool> MakeFirstUserAdminAsync();
    }
}
