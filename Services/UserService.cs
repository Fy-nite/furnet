using Microsoft.EntityFrameworkCore;
using furnet.Data;
using furnet.Models;

namespace furnet.Services
{
    public class UserService : IUserService
    {
        private readonly FurDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(FurDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<User?> GetUserByGitHubIdAsync(string gitHubId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.GitHubId == gitHubId);
        }

        public async Task<User> CreateUserAsync(string gitHubId, string username, string email, string avatarUrl)
        {
            var user = new User
            {
                GitHubId = gitHubId,
                Username = username,
                Email = email,
                AvatarUrl = avatarUrl,
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow,
                IsAdmin = false
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created new user {Username} with GitHub ID {GitHubId}", username, gitHubId);
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            user.LastLoginAt = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> IsAdminAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user?.IsAdmin ?? false;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.OrderBy(u => u.Username).ToListAsync();
        }

        public async Task<bool> PromoteToAdminAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    user.IsAdmin = true;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("User {Username} promoted to admin", user.Username);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error promoting user {UserId} to admin", userId);
                return false;
            }
        }

        public async Task<bool> RevokeAdminAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    user.IsAdmin = false;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Admin privileges revoked for user {Username}", user.Username);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking admin privileges for user {UserId}", userId);
                return false;
            }
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<List<Package>> GetUserPackagesAsync(int userId)
        {
            try
            {
                return await _context.Packages
                    .Where(p => p.OwnerId == userId && p.IsActive)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting packages for user {UserId}", userId);
                return new List<Package>();
            }
        }

        public async Task<List<Package>> GetUserMaintainedPackagesAsync(int userId)
        {
            try
            {
                // For now, return packages where user is listed as maintainer
                // This could be expanded with a proper many-to-many relationship
                return await _context.Packages
                    .Where(p => p.IsActive && p.Maintainers.Any(m => m.Id == userId))
                    .OrderByDescending(p => p.LastUpdated)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting maintained packages for user {UserId}", userId);
                return new List<Package>();
            }
        }


        public async Task<bool> MakeFirstUserAdminAsync()
        {
            var userCount = await _context.Users.CountAsync();
            if (userCount == 1)
            {
                var firstUser = await _context.Users.FirstAsync();
                firstUser.IsAdmin = true;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
