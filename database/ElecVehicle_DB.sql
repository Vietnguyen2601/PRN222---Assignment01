-- Tạo hoặc sử dụng database
IF DB_ID('evdms_database') IS NULL
    CREATE DATABASE evdms_database;
GO
USE evdms_database;
GO

-- Xóa các foreign key constraints và bảng
BEGIN TRY
    -- Xóa foreign key constraints động
    DECLARE @sql NVARCHAR(MAX) = '';
    SELECT @sql += 'ALTER TABLE ' + QUOTENAME(OBJECT_NAME(parent_object_id)) + 
                   ' DROP CONSTRAINT ' + QUOTENAME(name) + ';'
    FROM sys.foreign_keys
    WHERE parent_object_id IN (SELECT object_id FROM sys.tables WHERE name LIKE 'evdms_%');
    IF @sql != ''
        EXEC sp_executesql @sql;

    -- Xóa bảng
    DROP TABLE IF EXISTS evdms_reports, evdms_payments, evdms_contracts, evdms_quotes, 
        evdms_vehicle_bookings, evdms_test_drives, evdms_feedback, evdms_promotions, 
        evdms_orders, evdms_vehicle_features, evdms_vehicles, evdms_customers, 
        evdms_accounts, evdms_users, evdms_roles;
    PRINT 'Existing tables and constraints dropped.';
END TRY
BEGIN CATCH
    PRINT 'Error dropping tables/constraints: ' + ERROR_MESSAGE();
END CATCH
GO

-- Tạo bảng
BEGIN TRY
    -- Roles
    CREATE TABLE evdms_roles (
        role_id INT IDENTITY(1,1) PRIMARY KEY,
        role_name VARCHAR(50) NOT NULL UNIQUE,
        description TEXT,
        created_at DATETIME DEFAULT GETDATE()
    );

    -- Users
    CREATE TABLE evdms_users (
        user_id INT IDENTITY(1,1) PRIMARY KEY,
        full_name VARCHAR(100) NOT NULL,
        contact_info VARCHAR(100),
        role_id INT NOT NULL,
        created_at DATETIME DEFAULT GETDATE(),
        CONSTRAINT FK__evdms_users__role_id FOREIGN KEY (role_id) REFERENCES evdms_roles(role_id) ON DELETE NO ACTION
    );

    -- Accounts
    CREATE TABLE evdms_accounts (
        account_id INT IDENTITY(1,1) PRIMARY KEY,
        user_id INT NOT NULL,
        username VARCHAR(50) NOT NULL UNIQUE,
        email VARCHAR(100) NOT NULL UNIQUE,
        Password VARCHAR(255) NOT NULL,
        refresh_token VARCHAR(255),
        token_expiry DATETIME,
        last_login DATETIME,
        is_active BIT DEFAULT 1,
        created_at DATETIME DEFAULT GETDATE(),
        CONSTRAINT FK__evdms_accounts__user_id FOREIGN KEY (user_id) REFERENCES evdms_users(user_id) ON DELETE CASCADE
    );

    -- Customers
    CREATE TABLE evdms_customers (
        customer_id INT IDENTITY(1,1) PRIMARY KEY,
        full_name VARCHAR(100) NOT NULL,
        contact_info VARCHAR(100) NOT NULL,
        address TEXT,
        transaction_history NVARCHAR(MAX),
        assigned_staff_id INT NULL,
        created_at DATETIME DEFAULT GETDATE(),
        CONSTRAINT FK__evdms_customers__assigned_staff_id FOREIGN KEY (assigned_staff_id) REFERENCES evdms_users(user_id) ON DELETE SET NULL
    );

    -- Vehicles
    CREATE TABLE evdms_vehicles (
        vehicle_id INT IDENTITY(1,1) PRIMARY KEY,
        model_name VARCHAR(100) NOT NULL,
        type VARCHAR(50),
        price DECIMAL(12, 2) NOT NULL,
        stock_quantity INT NOT NULL,
        configuration NVARCHAR(MAX),
        created_at DATETIME DEFAULT GETDATE()
    );

    -- Vehicle Features
    CREATE TABLE evdms_vehicle_features (
        feature_id INT IDENTITY(1,1) PRIMARY KEY,
        vehicle_id INT NOT NULL,
        feature_name VARCHAR(100) NOT NULL,
        feature_value TEXT,
        CONSTRAINT FK__evdms_vehicle_features__vehicle_id FOREIGN KEY (vehicle_id) REFERENCES evdms_vehicles(vehicle_id) ON DELETE CASCADE
    );

    -- Promotions
    CREATE TABLE evdms_promotions (
        promotion_id INT IDENTITY(1,1) PRIMARY KEY,
        promotion_name VARCHAR(100) NOT NULL,
        description TEXT,
        discount_percentage DECIMAL(5, 2),
        start_date DATETIME NOT NULL,
        end_date DATETIME NOT NULL,
        created_at DATETIME DEFAULT GETDATE()
    );

    -- Quotes
    CREATE TABLE evdms_quotes (
        quote_id INT IDENTITY(1,1) PRIMARY KEY,
        customer_id INT NOT NULL,
        vehicle_id INT NOT NULL,
        user_id INT NOT NULL,
        promotion_id INT NULL,
        quote_amount DECIMAL(12, 2) NOT NULL,
        quote_date DATETIME DEFAULT GETDATE(),
        status VARCHAR(20) NOT NULL DEFAULT 'Pending' 
            CHECK (status IN ('Pending', 'Accepted', 'Rejected')),
        CONSTRAINT FK__evdms_quotes__customer_id FOREIGN KEY (customer_id) REFERENCES evdms_customers(customer_id) ON DELETE CASCADE,
        CONSTRAINT FK__evdms_quotes__vehicle_id FOREIGN KEY (vehicle_id) REFERENCES evdms_vehicles(vehicle_id) ON DELETE CASCADE,
        CONSTRAINT FK__evdms_quotes__user_id FOREIGN KEY (user_id) REFERENCES evdms_users(user_id) ON DELETE CASCADE,
        CONSTRAINT FK__evdms_quotes__promotion_id FOREIGN KEY (promotion_id) REFERENCES evdms_promotions(promotion_id) ON DELETE SET NULL
    );

    -- Orders
    CREATE TABLE evdms_orders (
        order_id INT IDENTITY(1,1) PRIMARY KEY,
        customer_id INT NULL,
        vehicle_id INT NULL,
        user_id INT NULL,
        quote_id INT NULL,
        order_date DATETIME DEFAULT GETDATE(),
        quantity INT NOT NULL,
        total_amount DECIMAL(12, 2) NOT NULL,
        status VARCHAR(20) NOT NULL DEFAULT 'Pending' 
            CHECK (status IN ('Pending', 'Processing', 'Shipped', 'Delivered', 'Cancelled')),
        delivery_status VARCHAR(50) NULL,
        tracking_info TEXT,
        approved_by INT NULL,
        approved_date DATETIME NULL,
        CONSTRAINT FK__evdms_orders__customer_id FOREIGN KEY (customer_id) REFERENCES evdms_customers(customer_id) ON DELETE SET NULL,
        CONSTRAINT FK__evdms_orders__vehicle_id FOREIGN KEY (vehicle_id) REFERENCES evdms_vehicles(vehicle_id) ON DELETE SET NULL,
        CONSTRAINT FK__evdms_orders__user_id FOREIGN KEY (user_id) REFERENCES evdms_users(user_id) ON DELETE SET NULL,
        CONSTRAINT FK__evdms_orders__quote_id FOREIGN KEY (quote_id) REFERENCES evdms_quotes(quote_id) ON DELETE NO ACTION,
        CONSTRAINT FK__evdms_orders__approved_by FOREIGN KEY (approved_by) REFERENCES evdms_users(user_id) ON DELETE NO ACTION
    );

    -- Contracts
    CREATE TABLE evdms_contracts (
        contract_id INT IDENTITY(1,1) PRIMARY KEY,
        order_id INT NOT NULL,
        customer_id INT NOT NULL,
        contract_details NVARCHAR(MAX),
        contract_date DATETIME DEFAULT GETDATE(),
        status VARCHAR(20) NOT NULL DEFAULT 'Draft' 
            CHECK (status IN ('Draft', 'Signed', 'Terminated')),
        CONSTRAINT FK__evdms_contracts__order_id FOREIGN KEY (order_id) REFERENCES evdms_orders(order_id) ON DELETE CASCADE,
        CONSTRAINT FK__evdms_contracts__customer_id FOREIGN KEY (customer_id) REFERENCES evdms_customers(customer_id) ON DELETE NO ACTION
    );

    -- Vehicle Bookings
    CREATE TABLE evdms_vehicle_bookings (
        booking_id INT IDENTITY(1,1) PRIMARY KEY,
        vehicle_id INT NOT NULL,
        user_id INT NOT NULL,
        quantity INT NOT NULL,
        booking_date DATETIME DEFAULT GETDATE(),
        status VARCHAR(20) NOT NULL DEFAULT 'Pending' 
            CHECK (status IN ('Pending', 'Approved', 'Rejected')),
        CONSTRAINT FK__evdms_vehicle_bookings__vehicle_id FOREIGN KEY (vehicle_id) REFERENCES evdms_vehicles(vehicle_id) ON DELETE CASCADE,
        CONSTRAINT FK__evdms_vehicle_bookings__user_id FOREIGN KEY (user_id) REFERENCES evdms_users(user_id) ON DELETE CASCADE
    );

    -- Payments
    CREATE TABLE evdms_payments (
        payment_id INT IDENTITY(1,1) PRIMARY KEY,
        order_id INT NOT NULL,
        customer_id INT NOT NULL,
        payment_type VARCHAR(20) NOT NULL 
            CHECK (payment_type IN ('Direct', 'Installment')),
        amount DECIMAL(12, 2) NOT NULL,
        payment_date DATETIME DEFAULT GETDATE(),
        installment_plan NVARCHAR(MAX),
        debt_amount DECIMAL(12, 2) DEFAULT 0.00,
        status VARCHAR(20) NOT NULL DEFAULT 'Pending'
            CHECK (status IN ('Pending', 'Completed', 'Overdue')),
        CONSTRAINT FK__evdms_payments__order_id FOREIGN KEY (order_id) REFERENCES evdms_orders(order_id) ON DELETE CASCADE,
        CONSTRAINT FK__evdms_payments__customer_id FOREIGN KEY (customer_id) REFERENCES evdms_customers(customer_id) ON DELETE NO ACTION
    );

    -- Test Drives
    CREATE TABLE evdms_test_drives (
        test_drive_id INT IDENTITY(1,1) PRIMARY KEY,
        customer_id INT NOT NULL,
        vehicle_id INT NOT NULL,
        user_id INT NOT NULL,
        test_drive_date DATETIME NOT NULL,
        status VARCHAR(20) NOT NULL DEFAULT 'Scheduled' 
            CHECK (status IN ('Scheduled', 'Completed', 'Cancelled')),
        CONSTRAINT FK__evdms_test_drives__customer_id FOREIGN KEY (customer_id) REFERENCES evdms_customers(customer_id) ON DELETE CASCADE,
        CONSTRAINT FK__evdms_test_drives__vehicle_id FOREIGN KEY (vehicle_id) REFERENCES evdms_vehicles(vehicle_id) ON DELETE CASCADE,
        CONSTRAINT FK__evdms_test_drives__user_id FOREIGN KEY (user_id) REFERENCES evdms_users(user_id) ON DELETE CASCADE
    );

    -- Feedback
    CREATE TABLE evdms_feedback (
        feedback_id INT IDENTITY(1,1) PRIMARY KEY,
        customer_id INT NOT NULL,
        user_id INT NOT NULL,
        feedback_type VARCHAR(20) NOT NULL 
            CHECK (feedback_type IN ('Feedback', 'Complaint')),
        feedback_content TEXT NOT NULL,
        feedback_date DATETIME DEFAULT GETDATE(),
        status VARCHAR(20) NOT NULL DEFAULT 'Open' 
            CHECK (status IN ('Open', 'Resolved', 'Escalated')),
        CONSTRAINT FK__evdms_feedback__customer_id FOREIGN KEY (customer_id) REFERENCES evdms_customers(customer_id) ON DELETE CASCADE,
        CONSTRAINT FK__evdms_feedback__user_id FOREIGN KEY (user_id) REFERENCES evdms_users(user_id) ON DELETE CASCADE
    );

    -- Reports
    CREATE TABLE evdms_reports (
        report_id INT IDENTITY(1,1) PRIMARY KEY,
        user_id INT NULL,
        report_type VARCHAR(50) NULL CHECK (
            report_type IN ('Sales', 'Inventory', 'Customer', 'System', 'SalesBySalesperson', 'Debt', 'CustomerReport')
        ),
        report_data NVARCHAR(MAX),
        generated_at DATETIME DEFAULT GETDATE(),
        CONSTRAINT FK__evdms_reports__user_id FOREIGN KEY (user_id) REFERENCES evdms_users(user_id) ON DELETE SET NULL
    );

    PRINT 'All tables created successfully.';
END TRY
BEGIN CATCH
    PRINT 'Error creating tables: ' + ERROR_MESSAGE();
END CATCH
GO

-- Chèn dữ liệu mẫu
    -- Roles
    INSERT INTO evdms_roles (role_name, description) VALUES
    ('DealerStaff', 'Nhân viên đại lý, xử lý đơn hàng cơ bản'),
    ('DealerManager', 'Quản lý đại lý, có quyền duyệt đơn hàng'),
    ('EVMStaff', 'Nhân viên EVM, quản lý xe và kho'),
    ('Admin', 'Quản trị viên hệ thống'),
    ('SuperAdmin', 'Quản trị viên cấp cao, toàn quyền hệ thống'),
    ('Guest', 'Khách, quyền truy cập hạn chế');

    -- Users
    INSERT INTO evdms_users (full_name, contact_info, role_id) VALUES
    ('Nguyen Van A', '0901234567', 1),
    ('Tran Van B', '0902345678', 2),
    ('Le Thi C', '0903456789', 3),
    ('Pham Van D', '0904567890', 4),
    ('Hoang Van E', '0905678901', 5),
    ('Khach Moi', '0906789012', 6);

    -- Accounts
    INSERT INTO evdms_accounts (user_id, username, email, Password, is_active) VALUES
    (1, 'staff1', 'a.nguyen@email.com', 'password1', 1),
    (2, 'manager1', 'b.tran@email.com', 'password2', 1),
    (3, 'evm1', 'c.le@email.com', 'password3', 1),
    (4, 'admin1', 'd.pham@email.com', 'password4', 1),
    (5, 'superadmin1', 'superadmin@email.com', 'password5', 1),
    (6, 'guest1', 'guest@email.com', 'password6', 1);

    -- Customers
    INSERT INTO evdms_customers (full_name, contact_info, address, transaction_history, assigned_staff_id) VALUES
    ('Hoang Van E', 'e.hoang@email.com', '123 Hanoi', '{"orders": 1, "total_spent": 50000.00}', 1),
    ('Pham Thi F', 'f.pham@email.com', '456 HCMC', '{"orders": 2, "total_spent": 120000.00}', 2);

    -- Vehicles
    INSERT INTO evdms_vehicles (model_name, type, price, stock_quantity, configuration) VALUES
    ('Model X', 'Sedan', 50000.00, 10, '{"engine": "2.0L", "color": "Black"}'),
    ('Model Y', 'SUV', 60000.00, 5, '{"engine": "3.0L", "color": "White"}');

    -- Vehicle Features
    INSERT INTO evdms_vehicle_features (vehicle_id, feature_name, feature_value) VALUES
    (1, 'Top Speed', '200 km/h'),
    (1, 'Fuel Type', 'Petrol'),
    (2, 'Top Speed', '220 km/h'),
    (2, 'Fuel Type', 'Hybrid');

    -- Promotions
    INSERT INTO evdms_promotions (promotion_name, description, discount_percentage, start_date, end_date) VALUES
    ('Summer Sale', 'Giảm giá mùa hè', 10.00, '2025-06-01', '2025-08-31'),
    ('Year-End Promo', 'Khuyến mãi cuối năm', 15.00, '2025-12-01', '2025-12-31');

    -- Quotes
    INSERT INTO evdms_quotes (customer_id, vehicle_id, user_id, promotion_id, quote_amount, status) VALUES
    (1, 1, 1, 1, 45000.00, 'Accepted'),
    (2, 2, 2, 2, 102000.00, 'Pending');

    -- Orders
    INSERT INTO evdms_orders (customer_id, vehicle_id, user_id, quote_id, quantity, total_amount, status, delivery_status, tracking_info, approved_by, approved_date) VALUES
    (1, 1, 1, 1, 1, 45000.00, 'Shipped', 'In Transit', 'Shipped to Hanoi', 2, GETDATE()),
    (2, 2, 2, 2, 2, 102000.00, 'Processing', 'Preparing', 'In transit to HCMC', NULL, NULL);

    -- Contracts
    INSERT INTO evdms_contracts (order_id, customer_id, contract_details, status) VALUES
    (1, 1, '{"terms": "Pay within 30 days"}', 'Signed'),
    (2, 2, '{"terms": "Installment plan"}', 'Draft');

    -- Vehicle Bookings
    INSERT INTO evdms_vehicle_bookings (vehicle_id, user_id, quantity, status) VALUES
    (1, 1, 2, 'Approved'),
    (2, 2, 1, 'Pending');

    -- Payments
    INSERT INTO evdms_payments (order_id, customer_id, payment_type, amount, installment_plan, debt_amount, status) VALUES
    (1, 1, 'Direct', 45000.00, NULL, 0.00, 'Completed'),
    (2, 2, 'Installment', 51000.00, '{"months": 12, "monthly_payment": 4250.00}', 51000.00, 'Pending');

    -- Test Drives
    INSERT INTO evdms_test_drives (customer_id, vehicle_id, user_id, test_drive_date, status) VALUES
    (1, 1, 1, '2025-09-25 10:00:00', 'Scheduled'),
    (2, 2, 2, '2025-09-26 14:00:00', 'Completed');

    -- Feedback
    INSERT INTO evdms_feedback (customer_id, user_id, feedback_type, feedback_content, status) VALUES
    (1, 1, 'Feedback', 'Great service!', 'Resolved'),
    (2, 2, 'Complaint', 'Delivery delayed', 'Open');

    -- Reports
    INSERT INTO evdms_reports (user_id, report_type, report_data) VALUES
    (1, 'SalesBySalesperson', '{"salesperson_id": 1, "total_sales": 45000.00, "orders": 1}'),
    (2, 'Debt', '{"customer_id": 2, "debt_amount": 51000.00}'),
    (4, 'Inventory', '{"model_x_stock": 10, "model_y_stock": 5}');
GO

-- Kiểm tra các bảng đã tạo
SELECT name AS TableName FROM sys.tables WHERE name LIKE 'evdms_%';
GO