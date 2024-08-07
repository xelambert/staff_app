-- НАПОВНЮЄМО ТАБЛИЦЮ ПЕРСОНАЛУ
CREATE PROC STAFF_IMPORT
@VALUES NVARCHAR(MAX) = NULL
AS
BEGIN
DECLARE @sql NVARCHAR(MAX)
BEGIN TRAN
	BEGIN TRY
		SET @sql = '
		INSERT INTO STAFF
		(PIB,
		BIRTHDATE,
		POSITIONID,
		SALARY)
		VALUES
		' + @VALUES;

		EXEC sp_executesql @sql

		COMMIT TRAN
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
		RAISERROR('Помилка при імпорті співробітників. Відкат транзакції.', 16, 1)
	END CATCH
END