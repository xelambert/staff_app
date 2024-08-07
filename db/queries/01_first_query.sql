-- СТВОРЮЄМО БАЗУ ДЛЯ ДОДАТКУ
CREATE DATABASE STAFF_APP

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


