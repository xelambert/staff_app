CREATE PROCEDURE STAFF_FIRE
@ID INT = NULL
AS
BEGIN
	DECLARE @errorMessage NVARCHAR(MAX)

	IF (@ID IS NULL)
	BEGIN
		SET @errorMessage = N'Не надано код співробітника'
		RAISERROR(@errorMessage, 16, 1)
	END;

	IF(EXISTS(SELECT * FROM STAFF
			   WHERE ID = @ID))
	BEGIN
		UPDATE STAFF
		SET ISFIRED = 1
		WHERE ID = @ID
	END;
	ELSE
	BEGIN
		
		SET @errorMessage = N'Працівник з кодом "' + CAST(@ID AS NVARCHAR(MAX)) + N'" відсутній у базі'
		RAISERROR(@errorMessage, 16, 1)
	END;
END;