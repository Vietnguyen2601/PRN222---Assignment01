ALTER DATABASE evdms_database
SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

DROP DATABASE evdms_database;

-- Tạo hoặc sử dụng database
IF DB_ID('vehicle_management_database') IS NULL
    CREATE DATABASE evdms_database;
GO
USE evdms_database;
GO

-- Xóa các foreign key constraints và bảng evdms_ cũ
BEGIN TRY
    -- Xóa foreign key constraints động cho cả bảng cũ và mới
    DECLARE @sql NVARCHAR(MAX) = '';
    SELECT @sql += 'ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(parent_object_id)) + '.' + 
                   QUOTENAME(OBJECT_NAME(parent_object_id)) + 
                   ' DROP CONSTRAINT ' + QUOTENAME(name) + ';'
    FROM sys.foreign_keys
    WHERE parent_object_id IN (SELECT object_id FROM sys.tables 
        WHERE name IN ('Account', 'Role', 'Account_Role', 'Station', 'Vehicle', 'Inventory', 
                       'Promotion', 'Orders', 'Contract', 'Payment', 'Feedback', 'Report', 
                       'Staff_Revenue', 'evdms_reports', 'evdms_payments', 'evdms_contracts', 
                       'evdms_quotes', 'evdms_vehicle_bookings', 'evdms_test_drives', 
                       'evdms_feedback', 'evdms_promotions', 'evdms_orders', 
                       'evdms_vehicle_features', 'evdms_vehicles', 'evdms_customers', 
                       'evdms_accounts', 'evdms_users', 'evdms_roles'));
    IF @sql != ''
        EXEC sp_executesql @sql;

    -- Xóa các bảng evdms_ cũ và bảng mới
    DROP TABLE IF EXISTS evdms_reports, evdms_payments, evdms_contracts, evdms_quotes, 
        evdms_vehicle_bookings, evdms_test_drives, evdms_feedback, evdms_promotions, 
        evdms_orders, evdms_vehicle_features, evdms_vehicles, evdms_customers, 
        evdms_accounts, evdms_users, evdms_roles,
        Account, Role, Account_Role, Station, Vehicle, Inventory, 
        Promotion, Orders, Contract, Payment, Feedback, Report, Staff_Revenue;

    PRINT 'All existing tables and constraints dropped.';
END TRY
BEGIN CATCH
    PRINT 'Error dropping tables/constraints: ' + ERROR_MESSAGE();
END CATCH
GO

-- Tạo bảng mới
BEGIN TRY
    -- Role
    CREATE TABLE [Role] (
        role_id INT IDENTITY(1,1) PRIMARY KEY,
        role_name NVARCHAR(255) NOT NULL UNIQUE,
        is_active BIT DEFAULT 1
    );
    PRINT 'Table [Role] created.';

    -- Account
    CREATE TABLE [Account] (
        account_id INT IDENTITY(1,1) PRIMARY KEY,
        username NVARCHAR(255) NOT NULL UNIQUE,
        password NVARCHAR(255) NOT NULL,
        email NVARCHAR(255) NOT NULL UNIQUE,
        contact_number NVARCHAR(255),
        created_at DATETIME DEFAULT GETDATE(),
        is_active BIT DEFAULT 1
    );
    PRINT 'Table [Account] created.';

    -- Account_Role
    CREATE TABLE [Account_Role] (
        account_role_id INT IDENTITY(1,1) PRIMARY KEY,
        account_id INT NOT NULL,
        role_id INT NOT NULL,
        is_active BIT DEFAULT 1,
        CONSTRAINT FK__Account_Role__account_id FOREIGN KEY (account_id) REFERENCES [Account](account_id) ON DELETE CASCADE,
        CONSTRAINT FK__Account_Role__role_id FOREIGN KEY (role_id) REFERENCES [Role](role_id) ON DELETE CASCADE
    );
    PRINT 'Table [Account_Role] created.';

    -- Station
    CREATE TABLE [Station] (
        station_id INT IDENTITY(1,1) PRIMARY KEY,
        station_name NVARCHAR(255) NOT NULL,
        location NVARCHAR(255),
        contact_number NVARCHAR(255),
        admin_id INT,
        is_active BIT DEFAULT 1,
        CONSTRAINT FK__Station__admin_id FOREIGN KEY (admin_id) REFERENCES [Account](account_id) ON DELETE NO ACTION
    );
    PRINT 'Table [Station] created.';

    -- Vehicle
    CREATE TABLE [Vehicle] (
        vehicle_id INT IDENTITY(1,1) PRIMARY KEY,
        model NVARCHAR(255) NOT NULL,
        type NVARCHAR(255),
        color NVARCHAR(255),
        price DECIMAL(12, 2) NOT NULL,
        availability BIT NOT NULL,
        station_id INT,
        is_active BIT DEFAULT 1,
        quantity INT 
    );
    PRINT 'Table [Vehicle] created.';

    -- Inventory
    CREATE TABLE [Inventory] (
        inventory_id INT IDENTITY(1,1) PRIMARY KEY,
        vehicle_id INT NOT NULL,
        station_id INT NOT NULL,
        quantity INT NOT NULL,
        last_updated DATETIME DEFAULT GETDATE(),
        is_active BIT DEFAULT 1,
        CONSTRAINT FK__Inventory__vehicle_id FOREIGN KEY (vehicle_id) REFERENCES [Vehicle](vehicle_id) ON DELETE CASCADE,
        CONSTRAINT FK__Inventory__station_id FOREIGN KEY (station_id) REFERENCES [Station](station_id) ON DELETE CASCADE
    );
    PRINT 'Table [Inventory] created.';

    -- Promotion
    CREATE TABLE [Promotion] (
        promotion_id INT IDENTITY(1,1) PRIMARY KEY,
        promo_code NVARCHAR(255) NOT NULL UNIQUE,
        discount_percentage DECIMAL(5, 2),
        start_date DATETIME NOT NULL,
        end_date DATETIME NOT NULL,
        applicable_to NVARCHAR(255),
        station_id INT,
        is_active BIT DEFAULT 1,
        CONSTRAINT FK__Promotion__station_id FOREIGN KEY (station_id) REFERENCES [Station](station_id) ON DELETE SET NULL
    );
    PRINT 'Table [Promotion] created.';

    -- Orders
    CREATE TABLE [Orders] (
        order_id INT IDENTITY(1,1) PRIMARY KEY,
        customer_id INT,
        inventory_id INT,
        order_date DATETIME DEFAULT GETDATE(),
        total_price DECIMAL(12, 2) NOT NULL,
        status NVARCHAR(255) NOT NULL DEFAULT 'Pending' CHECK (status IN ('Pending', 'Processing', 'Shipped', 'Delivered', 'Cancelled')),
        promotion_id INT,
        staff_id INT,
        is_active BIT DEFAULT 1,
        CONSTRAINT FK__Orders__customer_id FOREIGN KEY (customer_id) REFERENCES [Account](account_id) ON DELETE NO ACTION,
        CONSTRAINT FK__Orders__inventory_id FOREIGN KEY (inventory_id) REFERENCES [Inventory](inventory_id) ON DELETE SET NULL,
        CONSTRAINT FK__Orders__staff_id FOREIGN KEY (staff_id) REFERENCES [Account](account_id) ON DELETE NO ACTION
    );
    IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Orders')
        PRINT 'Table [Orders] created successfully.';
    ELSE
        RAISERROR ('Table [Orders] creation failed.', 16, 1);

    -- Contract
    CREATE TABLE [Contract] (
        contract_id INT IDENTITY(1,1) PRIMARY KEY,
        order_id INT NOT NULL,
        contract_date DATETIME DEFAULT GETDATE(),
        terms TEXT,
        signature NVARCHAR(255),
        status NVARCHAR(255) NOT NULL DEFAULT 'Draft' CHECK (status IN ('Draft', 'Signed', 'Terminated')),
        is_active BIT DEFAULT 1,
        CONSTRAINT FK__Contract__order_id FOREIGN KEY (order_id) REFERENCES [Orders](order_id) ON DELETE NO ACTION
    );
    PRINT 'Table [Contract] created.';

    -- Payment
    CREATE TABLE [Payment] (
        payment_id INT IDENTITY(1,1) PRIMARY KEY,
        order_id INT NOT NULL,
        amount DECIMAL(12, 2) NOT NULL,
        payment_date DATETIME DEFAULT GETDATE(),
        payment_method NVARCHAR(255) NOT NULL CHECK (payment_method IN ('Cash', 'Credit Card', 'Bank Transfer', 'Installment')),
        status NVARCHAR(255) NOT NULL DEFAULT 'Pending' CHECK (status IN ('Pending', 'Completed', 'Failed')),
        is_active BIT DEFAULT 1,
        CONSTRAINT FK__Payment__order_id FOREIGN KEY (order_id) REFERENCES [Orders](order_id) ON DELETE NO ACTION
    );
    PRINT 'Table [Payment] created.';

    -- Feedback
    CREATE TABLE [Feedback] (
        feedback_id INT IDENTITY(1,1) PRIMARY KEY,
        customer_id INT NOT NULL,
        vehicle_id INT NOT NULL,
        rating INT CHECK (rating BETWEEN 1 AND 5),
        comment TEXT,
        feedback_date DATETIME DEFAULT GETDATE(),
        is_active BIT DEFAULT 1,
        CONSTRAINT FK__Feedback__customer_id FOREIGN KEY (customer_id) REFERENCES [Account](account_id) ON DELETE CASCADE,
        CONSTRAINT FK__Feedback__vehicle_id FOREIGN KEY (vehicle_id) REFERENCES [Vehicle](vehicle_id) ON DELETE CASCADE
    );
    PRINT 'Table [Feedback] created.';

    -- Report
    CREATE TABLE [Report] (
        report_id INT IDENTITY(1,1) PRIMARY KEY,
        report_type NVARCHAR(255) CHECK (report_type IN ('Sales', 'Inventory', 'Customer', 'Revenue')),
        generated_date DATETIME DEFAULT GETDATE(),
        data NVARCHAR(MAX),
        account_id INT,
        is_active BIT DEFAULT 1,
        CONSTRAINT FK__Report__account_id FOREIGN KEY (account_id) REFERENCES [Account](account_id) ON DELETE NO ACTION
    );
    PRINT 'Table [Report] created.';

    -- Staff_Revenue
    CREATE TABLE [Staff_Revenue] (
        staff_revenue_id INT IDENTITY(1,1) PRIMARY KEY,
        staff_id INT NOT NULL,
        revenue_date DATETIME DEFAULT GETDATE(),
        total_revenue DECIMAL(12, 2) NOT NULL,
        commission DECIMAL(12, 2),
        is_active BIT DEFAULT 1,
        CONSTRAINT FK__Staff_Revenue__staff_id FOREIGN KEY (staff_id) REFERENCES [Account](account_id) ON DELETE CASCADE
    );
    PRINT 'Table [Staff_Revenue] created.';

    PRINT 'All tables created successfully.';
END TRY
BEGIN CATCH
    PRINT 'Error creating tables: ' + ERROR_MESSAGE();
END CATCH
GO

-- Chèn dữ liệu mẫu
    -- Role
    INSERT INTO [Role] (role_name) VALUES
    ('Admin'), ('Staff'), ('Customer');

    -- Account
    INSERT INTO [Account] (username, password, email, contact_number) VALUES
    ('admin1', 'pass123', 'admin1@email.com', '0901234567'),
    ('staff1', 'pass456', 'staff1@email.com', '0902345678'),
    ('customer1', 'pass789', 'customer1@email.com', '0903456789'),
    ('customer2', 'pass101', 'customer2@email.com', '0904567890');

    -- Account_Role
    INSERT INTO [Account_Role] (account_id, role_id) VALUES
    (1, 1), (2, 2), (3, 3), (4, 3);

    -- Station
    INSERT INTO [Station] (station_name, location, contact_number, admin_id) VALUES
    ('Hanoi Station', '123 Hanoi St', '0901112223', 1),
    ('HCMC Station', '456 HCMC St', '0902223334', 1);

    -- Vehicle
    INSERT INTO [Vehicle] (model, type, color, price, availability, quantity) VALUES
    ('Model X', 'Sedan', 'Black', 50000.00, 1, 1),
    ('Model Y', 'SUV', 'White', 60000.00, 1, 2);

    -- Inventory
    INSERT INTO [Inventory] (vehicle_id, station_id, quantity) VALUES
    (1, 1, 10),
    (2, 2, 5);

    -- Promotion
    INSERT INTO [Promotion] (promo_code, discount_percentage, start_date, end_date, applicable_to, station_id) VALUES
    ('SUMMER25', 10.00, '2025-06-01', '2025-08-31', 'All Vehicles', 1),
    ('YEAR_END', 15.00, '2025-12-01', '2025-12-31', 'SUV', 2);

    -- Orders
    INSERT INTO [Orders] (customer_id, inventory_id, total_price, status, promotion_id, staff_id) VALUES
    (3, 1, 45000.00, 'Shipped', 1, 2),
    (4, 2, 51000.00, 'Pending', 2, 2);

    -- Contract
    INSERT INTO [Contract] (order_id, terms, signature, status) VALUES
    (1, 'Pay within 30 days', 'Customer1', 'Signed'),
    (2, 'Installment plan', NULL, 'Draft');

    -- Payment
    INSERT INTO [Payment] (order_id, amount, payment_method, status) VALUES
    (1, 45000.00, 'Cash', 'Completed'),
    (2, 51000.00, 'Installment', 'Pending');

    -- Feedback
    INSERT INTO [Feedback] (customer_id, vehicle_id, rating, comment) VALUES
    (3, 1, 5, 'Great vehicle!'),
    (4, 2, 3, 'Delivery delayed');

    -- Report
    INSERT INTO [Report] (report_type, data, account_id) VALUES
    ('Sales', '{"total_sales": 45000.00, "orders": 1}', 2),
    ('Inventory', '{"model_x_stock": 10, "model_y_stock": 5}', 1);

    -- Staff_Revenue
    INSERT INTO [Staff_Revenue] (staff_id, total_revenue, commission) VALUES
    (2, 45000.00, 4500.00),
    (2, 51000.00, 5100.00);
GO

-- Kiểm tra các bảng đã tạo
SELECT name AS TableName FROM sys.tables 
WHERE name IN ('Account', 'Role', 'Account_Role', 'Station', 'Vehicle', 'Inventory', 
               'Promotion', 'Orders', 'Contract', 'Payment', 'Feedback', 'Report', 
               'Staff_Revenue')
ORDER BY name;
GO