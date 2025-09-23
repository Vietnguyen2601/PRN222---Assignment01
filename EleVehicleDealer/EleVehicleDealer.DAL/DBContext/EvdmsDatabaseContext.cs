using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace EleVehicleDealer.DAL.EntityModels;

public partial class EvdmsDatabaseContext : DbContext
{
    public EvdmsDatabaseContext()
    {
    }

    public EvdmsDatabaseContext(DbContextOptions<EvdmsDatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<EvdmsCustomer> EvdmsCustomers { get; set; }

    public virtual DbSet<EvdmsOrder> EvdmsOrders { get; set; }

    public virtual DbSet<EvdmsReport> EvdmsReports { get; set; }

    public virtual DbSet<EvdmsUser> EvdmsUsers { get; set; }

    public virtual DbSet<EvdmsVehicle> EvdmsVehicles { get; set; }

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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(GetConnectionString("DefaultConnection"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EvdmsCustomer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__evdms_cu__CD65CB857CEA8366");

            entity.ToTable("evdms_customers");

            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Address)
                .HasColumnType("text")
                .HasColumnName("address");
            entity.Property(e => e.ContactInfo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("contact_info");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("full_name");
        });

        modelBuilder.Entity<EvdmsOrder>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__evdms_or__46596229F2334B67");

            entity.ToTable("evdms_orders");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("order_date");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending")
                .HasColumnName("status");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("total_amount");
            entity.Property(e => e.TrackingInfo)
                .HasColumnType("text")
                .HasColumnName("tracking_info");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VehicleId).HasColumnName("vehicle_id");

            entity.HasOne(d => d.Customer).WithMany(p => p.EvdmsOrders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__evdms_ord__custo__04E4BC85");

            entity.HasOne(d => d.User).WithMany(p => p.EvdmsOrders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__evdms_ord__user___06CD04F7");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.EvdmsOrders)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__evdms_ord__vehic__05D8E0BE");
        });

        modelBuilder.Entity<EvdmsReport>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PK__evdms_re__779B7C58FCCF50DE");

            entity.ToTable("evdms_reports");

            entity.Property(e => e.ReportId).HasColumnName("report_id");
            entity.Property(e => e.GeneratedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("generated_at");
            entity.Property(e => e.ReportData).HasColumnName("report_data");
            entity.Property(e => e.ReportType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("report_type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.EvdmsReports)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__evdms_rep__user___0B91BA14");
        });

        modelBuilder.Entity<EvdmsUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__evdms_us__B9BE370F80189256");

            entity.ToTable("evdms_users");

            entity.HasIndex(e => e.Username, "UQ__evdms_us__F3DBC572F23D3FEA").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.ContactInfo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("contact_info");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("full_name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password_hash");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("role");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        modelBuilder.Entity<EvdmsVehicle>(entity =>
        {
            entity.HasKey(e => e.VehicleId).HasName("PK__evdms_ve__F2947BC1B1E7E157");

            entity.ToTable("evdms_vehicles");

            entity.Property(e => e.VehicleId).HasColumnName("vehicle_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ModelName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("model_name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("price");
            entity.Property(e => e.StockQuantity).HasColumnName("stock_quantity");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("type");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
