/*
  LegacyBizDb — SQL Server database for the Legacy Auth ASP.NET app.
  Run: sqlcmd -S "(localdb)\MSSQLLocalDB" -E -i LegacyBizDb.sql
*/

USE master;
GO

IF DB_ID(N'LegacyBizDb') IS NOT NULL
BEGIN
    ALTER DATABASE LegacyBizDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE LegacyBizDb;
END
GO

CREATE DATABASE LegacyBizDb;
GO

USE LegacyBizDb;
GO

/* ========== Tables ========== */

CREATE TABLE dbo.Users (
    UserId         INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
    FullName       NVARCHAR(100) NOT NULL,
    Email          NVARCHAR(150) NOT NULL,
    PasswordHash   NVARCHAR(100) NOT NULL, -- demo only; hash properly in production
    RoleName       NVARCHAR(50) NOT NULL CONSTRAINT DF_Users_RoleName DEFAULT (N'Member'),
    IsActive       BIT NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT (1),
    CreatedAt      DATETIME2(0) NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT UQ_Users_Email UNIQUE (Email)
);
GO

CREATE TABLE dbo.UserPreferences (
    UserId         INT NOT NULL CONSTRAINT PK_UserPreferences PRIMARY KEY,
    EmailAlerts    BIT NOT NULL CONSTRAINT DF_UserPreferences_EmailAlerts DEFAULT (1),
    WeeklyDigest   BIT NOT NULL CONSTRAINT DF_UserPreferences_WeeklyDigest DEFAULT (1),
    CompactNav     BIT NOT NULL CONSTRAINT DF_UserPreferences_CompactNav DEFAULT (0),
    UpdatedAt      DATETIME2(0) NOT NULL CONSTRAINT DF_UserPreferences_UpdatedAt DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT FK_UserPreferences_Users FOREIGN KEY (UserId)
        REFERENCES dbo.Users (UserId)
);
GO

CREATE TABLE dbo.Tasks (
    TaskId         INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Tasks PRIMARY KEY,
    Title          NVARCHAR(200) NOT NULL,
    AssignedToUserId INT NULL,
    DueDate        DATE NULL,
    Status         NVARCHAR(20) NOT NULL CONSTRAINT DF_Tasks_Status DEFAULT (N'Open'),
    CreatedAt      DATETIME2(0) NOT NULL CONSTRAINT DF_Tasks_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT CK_Tasks_Status CHECK (Status IN (N'Open', N'InProgress', N'Done', N'Cancelled')),
    CONSTRAINT FK_Tasks_Users FOREIGN KEY (AssignedToUserId)
        REFERENCES dbo.Users (UserId)
);
GO

CREATE TABLE dbo.Reports (
    ReportId       INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Reports PRIMARY KEY,
    Title          NVARCHAR(150) NOT NULL,
    OwnerTeam      NVARCHAR(80) NOT NULL,
    PeriodLabel    NVARCHAR(80) NOT NULL,
    Status         NVARCHAR(20) NOT NULL CONSTRAINT DF_Reports_Status DEFAULT (N'Draft'),
    CreatedAt      DATETIME2(0) NOT NULL CONSTRAINT DF_Reports_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT CK_Reports_Status CHECK (Status IN (N'Ready', N'Review', N'Draft'))
);
GO

CREATE TABLE dbo.Notifications (
    NotificationId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Notifications PRIMARY KEY,
    UserId         INT NULL, -- NULL = all users
    Title          NVARCHAR(150) NOT NULL,
    Body           NVARCHAR(500) NOT NULL,
    IsRead         BIT NOT NULL CONSTRAINT DF_Notifications_IsRead DEFAULT (0),
    CreatedAt      DATETIME2(0) NOT NULL CONSTRAINT DF_Notifications_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT FK_Notifications_Users FOREIGN KEY (UserId)
        REFERENCES dbo.Users (UserId)
);
GO

CREATE TABLE dbo.ActivityLog (
    ActivityId     INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ActivityLog PRIMARY KEY,
    UserId         INT NULL,
    Description    NVARCHAR(250) NOT NULL,
    CreatedAt      DATETIME2(0) NOT NULL CONSTRAINT DF_ActivityLog_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT FK_ActivityLog_Users FOREIGN KEY (UserId)
        REFERENCES dbo.Users (UserId)
);
GO

/* ========== Indexes ========== */

CREATE INDEX IX_Tasks_Status ON dbo.Tasks (Status);
CREATE INDEX IX_Tasks_AssignedToUserId ON dbo.Tasks (AssignedToUserId);
CREATE INDEX IX_Reports_Status ON dbo.Reports (Status);
CREATE INDEX IX_Notifications_UserId ON dbo.Notifications (UserId);
CREATE INDEX IX_Notifications_IsRead ON dbo.Notifications (IsRead);
CREATE INDEX IX_ActivityLog_UserId ON dbo.ActivityLog (UserId);
CREATE INDEX IX_ActivityLog_CreatedAt ON dbo.ActivityLog (CreatedAt DESC);
GO

/* ========== Sample data ========== */

INSERT INTO dbo.Users (FullName, Email, PasswordHash, RoleName) VALUES
(N'Demo User',  N'demo@legacyauth.local',  N'demo123',  N'Member'),
(N'Admin User', N'admin@legacyauth.local', N'admin123', N'Admin'),
(N'Sarah Chen', N'sarah.chen@legacybiz.local', N'sarah123', N'Member'),
(N'Mike Torres',N'mike.torres@legacybiz.local', N'mike123', N'Member'),
(N'Priya Nair', N'priya.nair@legacybiz.local', N'priya123', N'Member'),
(N'Alex Kim',   N'alex.kim@legacybiz.local', N'alex123', N'Member');
GO

INSERT INTO dbo.UserPreferences (UserId, EmailAlerts, WeeklyDigest, CompactNav) VALUES
(1, 1, 1, 0),
(2, 1, 1, 0),
(3, 1, 0, 0),
(4, 0, 1, 1),
(5, 1, 1, 0),
(6, 1, 1, 0);
GO

INSERT INTO dbo.Tasks (Title, AssignedToUserId, DueDate, Status) VALUES
(N'Review weekly login report', 1, CAST(DATEADD(DAY, 2, SYSUTCDATETIME()) AS DATE), N'Open'),
(N'Update profile contact info', 1, CAST(DATEADD(DAY, 5, SYSUTCDATETIME()) AS DATE), N'Open'),
(N'Approve new registrations', 2, CAST(DATEADD(DAY, 1, SYSUTCDATETIME()) AS DATE), N'Open'),
(N'Audit failed sign-ins', 2, CAST(DATEADD(DAY, 3, SYSUTCDATETIME()) AS DATE), N'InProgress'),
(N'Prepare security digest', 1, CAST(DATEADD(DAY, 4, SYSUTCDATETIME()) AS DATE), N'Open'),
(N'Archive old sessions', 4, CAST(DATEADD(DAY, 7, SYSUTCDATETIME()) AS DATE), N'Open'),
(N'Cleanup demo accounts', 2, CAST(DATEADD(DAY, -1, SYSUTCDATETIME()) AS DATE), N'Done'),
(N'Sync notification templates', 3, CAST(DATEADD(DAY, 6, SYSUTCDATETIME()) AS DATE), N'Open');
GO

INSERT INTO dbo.Reports (Title, OwnerTeam, PeriodLabel, Status) VALUES
(N'Login activity',      N'Ops',      N'This week',    N'Ready'),
(N'New registrations',   N'HR',       N'This week',    N'Ready'),
(N'Failed sign-ins',     N'Security', N'Last 7 days',  N'Review'),
(N'Session summary',     N'IT',       N'Last 30 days', N'Draft'),
(N'Active users',        N'Ops',      N'This month',   N'Ready'),
(N'Password resets',     N'Security', N'This week',    N'Ready'),
(N'Notification volume', N'IT',       N'Last 14 days', N'Ready'),
(N'Task completion',     N'Ops',      N'This week',    N'Review'),
(N'Regional access',     N'IT',       N'Last 30 days', N'Draft'),
(N'Admin actions',       N'Security', N'This month',   N'Ready'),
(N'Support tickets',     N'Support',  N'This week',    N'Ready'),
(N'Weekly digest stats', N'HR',       N'This week',    N'Ready');
GO

INSERT INTO dbo.Notifications (UserId, Title, Body, IsRead) VALUES
(1, N'Password policy reminder', N'Use a unique password. Accounts are stored in SQL Server.', 0),
(1, N'Weekly report is ready', N'Open Reports to review login and registration activity.', 0),
(1, N'Welcome to Legacy Auth', N'Your dashboard, profile, and settings pages load from the database.', 1),
(1, N'Session notice', N'Signing out clears this session. User accounts remain in the database.', 1),
(2, N'Admin checklist', N'Review failed sign-ins and open tasks assigned to Admin.', 0),
(NULL, N'System maintenance', N'Scheduled maintenance window this Sunday at 02:00 UTC.', 0);
GO

INSERT INTO dbo.ActivityLog (UserId, Description, CreatedAt) VALUES
(1, N'Signed in', DATEADD(MINUTE, -2, SYSUTCDATETIME())),
(1, N'Weekly summary generated', DATEADD(DAY, -1, SYSUTCDATETIME())),
(1, N'Profile viewed', DATEADD(DAY, -2, SYSUTCDATETIME())),
(2, N'Password last changed', DATEADD(DAY, -14, SYSUTCDATETIME())),
(2, N'Report exported', DATEADD(HOUR, -6, SYSUTCDATETIME())),
(3, N'Notification marked read', DATEADD(DAY, -3, SYSUTCDATETIME()));
GO

/* ========== Stored procedure ========== */

CREATE PROCEDURE dbo.usp_GetDashboardStats
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        OpenTasks = (SELECT COUNT(*) FROM dbo.Tasks WHERE Status IN (N'Open', N'InProgress')),
        TasksDueThisWeek = (
            SELECT COUNT(*) FROM dbo.Tasks
            WHERE Status IN (N'Open', N'InProgress')
              AND DueDate IS NOT NULL
              AND DueDate <= CAST(DATEADD(DAY, 7, SYSUTCDATETIME()) AS DATE)
        ),
        ReportCount = (SELECT COUNT(*) FROM dbo.Reports),
        NotificationCount = (
            SELECT COUNT(*) FROM dbo.Notifications
            WHERE UserId IS NULL OR UserId = @UserId
        ),
        UnreadNotifications = (
            SELECT COUNT(*) FROM dbo.Notifications
            WHERE IsRead = 0 AND (UserId IS NULL OR UserId = @UserId)
        ),
        TeamMembers = (SELECT COUNT(*) FROM dbo.Users WHERE IsActive = 1);
END
GO

PRINT N'LegacyBizDb created successfully.';
GO
