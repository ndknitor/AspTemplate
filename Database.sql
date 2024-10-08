create database Etdb
go
use Etdb;
go

CREATE TABLE [User](
    UserId int NOT NULL,
    Email nvarchar(128) NOT NULL,
    Fullname nvarchar(128) NOT NULL DEFAULT (''),
    Phone nvarchar(16) NOT NULL DEFAULT (''),
    [Address] nvarchar(128) NOT NULL DEFAULT (''),
    RoleId int NOT NULL DEFAULT ((0)),
    Password nvarchar(128) NOT NULL DEFAULT (''),
    PRIMARY KEY(UserId)
);;

CREATE TABLE Bus(
    BusId int NOT NULL,
    [Name] nvarchar(128),
    LicensePlate nvarchar(16) NOT NULL,
    Deleted bit NOT NULL DEFAULT ((0)),
    PRIMARY KEY(BusId)
);;

CREATE TABLE [Route](
    RouteId int NOT NULL,
    [From] nvarchar(128) NOT NULL DEFAULT (''),
    [To] nvarchar(128) NOT NULL DEFAULT (''),
    BasePrice int NOT NULL,
    Deleted bit NOT NULL,
    PRIMARY KEY(RouteId)
);;

CREATE TABLE Trip(
    TripId int NOT NULL,
    RouteId int NOT NULL,
    StartDate datetime NOT NULL,
    EndDate datetime NOT NULL,
    BusId int NOT NULL,
    PRIMARY KEY(TripId),
    FOREIGN KEY (RouteId) REFERENCES [Route](RouteId)
);;

CREATE TABLE Seat(
    SeatId int NOT NULL,
    BusId int NOT NULL,
    Price int NOT NULL DEFAULT ((0)),
    Deleted bit NOT NULL DEFAULT ((0)),
    [Name] nvarchar(128) NOT NULL,
    PRIMARY KEY(SeatId),
    FOREIGN KEY (BusId) REFERENCES Bus(BusId)
);;

CREATE TABLE Ticket(
    TicketId int NOT NULL,
    [Status] int NOT NULL DEFAULT ((0)),
    Price int NOT NULL DEFAULT ((0)),
    TripId int NOT NULL,
    SeatId int NOT NULL,
    BookedDate datetime NOT NULL,
    UserId int NOT NULL,
    [From] nvarchar(128),
    [To] nvarchar(128),
    PRIMARY KEY(TicketId),
    FOREIGN KEY (TripId) REFERENCES Trip(TripId),
    FOREIGN KEY (SeatId) REFERENCES Seat(SeatId),
    FOREIGN KEY (UserId) REFERENCES [User](UserId)
);;

BEGIN TRANSACTION;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Bus' AND COLUMN_NAME = 'SeatCount')
BEGIN
    ALTER TABLE Bus
    DROP COLUMN SeatCount;
END;
go

CREATE OR ALTER FUNCTION dbo.GetSeatCountForBus(@busId INT)
RETURNS INT
AS
BEGIN
    DECLARE @count INT;
    SELECT @count = COUNT(*) FROM Seat WHERE BusId = @busId;
    RETURN @count;
END;

GO
ALTER TABLE Bus add SeatCount as dbo.GetSeatCountForBus(Bus.BusId);
COMMIT;

INSERT into [User] ([UserId], [Email], [Fullname], [Phone], [Address], [RoleId], [Password]) values 
(0, 'ngodinhkhoinguyen69@gmail.com', 'Ngo Dinh Khoi Nguyen', '349-548-8233', '69 Asplie Lica', 0, '123456'),
(1, 'akenewel0@gravatar.com', 'Ainsley Kenewel', '521-396-4437', '8 Alpine Junction', 3, '123456'),
(2, 'dguidi1@google.it', 'Dannie Guidi', '834-807-3579', '3134 Talisman Avenue', 2, '123456'),
(3, 'greppaport2@bloglines.com', 'Gypsy Reppaport', '178-549-8246', '6 Rockefeller Trail', 0, '123456'),
(4, 'gsercombe3@jiathis.com', 'Gill Sercombe', '210-267-1214', '103 Montana Crossing', 0, '123456'),
(5, 'wchestney4@time.com', 'Whitaker Chestney', '765-243-8331', '57 Lakeland Court', 3, '123456'),
(6, 'jpease5@networksolutions.com', 'Justin Pease', '648-570-8710', '398 Sunbrook Parkway', 3, '123456'),
(7, 'amadgin6@amazon.de', 'Ayn Madgin', '854-464-2842', '39843 Talisman Hill', 0, '123456'),
(8, 'rhalleday7@economist.com', 'Rex Halleday', '541-709-3920', '56 Crescent Oaks Place', 0, '123456'),
(9, 'fgonnel8@wiley.com', 'Frederigo Gonnel', '750-973-2099', '74 Karstens Trail', 3, '123456'),
(10, 'jpeirazzi9@yahoo.co.jp', 'Juliana Peirazzi', '391-480-0322', '50 Lyons Park', 3, '123456');

INSERT INTO Bus(BusId,[Name],LicensePlate,Deleted) VALUES('1','Sdasad','41A23435','false'),('2','Sdasad','43278322','false'),('3','Sdasad','23434322','false'),('4','Sdasad','23423234','false'),('5','Sdasad','23423423','false'),('7','Sdasad','34534532','false'),('10','Sdasad','34534534','false'),('11','Sdasad','23425464','false'),('12','Sdasad','34657432','false'),('13','Sdasad','43543434','false'),('14','From the nam','47A82783','false');

INSERT INTO [Route](RouteId,[From],[To],BasePrice,Deleted) VALUES('1','Can Tho','Ca Mau','10000','false'),('2','Can Tho','Singapore','10000','false'),('3','Can Tho','Can Tho','10000','false'),('4','Can Tho','Can Tho','10000','false'),('5','Can Tho','Can Tho','10000','false'),('6','Can Tho','Can Tho','10000','false'),('7','Can Tho','Can Tho 2','10000','false'),('8','Can Tho','Can Tho','10000','true'),('9','Can Tho','Can Tho','10000','true'),('10','Can Tho','Can Tho','10000','true'),('11','Can Tho','Can Tho','10000','false'),('12','Can Tho','Can Tho 3','10000','false'),('13','Can Tho','Can Tho','10000','true'),('14','Can Tho','Can Tho','10000','false'),('15','Can Tho','Can Tho','10000','false'),('16','Can Tho','Can Tho','10000','false'),('17','Can Tho','Can Tho','10000','false'),('18','Can Tho','Ca Mau','500000','false'),('19','Can Tho','Ca Mau','200000','false'),('20','Can Tho','Can Tho','10000','true'),('21','Long An','Ho Chi Minh','2000','true'),('22','Long An','Ha Noi','500000','true'),('23','Long An','Ho Chi Minh','500000','true');

INSERT INTO Seat(SeatId,BusId,Price,Deleted,[Name]) VALUES('21','1','250000','false','A1'),('22','1','250000','false','A2'),('23','1','250000','false','A3'),('24','1','250000','false','A4'),('25','1','450000','false','A5'),('26','1','250000','false','A6'),('27','1','250000','false','A7'),('28','1','250000','false','A8'),('29','1','250000','false','A9'),('30','1','250000','false','A10'),('31','1','250000','false','A11'),('32','2','26000','false','A11'),('33','2','250000','false','A11'),('34','2','250000','false','A11'),('35','2','250000','false','A11'),('36','2','250000','false','A11'),('37','2','250000','false','A11'),('38','2','250000','false','A11'),('39','2','250000','false','A11'),('40','2','250000','false','A11'),('41','2','250000','false','A11'),('42','3','250000','false','A11'),('43','3','250000','false','A11'),('44','3','250000','false','A11'),('45','3','250000','false','A11'),('46','3','250000','false','A11'),('47','3','250000','false','A11'),('48','3','250000','false','A11'),('49','3','250000','false','A11'),('50','3','250000','false','A11'),('51','3','250000','false','A11'),('52','3','250000','false','A11'),('53','3','250000','false','A11'),('54','3','250000','false','A11'),('55','3','250000','false','A11'),('56','4','26000','false','A11'),('57','4','26000','false','A11'),('58','4','26000','false','A11'),('59','4','26000','false','A11'),('60','4','26000','false','A11'),('61','4','26000','false','A11'),('62','4','26000','false','A11'),('63','4','26000','false','A11'),('64','4','26000','false','A11'),('65','4','26000','false','A11'),('66','4','26000','false','A11'),('67','4','26000','false','A11'),('68','4','26000','false','A11'),('69','4','26000','false','A11'),('70','4','26000','false','A11'),('71','1','250000','false','B11'),('72','1','250000','false','B11'),('73','1','250000','false','B11'),('74','1','250000','false','B11'),('75','1','250000','false','B11'),('76','1','250000','false','B11'),('77','1','250000','false','B11'),('78','1','250000','false','B11'),('79','1','250000','false','B11'),('80','1','250000','false','B11'),('81','1','250000','false','B11'),('82','1','250000','false','B11'),('83','1','250000','false','B11'),('84','1','250000','false','B11'),('85','1','250000','false','B11'),('86','1','250000','false','B11'),('87','1','250000','false','B11'),('88','1','250000','false','B11'),('89','1','250000','false','B11'),('90','1','250000','false','B11'),('91','1','250000','false','B11'),('92','2','250000','false','B11'),('93','2','250000','false','B11'),('94','2','250000','false','B11'),('95','2','250000','false','B11'),('96','2','250000','false','B11'),('97','2','250000','false','B11'),('98','2','250000','false','B11'),('99','2','250000','false','B11'),('100','2','250000','false','B11'),('101','2','250000','false','B11'),('102','2','250000','false','B11'),('103','2','250000','false','B11'),('104','2','250000','false','B11'),('105','2','250000','false','B11'),('106','2','250000','false','B11'),('107','2','250000','false','B11'),('108','2','250000','false','B11'),('109','2','250000','false','B11'),('110','2','250000','false','B11'),('111','2','250000','false','B11'),('112','2','250000','false','B11'),('113','2','250000','false','B11'),('114','2','250000','false','B11'),('115','2','250000','false','B11'),('116','2','250000','false','B11'),('117','2','250000','false','B11'),('118','2','250000','false','B11'),('119','2','250000','false','B11'),('120','2','250000','false','B11'),('121','2','250000','false','B11'),('122','2','250000','false','B11'),('123','2','250000','false','B11'),('124','2','250000','false','B11'),('125','2','250000','false','B11'),('126','2','250000','false','B11'),('127','2','250000','false','B11'),('128','2','250000','false','B11'),('129','2','250000','false','B11'),('130','2','250000','false','B11'),('131','2','250000','false','B11'),('132','2','250000','false','B11'),('133','2','250000','false','B11'),('134','14','300000','false','A1'),('135','14','300000','false','A2'),('136','14','300000','false','A3'),('137','14','300000','false','A4'),('138','14','300000','false','A5'),('139','14','300000','false','A6'),('140','14','300000','false','A7'),('141','14','300000','false','A8'),('142','14','300000','false','A9'),('143','14','300000','false','A10'),('144','14','300000','false','A11'),('145','14','300000','false','A12'),('146','14','300000','false','A13'),('147','14','300000','false','A14'),('148','14','300000','false','A15'),('149','14','300000','false','A16'),('150','14','300000','false','A17'),('151','14','300000','false','A18'),('152','14','300000','false','A19'),('153','14','300000','false','A20'),('154','14','300000','false','A21'),('155','14','300000','false','A22'),('156','14','300000','false','A23'),('157','14','300000','false','A24'),('158','14','300000','false','A25'),('159','14','300000','false','A26'),('160','14','300000','false','A27'),('161','14','300000','false','A28'),('162','14','300000','false','A29'),('163','14','300000','false','A30'),('164','14','300000','false','A31'),('165','14','300000','false','A32'),('166','14','300000','false','A33'),('167','14','300000','false','A34'),('168','14','300000','false','A35'),('169','14','300000','false','A36'),('170','14','300000','false','A37'),('171','14','300000','false','A38'),('172','14','300000','false','A39'),('173','14','300000','false','A40'),('174','14','300000','false','A41'),('175','14','300000','false','A42'),('176','14','300000','false','A43'),('177','14','300000','false','A44'),('178','14','300000','false','A45'),('179','14','300000','false','A46'),('180','14','300000','false','A47'),('181','14','300000','false','A48'),('182','14','300000','false','A49'),('183','14','300000','false','A50'),('184','14','300000','false','A51'),('185','14','300000','false','A52'),('186','14','300000','false','A53'),('187','14','300000','false','A54'),('188','14','300000','false','A55'),('189','14','300000','false','A56'),('190','14','300000','false','A57'),('191','14','300000','false','A58'),('192','14','300000','false','A59'),('193','14','300000','false','A60'),('194','14','300000','false','A61'),('195','14','300000','false','A62'),('196','14','300000','false','A63'),('197','14','300000','false','A64'),('198','14','300000','false','A65'),('199','14','300000','false','A66'),('200','14','300000','false','A67'),('201','14','300000','false','A68'),('202','14','300000','false','A69'),('203','14','300000','false','A70'),('204','14','300000','false','A71'),('205','14','300000','false','A72'),('206','14','300000','false','A73'),('207','14','300000','false','A74'),('208','14','300000','false','A75'),('209','14','300000','false','A76'),('210','14','300000','false','A77'),('211','14','300000','false','A78'),('212','14','300000','false','A79'),('213','14','300000','false','A80'),('214','14','300000','false','A81'),('215','14','300000','false','A82'),('216','14','300000','false','A83'),('217','14','300000','false','A84'),('218','14','300000','false','A85'),('219','14','300000','false','A86'),('220','14','300000','false','A87'),('221','14','300000','false','A88'),('222','14','300000','false','A89'),('223','14','300000','false','A90'),('224','14','300000','false','A91'),('225','14','300000','false','A92'),('226','14','300000','false','A93'),('227','14','300000','false','A94'),('228','14','300000','false','A95'),('229','14','300000','false','A96'),('230','14','300000','false','A97'),('231','14','300000','false','A98'),('232','14','300000','false','A99'),('233','14','300000','false','A100'),('234','14','300000','false','A101'),('235','14','300000','false','A102');

INSERT INTO Trip(TripId,RouteId,StartDate,EndDate,BusId) VALUES('2','2','2022-11-13 08:14:27','2022-11-15 08:14:27','2'),('3','2','2022-11-15 21:00:00','2022-11-15 22:00:00','2'),('4','2','2022-11-15 19:00:00','2022-11-15 22:00:00','3'),('5','2','2022-11-15 18:00:00','2022-11-15 22:00:00','4'),('6','2','2022-11-15 17:00:00','2022-11-15 22:00:00','5'),('35','2','2022-11-15 20:00:00','2022-11-15 22:00:00','3'),('36','2','2022-11-22 06:49:02','2022-11-22 11:49:02','14'),('37','17','2022-10-30 15:35:47','2022-11-08 15:35:47','14');
