-- =============================================
-- EV Dealer Management System - DB Script (Match exactly with DBML)
-- Database: evdms_database
-- Date: November 17, 2025
-- =============================================

-- Drop and recreate database
ALTER DATABASE evdms_database
    SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
DROP DATABASE IF EXISTS evdms_database;
GO

CREATE DATABASE evdms_database;
GO

USE evdms_database;
GO

-- Drop existing tables if any (safe way)
DECLARE @sql NVARCHAR(MAX) = '';
SELECT @sql += 'ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(parent_object_id)) + '.' 
               + QUOTENAME(OBJECT_NAME(parent_object_id)) 
               + ' DROP CONSTRAINT ' + QUOTENAME(name) + ';' + CHAR(13)
FROM sys.foreign_keys;

IF LEN(@sql) > 0 EXEC sp_executesql @sql;

DROP TABLE IF EXISTS 
    Order_Item, Payment, [Order], Schedule, Station_Car, 
    Vehicle, Station, Account_Role, Account, [Role];
GO

-- =============================================
-- Create Tables (exactly as in DBML)
-- =============================================

-- Role
CREATE TABLE [Role] (
    role_id     INT IDENTITY(1,1) PRIMARY KEY,
    role_name   NVARCHAR(255) NOT NULL,
    created_at  DATETIME      NOT NULL DEFAULT GETDATE(),
    updated_at  DATETIME,
    isActive    BIT           NOT NULL DEFAULT 1
);
PRINT 'Table Role created';

-- Account
CREATE TABLE Account (
    account_id      INT IDENTITY(1,1) PRIMARY KEY,
    username        NVARCHAR(255) NOT NULL,
    password        NVARCHAR(255) NOT NULL,
    email           NVARCHAR(255) NOT NULL,
    contact_number  NVARCHAR(255),
    created_at      DATETIME      NOT NULL DEFAULT GETDATE(),
    updated_at      DATETIME,
    isActive        BIT           NOT NULL DEFAULT 1
);
PRINT 'Table Account created';

-- Account_Role
CREATE TABLE Account_Role (
    account_role_id INT IDENTITY(1,1) PRIMARY KEY,
    account_id      INT NOT NULL,
    role_id         INT NOT NULL,
    created_at      DATETIME NOT NULL DEFAULT GETDATE(),
    updated_at      DATETIME,
    isActive        BIT      NOT NULL DEFAULT 1,

    CONSTRAINT FK_Account_Role_Account 
        FOREIGN KEY (account_id) REFERENCES Account(account_id) ON DELETE CASCADE,
    CONSTRAINT FK_Account_Role_Role 
        FOREIGN KEY (role_id)    REFERENCES [Role](role_id)     ON DELETE CASCADE
);
PRINT 'Table Account_Role created';

-- Station
CREATE TABLE Station (
    station_id      INT IDENTITY(1,1) PRIMARY KEY,
    station_name    NVARCHAR(255) NOT NULL,
    location        NVARCHAR(255) NOT NULL,
    contact_number  NVARCHAR(255),
    created_at      DATETIME      NOT NULL DEFAULT GETDATE(),
    updated_at      DATETIME,
    isActive        BIT           NOT NULL DEFAULT 1
);
PRINT 'Table Station created';

-- Vehicle
CREATE TABLE Vehicle (
    vehicle_id       INT IDENTITY(1,1) PRIMARY KEY,
    model            NVARCHAR(255) NOT NULL,
    type             NVARCHAR(255) NOT NULL,
    color            NVARCHAR(255),
    price            DECIMAL(12,2) NOT NULL,
    manufacturer     NVARCHAR(255) NOT NULL,   -- [not null] in DBML
    battery_capacity INT,
    range            INT,
    created_at       DATETIME      NOT NULL DEFAULT GETDATE(),
    updated_at       DATETIME,
    isActive         BIT           NOT NULL DEFAULT 1
);
PRINT 'Table Vehicle created';

-- Station_Car
CREATE TABLE Station_Car (
    station_car_id  INT IDENTITY(1,1) PRIMARY KEY,
    vehicle_id      INT NOT NULL,
    station_id      INT NOT NULL,
    quantity        INT NOT NULL,
    created_at      DATETIME NOT NULL DEFAULT GETDATE(),
    updated_at      DATETIME,
    isActive        BIT      NOT NULL DEFAULT 1,

    CONSTRAINT FK_Station_Car_Vehicle 
        FOREIGN KEY (vehicle_id) REFERENCES Vehicle(vehicle_id) ON DELETE CASCADE,
    CONSTRAINT FK_Station_Car_Station 
        FOREIGN KEY (station_id) REFERENCES Station(station_id) ON DELETE CASCADE
);
PRINT 'Table Station_Car created';

-- Schedule
CREATE TABLE Schedule (
    schedule_id     INT IDENTITY(1,1) PRIMARY KEY,
    customer_id     INT NOT NULL,
    station_car_id  INT NOT NULL,
    status          NVARCHAR(255) NOT NULL,
    schedule_time   DATETIME      NOT NULL,
    created_at      DATETIME      NOT NULL DEFAULT GETDATE(),
    updated_at      DATETIME,
    isActive        BIT           NOT NULL DEFAULT 1,

    CONSTRAINT FK_Schedule_Customer 
        FOREIGN KEY (customer_id)    REFERENCES Account(account_id)    ON DELETE CASCADE,
    CONSTRAINT FK_Schedule_StationCar 
        FOREIGN KEY (station_car_id) REFERENCES Station_Car(station_car_id) ON DELETE CASCADE
);
PRINT 'Table Schedule created';

-- Order
CREATE TABLE [Order] (
    order_id        INT IDENTITY(1,1) PRIMARY KEY,
    customer_id     INT NOT NULL,
    order_date      DATETIME      NOT NULL DEFAULT GETDATE(),
    total_price     DECIMAL(12,2) NOT NULL,
    status          NVARCHAR(255) NOT NULL,
    staff_id        INT NOT NULL,
    created_at      DATETIME      NOT NULL DEFAULT GETDATE(),
    updated_at      DATETIME,
    isActive        BIT           NOT NULL DEFAULT 1,

    CONSTRAINT FK_Order_Customer 
        FOREIGN KEY (customer_id) REFERENCES Account(account_id),
    CONSTRAINT FK_Order_Staff 
        FOREIGN KEY (staff_id)    REFERENCES Account(account_id)
);
PRINT 'Table Order created';

-- Order_Item
CREATE TABLE Order_Item (
    order_item_id   INT IDENTITY(1,1) PRIMARY KEY,
    order_id        INT NOT NULL,
    station_car_id  INT NOT NULL,
    quantity        INT NOT NULL,
    price           DECIMAL(12,2) NOT NULL,
    created_at      DATETIME NOT NULL DEFAULT GETDATE(),
    updated_at      DATETIME,
    isActive        BIT      NOT NULL DEFAULT 1,

    CONSTRAINT FK_OrderItem_Order 
        FOREIGN KEY (order_id)       REFERENCES [Order](order_id)       ON DELETE CASCADE,
    CONSTRAINT FK_OrderItem_StationCar 
        FOREIGN KEY (station_car_id) REFERENCES Station_Car(station_car_id) ON DELETE CASCADE
);
PRINT 'Table Order_Item created';

-- Payment
CREATE TABLE Payment (
    payment_id      INT IDENTITY(1,1) PRIMARY KEY,
    order_id        INT NOT NULL,
    amount          DECIMAL(12,2) NOT NULL,
    payment_date    DATETIME      NOT NULL DEFAULT GETDATE(),
    payment_method  NVARCHAR(255) NOT NULL,
    status          NVARCHAR(255) NOT NULL,
    created_at      DATETIME      NOT NULL DEFAULT GETDATE(),
    updated_at      DATETIME,
    isActive        BIT           NOT NULL DEFAULT 1,

    CONSTRAINT FK_Payment_Order 
        FOREIGN KEY (order_id) REFERENCES [Order](order_id) ON DELETE CASCADE
);
PRINT 'Table Payment created';

PRINT 'All tables created successfully (exactly matching DBML)';

-- =============================================
-- Insert Sample Data (still valid with the new schema)
-- =============================================
SET IDENTITY_INSERT [Role] ON;
INSERT INTO [Role] (role_id, role_name, created_at) VALUES
(1, 'Admin',     GETDATE()),
(2, 'Staff',     GETDATE()),
(3, 'Customer',  GETDATE());
SET IDENTITY_INSERT [Role] OFF;

SET IDENTITY_INSERT Account ON;
INSERT INTO Account (account_id, username, password, email, contact_number, created_at) VALUES
(1, 'admin1',    'pass123', 'admin@ev.com',    '0901000001', GETDATE()),
(2, 'staff1',    'pass456', 'staff@ev.com',    '0902000002', GETDATE()),
(3, 'cust1',     'pass789', 'cust1@ev.com',    '0903000003', GETDATE()),
(4, 'cust2',     'pass101', 'cust2@ev.com',    '0904000004', GETDATE());
SET IDENTITY_INSERT Account OFF;

INSERT INTO Account_Role (account_id, role_id, created_at) VALUES
(1,1,GETDATE()), (2,2,GETDATE()), (3,3,GETDATE()), (4,3,GETDATE());

INSERT INTO Station (station_name, location, contact_number) VALUES
('Hà Nội Station', '123 Hà Nội', '0241234567'),
('HCMC Station',   '456 TP.HCM', '0287654321');

SET IDENTITY_INSERT Vehicle ON;
INSERT INTO Vehicle (vehicle_id, model, type, color, price, manufacturer, battery_capacity, range, created_at) VALUES
(1, 'VinFast VF8',  'SUV',    'Red',   45000.00, 'VinFast', 90, 420, GETDATE()),
(2, 'Tesla Model Y','SUV',    'White', 55000.00, 'Tesla',   75, 500, GETDATE());
SET IDENTITY_INSERT Vehicle OFF;

SET IDENTITY_INSERT Station_Car ON;
INSERT INTO Station_Car (station_car_id, vehicle_id, station_id, quantity) VALUES
(1, 1, 1, 15),
(2, 2, 2, 8);
SET IDENTITY_INSERT Station_Car OFF;

INSERT INTO Schedule (customer_id, station_car_id, status, schedule_time) VALUES
(3, 1, 'Scheduled', '2025-12-01 10:00:00'),
(4, 2, 'Confirmed', '2025-12-05 14:30:00');

SET IDENTITY_INSERT [Order] ON;
INSERT INTO [Order] (order_id, customer_id, order_date, total_price, status, staff_id) VALUES
(1, 3, GETDATE(), 45000.00, 'Processing', 2),
(2, 4, GETDATE(), 55000.00, 'Confirmed',  2);
SET IDENTITY_INSERT [Order] OFF;

INSERT INTO Order_Item (order_id, station_car_id, quantity, price) VALUES
(1, 1, 1, 45000.00),
(2, 2, 1, 55000.00);

INSERT INTO Payment (order_id, amount, payment_method, status) VALUES
(1, 45000.00, 'Bank Transfer', 'Completed'),
(2, 55000.00, 'Credit Card',   'Pending');

PRINT 'Sample data inserted successfully';

USE evdms_database;
GO

-- 1. Role (đã có 3, thêm 1 cái nữa cho đủ 4)
INSERT INTO [Role] (role_name) VALUES 
('Manager'),
('Sales'),
('Technician'),
('Guest');
PRINT 'Added more Roles';

-- 2. Account (đã có 4, thêm 6 cái nữa)
INSERT INTO Account (username, password, email, contact_number) VALUES
('manager1',   'mgr123',  'manager1@ev.com',   '0911111111'),
('sales1',     'sales123','sales1@ev.com',     '0912222222'),
('tech1',      'tech123', 'tech1@ev.com',      '0913333333'),
('cust5',      'cust555', 'cust5@gmail.com',   '0925555555'),
('cust6',      'cust666', 'cust6@yahoo.com',   '0926666666'),
('guest_user', 'guest99','guest@ev.com',      '0939999999');
PRINT 'Added more Accounts';

-- 3. Account_Role (gán role cho các account mới)
INSERT INTO Account_Role (account_id, role_id) VALUES
(5, 4),  -- manager1 → Manager
(6, 5),  -- sales1   → Sales
(7, 6),  -- tech1    → Technician
(8, 3),  -- cust5    → Customer
(9, 3),  -- cust6    → Customer
(10,7);  -- guest_user → Guest
PRINT 'Added more Account_Role';

-- 4. Station (đã có 2, thêm 3 cái nữa)
INSERT INTO Station (station_name, location, contact_number) VALUES
('Đà Nẵng Station',   '88 Lê Duẩn, Đà Nẵng',   '0236888888'),
('Cần Thơ Station',   '123 Nguyễn Văn Cừ, Cần Thơ', '0292999999'),
('Nha Trang Station', '45 Trần Phú, Nha Trang', '0258777777');
PRINT 'Added more Stations';

-- 5. Vehicle (đã có 2, thêm 6 cái nữa)
INSERT INTO Vehicle (model, type, color, price, manufacturer, battery_capacity, range) VALUES
('VinFast VF9',     'SUV',     'Blue',     58000.00, 'VinFast',   100, 450),
('VinFast VF e34',  'Sedan',   'Silver',   28000.00, 'VinFast',    42, 285),
('Tesla Model 3',   'Sedan',   'Black',    48000.00, 'Tesla',      75, 550),
('BYD Atto 3',      'SUV',     'Green',    35000.00, 'BYD',        60, 420),
('KIA EV6',         'Crossover','Red',      52000.00, 'KIA',        77, 510),
('Hyundai Ioniq 5', 'SUV',     'White',    49000.00, 'Hyundai',    72, 480);
PRINT 'Added more Vehicles';

-- 6. Station_Car (kho xe tại các trạm)
INSERT INTO Station_Car (vehicle_id, station_id, quantity) VALUES
(1, 1, 12),  -- VF8 tại Hà Nội
(2, 2, 20),  -- Model Y tại HCM
(3, 1, 8),   -- VF9 tại Hà Nội
(3, 3, 15),  -- VF9 tại Đà Nẵng
(4, 2, 25),  -- VF e34 tại HCM
(5, 4, 10),  -- Model 3 tại Cần Thơ
(6, 3, 18),  -- BYD Atto 3 tại Đà Nẵng
(7, 5, 7),   -- KIA EV6 tại Nha Trang
(8, 1, 14);  -- Ioniq 5 tại Hà Nội
PRINT 'Added more Station_Car';

-- 7. Schedule (lịch hẹn lái thử)
INSERT INTO Schedule (customer_id, station_car_id, status, schedule_time) VALUES
(3, 1, 'Completed',   '2025-11-10 09:00:00'),
(8, 3, 'Scheduled',   '2025-11-20 14:00:00'),
(9, 5, 'Confirmed',   '2025-11-22 10:30:00'),
(4, 7, 'Cancelled',   '2025-11-15 16:00:00'),
(10,2, 'Scheduled',   '2025-11-25 11:00:00');
PRINT 'Added more Schedules';

-- 8. Order (đơn hàng)
INSERT INTO [Order] (customer_id, order_date, total_price, status, staff_id) VALUES
(3, '2025-11-05 10:15:00', 58000.00, 'Delivered',   6),  -- sales1
(8, '2025-11-08 14:22:00', 28000.00, 'Processing',  2),
(9, '2025-11-12 09:40:00', 48000.00, 'Shipped',     6),
(4, '2025-11-14 17:55:00', 52000.00, 'Confirmed',   2);
PRINT 'Added more Orders';

-- 9. Order_Item (chi tiết đơn hàng)
INSERT INTO Order_Item (order_id, station_car_id, quantity, price) VALUES
(3, 3, 1, 58000.00),  -- VF9
(4, 5, 1, 28000.00),  -- VF e34
(5, 6, 1, 48000.00),  -- Model 3
(6, 8, 1, 52000.00);  -- KIA EV6
PRINT 'Added more Order_Items';

-- 10. Payment (thanh toán)
INSERT INTO Payment (order_id, amount, payment_date, payment_method, status) VALUES
(3, 58000.00, '2025-11-06 08:30:00', 'Bank Transfer',   'Completed'),
(4, 14000.00, '2025-11-09 11:00:00', 'Installment',     'Processing'),  -- trả góp 50%
(5, 48000.00, '2025-11-13 15:20:00', 'Credit Card',     'Completed'),
(6, 52000.00, '2025-11-15 09:10:00', 'Cash',            'Completed');
PRINT 'Added more Payments';

PRINT '=== ĐÃ THÊM XONG 3–6 BẢN GHI CHO MỌI BẢNG ===';

-- List created tables
SELECT name AS TableName 
FROM sys.tables 
WHERE name IN ('Role','Account','Account_Role','Station','Vehicle','Station_Car','Schedule','Order','Order_Item','Payment')
ORDER BY name;
GO

