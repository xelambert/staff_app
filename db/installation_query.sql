----------------------------------------01_first_query---------------------
-- СТВОРЮЄМО БАЗУ ДЛЯ ДОДАТКУ
CREATE DATABASE STAFF_APP
GO

-- ОБИРАЄМО ЦЮ БАЗУ ДЛЯ НАДСИЛАННЯ ЗАПИТІВ
USE STAFF_APP

-- СТВОРЮЄМО ТАБЛИЦЮ ПОСАД
CREATE TABLE POSITIONS
(
ID INT PRIMARY KEY IDENTITY(1,1),
TITLE NVARCHAR(20) NOT NULL
)

-- НАПОВНЮЄМО ТАБЛИЦЮ ПОСАД
INSERT INTO POSITIONS
(TITLE)
VALUES
(N'Касир'),
(N'Адміністратор'),
(N'Охоронець'),
(N'Водій')

-- СТВОРЮЄМО ТАБЛИЦЮ ПЕРСОНАЛУ
CREATE TABLE STAFF
(
	ID INT PRIMARY KEY IDENTITY(1,1),
	PIB NVARCHAR(100) NOT NULL,
	BIRTHDATE DATE NOT NULL,
	POSITIONID INT NOT NULL
		FOREIGN KEY REFERENCES POSITIONS(ID)
		ON DELETE NO ACTION,
	SALARY FLOAT NOT NULL,
	ISFIRED BIT NOT NULL 
		DEFAULT 0
)

-- НАПОВНЮЄМО ТАБЛИЦЮ ПЕРСОНАЛУ
INSERT INTO STAFF
(PIB,
BIRTHDATE,
POSITIONID,
SALARY)
VALUES
(N'Загальна Марія Іванівна',
CONVERT(DATE, '20.01.1985', 104),
1,
100),
(N'Курносенко Юрій Вікторович',
CONVERT(DATE, '01.12.1990', 104),
2,
150),
(N'Чижов Дмитро Андрійович',
CONVERT(DATE, '05.08.1999', 104),
3,
90),
(N'Шпала Петро Олексійович',
CONVERT(DATE, '20.01.2000', 104),
4,
110),
(N'Григоренко Ян Станіславович',
CONVERT(DATE, '13.03.2003', 104),
1,
105),
(N'Шевченко Світлана Олександрівна',
CONVERT(DATE, '19.04.1996', 104),
1,
95)

--ПОМІЧАЄМО ОДНОГО СПІВРОБІТНИКА, ЯК ЗВІЛЬНЕНОГО
update STAFF
set ISFIRED = 1
where id = 4

GO
----------------------------------------proc_fire---------------------
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

GO
----------------------------------------proc_import---------------------
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

GO
----------------------------------------proc_positions---------------------
CREATE PROCEDURE POSITIONS_SELECT
AS
BEGIN
	SELECT 
		p.ID,
		p.TITLE
	FROM POSITIONS p
END

GO
----------------------------------------proc_staff_add---------------------
CREATE PROCEDURE STAFF_ADD
@PIB NVARCHAR(100) = NULL,
@BIRTHDATE DATE = NULL,
@POSITIONID INT = NULL,
@SALARY FLOAT = NULL,
@ISFIRED BIT = 0
AS
BEGIN
BEGIN TRAN
	BEGIN TRY
		INSERT INTO STAFF
		(PIB,
		BIRTHDATE,
		POSITIONID,
		SALARY,
		ISFIRED)
		VALUES
		(@PIB,
		@BIRTHDATE,
		@POSITIONID,
		@SALARY,
		@ISFIRED)
		COMMIT TRAN
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
		RAISERROR('Помилка при додаванні користувача. Відкат транзакції.', 16, 1)
	END CATCH
END

GO
----------------------------------------proc_staff_edit---------------------
CREATE PROCEDURE STAFF_EDIT
@PIB NVARCHAR(100) = NULL,
@BIRTHDATE DATE = NULL,
@POSITIONID INT = NULL,
@SALARY FLOAT = NULL,
@STAFFID INT = NULL
AS
BEGIN
BEGIN TRAN
	BEGIN TRY
		UPDATE STAFF
		SET PIB = @PIB,
		BIRTHDATE = @BIRTHDATE,
		POSITIONID = @POSITIONID,
		SALARY = @SALARY
		WHERE ID = @STAFFID

		COMMIT TRAN
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
		RAISERROR('Помилка при редагуванні даних користувача. Відкат транзакції.', 16, 1)
	END CATCH
END

GO
----------------------------------------proc_staff_select---------------------
-- ПРОЦЕДУРА СЕЛЕКТУ ТА ПОШУКУ СПІВРОБІТНИКІВ
CREATE PROCEDURE STAFF_SELECT
@PIB NVARCHAR(100) = NULL
AS
BEGIN
DECLARE @sql NVARCHAR(MAX)

SET @sql = '
SELECT
s.ID as staffID,
s.PIB,
s.BIRTHDATE,
s.SALARY,
s.ISFIRED,
p.ID as positionID,
p.TITLE
FROM STAFF s
JOIN POSITIONS p
ON s.POSITIONID = p.ID
'

IF (@PIB IS NOT NULL)
BEGIN
	SET @sql += ' WHERE s.PIB LIKE N''%' + @PIB + '%''';
END;

EXEC sp_executesql @sql

print @sql
END;

GO
----------------------------------------proc_statistics---------------------
CREATE PROCEDURE STAFF_STATISTICS
AS
BEGIN
	SELECT CAST(COUNT(*) AS NVARCHAR(100)) FROM STAFF
	WHERE ISFIRED = 0
	UNION ALL
	SELECT CAST(ROUND(AVG(SALARY), 2) AS NVARCHAR(100)) FROM STAFF
	WHERE ISFIRED = 0
END;
GO
