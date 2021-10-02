/*CREATE DATABASE gameTracker*/
USE gameTracker

/*ASP Tables for the Users */
/* AspNetUsers Table */
CREATE TABLE [dbo].[AspNetUsers] (
    [Id]                   NVARCHAR (128) NOT NULL,
    [Email]                NVARCHAR (256) NULL,
    [EmailConfirmed]       BIT            NOT NULL,
    [PasswordHash]         NVARCHAR (MAX) NULL,
    [SecurityStamp]        NVARCHAR (MAX) NULL,
    [PhoneNumber]          NVARCHAR (MAX) NULL,
    [PhoneNumberConfirmed] BIT            NOT NULL,
    [TwoFactorEnabled]     BIT            NOT NULL,
    [LockoutEndDateUtc]    DATETIME       NULL,
    [LockoutEnabled]       BIT            NOT NULL,
    [AccessFailedCount]    INT            NOT NULL,
    [UserName]             NVARCHAR (256) NOT NULL,
    CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex]
    ON [dbo].[AspNetUsers]([UserName] ASC);

/* AspNetRoles Table */
CREATE TABLE [dbo].[AspNetRoles] (
    [Id]   NVARCHAR (128) NOT NULL,
    [Name] NVARCHAR (256) NOT NULL,
    CONSTRAINT [PK_dbo.AspNetRoles] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex]
    ON [dbo].[AspNetRoles]([Name] ASC);

/* AspNetUserClaims Table */
CREATE TABLE [dbo].[AspNetUserClaims] (
    [Id]         INT IDENTITY (1, 1) NOT NULL,
    [UserId]     NVARCHAR (128) NOT NULL,
    [ClaimType]  NVARCHAR (MAX) NULL,
    [ClaimValue] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_dbo.AspNetUserClaims] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);

GO
CREATE NONCLUSTERED INDEX [IX_UserId]
    ON [dbo].[AspNetUserClaims]([UserId] ASC);

/* AspNetUserLogins Table */
CREATE TABLE [dbo].[AspNetUserLogins] (
    [LoginProvider] NVARCHAR (128) NOT NULL,
    [ProviderKey]   NVARCHAR (128) NOT NULL,
    [UserId]        NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_dbo.AspNetUserLogins] PRIMARY KEY CLUSTERED ([LoginProvider] ASC, [ProviderKey] ASC, [UserId] ASC),
    CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);

GO
CREATE NONCLUSTERED INDEX [IX_UserId]
    ON [dbo].[AspNetUserLogins]([UserId] ASC);

/* AspNetUserRoles Table */
CREATE TABLE [dbo].[AspNetUserRoles] (
    [UserId] NVARCHAR (128) NOT NULL,
    [RoleId] NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_dbo.AspNetUserRoles] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
    CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);

GO
CREATE NONCLUSTERED INDEX [IX_UserId]
    ON [dbo].[AspNetUserRoles]([UserId] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_RoleId]
    ON [dbo].[AspNetUserRoles]([RoleId] ASC);


CREATE TABLE [dbo].[GameUsers]
(
	[UserId] INT IDENTITY(1,1) NOT NULL,
	[AspNetUserId] NVARCHAR(128) NOT NULL,
	[Name] NVARCHAR(128) NOT NULL,
	CONSTRAINT [PK_dbo.GameUsers] PRIMARY KEY CLUSTERED ([UserId] ASC),
	CONSTRAINT [FK_dbo.GameUsers] FOREIGN KEY (AspNetUserId) REFERENCES [dbo].[AspNetUsers] (Id)
);

CREATE TABLE [dbo].[GameSystems]
(
	[Name] NVARCHAR(128) NOT NULL,
	CONSTRAINT [PK_dbo.GameSystems] PRIMARY KEY (Name)
);

CREATE TABLE [dbo].[Games]
(
	[Id]				INT IDENTITY(1,1) NOT NULL,
	[UserId]			INT NOT NULL,
	[Title]				NVARCHAR(128) NOT NULL,
	[Price]				INT,
	[SystemName]		NVARCHAR(128) NOT NULL,
	[DateOfPurchase]	DATE,
	[DatePlayed]		DATE,
	[Borrowed]			BIT NOT NULL,
	[Physical]			BIT NOT NULL,
	[Replayed]			BIT NOT NULL,
	CONSTRAINT [PK_dbo.Games] PRIMARY KEY (Id),
	CONSTRAINT [FK_dbo.Games] FOREIGN KEY (SystemName) REFERENCES [dbo].[GameSystems] (Name),
	CONSTRAINT [FK_dbo.Games2] FOREIGN KEY (UserId) REFERENCES [dbo].[GameUsers] (UserId)
);

/*Seeded data*/
INSERT INTO [dbo].[GameSystems] (Name) VALUES
	('PS3'),
	('PS4'),
	('Switch'),
	('3ds'),
	('DS'),
	('Xbox 360'),
	('Xbox One'),
	('PC');
