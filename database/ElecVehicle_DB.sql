-- Drop and recreate the database
ALTER DATABASE evdms_database
SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

DROP DATABASE IF EXISTS evdms_database;
GO

CREATE DATABASE evdms_database;
GO
USE evdms_database;
GO

-- Drop existing tables and constraints
BEGIN TRY
    DECLARE @sql NVARCHAR(MAX) = '';
    SELECT @sql += 'ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(parent_object_id)) + '.' + 
                   QUOTENAME(OBJECT_NAME(parent_object_id)) + 
                   ' DROP CONSTRAINT ' + QUOTENAME(name) + ';'
    FROM sys.foreign_keys
    WHERE parent_object_id IN (SELECT object_id FROM sys.tables 
        WHERE name IN ('Account', 'Role', 'Account_Role', 'Station', 'Vehicle', 'Station_Car', 
                       'Promotion', 'Schedule', 'Order', 'Contract', 'Payment', 'Feedback', 
                       'Report', 'Staff_Revenue'));
    IF @sql != ''
        EXEC sp_executesql @sql;

    DROP TABLE IF EXISTS Account, Role, Account_Role, Station, Vehicle, Station_Car, 
        Promotion, Schedule, [Order], Contract, Payment, Feedback, Report, Staff_Revenue;

    PRINT 'All existing tables and constraints dropped.';
END TRY
BEGIN CATCH
    PRINT 'Error dropping tables/constraints: ' + ERROR_MESSAGE();
END CATCH
GO

-- Create tables
BEGIN TRY
    -- Role
    CREATE TABLE [Role] (
        role_id INT IDENTITY(1,1) PRIMARY KEY,
        role_name NVARCHAR(255) NOT NULL,
        created_at DATETIME NOT NULL DEFAULT GETDATE(),
        updated_at DATETIME,
        isActive BIT NOT NULL DEFAULT 1
    );
    PRINT 'Table [Role] created.';

    -- Account
    CREATE TABLE [Account] (
        account_id INT IDENTITY(1,1) PRIMARY KEY,
        username NVARCHAR(255) NOT NULL,
        password NVARCHAR(255) NOT NULL,
        email NVARCHAR(255) NOT NULL,
        contact_number NVARCHAR(255),
        created_at DATETIME NOT NULL DEFAULT GETDATE(),
        updated_at DATETIME,
        isActive BIT NOT NULL DEFAULT 1
    );
    PRINT 'Table [Account] created.';

    -- Account_Role
    CREATE TABLE [Account_Role] (
        account_role_id INT IDENTITY(1,1) PRIMARY KEY,
        account_id INT NOT NULL,
        role_id INT NOT NULL,
        created_at DATETIME NOT NULL DEFAULT GETDATE(),
        updated_at DATETIME,
        isActive BIT NOT NULL DEFAULT 1,
        CONSTRAINT FK__Account_Role__account_id FOREIGN KEY (account_id) REFERENCES [Account](account_id) ON DELETE CASCADE,
        CONSTRAINT FK__Account_Role__role_id FOREIGN KEY (role_id) REFERENCES [Role](role_id) ON DELETE CASCADE
    );
    PRINT 'Table [Account_Role] created.';

    -- Station
    CREATE TABLE [Station] (
        station_id INT IDENTITY(1,1) PRIMARY KEY,
        station_name NVARCHAR(255) NOT NULL,
        location NVARCHAR(255) NOT NULL,
        contact_number NVARCHAR(255),
        created_at DATETIME NOT NULL DEFAULT GETDATE(),
        updated_at DATETIME,
        isActive BIT NOT NULL DEFAULT 1
    );
    PRINT 'Table [Station] created.';

    -- Vehicle
    CREATE TABLE [Vehicle] (
        vehicle_id INT IDENTITY(1,1) PRIMARY KEY,
        model NVARCHAR(255) NOT NULL,
        type NVARCHAR(255) NOT NULL,
        color NVARCHAR(255),
        price DECIMAL(12,2) NOT NULL,
        created_at DATETIME NOT NULL DEFAULT GETDATE(),
        updated_at DATETIME,
        isActive BIT NOT NULL DEFAULT 1
    );
    PRINT 'Table [Vehicle] created.';

    -- Station_Car
    CREATE TABLE [Station_Car] (
        station_car_id INT IDENTITY(1,1) PRIMARY KEY,
        vehicle_id INT NOT NULL,
        station_id INT NOT NULL,
        quantity INT NOT NULL,
        created_at DATETIME NOT NULL DEFAULT GETDATE(),
        updated_at DATETIME,
        isActive BIT NOT NULL DEFAULT 1,
        CONSTRAINT FK__Station_Car__vehicle_id FOREIGN KEY (vehicle_id) REFERENCES [Vehicle](vehicle_id) ON DELETE CASCADE,
        CONSTRAINT FK__Station_Car__station_id FOREIGN KEY (station_id) REFERENCES [Station](station_id) ON DELETE CASCADE
    );
    PRINT 'Table [Station_Car] created.';

    -- Promotion
    CREATE TABLE [Promotion] (
        promotion_id INT IDENTITY(1,1) PRIMARY KEY,
        promo_code NVARCHAR(255) NOT NULL,
        discount_percentage DECIMAL(5,2) NOT NULL,
        start_date DATETIME NOT NULL,
        end_date DATETIME NOT NULL,
        applicable_to NVARCHAR(255),
        station_id INT,
        created_at DATETIME NOT NULL DEFAULT GETDATE(),
        updated_at DATETIME,
        isActive BIT NOT NULL DEFAULT 1,
        CONSTRAINT FK__Promotion__station_id FOREIGN KEY (station_id) REFERENCES [Station](station_id) ON DELETE SET NULL
    );
    PRINT 'Table [Promotion] created.';

    -- Schedule
    CREATE TABLE [Schedule] (
        schedule_id INT IDENTITY(1,1) PRIMARY KEY,
        customer_id INT NOT NULL,
        station_car_id INT NOT NULL,
        status NVARCHAR(255) NOT NULL,
        schedule_time DATETIME NOT NULL,
        created_at DATETIME NOT NULL DEFAULT GETDATE(),
        updated_at DATETIME,
        isActive BIT NOT NULL DEFAULT 1,
        CONSTRAINT FK__Schedule__customer_id FOREIGN KEY (customer_id) REFERENCES [Account](account_id) ON DELETE CASCADE,
        CONSTRAINT FK__Schedule__station_car_id FOREIGN KEY (station_car_id) REFERENCES [Station_Car](station_car_id) ON DELETE CASCADE
    );
    PRINT 'Table [Schedule] created.';

    -- Order
    CREATE TABLE [Order] (
        order_id INT IDENTITY(1,1) PRIMARY KEY,
        customer_id INT NOT NULL,
        station_car_id INT NOT NULL,
        order_date DATETIME NOT NULL DEFAULT GETDATE(),
        total_price DECIMAL(12,2) NOT NULL,
        status NVARCHAR(255) NOT NULL,
        promotion_id INT,
        staff_id INT NOT NULL,
        created_at DATETIME NOT NULL DEFAULT GETDATE(),
        updated_at DATETIME,
        isActive BIT NOT NULL DEFAULT 1,
        CONSTRAINT FK__Order__customer_id FOREIGN KEY (customer_id) REFERENCES [Account](account_id) ON DELETE NO ACTION,
        CONSTRAINT FK__Order__station_car_id FOREIGN KEY (station_car_id) REFERENCES [Station_Car](station_car_id) ON DELETE NO ACTION,
        CONSTRAINT FK__Order__promotion_id FOREIGN KEY (promotion_id) REFERENCES [Promotion](promotion_id) ON DELETE SET NULL,
        CONSTRAINT FK__Order__staff_id FOREIGN KEY (staff_id) REFERENCES [Account](account_id) ON DELETE NO ACTION
    );
    PRINT 'Table [Order] created.';

    -- Contract
    CREATE TABLE [Contract] (
        contract_id INT IDENTITY(1,1) PRIMARY KEY,
        order_id INT NOT NULL,
        contract_date DATETIME NOT NULL DEFAULT GETDATE(),
        terms TEXT NOT NULL,
        signature NVARCHAR(255),
        status NVARCHAR(255) NOT NULL,
        created_at DATETIME NOT NULL DEFAULT GETDATE(),
        updated_at DATETIME,
        isActive BIT NOT NULL DEFAULT 1,
        CONSTRAINT FK__Contract__order_id FOREIGN KEY (order_id) REFERENCES [Order](order_id) ON DELETE NO ACTION
    );
    PRINT 'Table [Contract] created.';

    -- Payment
    CREATE TABLE [Payment] (
        payment_id INT IDENTITY(1,1) PRIMARY KEY,
        order_id INT NOT NULL,
        amount DECIMAL(12,2) NOT NULL,
        payment_date DATETIME NOT NULL DEFAULT GETDATE(),
        payment_method NVARCHAR(255) NOT NULL,
        status NVARCHAR(255) NOT NULL,
        created_at DATETIME NOT NULL DEFAULT GETDATE(),
        updated_at DATETIME,
        isActive BIT NOT NULL DEFAULT 1,
        CONSTRAINT FK__Payment__order_id FOREIGN KEY (order_id) REFERENCES [Order](order_id) ON DELETE NO ACTION
    );
    PRINT 'Table [Payment] created.';

    -- Feedback
    CREATE TABLE [Feedback] (
        feedback_id INT IDENTITY(1,1) PRIMARY KEY,
        customer_id INT NOT NULL,
        vehicle_id INT NOT NULL,
        rating INT NOT NULL CHECK (rating BETWEEN 1 AND 5),
        comment TEXT,
        feedback_date DATETIME NOT NULL DEFAULT GETDATE(),
        created_at DATETIME NOT NULL DEFAULT GETDATE(),
        updated_at DATETIME,
        isActive BIT NOT NULL DEFAULT 1,
        CONSTRAINT FK__Feedback__customer_id FOREIGN KEY (customer_id) REFERENCES [Account](account_id) ON DELETE CASCADE,
        CONSTRAINT FK__Feedback__vehicle_id FOREIGN KEY (vehicle_id) REFERENCES [Vehicle](vehicle_id) ON DELETE CASCADE
    );
    PRINT 'Table [Feedback] created.';

    -- Report
    CREATE TABLE [Report] (
        report_id INT IDENTITY(1,1) PRIMARY KEY,
        report_type NVARCHAR(255) NOT NULL,
        generated_date DATETIME NOT NULL DEFAULT GETDATE(),
        data NVARCHAR(MAX) NOT NULL,
        account_id INT NOT NULL,
        created_at DATETIME NOT NULL DEFAULT GETDATE(),
        updated_at DATETIME,
        isActive BIT NOT NULL DEFAULT 1,
        CONSTRAINT FK__Report__account_id FOREIGN KEY (account_id) REFERENCES [Account](account_id) ON DELETE NO ACTION
    );
    PRINT 'Table [Report] created.';

    -- Staff_Revenue
    CREATE TABLE [Staff_Revenue] (
        staff_revenue_id INT IDENTITY(1,1) PRIMARY KEY,
        staff_id INT NOT NULL,
        revenue_date DATETIME NOT NULL DEFAULT GETDATE(),
        total_revenue DECIMAL(12,2) NOT NULL,
        commission DECIMAL(12,2),
        created_at DATETIME NOT NULL DEFAULT GETDATE(),
        updated_at DATETIME,
        isActive BIT NOT NULL DEFAULT 1,
        CONSTRAINT FK__Staff_Revenue__staff_id FOREIGN KEY (staff_id) REFERENCES [Account](account_id) ON DELETE CASCADE
    );
    PRINT 'Table [Staff_Revenue] created.';

    PRINT 'All tables created successfully.';
END TRY
BEGIN CATCH
    PRINT 'Error creating tables: ' + ERROR_MESSAGE();
END CATCH
GO

-- Insert sample data
BEGIN TRY
    -- Role
    INSERT INTO [Role] (role_name, created_at) VALUES
    ('Admin', GETDATE()),
    ('Staff', GETDATE()),
    ('Customer', GETDATE());

    -- Account
    INSERT INTO [Account] (username, password, email, contact_number, created_at) VALUES
    ('admin1', 'pass123', 'admin1@email.com', '0901234567', GETDATE()),
    ('staff1', 'pass456', 'staff1@email.com', '0902345678', GETDATE()),
    ('customer1', 'pass789', 'customer1@email.com', '0903456789', GETDATE()),
    ('customer2', 'pass101', 'customer2@email.com', '0904567890', GETDATE());

    -- Account_Role
    INSERT INTO [Account_Role] (account_id, role_id, created_at) VALUES
    (1, 1, GETDATE()),
    (2, 2, GETDATE()),
    (3, 3, GETDATE()),
    (4, 3, GETDATE());

    -- Station
    INSERT INTO [Station] (station_name, location, contact_number, created_at) VALUES
    ('Hanoi Station', '123 Hanoi St', '0901112223', GETDATE()),
    ('HCMC Station', '456 HCMC St', '0902223334', GETDATE());

    -- Vehicle
    INSERT INTO [Vehicle] (model, type, color, price, created_at) VALUES
    ('Model X', 'Sedan', 'Black', 50000.00, GETDATE()),
    ('Model Y', 'SUV', 'White', 60000.00, GETDATE());

    -- Station_Car
    INSERT INTO [Station_Car] (vehicle_id, station_id, quantity, created_at) VALUES
    (1, 1, 10, GETDATE()),
    (2, 2, 5, GETDATE());

    -- Promotion
    INSERT INTO [Promotion] (promo_code, discount_percentage, start_date, end_date, applicable_to, station_id, created_at) VALUES
    ('SUMMER25', 10.00, '2025-06-01', '2025-08-31', 'All Vehicles', 1, GETDATE()),
    ('YEAR_END', 15.00, '2025-12-01', '2025-12-31', 'SUV', 2, GETDATE());

    -- Schedule
    INSERT INTO [Schedule] (customer_id, station_car_id, status, schedule_time, created_at) VALUES
    (3, 1, 'Scheduled', '2025-10-01 10:00:00', GETDATE()),
    (4, 2, 'Pending', '2025-10-02 14:00:00', GETDATE());

    -- Order
    INSERT INTO [Order] (customer_id, station_car_id, order_date, total_price, status, promotion_id, staff_id, created_at) VALUES
    (3, 1, GETDATE(), 45000.00, 'Shipped', 1, 2, GETDATE()),
    (4, 2, GETDATE(), 51000.00, 'Pending', 2, 2, GETDATE());

    -- Contract
    INSERT INTO [Contract] (order_id, contract_date, terms, signature, status, created_at) VALUES
    (1, GETDATE(), 'Pay within 30 days', 'Customer1', 'Signed', GETDATE()),
    (2, GETDATE(), 'Installment plan', NULL, 'Draft', GETDATE());

    -- Payment
    INSERT INTO [Payment] (order_id, amount, payment_date, payment_method, status, created_at) VALUES
    (1, 45000.00, GETDATE(), 'Cash', 'Completed', GETDATE()),
    (2, 51000.00, GETDATE(), 'Installment', 'Pending', GETDATE());

    -- Feedback
    INSERT INTO [Feedback] (customer_id, vehicle_id, rating, comment, feedback_date, created_at) VALUES
    (3, 1, 5, 'Great vehicle!', GETDATE(), GETDATE()),
    (4, 2, 3, 'Delivery delayed', GETDATE(), GETDATE());

    -- Report
    INSERT INTO [Report] (report_type, generated_date, data, account_id, created_at) VALUES
    ('Sales', GETDATE(), '{"total_sales": 45000.00, "orders": 1}', 2, GETDATE()),
    ('Inventory', GETDATE(), '{"model_x_stock": 10, "model_y_stock": 5}', 1, GETDATE());

    -- Staff_Revenue
    INSERT INTO [Staff_Revenue] (staff_id, revenue_date, total_revenue, commission, created_at) VALUES
    (2, GETDATE(), 45000.00, 4500.00, GETDATE()),
    (2, GETDATE(), 51000.00, 5100.00, GETDATE());

    PRINT 'Sample data inserted successfully.';
END TRY
BEGIN CATCH
    PRINT 'Error inserting sample data: ' + ERROR_MESSAGE();
END CATCH
GO

-- Check created tables
SELECT name AS TableName 
FROM sys.tables 
WHERE name IN ('Account', 'Role', 'Account_Role', 'Station', 'Vehicle', 'Station_Car', 
               'Promotion', 'Schedule', 'Order', 'Contract', 'Payment', 'Feedback', 
               'Report', 'Staff_Revenue')
ORDER BY name;
GO