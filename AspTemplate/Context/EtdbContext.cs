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

    public virtual DbSet<Route> Route { get; set; }

    public virtual DbSet<Seat> Seat { get; set; }

    public virtual DbSet<Ticket> Ticket { get; set; }

    public virtual DbSet<Trip> Trip { get; set; }

    public virtual DbSet<User> User { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bus>(entity =>
        {
            entity.HasKey(e => e.BusId).HasName("PK__Bus__6A0F60B5D6D555C3");

            entity.Property(e => e.BusId).ValueGeneratedNever();
            entity.Property(e => e.LicensePlate)
                .IsRequired()
                .HasMaxLength(16);
            entity.Property(e => e.Name).HasMaxLength(128);
        });

        modelBuilder.Entity<Route>(entity =>
        {
            entity.HasKey(e => e.RouteId).HasName("PK__Route__80979B4D479B8A2C");

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
            entity.HasKey(e => e.SeatId).HasName("PK__Seat__311713F3695CC58D");

            entity.Property(e => e.SeatId).ValueGeneratedNever();
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(128);

            entity.HasOne(d => d.Bus).WithMany(p => p.Seat)
                .HasForeignKey(d => d.BusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Seat__BusId__48CFD27E");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketId).HasName("PK__Ticket__712CC60730C47389");

            entity.Property(e => e.TicketId).ValueGeneratedNever();
            entity.Property(e => e.BookedDate).HasColumnType("datetime");
            entity.Property(e => e.From).HasMaxLength(128);
            entity.Property(e => e.To).HasMaxLength(128);

            entity.HasOne(d => d.Seat).WithMany(p => p.Ticket)
                .HasForeignKey(d => d.SeatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Ticket__SeatId__4E88ABD4");

            entity.HasOne(d => d.Trip).WithMany(p => p.Ticket)
                .HasForeignKey(d => d.TripId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Ticket__TripId__4D94879B");

            entity.HasOne(d => d.User).WithMany(p => p.Ticket)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Ticket__UserId__4F7CD00D");
        });

        modelBuilder.Entity<Trip>(entity =>
        {
            entity.HasKey(e => e.TripId).HasName("PK__Trip__51DC713E918B759A");

            entity.Property(e => e.TripId).ValueGeneratedNever();
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");

            entity.HasOne(d => d.Route).WithMany(p => p.Trip)
                .HasForeignKey(d => d.RouteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Trip__RouteId__440B1D61");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4C096A5A0B");

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
