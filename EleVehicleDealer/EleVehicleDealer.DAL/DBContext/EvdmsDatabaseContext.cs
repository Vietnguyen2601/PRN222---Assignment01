using System;
using System.Collections.Generic;
using EleVehicleDealer.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EleVehicleDealer.DAL.DBContext;

public partial class EvdmsDatabaseContext : DbContext
{
    public EvdmsDatabaseContext()
    {
    }

    public EvdmsDatabaseContext(DbContextOptions<EvdmsDatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<AccountRole> AccountRoles { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<Station> Stations { get; set; }

    public virtual DbSet<StationCar> StationCars { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    public static string GetConnectionString(string connectionStringName)
    {
        // Kiểm tra biến môi trường trước
        string envConnectionString = Environment.GetEnvironmentVariable($"ConnectionStrings__{connectionStringName}");
        if (!string.IsNullOrEmpty(envConnectionString))
        {
            return envConnectionString;
        }

        // Nếu không có biến môi trường, đọc từ appsettings
        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
            .AddEnvironmentVariables() // Thêm biến môi trường vào cấu hình
            .Build();

        string connectionString = config.GetConnectionString(connectionStringName);
        return connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(GetConnectionString("DefaultConnection"))
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Account__46A222CD9CD58163");

            entity.ToTable("Account");

            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.ContactNumber)
                .HasMaxLength(255)
                .HasColumnName("contact_number");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .HasColumnName("username");
        });

        modelBuilder.Entity<AccountRole>(entity =>
        {
            entity.HasKey(e => e.AccountRoleId).HasName("PK__Account___9E0E18311E28CBF2");

            entity.ToTable("Account_Role");

            entity.Property(e => e.AccountRoleId).HasColumnName("account_role_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Account).WithMany(p => p.AccountRoles)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_Account_Role_Account");

            entity.HasOne(d => d.Role).WithMany(p => p.AccountRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_Account_Role_Role");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Order__46596229D0815DB9");

            entity.ToTable("Order");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("order_date");
            entity.Property(e => e.StaffId).HasColumnName("staff_id");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("total_price");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Customer).WithMany(p => p.OrderCustomers)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Customer");

            entity.HasOne(d => d.Staff).WithMany(p => p.OrderStaffs)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Staff");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PK__Order_It__3764B6BC4D7DA032");

            entity.ToTable("Order_Item");

            entity.Property(e => e.OrderItemId).HasColumnName("order_item_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.StationCarId).HasColumnName("station_car_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_OrderItem_Order");

            entity.HasOne(d => d.StationCar).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.StationCarId)
                .HasConstraintName("FK_OrderItem_StationCar");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__ED1FC9EA12286A2D");

            entity.ToTable("Payment");

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("payment_date");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(255)
                .HasColumnName("payment_method");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_Payment_Order");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__760965CCACB0F1FD");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");
            entity.Property(e => e.RoleName)
                .HasMaxLength(255)
                .HasColumnName("role_name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__Schedule__C46A8A6F1EA39C43");

            entity.ToTable("Schedule");

            entity.Property(e => e.ScheduleId).HasColumnName("schedule_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");
            entity.Property(e => e.ScheduleTime)
                .HasColumnType("datetime")
                .HasColumnName("schedule_time");
            entity.Property(e => e.StationCarId).HasColumnName("station_car_id");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Customer).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_Schedule_Customer");

            entity.HasOne(d => d.StationCar).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.StationCarId)
                .HasConstraintName("FK_Schedule_StationCar");
        });

        modelBuilder.Entity<Station>(entity =>
        {
            entity.HasKey(e => e.StationId).HasName("PK__Station__44B370E915109C45");

            entity.ToTable("Station");

            entity.Property(e => e.StationId).HasColumnName("station_id");
            entity.Property(e => e.ContactNumber)
                .HasMaxLength(255)
                .HasColumnName("contact_number");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .HasColumnName("location");
            entity.Property(e => e.StationName)
                .HasMaxLength(255)
                .HasColumnName("station_name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<StationCar>(entity =>
        {
            entity.HasKey(e => e.StationCarId).HasName("PK__Station___ED3F4FC5A100C89B");

            entity.ToTable("Station_Car");

            entity.Property(e => e.StationCarId).HasColumnName("station_car_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.StationId).HasColumnName("station_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.VehicleId).HasColumnName("vehicle_id");

            entity.HasOne(d => d.Station).WithMany(p => p.StationCars)
                .HasForeignKey(d => d.StationId)
                .HasConstraintName("FK_Station_Car_Station");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.StationCars)
                .HasForeignKey(d => d.VehicleId)
                .HasConstraintName("FK_Station_Car_Vehicle");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.VehicleId).HasName("PK__Vehicle__F2947BC15E96E012");

            entity.ToTable("Vehicle");

            entity.Property(e => e.VehicleId).HasColumnName("vehicle_id");
            entity.Property(e => e.BatteryCapacity).HasColumnName("battery_capacity");
            entity.Property(e => e.Color)
                .HasMaxLength(255)
                .HasColumnName("color");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");
            entity.Property(e => e.Manufacturer)
                .HasMaxLength(255)
                .HasColumnName("manufacturer");
            entity.Property(e => e.Model)
                .HasMaxLength(255)
                .HasColumnName("model");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Range).HasColumnName("range");
            entity.Property(e => e.Type)
                .HasMaxLength(255)
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
