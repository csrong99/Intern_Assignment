CREATE TABLE [dbo].[Employee] (
    [Username]        NVARCHAR (15)  NOT NULL,
    [Employee_ID]     INT            NOT NULL,
    [Email]           NVARCHAR (MAX) NOT NULL,
    [Full_Name]       NVARCHAR (20)  NULL,
    [Password]        NVARCHAR (60)  NOT NULL,
    [Join_Date]       DATE           NOT NULL,
    [Position]        INT            NOT NULL,
    [Team]            INT            NOT NULL,
    [Security_Phrase] NVARCHAR (40)  NULL,
    [Status]          INT            NOT NULL,
    [Login_Attempts]  INT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Employee_ID] ASC),
    UNIQUE NONCLUSTERED ([Username] ASC),
    CONSTRAINT [FK_EmployeePosition] FOREIGN KEY ([Position]) REFERENCES [dbo].[Position] ([Position_ID]),
    CONSTRAINT [FK_EmployeeStatus] FOREIGN KEY ([Status]) REFERENCES [dbo].[Status] ([Status_ID]),
    CONSTRAINT [FK_EmployeeTeam] FOREIGN KEY ([Team]) REFERENCES [dbo].[Team] ([Team_ID])
);


GO
CREATE   TRIGGER [trg_suspend] ON [Employee]
AFTER UPDATE 
AS 
BEGIN
	DECLARE @Employee_ID AS INT
	DECLARE @log_attempts AS INT

	SELECT @log_attempts=Login_Attempts from inserted
	SELECT @Employee_ID=Employee_ID from inserted
	IF @log_attempts = 10
	BEGIN
		UPDATE Employee 
		SET [Status] = 3
		WHERE Employee_ID=@Employee_ID
	END
END

CREATE TABLE [dbo].[Log] (
    [Id]           INT           IDENTITY (1, 1) NOT NULL,
    [Attempt_Time] DATETIME      NOT NULL,
    [Employee_ID]  INT  NULL,
    [Ipv4]         NCHAR (20)    NOT NULL,
    [successful]   BIT           NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_LogEmployee] FOREIGN KEY ([Employee_ID]) REFERENCES [dbo].[Employee] ([Employee_ID])
);


GO
CREATE   TRIGGER [trg_log_attempts] ON [Log]
AFTER INSERT 
AS 
BEGIN
	DECLARE @Employee_ID as INT
	DECLARE @successful as bit

	SELECT @Employee_ID=Employee_ID from inserted
	SELECT @successful=successful from inserted

	IF @successful = 1
	BEGIN
		UPDATE Employee 
		SET Login_Attempts = 0
		WHERE Employee_ID=@Employee_ID
	END
 
	ELSE
	BEGIN
		UPDATE Employee 
		SET Login_Attempts = Login_Attempts+1
		WHERE Employee_ID=@Employee_ID
	END
END

CREATE TABLE [dbo].[Position] (
    [Position_ID] INT           NOT NULL,
    [Name]        NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([Position_ID] ASC)
);

CREATE TABLE [dbo].[Status] (
    [Status_ID] INT           NOT NULL,
    [Name]      NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([Status_ID] ASC)
);

CREATE TABLE [dbo].[Team] (
    [Team_ID] INT           NOT NULL,
    [Name]    NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([Team_ID] ASC)
);



