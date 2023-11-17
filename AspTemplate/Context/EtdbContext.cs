using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AspTemplate.Context;

public partial class EtdbContext : DbContext
{
    public EtdbContext(DbContextOptions<EtdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bus> Bus { get; set; }

    public virtual DbSet<Product> Product { get; set; }

    public virtual DbSet<Route> Route { get; set; }

    public virtual DbSet<Seat> Seat { get; set; }

    public virtual DbSet<Ticket> Ticket { get; set; }

    public virtual DbSet<Trip> Trip { get; set; }

    public virtual DbSet<User> User { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bus>(entity =>
        {
            entity.Property(e => e.BusId).ValueGeneratedNever();
            entity.Property(e => e.LicensePlate)
                .IsRequired()
                .HasMaxLength(16);
            entity.Property(e => e.Name).HasMaxLength(128);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Product__B40CC6ED8480B1AD");

            entity.Property(e => e.ProductId)
                .ValueGeneratedNever()
                .HasColumnName("ProductID");
            entity.Property(e => e.Brand).HasMaxLength(100);
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.ProductName).HasMaxLength(255);
            entity.Property(e => e.ProductPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ProductWeight).HasMaxLength(20);
            entity.Property(e => e.Sku)
                .HasMaxLength(50)
                .HasColumnName("SKU");
            entity.Property(e => e.TaxRate).HasColumnType("decimal(5, 2)");
        });

        modelBuilder.Entity<Route>(entity =>
        {
            entity.Property(e => e.RouteId).ValueGeneratedNever();
            entity.Property(e => e.From)
                .IsRequired()
                .HasMaxLength(128)
                .HasDefaultValue("");
            entity.Property(e => e.To)
                .IsRequired()
                .HasMaxLength(128)
                .HasDefaultValue("");
        });

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasIndex(e => e.BusId, "IX_Seat_BusId");

            entity.Property(e => e.SeatId).ValueGeneratedNever();
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(128);

            entity.HasOne(d => d.Bus).WithMany(p => p.Seat)
                .HasForeignKey(d => d.BusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Seat__BusId__3C69FB99");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.Property(e => e.TicketId).ValueGeneratedNever();
            entity.Property(e => e.BookedDate).HasColumnType("datetime");
            entity.Property(e => e.From).HasMaxLength(128);
            entity.Property(e => e.To).HasMaxLength(128);
        });

        modelBuilder.Entity<Trip>(entity =>
        {
            entity.Property(e => e.TripId).ValueGeneratedNever();
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.Address)
                .IsRequired()
                .HasMaxLength(128)
                .HasDefaultValue("");
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(128);
            entity.Property(e => e.Fullname)
                .IsRequired()
                .HasMaxLength(128)
                .HasDefaultValue("");
            entity.Property(e => e.Phone)
                .IsRequired()
                .HasMaxLength(16)
                .HasDefaultValue("");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
