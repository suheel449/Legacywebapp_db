using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

public class AppUser
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string RoleName { get; set; }
    public bool IsActive { get; set; }
}

public class UserPreferences
{
    public bool EmailAlerts { get; set; }
    public bool WeeklyDigest { get; set; }
    public bool CompactNav { get; set; }
}

public class ReportItem
{
    public string Title { get; set; }
    public string OwnerTeam { get; set; }
    public string PeriodLabel { get; set; }
    public string Status { get; set; }
}

public class NotificationItem
{
    public int NotificationId { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public bool IsRead { get; set; }
}

public class ActivityItem
{
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class DashboardStats
{
    public int OpenTasks { get; set; }
    public int TasksDueThisWeek { get; set; }
    public int ReportCount { get; set; }
    public int NotificationCount { get; set; }
    public int UnreadNotifications { get; set; }
    public int TeamMembers { get; set; }
}

/// <summary>
/// MySQL data access for all Legacy Auth pages.
/// </summary>
public static class AppDb
{
    private static string ConnectionString
    {
        get { return ConfigurationManager.ConnectionStrings["LegacyBizDb"].ConnectionString; }
    }

    public static bool EmailExists(string email)
    {
        const string sql = "SELECT COUNT(1) FROM Users WHERE Email = @Email";
        using (MySqlConnection conn = new MySqlConnection(ConnectionString))
        using (MySqlCommand cmd = new MySqlCommand(sql, conn))
        {
            cmd.Parameters.Add("@Email", MySqlDbType.VarChar, 150).Value = email.Trim().ToLowerInvariant();
            conn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }
    }

    public static AppUser Validate(string email, string password)
    {
        const string sql = @"
            SELECT UserId, FullName, Email, RoleName, IsActive
            FROM Users
            WHERE Email = @Email AND PasswordHash = @Password AND IsActive = 1";

        using (MySqlConnection conn = new MySqlConnection(ConnectionString))
        using (MySqlCommand cmd = new MySqlCommand(sql, conn))
        {
            cmd.Parameters.Add("@Email", MySqlDbType.VarChar, 150).Value = email.Trim().ToLowerInvariant();
            cmd.Parameters.Add("@Password", MySqlDbType.VarChar, 100).Value = password ?? string.Empty;
            conn.Open();
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (!reader.Read())
                {
                    return null;
                }

                return ReadUser(reader);
            }
        }
    }

    public static AppUser Register(string name, string email, string password)
    {
        string normalizedEmail = email.Trim().ToLowerInvariant();

        using (MySqlConnection conn = new MySqlConnection(ConnectionString))
        {
            conn.Open();
            using (MySqlTransaction tx = conn.BeginTransaction())
            {
                const string insertUser = @"
                    INSERT INTO Users (FullName, Email, PasswordHash, RoleName)
                    VALUES (@FullName, @Email, @Password, 'Member')";

                int userId;
                using (MySqlCommand cmd = new MySqlCommand(insertUser, conn, tx))
                {
                    cmd.Parameters.Add("@FullName", MySqlDbType.VarChar, 100).Value = name.Trim();
                    cmd.Parameters.Add("@Email", MySqlDbType.VarChar, 150).Value = normalizedEmail;
                    cmd.Parameters.Add("@Password", MySqlDbType.VarChar, 100).Value = password;
                    cmd.ExecuteNonQuery();
                    userId = Convert.ToInt32(cmd.LastInsertedId);
                }

                const string insertPrefs = @"
                    INSERT INTO UserPreferences (UserId, EmailAlerts, WeeklyDigest, CompactNav)
                    VALUES (@UserId, 1, 1, 0)";
                using (MySqlCommand cmd = new MySqlCommand(insertPrefs, conn, tx))
                {
                    cmd.Parameters.Add("@UserId", MySqlDbType.Int32).Value = userId;
                    cmd.ExecuteNonQuery();
                }

                const string insertNotice = @"
                    INSERT INTO Notifications (UserId, Title, Body, IsRead)
                    VALUES (@UserId, 'Welcome to Legacy Auth', 'Your account was created and stored in MySQL.', 0)";
                using (MySqlCommand cmd = new MySqlCommand(insertNotice, conn, tx))
                {
                    cmd.Parameters.Add("@UserId", MySqlDbType.Int32).Value = userId;
                    cmd.ExecuteNonQuery();
                }

                const string insertActivity = @"
                    INSERT INTO ActivityLog (UserId, Description)
                    VALUES (@UserId, 'Account registered')";
                using (MySqlCommand cmd = new MySqlCommand(insertActivity, conn, tx))
                {
                    cmd.Parameters.Add("@UserId", MySqlDbType.Int32).Value = userId;
                    cmd.ExecuteNonQuery();
                }

                tx.Commit();

                return new AppUser
                {
                    UserId = userId,
                    Name = name.Trim(),
                    Email = normalizedEmail,
                    RoleName = "Member",
                    IsActive = true
                };
            }
        }
    }

    public static AppUser GetUserById(int userId)
    {
        const string sql = @"
            SELECT UserId, FullName, Email, RoleName, IsActive
            FROM Users
            WHERE UserId = @UserId";

        using (MySqlConnection conn = new MySqlConnection(ConnectionString))
        using (MySqlCommand cmd = new MySqlCommand(sql, conn))
        {
            cmd.Parameters.Add("@UserId", MySqlDbType.Int32).Value = userId;
            conn.Open();
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (!reader.Read())
                {
                    return null;
                }

                return ReadUser(reader);
            }
        }
    }

    public static void LogActivity(int userId, string description)
    {
        const string sql = @"
            INSERT INTO ActivityLog (UserId, Description)
            VALUES (@UserId, @Description)";

        using (MySqlConnection conn = new MySqlConnection(ConnectionString))
        using (MySqlCommand cmd = new MySqlCommand(sql, conn))
        {
            cmd.Parameters.Add("@UserId", MySqlDbType.Int32).Value = userId;
            cmd.Parameters.Add("@Description", MySqlDbType.VarChar, 250).Value = description;
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public static UserPreferences GetPreferences(int userId)
    {
        const string sql = @"
            SELECT EmailAlerts, WeeklyDigest, CompactNav
            FROM UserPreferences
            WHERE UserId = @UserId";

        using (MySqlConnection conn = new MySqlConnection(ConnectionString))
        using (MySqlCommand cmd = new MySqlCommand(sql, conn))
        {
            cmd.Parameters.Add("@UserId", MySqlDbType.Int32).Value = userId;
            conn.Open();
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (!reader.Read())
                {
                    return new UserPreferences
                    {
                        EmailAlerts = true,
                        WeeklyDigest = true,
                        CompactNav = false
                    };
                }

                return new UserPreferences
                {
                    EmailAlerts = ReadBool(reader, 0),
                    WeeklyDigest = ReadBool(reader, 1),
                    CompactNav = ReadBool(reader, 2)
                };
            }
        }
    }

    public static void SavePreferences(int userId, UserPreferences prefs)
    {
        const string sql = @"
            INSERT INTO UserPreferences (UserId, EmailAlerts, WeeklyDigest, CompactNav)
            VALUES (@UserId, @EmailAlerts, @WeeklyDigest, @CompactNav) AS new
            ON DUPLICATE KEY UPDATE
                EmailAlerts = new.EmailAlerts,
                WeeklyDigest = new.WeeklyDigest,
                CompactNav = new.CompactNav,
                UpdatedAt = UTC_TIMESTAMP()";

        using (MySqlConnection conn = new MySqlConnection(ConnectionString))
        using (MySqlCommand cmd = new MySqlCommand(sql, conn))
        {
            cmd.Parameters.Add("@UserId", MySqlDbType.Int32).Value = userId;
            cmd.Parameters.Add("@EmailAlerts", MySqlDbType.Byte).Value = prefs.EmailAlerts ? 1 : 0;
            cmd.Parameters.Add("@WeeklyDigest", MySqlDbType.Byte).Value = prefs.WeeklyDigest ? 1 : 0;
            cmd.Parameters.Add("@CompactNav", MySqlDbType.Byte).Value = prefs.CompactNav ? 1 : 0;
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public static DashboardStats GetDashboardStats(int userId)
    {
        using (MySqlConnection conn = new MySqlConnection(ConnectionString))
        using (MySqlCommand cmd = new MySqlCommand("usp_GetDashboardStats", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("p_UserId", MySqlDbType.Int32).Value = userId;
            conn.Open();
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (!reader.Read())
                {
                    return new DashboardStats();
                }

                return new DashboardStats
                {
                    OpenTasks = Convert.ToInt32(reader.GetValue(0)),
                    TasksDueThisWeek = Convert.ToInt32(reader.GetValue(1)),
                    ReportCount = Convert.ToInt32(reader.GetValue(2)),
                    NotificationCount = Convert.ToInt32(reader.GetValue(3)),
                    UnreadNotifications = Convert.ToInt32(reader.GetValue(4)),
                    TeamMembers = Convert.ToInt32(reader.GetValue(5))
                };
            }
        }
    }

    public static List<ActivityItem> GetRecentActivity(int userId, int top)
    {
        const string sql = @"
            SELECT Description, CreatedAt
            FROM ActivityLog
            WHERE UserId = @UserId OR UserId IS NULL
            ORDER BY CreatedAt DESC
            LIMIT @Top";

        List<ActivityItem> items = new List<ActivityItem>();
        using (MySqlConnection conn = new MySqlConnection(ConnectionString))
        using (MySqlCommand cmd = new MySqlCommand(sql, conn))
        {
            cmd.Parameters.Add("@UserId", MySqlDbType.Int32).Value = userId;
            cmd.Parameters.Add("@Top", MySqlDbType.Int32).Value = top;
            conn.Open();
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    items.Add(new ActivityItem
                    {
                        Description = reader.GetString(0),
                        CreatedAt = reader.GetDateTime(1)
                    });
                }
            }
        }

        return items;
    }

    public static List<ReportItem> GetReports()
    {
        const string sql = @"
            SELECT Title, OwnerTeam, PeriodLabel, Status
            FROM Reports
            ORDER BY ReportId";

        List<ReportItem> items = new List<ReportItem>();
        using (MySqlConnection conn = new MySqlConnection(ConnectionString))
        using (MySqlCommand cmd = new MySqlCommand(sql, conn))
        {
            conn.Open();
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    items.Add(new ReportItem
                    {
                        Title = reader.GetString(0),
                        OwnerTeam = reader.GetString(1),
                        PeriodLabel = reader.GetString(2),
                        Status = reader.GetString(3)
                    });
                }
            }
        }

        return items;
    }

    public static List<NotificationItem> GetNotifications(int userId)
    {
        const string sql = @"
            SELECT NotificationId, Title, Body, IsRead
            FROM Notifications
            WHERE UserId IS NULL OR UserId = @UserId
            ORDER BY IsRead ASC, CreatedAt DESC";

        List<NotificationItem> items = new List<NotificationItem>();
        using (MySqlConnection conn = new MySqlConnection(ConnectionString))
        using (MySqlCommand cmd = new MySqlCommand(sql, conn))
        {
            cmd.Parameters.Add("@UserId", MySqlDbType.Int32).Value = userId;
            conn.Open();
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    items.Add(new NotificationItem
                    {
                        NotificationId = reader.GetInt32(0),
                        Title = reader.GetString(1),
                        Body = reader.GetString(2),
                        IsRead = ReadBool(reader, 3)
                    });
                }
            }
        }

        return items;
    }

    private static AppUser ReadUser(MySqlDataReader reader)
    {
        return new AppUser
        {
            UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
            Name = reader.GetString(reader.GetOrdinal("FullName")),
            Email = reader.GetString(reader.GetOrdinal("Email")),
            RoleName = reader.GetString(reader.GetOrdinal("RoleName")),
            IsActive = ReadBool(reader, reader.GetOrdinal("IsActive"))
        };
    }

    private static bool ReadBool(MySqlDataReader reader, int ordinal)
    {
        return Convert.ToBoolean(reader.GetValue(ordinal));
    }
}
