using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

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
/// SQL Server data access for all Legacy Auth pages.
/// </summary>
public static class AppDb
{
    private static string ConnectionString
    {
        get { return ConfigurationManager.ConnectionStrings["LegacyBizDb"].ConnectionString; }
    }

    public static bool EmailExists(string email)
    {
        const string sql = "SELECT COUNT(1) FROM dbo.Users WHERE Email = @Email";
        using (SqlConnection conn = new SqlConnection(ConnectionString))
        using (SqlCommand cmd = new SqlCommand(sql, conn))
        {
            cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 150).Value = email.Trim().ToLowerInvariant();
            conn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }
    }

    public static AppUser Validate(string email, string password)
    {
        const string sql = @"
            SELECT UserId, FullName, Email, RoleName, IsActive
            FROM dbo.Users
            WHERE Email = @Email AND PasswordHash = @Password AND IsActive = 1";

        using (SqlConnection conn = new SqlConnection(ConnectionString))
        using (SqlCommand cmd = new SqlCommand(sql, conn))
        {
            cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 150).Value = email.Trim().ToLowerInvariant();
            cmd.Parameters.Add("@Password", SqlDbType.NVarChar, 100).Value = password ?? string.Empty;
            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
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

        using (SqlConnection conn = new SqlConnection(ConnectionString))
        {
            conn.Open();
            using (SqlTransaction tx = conn.BeginTransaction())
            {
                int userId;
                const string insertUser = @"
                    INSERT INTO dbo.Users (FullName, Email, PasswordHash, RoleName)
                    OUTPUT INSERTED.UserId
                    VALUES (@FullName, @Email, @Password, N'Member')";

                using (SqlCommand cmd = new SqlCommand(insertUser, conn, tx))
                {
                    cmd.Parameters.Add("@FullName", SqlDbType.NVarChar, 100).Value = name.Trim();
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 150).Value = normalizedEmail;
                    cmd.Parameters.Add("@Password", SqlDbType.NVarChar, 100).Value = password;
                    userId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                const string insertPrefs = @"
                    INSERT INTO dbo.UserPreferences (UserId, EmailAlerts, WeeklyDigest, CompactNav)
                    VALUES (@UserId, 1, 1, 0)";
                using (SqlCommand cmd = new SqlCommand(insertPrefs, conn, tx))
                {
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    cmd.ExecuteNonQuery();
                }

                const string insertNotice = @"
                    INSERT INTO dbo.Notifications (UserId, Title, Body, IsRead)
                    VALUES (@UserId, N'Welcome to Legacy Auth', N'Your account was created and stored in SQL Server.', 0)";
                using (SqlCommand cmd = new SqlCommand(insertNotice, conn, tx))
                {
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    cmd.ExecuteNonQuery();
                }

                const string insertActivity = @"
                    INSERT INTO dbo.ActivityLog (UserId, Description)
                    VALUES (@UserId, N'Account registered')";
                using (SqlCommand cmd = new SqlCommand(insertActivity, conn, tx))
                {
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
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
            FROM dbo.Users
            WHERE UserId = @UserId";

        using (SqlConnection conn = new SqlConnection(ConnectionString))
        using (SqlCommand cmd = new SqlCommand(sql, conn))
        {
            cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
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
            INSERT INTO dbo.ActivityLog (UserId, Description)
            VALUES (@UserId, @Description)";

        using (SqlConnection conn = new SqlConnection(ConnectionString))
        using (SqlCommand cmd = new SqlCommand(sql, conn))
        {
            cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
            cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 250).Value = description;
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public static UserPreferences GetPreferences(int userId)
    {
        const string sql = @"
            SELECT EmailAlerts, WeeklyDigest, CompactNav
            FROM dbo.UserPreferences
            WHERE UserId = @UserId";

        using (SqlConnection conn = new SqlConnection(ConnectionString))
        using (SqlCommand cmd = new SqlCommand(sql, conn))
        {
            cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
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
                    EmailAlerts = reader.GetBoolean(0),
                    WeeklyDigest = reader.GetBoolean(1),
                    CompactNav = reader.GetBoolean(2)
                };
            }
        }
    }

    public static void SavePreferences(int userId, UserPreferences prefs)
    {
        const string sql = @"
            IF EXISTS (SELECT 1 FROM dbo.UserPreferences WHERE UserId = @UserId)
                UPDATE dbo.UserPreferences
                SET EmailAlerts = @EmailAlerts,
                    WeeklyDigest = @WeeklyDigest,
                    CompactNav = @CompactNav,
                    UpdatedAt = SYSUTCDATETIME()
                WHERE UserId = @UserId
            ELSE
                INSERT INTO dbo.UserPreferences (UserId, EmailAlerts, WeeklyDigest, CompactNav)
                VALUES (@UserId, @EmailAlerts, @WeeklyDigest, @CompactNav)";

        using (SqlConnection conn = new SqlConnection(ConnectionString))
        using (SqlCommand cmd = new SqlCommand(sql, conn))
        {
            cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
            cmd.Parameters.Add("@EmailAlerts", SqlDbType.Bit).Value = prefs.EmailAlerts;
            cmd.Parameters.Add("@WeeklyDigest", SqlDbType.Bit).Value = prefs.WeeklyDigest;
            cmd.Parameters.Add("@CompactNav", SqlDbType.Bit).Value = prefs.CompactNav;
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public static DashboardStats GetDashboardStats(int userId)
    {
        using (SqlConnection conn = new SqlConnection(ConnectionString))
        using (SqlCommand cmd = new SqlCommand("dbo.usp_GetDashboardStats", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (!reader.Read())
                {
                    return new DashboardStats();
                }

                return new DashboardStats
                {
                    OpenTasks = reader.GetInt32(0),
                    TasksDueThisWeek = reader.GetInt32(1),
                    ReportCount = reader.GetInt32(2),
                    NotificationCount = reader.GetInt32(3),
                    UnreadNotifications = reader.GetInt32(4),
                    TeamMembers = reader.GetInt32(5)
                };
            }
        }
    }

    public static List<ActivityItem> GetRecentActivity(int userId, int top)
    {
        const string sql = @"
            SELECT TOP (@Top) Description, CreatedAt
            FROM dbo.ActivityLog
            WHERE UserId = @UserId OR UserId IS NULL
            ORDER BY CreatedAt DESC";

        List<ActivityItem> items = new List<ActivityItem>();
        using (SqlConnection conn = new SqlConnection(ConnectionString))
        using (SqlCommand cmd = new SqlCommand(sql, conn))
        {
            cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
            cmd.Parameters.Add("@Top", SqlDbType.Int).Value = top;
            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
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
            FROM dbo.Reports
            ORDER BY ReportId";

        List<ReportItem> items = new List<ReportItem>();
        using (SqlConnection conn = new SqlConnection(ConnectionString))
        using (SqlCommand cmd = new SqlCommand(sql, conn))
        {
            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
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
            FROM dbo.Notifications
            WHERE UserId IS NULL OR UserId = @UserId
            ORDER BY IsRead ASC, CreatedAt DESC";

        List<NotificationItem> items = new List<NotificationItem>();
        using (SqlConnection conn = new SqlConnection(ConnectionString))
        using (SqlCommand cmd = new SqlCommand(sql, conn))
        {
            cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    items.Add(new NotificationItem
                    {
                        NotificationId = reader.GetInt32(0),
                        Title = reader.GetString(1),
                        Body = reader.GetString(2),
                        IsRead = reader.GetBoolean(3)
                    });
                }
            }
        }

        return items;
    }

    private static AppUser ReadUser(SqlDataReader reader)
    {
        return new AppUser
        {
            UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
            Name = reader.GetString(reader.GetOrdinal("FullName")),
            Email = reader.GetString(reader.GetOrdinal("Email")),
            RoleName = reader.GetString(reader.GetOrdinal("RoleName")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
        };
    }
}
