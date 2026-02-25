using Domain.Entities;

namespace Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (context.Users.Any()) return;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "demo@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            Name = "Demo User",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        context.Users.Add(user);

        context.AlertDefs.AddRange(
            new AlertDef
            {
                Id = Guid.NewGuid(),
                Name = "Production AWS Alert",
                AwsAccountId = "123456789012",
                MaxBillAmount = 500,
                AlertRecipientEmails = "ops@example.com",
                UserId = user.Id,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            },
            new AlertDef
            {
                Id = Guid.NewGuid(),
                Name = "Dev AWS Alert",
                AwsAccountId = "210987654321",
                MaxBillAmount = 100,
                AlertRecipientEmails = "dev@example.com",
                UserId = user.Id,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            }
        );

        await context.SaveChangesAsync();
    }
}
