using Microsoft.EntityFrameworkCore;
using furnet.Data;
using furnet.Models;

namespace furnet.Commands
{
    public static class AdminCommand
    {
        public static async Task<int> ExecuteAsync(string[] args)
        {
            if (args.Length < 2 || args[0] != "--admin")
            {
                ShowHelp();
                return 1;
            }

            var command = args[1];
            var connectionString = GetConnectionString();

            var options = new DbContextOptionsBuilder<FurDbContext>()
                .UseSqlite(connectionString)
                .Options;

            using var context = new FurDbContext(options);

            return command.ToLower() switch
            {
                "promote" when args.Length >= 3 => await PromoteUserAsync(context, args[2]),
                "revoke" when args.Length >= 3 => await RevokeAdminAsync(context, args[2]),
                "list" => await ListAdminsAsync(context),
                "list-users" => await ListAllUsersAsync(context),
                _ => ShowHelp()
            };
        }

        private static async Task<int> PromoteUserAsync(FurDbContext context, string username)
        {
            try
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Username == username);
                if (user == null)
                {
                    Console.WriteLine($"❌ User '{username}' not found.");
                    return 1;
                }

                if (user.IsAdmin)
                {
                    Console.WriteLine($"ℹ️  User '{username}' is already an admin.");
                    return 0;
                }

                user.IsAdmin = true;
                await context.SaveChangesAsync();

                Console.WriteLine($"✅ Successfully promoted '{username}' to admin.");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error promoting user: {ex.Message}");
                return 1;
            }
        }

        private static async Task<int> RevokeAdminAsync(FurDbContext context, string username)
        {
            try
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Username == username);
                if (user == null)
                {
                    Console.WriteLine($"❌ User '{username}' not found.");
                    return 1;
                }

                if (!user.IsAdmin)
                {
                    Console.WriteLine($"ℹ️  User '{username}' is not an admin.");
                    return 0;
                }

                user.IsAdmin = false;
                await context.SaveChangesAsync();

                Console.WriteLine($"✅ Successfully revoked admin privileges from '{username}'.");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error revoking admin: {ex.Message}");
                return 1;
            }
        }

        private static async Task<int> ListAdminsAsync(FurDbContext context)
        {
            try
            {
                var admins = await context.Users
                    .Where(u => u.IsAdmin)
                    .OrderBy(u => u.Username)
                    .ToListAsync();

                if (!admins.Any())
                {
                    Console.WriteLine("No admin users found.");
                    return 0;
                }

                Console.WriteLine("Admin users:");
                foreach (var admin in admins)
                {
                    Console.WriteLine($"  👑 {admin.Username} (GitHub ID: {admin.GitHubId})");
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error listing admins: {ex.Message}");
                return 1;
            }
        }

        private static async Task<int> ListAllUsersAsync(FurDbContext context)
        {
            try
            {
                var users = await context.Users
                    .OrderBy(u => u.Username)
                    .ToListAsync();

                if (!users.Any())
                {
                    Console.WriteLine("No users found.");
                    return 0;
                }

                Console.WriteLine("All users:");
                foreach (var user in users)
                {
                    var status = user.IsAdmin ? "👑 Admin" : "👤 User";
                    Console.WriteLine($"  {status} {user.Username} (GitHub ID: {user.GitHubId})");
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error listing users: {ex.Message}");
                return 1;
            }
        }

        private static string GetConnectionString()
        {
            // Try to get from environment variable first, then fallback to default
            return Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? "Data Source=furnet.db";
        }

        private static int ShowHelp()
        {
            Console.WriteLine("FurNet Admin Management Tool");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  dotnet run -- --admin <command> [options]");
            Console.WriteLine();
            Console.WriteLine("Commands:");
            Console.WriteLine("  promote <username>   Promote a user to admin");
            Console.WriteLine("  revoke <username>    Revoke admin privileges from a user");
            Console.WriteLine("  list                 List all admin users");
            Console.WriteLine("  list-users          List all users");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  dotnet run -- --admin promote john_doe");
            Console.WriteLine("  dotnet run -- --admin list");
            Console.WriteLine("  dotnet run -- --admin list-users");
            Console.WriteLine();
            return 1;
        }
    }
}
