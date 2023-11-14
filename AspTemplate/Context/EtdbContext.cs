using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace NewTemplate.Context
{
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
                entity.Property(e => e.BusId).ValueGeneratedNever();

                entity.Property(e => e.LicensePlate)
                    .IsRequired()
                    .HasMaxLength(16);

                entity.Property(e => e.Name).HasMaxLength(128);
            });

            modelBuilder.Entity<Route>(entity =>
            {
                entity.Property(e => e.RouteId).ValueGeneratedNever();

                entity.Property(e => e.From)
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.To)
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasDefaultValueSql("('')");
            });

            modelBuilder.Entity<Seat>(entity =>
            {
                entity.Property(e => e.SeatId).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.HasOne(d => d.Bus)
                    .WithMany(p => p.Seat)
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
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.Fullname)
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(16)
                    .HasDefaultValueSql("('')");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
