using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AMI_WebAPI.Models;

public partial class AmidbContext : DbContext
{
    public AmidbContext()
    {
    }

    public AmidbContext(DbContextOptions<AmidbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Consumer> Consumers { get; set; }

    public virtual DbSet<DailyReading> DailyReadings { get; set; }

    public virtual DbSet<Meter> Meters { get; set; }

    public virtual DbSet<MonthlyBill> MonthlyBills { get; set; }

    public virtual DbSet<OrgUnit> OrgUnits { get; set; }

    public virtual DbSet<Tariff> Tariffs { get; set; }

    public virtual DbSet<TariffSlab> TariffSlabs { get; set; }

    public virtual DbSet<User> Users { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-VR9HLP3P\\SQLEXPRESS;Initial Catalog=AMIdb;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Consumer>(entity =>
        {
            entity.HasKey(e => e.ConsumerId).HasName("PK__Consumer__63BBE9BABDCE2761");

            entity.ToTable("Consumer", "ami");

            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(30);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Active");
            entity.Property(e => e.UpdatedAt).HasPrecision(3);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ConsumerCreatedByNavigations)
                .HasPrincipalKey(p => p.Username)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Consumer__Create__656C112C");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.ConsumerUpdatedByNavigations)
                .HasPrincipalKey(p => p.Username)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__Consumer__Update__66603565");
        });

        modelBuilder.Entity<DailyReading>(entity =>
        {
            entity.HasKey(e => e.ReadingId).HasName("PK__DailyRea__C80F9C4EBFC17C67");

            entity.ToTable("DailyReading", "ami");

            entity.HasIndex(e => e.MeterSerialNo, "IX_DailyReading_MeterSerialNo");

            entity.HasIndex(e => e.ReadingDate, "IX_DailyReading_ReadingDate");

            entity.HasIndex(e => new { e.MeterSerialNo, e.ReadingDate }, "UQ_Meter_Date").IsUnique();

            entity.Property(e => e.MeterSerialNo).HasMaxLength(50);
            entity.Property(e => e.ReadingKwh).HasColumnType("decimal(18, 6)");

            entity.HasOne(d => d.MeterSerialNoNavigation).WithMany(p => p.DailyReadings)
                .HasForeignKey(d => d.MeterSerialNo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DailyRead__Meter__72C60C4A");
        });

        modelBuilder.Entity<Meter>(entity =>
        {
            entity.HasKey(e => e.MeterSerialNo).HasName("PK__Meter__5C498B0FAC51F3D5");

            entity.ToTable("Meter", "ami");

            entity.HasIndex(e => e.ConsumerId, "IX_Meter_Consumer");

            entity.HasIndex(e => e.OrgUnitId, "IX_Meter_OrgUnit");

            entity.HasIndex(e => e.TariffId, "IX_Meter_Tariff");

            entity.HasIndex(e => e.Iccid, "UQ__Meter__8A69BC4CB16657B8").IsUnique();

            entity.HasIndex(e => e.Imsi, "UQ__Meter__8DF3A70E507DB3EC").IsUnique();

            entity.Property(e => e.MeterSerialNo).HasMaxLength(50);
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.Firmware).HasMaxLength(50);
            entity.Property(e => e.Iccid)
                .HasMaxLength(30)
                .HasColumnName("ICCID");
            entity.Property(e => e.Imsi)
                .HasMaxLength(30)
                .HasColumnName("IMSI");
            entity.Property(e => e.InstallTsUtc).HasPrecision(3);
            entity.Property(e => e.IpAddress).HasMaxLength(45);
            entity.Property(e => e.Manufacturer).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Active");

            entity.HasOne(d => d.Consumer).WithMany(p => p.Meters)
                .HasForeignKey(d => d.ConsumerId)
                .HasConstraintName("FK__Meter__ConsumerI__6D0D32F4");

            entity.HasOne(d => d.OrgUnit).WithMany(p => p.Meters)
                .HasForeignKey(d => d.OrgUnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Meter__OrgUnitId__6E01572D");

            entity.HasOne(d => d.Tariff).WithMany(p => p.Meters)
                .HasForeignKey(d => d.TariffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Meter__TariffId__6EF57B66");
        });

        modelBuilder.Entity<MonthlyBill>(entity =>
        {
            entity.HasKey(e => e.BillId).HasName("PK__MonthlyB__11F2FC6A1E90040A");

            entity.ToTable("MonthlyBill", "ami");

            entity.Property(e => e.BaseRateApplied).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.GeneratedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.MeterSerialNo).HasMaxLength(50);
            entity.Property(e => e.SlabCharge).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Unpaid");
            entity.Property(e => e.TotalBillAmount).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TotalKwh).HasColumnType("decimal(18, 6)");

            entity.HasOne(d => d.MeterSerialNoNavigation).WithMany(p => p.MonthlyBills)
                .HasForeignKey(d => d.MeterSerialNo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MonthlyBi__Meter__75A278F5");
        });

        modelBuilder.Entity<OrgUnit>(entity =>
        {
            entity.HasKey(e => e.OrgUnitId).HasName("PK__OrgUnit__4A793BEECA382B36");

            entity.ToTable("OrgUnit", "ami");

            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK_OrgUnit_Parent");
        });

        modelBuilder.Entity<Tariff>(entity =>
        {
            entity.HasKey(e => e.TariffId).HasName("PK__Tariff__EBAF9DB3DE475801");

            entity.ToTable("Tariff", "ami");

            entity.Property(e => e.BaseRate).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.TaxRate).HasColumnType("decimal(18, 4)");
        });

        modelBuilder.Entity<TariffSlab>(entity =>
        {
            entity.HasKey(e => e.TariffSlabId).HasName("PK__TariffSl__64EAAA2227452D1F");

            entity.ToTable("TariffSlab", "ami");

            entity.Property(e => e.FromKwh).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.RatePerKwh).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.ToKwh).HasColumnType("decimal(18, 6)");

            entity.HasOne(d => d.Tariff).WithMany(p => p.TariffSlabs)
                .HasForeignKey(d => d.TariffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TariffSla__Tarif__5441852A");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC6F7E8203");

            entity.ToTable("Users", "ami");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E4BAD0F643").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534CE8EB271").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.DisplayName).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastLogin).HasPrecision(3);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
