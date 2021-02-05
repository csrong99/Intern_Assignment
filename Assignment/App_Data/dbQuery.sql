CREATE TABLE [dbo].[Status]
(
	[Status_ID] INT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NOT NULL
)

CREATE TABLE [dbo].[Position]
(
	[Position_ID] INT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NOT NULL
)

CREATE TABLE [dbo].[Team]
(
	[Team_ID] INT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NOT NULL
)

CREATE TABLE [dbo].[Employee]
(
	[Username] NVARCHAR(15) NOT NULL PRIMARY KEY, 
    [Employee_ID] INT NOT NULL UNIQUE, 
    [Email] NVARCHAR(MAX) NOT NULL, 
    [Full_Name] NVARCHAR(20) NULL, 
    [Password] NVARCHAR(60) NOT NULL, 
    [Join_Date] DATE NOT NULL, 
    [Position] INT NOT NULL, 
    [Team] INT NOT NULL, 
    [Security_Phrase] NVARCHAR(40) NULL, 
    [Status] INT NOT NULL,
	CONSTRAINT FK_EmployeePosition FOREIGN KEY (Position) REFERENCES Position(Position_ID),
	CONSTRAINT FK_EmployeeStatus FOREIGN KEY ([Status]) REFERENCES [Status](Status_ID),
	CONSTRAINT FK_EmployeeTeam FOREIGN KEY (Team) REFERENCES Team(Team_ID)
)

CREATE TABLE [dbo].[Log] (
    [Id]           INT           NOT NULL IDENTITY,
    [Attempt_Time] DATETIME      NOT NULL,
    [Username]     NVARCHAR (15) NULL,
    [Ipv4]         NCHAR (20)    NOT NULL,
    [successful]   BIT           NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_LogEmployee] FOREIGN KEY ([Username]) REFERENCES [dbo].[Employee] ([Username])
);

ALTER TABLE [dbo].[Employee]
ADD [Login_Attempts] INT NOT NULL
DEFAULT (0);

DBCC CHECKIDENT ('Log', RESEED, 1)


CREATE TABLE [dbo].[Ipv4Blacklist]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [Ipv4] NCHAR(20) NOT NULL
)



