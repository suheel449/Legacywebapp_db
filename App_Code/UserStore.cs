using System;
using System.Collections.Generic;
using System.Linq;

public class AppUser
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}

/// <summary>
/// In-memory user store (no database / no external backend).
/// Data is lost when the app pool recycles.
/// </summary>
public static class UserStore
{
    private static readonly object Sync = new object();
    private static readonly List<AppUser> Users = new List<AppUser>();

    static UserStore()
    {
        Users.Add(new AppUser
        {
            Name = "Demo User",
            Email = "demo@legacyauth.local",
            Password = "demo123"
        });
        Users.Add(new AppUser
        {
            Name = "Admin User",
            Email = "admin@legacyauth.local",
            Password = "admin123"
        });
    }

    public static bool EmailExists(string email)
    {
        lock (Sync)
        {
            return Users.Any(u =>
                string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
        }
    }

    public static void Register(string name, string email, string password)
    {
        lock (Sync)
        {
            Users.Add(new AppUser
            {
                Name = name,
                Email = email.Trim().ToLowerInvariant(),
                Password = password
            });
        }
    }

    public static AppUser Validate(string email, string password)
    {
        lock (Sync)
        {
            return Users.FirstOrDefault(u =>
                string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase)
                && u.Password == password);
        }
    }
}
