using EleVehicleDealer.DAL.EntityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

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

    public virtual DbSet<Contract> Contracts { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<StaffRevenue> StaffRevenues { get; set; }

    public virtual DbSet<Station> Stations { get; set; }

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
            entity.HasKey(e => e.AccountId).HasName("PK__Account__46A222CD64C1F646");

            entity.ToTable("Account");

            entity.HasIndex(e => e.Email, "UQ__Account__AB6E616476805293").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__Account__F3DBC572C9CC8844").IsUnique();

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
                .HasColumnName("is_active");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .HasColumnName("username");
        });

        modelBuilder.Entity<AccountRole>(entity =>
        {
            entity.HasKey(e => e.AccountRoleId).HasName("PK__Account___9E0E18317CD1482B");

            entity.ToTable("Account_Role");

            entity.Property(e => e.AccountRoleId).HasColumnName("account_role_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Account).WithMany(p => p.AccountRoles)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK__Account_Role__account_id");

            entity.HasOne(d => d.Role).WithMany(p => p.AccountRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__Account_Role__role_id");
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => e.ContractId).HasName("PK__Contract__F8D664238DA64245");

            entity.ToTable("Contract");

            entity.Property(e => e.ContractId).HasColumnName("contract_id");
            entity.Property(e => e.ContractDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("contract_date");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Signature)
                .HasMaxLength(255)
                .HasColumnName("signature");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasDefaultValue("Draft")
                .HasColumnName("status");
            entity.Property(e => e.Terms)
                .HasColumnType("text")
                .HasColumnName("terms");

            entity.HasOne(d => d.Order).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Contract__order_id");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__7A6B2B8CEA668898");

            entity.ToTable("Feedback");

            entity.Property(e => e.FeedbackId).HasColumnName("feedback_id");
            entity.Property(e => e.Comment)
                .HasColumnType("text")
                .HasColumnName("comment");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.FeedbackDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("feedback_date");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.VehicleId).HasColumnName("vehicle_id");

            entity.HasOne(d => d.Customer).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Feedback__customer_id");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.VehicleId)
                .HasConstraintName("FK__Feedback__vehicle_id");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.InventoryId).HasName("PK__Inventor__B59ACC49556AE518");

            entity.ToTable("Inventory");

            entity.Property(e => e.InventoryId).HasColumnName("inventory_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.LastUpdated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("last_updated");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.StationId).HasColumnName("station_id");
            entity.Property(e => e.VehicleId).HasColumnName("vehicle_id");

            entity.HasOne(d => d.Station).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.StationId)
                .HasConstraintName("FK__Inventory__station_id");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.VehicleId)
                .HasConstraintName("FK__Inventory__vehicle_id");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__465962291DF5D063");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.InventoryId).HasColumnName("inventory_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("order_date");
            entity.Property(e => e.PromotionId).HasColumnName("promotion_id");
            entity.Property(e => e.StaffId).HasColumnName("staff_id");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasDefaultValue("Pending")
                .HasColumnName("status");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("total_price");

            entity.HasOne(d => d.Customer).WithMany(p => p.OrderCustomers)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Orders__customer_id");

            entity.HasOne(d => d.Inventory).WithMany(p => p.Orders)
                .HasForeignKey(d => d.InventoryId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Orders__inventory_id");

            entity.HasOne(d => d.Staff).WithMany(p => p.OrderStaffs)
                .HasForeignKey(d => d.StaffId)
                .HasConstraintName("FK__Orders__staff_id");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__ED1FC9EADBAF1A5C");

            entity.ToTable("Payment");

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
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
                .HasDefaultValue("Pending")
                .HasColumnName("status");

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payment__order_id");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.PromotionId).HasName("PK__Promotio__2CB9556BDE4D2648");

            entity.ToTable("Promotion");

            entity.HasIndex(e => e.PromoCode, "UQ__Promotio__C07E231596579479").IsUnique();

            entity.Property(e => e.PromotionId).HasColumnName("promotion_id");
            entity.Property(e => e.ApplicableTo)
                .HasMaxLength(255)
                .HasColumnName("applicable_to");
            entity.Property(e => e.DiscountPercentage)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("discount_percentage");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.PromoCode)
                .HasMaxLength(255)
                .HasColumnName("promo_code");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.StationId).HasColumnName("station_id");

            entity.HasOne(d => d.Station).WithMany(p => p.Promotions)
                .HasForeignKey(d => d.StationId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Promotion__station_id");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PK__Report__779B7C584A53080A");

            entity.ToTable("Report");

            entity.Property(e => e.ReportId).HasColumnName("report_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.Data).HasColumnName("data");
            entity.Property(e => e.GeneratedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("generated_date");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.ReportType)
                .HasMaxLength(255)
                .HasColumnName("report_type");

            entity.HasOne(d => d.Account).WithMany(p => p.Reports)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK__Report__account_id");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__760965CCFF02FAAB");

            entity.ToTable("Role");

            entity.HasIndex(e => e.RoleName, "UQ__Role__783254B1B9D9446F").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.RoleName)
                .HasMaxLength(255)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<StaffRevenue>(entity =>
        {
            entity.HasKey(e => e.StaffRevenueId).HasName("PK__Staff_Re__62C349F1E9E28251");

            entity.ToTable("Staff_Revenue");

            entity.Property(e => e.StaffRevenueId).HasColumnName("staff_revenue_id");
            entity.Property(e => e.Commission)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("commission");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.RevenueDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("revenue_date");
            entity.Property(e => e.StaffId).HasColumnName("staff_id");
            entity.Property(e => e.TotalRevenue)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("total_revenue");

            entity.HasOne(d => d.Staff).WithMany(p => p.StaffRevenues)
                .HasForeignKey(d => d.StaffId)
                .HasConstraintName("FK__Staff_Revenue__staff_id");
        });

        modelBuilder.Entity<Station>(entity =>
        {
            entity.HasKey(e => e.StationId).HasName("PK__Station__44B370E93BBB0D78");

            entity.ToTable("Station");

            entity.Property(e => e.StationId).HasColumnName("station_id");
            entity.Property(e => e.AdminId).HasColumnName("admin_id");
            entity.Property(e => e.ContactNumber)
                .HasMaxLength(255)
                .HasColumnName("contact_number");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .HasColumnName("location");
            entity.Property(e => e.StationName)
                .HasMaxLength(255)
                .HasColumnName("station_name");

            entity.HasOne(d => d.Admin).WithMany(p => p.Stations)
                .HasForeignKey(d => d.AdminId)
                .HasConstraintName("FK__Station__admin_id");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.VehicleId).HasName("PK__Vehicle__F2947BC1AFA706E2");

            entity.ToTable("Vehicle");

            entity.Property(e => e.VehicleId).HasColumnName("vehicle_id");
            entity.Property(e => e.Availability).HasColumnName("availability");
            entity.Property(e => e.Color)
                .HasMaxLength(255)
                .HasColumnName("color");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Model)
                .HasMaxLength(255)
                .HasColumnName("model");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.StationId).HasColumnName("station_id");
            entity.Property(e => e.Type)
                .HasMaxLength(255)
                .HasColumnName("type");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
