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