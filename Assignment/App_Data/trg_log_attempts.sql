CREATE OR ALTER TRIGGER [trg_log_attempts] ON [Log]
AFTER INSERT 
AS 
BEGIN
	DECLARE @username as nvarchar(15)
	DECLARE @successful as bit

	SELECT @username=username from inserted
	SELECT @successful=successful from inserted

	IF @successful = 1
	BEGIN
		UPDATE Employee 
		SET Login_Attempts = 0
		WHERE username=@username
	END
 
	ELSE
	BEGIN
		UPDATE Employee 
		SET Login_Attempts = Login_Attempts+1
		WHERE username=@username
	END
END
GO