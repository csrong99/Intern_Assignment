CREATE OR ALTER TRIGGER [trg_suspend] ON [Employee]
AFTER UPDATE 
AS 
BEGIN
	DECLARE @username as nvarchar(15)
	DECLARE @log_attempts as int

	SELECT @log_attempts=Login_Attempts from inserted
	SELECT @username=username from inserted
	IF @log_attempts = 10
	BEGIN
		UPDATE Employee 
		SET [Status] = 3
		WHERE username=@username
	END
END
GO