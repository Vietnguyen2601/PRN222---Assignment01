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

    public virtual DbSet<EvdmsAccount> EvdmsAccounts { get; set; }

    public virtual DbSet<EvdmsContract> EvdmsContracts { get; set; }

    public virtual DbSet<EvdmsCustomer> EvdmsCustomers { get; set; }

    public virtual DbSet<EvdmsFeedback> EvdmsFeedbacks { get; set; }

    public virtual DbSet<EvdmsOrder> EvdmsOrders { get; set; }

    public virtual DbSet<EvdmsPayment> EvdmsPayments { get; set; }

    public virtual DbSet<EvdmsPromotion> EvdmsPromotions { get; set; }

    public virtual DbSet<EvdmsQuote> EvdmsQuotes { get; set; }

    public virtual DbSet<EvdmsReport> EvdmsReports { get; set; }

    public virtual DbSet<EvdmsRole> EvdmsRoles { get; set; }

    public virtual DbSet<EvdmsTestDrife> EvdmsTestDrives { get; set; }

    public virtual DbSet<EvdmsUser> EvdmsUsers { get; set; }

    public virtual DbSet<EvdmsVehicle> EvdmsVehicles { get; set; }

    public virtual DbSet<EvdmsVehicleBooking> EvdmsVehicleBookings { get; set; }

    public virtual DbSet<EvdmsVehicleFeature> EvdmsVehicleFeatures { get; set; }

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
            optionsBuilder.UseSqlServer(GetConnectionString("DefaultConnection"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EvdmsAccount>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__evdms_ac__46A222CD0696024F");

            entity.ToTable("evdms_accounts");

            entity.HasIndex(e => e.Email, "UQ__evdms_ac__AB6E6164EBC7545D").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__evdms_ac__F3DBC572BB6BC44E").IsUnique();

            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.LastLogin)
                .HasColumnType("datetime")
                .HasColumnName("last_login");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RefreshToken)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("refresh_token");
            entity.Property(e => e.TokenExpiry)
                .HasColumnType("datetime")
                .HasColumnName("token_expiry");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("username");

            entity.HasOne(d => d.User).WithMany(p => p.EvdmsAccounts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__evdms_accounts__user_id");
        });

        modelBuilder.Entity<EvdmsContract>(entity =>
        {
            entity.HasKey(e => e.ContractId).HasName("PK__evdms_co__F8D66423A54ACEAC");

            entity.ToTable("evdms_contracts");

            entity.Property(e => e.ContractId).HasColumnName("contract_id");
            entity.Property(e => e.ContractDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("contract_date");
            entity.Property(e => e.ContractDetails).HasColumnName("contract_details");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Draft")
                .HasColumnName("status");

            entity.HasOne(d => d.Customer).WithMany(p => p.EvdmsContracts)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__evdms_contracts__customer_id");

            entity.HasOne(d => d.Order).WithMany(p => p.EvdmsContracts)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__evdms_contracts__order_id");
        });

        modelBuilder.Entity<EvdmsCustomer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__evdms_cu__CD65CB852BC91C5F");

            entity.ToTable("evdms_customers");

            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Address)
                .HasColumnType("text")
                .HasColumnName("address");
            entity.Property(e => e.AssignedStaffId).HasColumnName("assigned_staff_id");
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
            entity.Property(e => e.TransactionHistory).HasColumnName("transaction_history");

            entity.HasOne(d => d.AssignedStaff).WithMany(p => p.EvdmsCustomers)
                .HasForeignKey(d => d.AssignedStaffId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__evdms_customers__assigned_staff_id");
        });

        modelBuilder.Entity<EvdmsFeedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__evdms_fe__7A6B2B8CCFEBFA55");

            entity.ToTable("evdms_feedback");

            entity.Property(e => e.FeedbackId).HasColumnName("feedback_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.FeedbackContent)
                .HasColumnType("text")
                .HasColumnName("feedback_content");
            entity.Property(e => e.FeedbackDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("feedback_date");
            entity.Property(e => e.FeedbackType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("feedback_type");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Open")
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Customer).WithMany(p => p.EvdmsFeedbacks)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__evdms_feedback__customer_id");

            entity.HasOne(d => d.User).WithMany(p => p.EvdmsFeedbacks)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__evdms_feedback__user_id");
        });

        modelBuilder.Entity<EvdmsOrder>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__evdms_or__46596229B99E3988");

            entity.ToTable("evdms_orders");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ApprovedBy).HasColumnName("approved_by");
            entity.Property(e => e.ApprovedDate)
                .HasColumnType("datetime")
                .HasColumnName("approved_date");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.DeliveryStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("delivery_status");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("order_date");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.QuoteId).HasColumnName("quote_id");
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

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.EvdmsOrderApprovedByNavigations)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("FK__evdms_orders__approved_by");

            entity.HasOne(d => d.Customer).WithMany(p => p.EvdmsOrders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__evdms_orders__customer_id");

            entity.HasOne(d => d.Quote).WithMany(p => p.EvdmsOrders)
                .HasForeignKey(d => d.QuoteId)
                .HasConstraintName("FK__evdms_orders__quote_id");

            entity.HasOne(d => d.User).WithMany(p => p.EvdmsOrderUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__evdms_orders__user_id");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.EvdmsOrders)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__evdms_orders__vehicle_id");
        });

        modelBuilder.Entity<EvdmsPayment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__evdms_pa__ED1FC9EA91E80442");

            entity.ToTable("evdms_payments");

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.DebtAmount)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("debt_amount");
            entity.Property(e => e.InstallmentPlan).HasColumnName("installment_plan");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("payment_date");
            entity.Property(e => e.PaymentType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("payment_type");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending")
                .HasColumnName("status");

            entity.HasOne(d => d.Customer).WithMany(p => p.EvdmsPayments)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__evdms_payments__customer_id");

            entity.HasOne(d => d.Order).WithMany(p => p.EvdmsPayments)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__evdms_payments__order_id");
        });

        modelBuilder.Entity<EvdmsPromotion>(entity =>
        {
            entity.HasKey(e => e.PromotionId).HasName("PK__evdms_pr__2CB9556BE695367C");

            entity.ToTable("evdms_promotions");

            entity.Property(e => e.PromotionId).HasColumnName("promotion_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.DiscountPercentage)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("discount_percentage");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.PromotionName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("promotion_name");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
        });

        modelBuilder.Entity<EvdmsQuote>(entity =>
        {
            entity.HasKey(e => e.QuoteId).HasName("PK__evdms_qu__0D37DF0C889CB920");

            entity.ToTable("evdms_quotes");

            entity.Property(e => e.QuoteId).HasColumnName("quote_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.PromotionId).HasColumnName("promotion_id");
            entity.Property(e => e.QuoteAmount)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("quote_amount");
            entity.Property(e => e.QuoteDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("quote_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending")
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VehicleId).HasColumnName("vehicle_id");

            entity.HasOne(d => d.Customer).WithMany(p => p.EvdmsQuotes)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__evdms_quotes__customer_id");

            entity.HasOne(d => d.Promotion).WithMany(p => p.EvdmsQuotes)
                .HasForeignKey(d => d.PromotionId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__evdms_quotes__promotion_id");

            entity.HasOne(d => d.User).WithMany(p => p.EvdmsQuotes)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__evdms_quotes__user_id");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.EvdmsQuotes)
                .HasForeignKey(d => d.VehicleId)
                .HasConstraintName("FK__evdms_quotes__vehicle_id");
        });

        modelBuilder.Entity<EvdmsReport>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PK__evdms_re__779B7C58E3946A04");

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
                .HasConstraintName("FK__evdms_reports__user_id");
        });

        modelBuilder.Entity<EvdmsRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__evdms_ro__760965CC06ACD7B2");

            entity.ToTable("evdms_roles");

            entity.HasIndex(e => e.RoleName, "UQ__evdms_ro__783254B15A203CCF").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<EvdmsTestDrife>(entity =>
        {
            entity.HasKey(e => e.TestDriveId).HasName("PK__evdms_te__7AC61E3019E32641");

            entity.ToTable("evdms_test_drives");

            entity.Property(e => e.TestDriveId).HasColumnName("test_drive_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Scheduled")
                .HasColumnName("status");
            entity.Property(e => e.TestDriveDate)
                .HasColumnType("datetime")
                .HasColumnName("test_drive_date");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VehicleId).HasColumnName("vehicle_id");

            entity.HasOne(d => d.Customer).WithMany(p => p.EvdmsTestDrives)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__evdms_test_drives__customer_id");

            entity.HasOne(d => d.User).WithMany(p => p.EvdmsTestDrives)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__evdms_test_drives__user_id");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.EvdmsTestDrives)
                .HasForeignKey(d => d.VehicleId)
                .HasConstraintName("FK__evdms_test_drives__vehicle_id");
        });

        modelBuilder.Entity<EvdmsUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__evdms_us__B9BE370F122AED11");

            entity.ToTable("evdms_users");

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
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Role).WithMany(p => p.EvdmsUsers)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__evdms_users__role_id");
        });

        modelBuilder.Entity<EvdmsVehicle>(entity =>
        {
            entity.HasKey(e => e.VehicleId).HasName("PK__evdms_ve__F2947BC158CB664D");

            entity.ToTable("evdms_vehicles");

            entity.Property(e => e.VehicleId).HasColumnName("vehicle_id");
            entity.Property(e => e.Configuration).HasColumnName("configuration");
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

        modelBuilder.Entity<EvdmsVehicleBooking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__evdms_ve__5DE3A5B1D882C76A");

            entity.ToTable("evdms_vehicle_bookings");

            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.BookingDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("booking_date");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending")
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VehicleId).HasColumnName("vehicle_id");

            entity.HasOne(d => d.User).WithMany(p => p.EvdmsVehicleBookings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__evdms_vehicle_bookings__user_id");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.EvdmsVehicleBookings)
                .HasForeignKey(d => d.VehicleId)
                .HasConstraintName("FK__evdms_vehicle_bookings__vehicle_id");
        });

        modelBuilder.Entity<EvdmsVehicleFeature>(entity =>
        {
            entity.HasKey(e => e.FeatureId).HasName("PK__evdms_ve__7906CBD762F0DE26");

            entity.ToTable("evdms_vehicle_features");

            entity.Property(e => e.FeatureId).HasColumnName("feature_id");
            entity.Property(e => e.FeatureName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("feature_name");
            entity.Property(e => e.FeatureValue)
                .HasColumnType("text")
                .HasColumnName("feature_value");
            entity.Property(e => e.VehicleId).HasColumnName("vehicle_id");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.EvdmsVehicleFeatures)
                .HasForeignKey(d => d.VehicleId)
                .HasConstraintName("FK__evdms_vehicle_features__vehicle_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
