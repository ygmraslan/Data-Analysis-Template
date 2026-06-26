using DataAnalysis.Domain.Entities.Identity;
using DataAnalysis.Domain.Entities.Permission;
using DataAnalysis.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DataAnalysis.Persistence.SeedData;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

        try
        {
            await context.Database.MigrateAsync();

            if (!await context.Users.AnyAsync())
            {
                var adminUser = new User
                {
                    FirstName = configuration["SeedData:AdminFirstName"]!,
                    LastName = configuration["SeedData:AdminLastName"]!,
                    Email = configuration["SeedData:AdminEmail"]!,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(configuration["SeedData:AdminPassword"]!),
                    IsActive = true,
                    IsPasswordChangeRequired = false,
                };

                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();
                logger.LogInformation("Admin user created successfully.");
            }

            var adminEmail = configuration["SeedData:AdminEmail"]!;
            var admin = await context.Users.FirstAsync(u => u.Email == adminEmail);
            var allPermissions = await context.Permissions.ToListAsync();
            var assignedPermissionIds = await context.UserPermissions
                .Where(x => x.UserId == admin.Id)
                .Select(x => x.PermissionId)
                .ToListAsync();

            var missingPermissions = allPermissions
                .Where(p => !assignedPermissionIds.Contains(p.Id))
                .Select(p => new UserPermission
                {
                    UserId = admin.Id,
                    PermissionId = p.Id,
                    CreatedDate = DateTime.UtcNow
                }).ToList();

            if (missingPermissions.Any())
            {
                await context.UserPermissions.AddRangeAsync(missingPermissions);
                await context.SaveChangesAsync();
                logger.LogInformation("{Count} new permissions assigned to admin.", missingPermissions.Count);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }
}