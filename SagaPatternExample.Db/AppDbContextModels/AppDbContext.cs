using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SagaPatternExample.Db.AppDbContextModels;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TbOrder> TbOrders { get; set; }

    public virtual DbSet<TbOrderDetail> TbOrderDetails { get; set; }

    public virtual DbSet<TbProduct> TbProducts { get; set; }

    public virtual DbSet<TbStockEntry> TbStockEntries { get; set; }

    public virtual DbSet<TbUser> TbUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TbOrder>(entity =>
        {
            entity.HasKey(e => e.OrderId);

            entity.ToTable("TbOrder");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.InvoiceNo).HasMaxLength(50);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.User).WithMany(p => p.TbOrders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TbOrder_TbUser");
        });

        modelBuilder.Entity<TbOrderDetail>(entity =>
        {
            entity.ToTable("TbOrderDetail");

            entity.Property(e => e.InvoiceNo).HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Product).WithMany(p => p.TbOrderDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TbOrderDetail_TbProduct");
        });

        modelBuilder.Entity<TbProduct>(entity =>
        {
            entity.HasKey(e => e.ProductId);

            entity.ToTable("TbProduct");

            entity.Property(e => e.CategoryName).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductName).HasMaxLength(50);
        });

        modelBuilder.Entity<TbStockEntry>(entity =>
        {
            entity.HasKey(e => e.StockId).HasName("PK_TblStockEntry");

            entity.ToTable("TbStockEntry");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Product).WithMany(p => p.TbStockEntries)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TblStockEntry_TbProduct");
        });

        modelBuilder.Entity<TbUser>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("TbUser");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.UserName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
