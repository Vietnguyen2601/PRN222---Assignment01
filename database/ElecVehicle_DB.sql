-- Tạo database
CREATE DATABASE evdms_database;
-- Sử dụng database
USE evdms_database;

-- Xóa bảng nếu tồn tại (theo thứ tự ngược để tránh lỗi khóa ngoại)
DROP TABLE IF EXISTS evdms_reports;
DROP TABLE IF EXISTS evdms_orders;
DROP TABLE IF EXISTS evdms_vehicles;
DROP TABLE IF EXISTS evdms_customers;
DROP TABLE IF EXISTS evdms_users;

-- Tạo bảng Users
CREATE TABLE evdms_users (
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    role VARCHAR(20) NOT NULL CHECK (role IN ('DealerStaff', 'DealerManager', 'EVMStaff', 'Admin')),
    full_name VARCHAR(100),
    contact_info VARCHAR(100),
    created_at DATETIME DEFAULT GETDATE()
);

-- Tạo bảng Customers
CREATE TABLE evdms_customers (
    customer_id INT IDENTITY(1,1) PRIMARY KEY,
    full_name VARCHAR(100) NOT NULL,
    contact_info VARCHAR(100) NOT NULL,
    address TEXT,
    created_at DATETIME DEFAULT GETDATE()
);

-- Tạo bảng Vehicles
CREATE TABLE evdms_vehicles (
    vehicle_id INT IDENTITY(1,1) PRIMARY KEY,
    model_name VARCHAR(100) NOT NULL,
    type VARCHAR(50),
    price DECIMAL(12, 2) NOT NULL,
    stock_quantity INT NOT NULL,
    created_at DATETIME DEFAULT GETDATE()
);

-- Tạo bảng Orders
CREATE TABLE evdms_orders (
    order_id INT IDENTITY(1,1) PRIMARY KEY,
    customer_id INT NULL,
    vehicle_id INT NULL,
    user_id INT NULL,
    order_date DATETIME DEFAULT GETDATE(),
    quantity INT NOT NULL,
    total_amount DECIMAL(12, 2) NOT NULL,
    status VARCHAR(20) NOT NULL DEFAULT 'Pending' CHECK (status IN ('Pending', 'Completed', 'Cancelled')),
    tracking_info TEXT,
    FOREIGN KEY (customer_id) REFERENCES evdms_customers(customer_id) ON DELETE SET NULL,
    FOREIGN KEY (vehicle_id) REFERENCES evdms_vehicles(vehicle_id) ON DELETE SET NULL,
    FOREIGN KEY (user_id) REFERENCES evdms_users(user_id) ON DELETE SET NULL
);

-- Tạo bảng Reports
CREATE TABLE evdms_reports (
    report_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NULL,
    report_type VARCHAR(50) NULL CHECK (report_type IN ('Sales', 'Inventory', 'Customer', 'System')),
    report_data NVARCHAR(MAX),
    generated_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (user_id) REFERENCES evdms_users(user_id) ON DELETE SET NULL
);

-- Thêm dữ liệu mẫu vào bảng evdms_users
INSERT INTO evdms_users (username, password_hash, role, full_name, contact_info) VALUES
('staff1', 'hashed_password_1', 'DealerStaff', 'Nguyen Van A', 'a.nguyen@email.com'),
('manager1', 'hashed_password_2', 'DealerManager', 'Tran Van B', 'b.tran@email.com'),
('evm1', 'hashed_password_3', 'EVMStaff', 'Le Thi C', 'c.le@email.com'),
('admin1', 'hashed_password_4', 'Admin', 'Pham Van D', 'd.pham@email.com');

-- Thêm dữ liệu mẫu vào bảng evdms_customers
INSERT INTO evdms_customers (full_name, contact_info, address) VALUES
('Hoang Van E', 'e.hoang@email.com', '123 Hanoi'),
('Pham Thi F', 'f.pham@email.com', '456 HCMC');

-- Thêm dữ liệu mẫu vào bảng evdms_vehicles
INSERT INTO evdms_vehicles (model_name, type, price, stock_quantity) VALUES
('Model X', 'Sedan', 50000.00, 10),
('Model Y', 'SUV', 60000.00, 5);

-- Thêm dữ liệu mẫu vào bảng evdms_orders
INSERT INTO evdms_orders (customer_id, vehicle_id, user_id, quantity, total_amount, tracking_info) VALUES
(1, 1, 1, 1, 50000.00, 'Shipped to Hanoi'),
(2, 2, 2, 2, 120000.00, 'In transit to HCMC');

-- Thêm dữ liệu mẫu vào bảng evdms_reports
INSERT INTO evdms_reports (user_id, report_type, report_data) VALUES
(2, 'Sales', '{"total_sales": 170000.00, "orders": 2}'),
(4, 'Inventory', '{"model_x_stock": 10, "model_y_stock": 5}');