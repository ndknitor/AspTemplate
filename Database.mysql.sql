create database Etdb;
use Etdb;
CREATE TABLE `User` (
    UserId INT NOT NULL,
    Email VARCHAR(128) NOT NULL,
    Fullname VARCHAR(128) NOT NULL DEFAULT '',
    Phone VARCHAR(16) NOT NULL DEFAULT '',
    Address VARCHAR(128) NOT NULL DEFAULT '',
    RoleId INT NOT NULL DEFAULT 0,
    Password VARCHAR(128) NOT NULL DEFAULT '',
    PRIMARY KEY (UserId)
);

CREATE TABLE Bus (
    BusId INT NOT NULL,
    Name VARCHAR(128),
    LicensePlate VARCHAR(16) NOT NULL,
    Deleted BIT NOT NULL DEFAULT 0,
    PRIMARY KEY (BusId)
);

CREATE TABLE `Route` (
    RouteId INT NOT NULL,
    `From` VARCHAR(128) NOT NULL DEFAULT '',
    `To` VARCHAR(128) NOT NULL DEFAULT '',
    BasePrice INT NOT NULL,
    Deleted BIT NOT NULL,
    PRIMARY KEY (RouteId)
);

CREATE TABLE Trip (
    TripId INT NOT NULL,
    RouteId INT NOT NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    BusId INT NOT NULL,
    PRIMARY KEY (TripId),
    FOREIGN KEY (RouteId) REFERENCES `Route`(RouteId)
);

CREATE TABLE Seat (
    SeatId INT NOT NULL,
    BusId INT NOT NULL,
    Price INT NOT NULL DEFAULT 0,
    Deleted BIT NOT NULL DEFAULT 0,
    Name VARCHAR(128) NOT NULL,
    PRIMARY KEY (SeatId),
    FOREIGN KEY (BusId) REFERENCES Bus(BusId)
);

CREATE TABLE Ticket (
    TicketId INT NOT NULL,
    Status INT NOT NULL DEFAULT 0,
    Price INT NOT NULL DEFAULT 0,
    TripId INT NOT NULL,
    SeatId INT NOT NULL,
    BookedDate DATETIME NOT NULL,
    UserId INT NOT NULL,
    `From` VARCHAR(128),
    `To` VARCHAR(128),
    PRIMARY KEY (TicketId),
    FOREIGN KEY (TripId) REFERENCES Trip(TripId),
    FOREIGN KEY (SeatId) REFERENCES Seat(SeatId),
    FOREIGN KEY (UserId) REFERENCES `User`(UserId)
);

INSERT into `User` (`UserId`, `Email`, `Fullname`, `Phone`, `Address`, `RoleId`, `Password`) values 
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


INSERT INTO `Bus` (`BusId`,`Name`,`LicensePlate`,`Deleted`) VALUES
  ('1','Sdasad','41A23435',0),
  ('2','Sdasad','43278322',0),
  ('3','Sdasad','23434322',0),
  ('4','Sdasad','23423234',0),
  ('5','Sdasad','23423423',0),
  ('7','Sdasad','34534532',0),
  ('10','Sdasad','34534534',0),
  ('11','Sdasad','23425464',0),
  ('12','Sdasad','34657432',0),
  ('13','Sdasad','43543434',0),
  ('14','From the nam','47A82783',0);

INSERT INTO `Route`(`RouteId`,`From`,`To`,`BasePrice`,`Deleted`) VALUES
('1','Can Tho','Ca Mau','10000',0)
,('2','Can Tho','Singapore','10000',0)
,('3','Can Tho','Can Tho','10000',0)
,('4','Can Tho','Can Tho','10000',0)
,('5','Can Tho','Can Tho','10000',0)
,('6','Can Tho','Can Tho','10000',0)
,('7','Can Tho','Can Tho 2','10000',0)
,('8','Can Tho','Can Tho','10000',1)
,('9','Can Tho','Can Tho','10000',1)
,('10','Can Tho','Can Tho','10000',1)
,('11','Can Tho','Can Tho','10000',0)
,('12','Can Tho','Can Tho 3','10000',0)
,('13','Can Tho','Can Tho','10000',1)
,('14','Can Tho','Can Tho','10000',0)
,('15','Can Tho','Can Tho','10000',0)
,('16','Can Tho','Can Tho','10000',0)
,('17','Can Tho','Can Tho','10000',0)
,('18','Can Tho','Ca Mau','500000',0)
,('19','Can Tho','Ca Mau','200000',0)
,('20','Can Tho','Can Tho','10000',1)
,('21','Long An','Ho Chi Minh','2000',1)
,('22','Long An','Ha Noi','500000',1)
,('23','Long An','Ho Chi Minh','500000',1);

INSERT INTO Seat(SeatId,BusId,Price,Deleted,[Name]) VALUES('21','1','250000',0,'A1')
,('22','1','250000',0,'A2')
,('23','1','250000',0,'A3')
,('24','1','250000',0,'A4')
,('25','1','450000',0,'A5')
,('26','1','250000',0,'A6')
,('27','1','250000',0,'A7')
,('28','1','250000',0,'A8')
,('29','1','250000',0,'A9')
,('30','1','250000',0,'A10')
,('31','1','250000',0,'A11')
,('32','2','26000',0,'A11')
,('33','2','250000',0,'A11')
,('34','2','250000',0,'A11')
,('35','2','250000',0,'A11')
,('36','2','250000',0,'A11')
,('37','2','250000',0,'A11')
,('38','2','250000',0,'A11')
,('55','3','250000',0,'A11')
,('56','4','26000',0,'A11')
,('57','4','26000',0,'A11')
,('58','4','26000',0,'A11')
,('59','4','26000',0,'A11')
,('60','4','26000',0,'A11')
,('61','4','26000',0,'A11')
,('62','4','26000',0,'A11')
,('63','4','26000',0,'A11')
,('64','4','26000',0,'A11')
,('65','4','26000',0,'A11')
,('66','4','26000',0,'A11')
,('67','4','26000',0,'A11')
,('68','4','26000',0,'A11')
,('69','4','26000',0,'A11')
,('70','4','26000',0,'A11')
,('71','1','250000',0,'B11')
,('72','1','250000',0,'B11')
,('73','1','250000',0,'B11')
,('74','1','250000',0,'B11')
,('75','1','250000',0,'B11')
,('76','1','250000',0,'B11')
,('77','1','250000',0,'B11')
,('78','1','250000',0,'B11')
,('99','2','250000',0,'B11')
,('100','2','250000',0,'B11')
,('101','2','250000',0,'B11')
,('102','2','250000',0,'B11')
,('103','2','250000',0,'B11')
,('104','2','250000',0,'B11')
,('105','2','250000',0,'B11')
,('106','2','250000',0,'B11')
,('107','2','250000',0,'B11')
,('108','2','250000',0,'B11')
,('109','2','250000',0,'B11')
,('110','2','250000',0,'B11')
,('111','2','250000',0,'B11')
,('112','2','250000',0,'B11')
,('113','2','250000',0,'B11')
,('114','2','250000',0,'B11')
,('115','2','250000',0,'B11')
,('116','2','250000',0,'B11')
,('134','14','300000',0,'A1')
,('135','14','300000',0,'A2')
,('136','14','300000',0,'A3')
,('137','14','300000',0,'A4')
,('138','14','300000',0,'A5')
,('139','14','300000',0,'A6')
,('140','14','300000',0,'A7')
,('141','14','300000',0,'A8')
,('142','14','300000',0,'A9')
,('143','14','300000',0,'A10')
,('144','14','300000',0,'A11')
,('145','14','300000',0,'A12')
,('146','14','300000',0,'A13')
,('147','14','300000',0,'A14')
,('148','14','300000',0,'A15')
,('149','14','300000',0,'A16')
,('150','14','300000',0,'A17')
,('151','14','300000',0,'A18')
,('152','14','300000',0,'A19')
,('153','14','300000',0,'A20')
,('154','14','300000',0,'A21')
,('155','14','300000',0,'A22')
,('156','14','300000',0,'A23')
,('157','14','300000',0,'A24')
,('158','14','300000',0,'A25')
,('159','14','300000',0,'A26')
,('160','14','300000',0,'A27')
,('161','14','300000',0,'A28')
,('162','14','300000',0,'A29')
,('163','14','300000',0,'A30');

INSERT INTO Trip(TripId,RouteId,StartDate,EndDate,BusId) VALUES('2','2','2022-11-13 08:14:27','2022-11-15 08:14:27','2')
,('3','2','2022-11-15 21:00:00','2022-11-15 22:00:00','2')
,('4','2','2022-11-15 19:00:00','2022-11-15 22:00:00','3')
,('5','2','2022-11-15 18:00:00','2022-11-15 22:00:00','4')
,('6','2','2022-11-15 17:00:00','2022-11-15 22:00:00','5')
,('35','2','2022-11-15 20:00:00','2022-11-15 22:00:00','3')
,('36','2','2022-11-22 06:49:02','2022-11-22 11:49:02','14')
,('37','17','2022-10-30 15:35:47','2022-11-08 15:35:47','14');
