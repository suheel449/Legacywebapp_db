/*
  LegacyBizDb — MySQL database for the Legacy Auth ASP.NET app.
  Run: mysql -u root -p < Database/LegacyBizDb.sql
*/

DROP DATABASE IF EXISTS LegacyBizDb;
CREATE DATABASE LegacyBizDb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE LegacyBizDb;

CREATE TABLE Users (
    UserId         INT NOT NULL AUTO_INCREMENT,
    FullName       VARCHAR(100) NOT NULL,
    Email          VARCHAR(150) NOT NULL,
    PasswordHash   VARCHAR(100) NOT NULL,
    RoleName       VARCHAR(50) NOT NULL DEFAULT 'Member',
    IsActive       TINYINT(1) NOT NULL DEFAULT 1,
    CreatedAt      DATETIME NOT NULL DEFAULT (UTC_TIMESTAMP()),
    PRIMARY KEY (UserId),
    UNIQUE KEY UQ_Users_Email (Email)
);

CREATE TABLE UserPreferences (
    UserId         INT NOT NULL,
    EmailAlerts    TINYINT(1) NOT NULL DEFAULT 1,
    WeeklyDigest   TINYINT(1) NOT NULL DEFAULT 1,
    CompactNav     TINYINT(1) NOT NULL DEFAULT 0,
    UpdatedAt      DATETIME NOT NULL DEFAULT (UTC_TIMESTAMP()),
    PRIMARY KEY (UserId),
    CONSTRAINT FK_UserPreferences_Users FOREIGN KEY (UserId) REFERENCES Users (UserId)
);

CREATE TABLE Tasks (
    TaskId           INT NOT NULL AUTO_INCREMENT,
    Title            VARCHAR(200) NOT NULL,
    AssignedToUserId INT NULL,
    DueDate          DATE NULL,
    Status           VARCHAR(20) NOT NULL DEFAULT 'Open',
    CreatedAt        DATETIME NOT NULL DEFAULT (UTC_TIMESTAMP()),
    PRIMARY KEY (TaskId),
    CONSTRAINT CK_Tasks_Status CHECK (Status IN ('Open', 'InProgress', 'Done', 'Cancelled')),
    CONSTRAINT FK_Tasks_Users FOREIGN KEY (AssignedToUserId) REFERENCES Users (UserId)
);

CREATE TABLE Reports (
    ReportId       INT NOT NULL AUTO_INCREMENT,
    Title          VARCHAR(150) NOT NULL,
    OwnerTeam      VARCHAR(80) NOT NULL,
    PeriodLabel    VARCHAR(80) NOT NULL,
    Status         VARCHAR(20) NOT NULL DEFAULT 'Draft',
    CreatedAt      DATETIME NOT NULL DEFAULT (UTC_TIMESTAMP()),
    PRIMARY KEY (ReportId),
    CONSTRAINT CK_Reports_Status CHECK (Status IN ('Ready', 'Review', 'Draft'))
);

CREATE TABLE Notifications (
    NotificationId INT NOT NULL AUTO_INCREMENT,
    UserId         INT NULL,
    Title          VARCHAR(150) NOT NULL,
    Body           VARCHAR(500) NOT NULL,
    IsRead         TINYINT(1) NOT NULL DEFAULT 0,
    CreatedAt      DATETIME NOT NULL DEFAULT (UTC_TIMESTAMP()),
    PRIMARY KEY (NotificationId),
    CONSTRAINT FK_Notifications_Users FOREIGN KEY (UserId) REFERENCES Users (UserId)
);

CREATE TABLE ActivityLog (
    ActivityId     INT NOT NULL AUTO_INCREMENT,
    UserId         INT NULL,
    Description    VARCHAR(250) NOT NULL,
    CreatedAt      DATETIME NOT NULL DEFAULT (UTC_TIMESTAMP()),
    PRIMARY KEY (ActivityId),
    CONSTRAINT FK_ActivityLog_Users FOREIGN KEY (UserId) REFERENCES Users (UserId)
);

CREATE INDEX IX_Tasks_Status ON Tasks (Status);
CREATE INDEX IX_Tasks_AssignedToUserId ON Tasks (AssignedToUserId);
CREATE INDEX IX_Reports_Status ON Reports (Status);
CREATE INDEX IX_Notifications_UserId ON Notifications (UserId);
CREATE INDEX IX_Notifications_IsRead ON Notifications (IsRead);
CREATE INDEX IX_ActivityLog_UserId ON ActivityLog (UserId);
CREATE INDEX IX_ActivityLog_CreatedAt ON ActivityLog (CreatedAt DESC);

INSERT INTO Users (FullName, Email, PasswordHash, RoleName) VALUES
('Demo User',  'demo@legacyauth.local',  'demo123',  'Member'),
('Admin User', 'admin@legacyauth.local', 'admin123', 'Admin'),
('Sarah Chen', 'sarah.chen@legacybiz.local', 'sarah123', 'Member'),
('Mike Torres','mike.torres@legacybiz.local', 'mike123', 'Member'),
('Priya Nair', 'priya.nair@legacybiz.local', 'priya123', 'Member'),
('Alex Kim',   'alex.kim@legacybiz.local', 'alex123', 'Member');

INSERT INTO UserPreferences (UserId, EmailAlerts, WeeklyDigest, CompactNav) VALUES
(1, 1, 1, 0),
(2, 1, 1, 0),
(3, 1, 0, 0),
(4, 0, 1, 1),
(5, 1, 1, 0),
(6, 1, 1, 0);

INSERT INTO Tasks (Title, AssignedToUserId, DueDate, Status) VALUES
('Review weekly login report', 1, DATE(DATE_ADD(UTC_TIMESTAMP(), INTERVAL 2 DAY)), 'Open'),
('Update profile contact info', 1, DATE(DATE_ADD(UTC_TIMESTAMP(), INTERVAL 5 DAY)), 'Open'),
('Approve new registrations', 2, DATE(DATE_ADD(UTC_TIMESTAMP(), INTERVAL 1 DAY)), 'Open'),
('Audit failed sign-ins', 2, DATE(DATE_ADD(UTC_TIMESTAMP(), INTERVAL 3 DAY)), 'InProgress'),
('Prepare security digest', 1, DATE(DATE_ADD(UTC_TIMESTAMP(), INTERVAL 4 DAY)), 'Open'),
('Archive old sessions', 4, DATE(DATE_ADD(UTC_TIMESTAMP(), INTERVAL 7 DAY)), 'Open'),
('Cleanup demo accounts', 2, DATE(DATE_ADD(UTC_TIMESTAMP(), INTERVAL -1 DAY)), 'Done'),
('Sync notification templates', 3, DATE(DATE_ADD(UTC_TIMESTAMP(), INTERVAL 6 DAY)), 'Open');

INSERT INTO Reports (Title, OwnerTeam, PeriodLabel, Status) VALUES
('Login activity',      'Ops',      'This week',    'Ready'),
('New registrations',   'HR',       'This week',    'Ready'),
('Failed sign-ins',     'Security', 'Last 7 days',  'Review'),
('Session summary',     'IT',       'Last 30 days', 'Draft'),
('Active users',        'Ops',      'This month',   'Ready'),
('Password resets',     'Security', 'This week',    'Ready'),
('Notification volume', 'IT',       'Last 14 days', 'Ready'),
('Task completion',     'Ops',      'This week',    'Review'),
('Regional access',     'IT',       'Last 30 days', 'Draft'),
('Admin actions',       'Security', 'This month',   'Ready'),
('Support tickets',     'Support',  'This week',    'Ready'),
('Weekly digest stats', 'HR',       'This week',    'Ready');

INSERT INTO Notifications (UserId, Title, Body, IsRead) VALUES
(1, 'Password policy reminder', 'Use a unique password. Accounts are stored in MySQL.', 0),
(1, 'Weekly report is ready', 'Open Reports to review login and registration activity.', 0),
(1, 'Welcome to Legacy Auth', 'Your dashboard, profile, and settings pages load from the database.', 1),
(1, 'Session notice', 'Signing out clears this session. User accounts remain in the database.', 1),
(2, 'Admin checklist', 'Review failed sign-ins and open tasks assigned to Admin.', 0),
(NULL, 'System maintenance', 'Scheduled maintenance window this Sunday at 02:00 UTC.', 0);

INSERT INTO ActivityLog (UserId, Description, CreatedAt) VALUES
(1, 'Signed in', DATE_ADD(UTC_TIMESTAMP(), INTERVAL -2 MINUTE)),
(1, 'Weekly summary generated', DATE_ADD(UTC_TIMESTAMP(), INTERVAL -1 DAY)),
(1, 'Profile viewed', DATE_ADD(UTC_TIMESTAMP(), INTERVAL -2 DAY)),
(2, 'Password last changed', DATE_ADD(UTC_TIMESTAMP(), INTERVAL -14 DAY)),
(2, 'Report exported', DATE_ADD(UTC_TIMESTAMP(), INTERVAL -6 HOUR)),
(3, 'Notification marked read', DATE_ADD(UTC_TIMESTAMP(), INTERVAL -3 DAY));

DELIMITER $$
CREATE PROCEDURE usp_GetDashboardStats(IN p_UserId INT)
BEGIN
    SELECT
        (SELECT COUNT(*) FROM Tasks WHERE Status IN ('Open', 'InProgress')) AS OpenTasks,
        (
            SELECT COUNT(*) FROM Tasks
            WHERE Status IN ('Open', 'InProgress')
              AND DueDate IS NOT NULL
              AND DueDate <= DATE(DATE_ADD(UTC_TIMESTAMP(), INTERVAL 7 DAY))
        ) AS TasksDueThisWeek,
        (SELECT COUNT(*) FROM Reports) AS ReportCount,
        (
            SELECT COUNT(*) FROM Notifications
            WHERE UserId IS NULL OR UserId = p_UserId
        ) AS NotificationCount,
        (
            SELECT COUNT(*) FROM Notifications
            WHERE IsRead = 0 AND (UserId IS NULL OR UserId = p_UserId)
        ) AS UnreadNotifications,
        (SELECT COUNT(*) FROM Users WHERE IsActive = 1) AS TeamMembers;
END$$
DELIMITER ;
