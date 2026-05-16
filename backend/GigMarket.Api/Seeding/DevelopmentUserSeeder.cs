using GigMarket.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace GigMarket.Api.Seeding;

public static class DevelopmentUserSeeder
{
    public static async Task SeedAsync(UserManager<User> userManager)
    {
        await CreateUserIfMissing(
            userManager,
            id: Guid.Parse("10000000-0000-0000-0000-000000000001"),
            username: "demo_buyer",
            email: "buyer@gigmarket.test",
            password: "Demo123!");

        await CreateUserIfMissing(
            userManager,
            id: Guid.Parse("10000000-0000-0000-0000-000000000002"),
            username: "startup_buyer",
            email: "startup@gigmarket.test",
            password: "Demo123!");

        await CreateUserIfMissing(
            userManager,
            id: Guid.Parse("10000000-0000-0000-0000-000000000003"),
            username: "alex_dev",
            email: "alex@gigmarket.test",
            password: "Demo123!");

        await CreateUserIfMissing(
            userManager,
            id: Guid.Parse("10000000-0000-0000-0000-000000000004"),
            username: "mia_design",
            email: "mia@gigmarket.test",
            password: "Demo123!");

        await CreateUserIfMissing(
            userManager,
            id: Guid.Parse("10000000-0000-0000-0000-000000000005"),
            username: "nora_video",
            email: "nora@gigmarket.test",
            password: "Demo123!");
    }

    private static async Task CreateUserIfMissing(
        UserManager<User> userManager,
        Guid id,
        string username,
        string email,
        string password)
    {
        var existing = await userManager.FindByEmailAsync(email);

        if (existing is not null)
        {
            Console.WriteLine($"User already exists: {email}");
            return;
        }

        var user = new User
        {
            Id = id,
            UserName = username,
            CustomUsername = username,
            Email = email,
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Failed to create user {email}: {errors}");
        }

        Console.WriteLine($"Created user: {email}");
    }
}