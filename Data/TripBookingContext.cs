using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TripBookingBE.Models;

namespace TripBookingBE.Data;

public partial class TripBookingContext : DbContext
{
    public TripBookingContext()
    {
    }

    public TripBookingContext(DbContextOptions<TripBookingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CustomerBookTrip> CustomerBookTrips { get; set; }

    public virtual DbSet<CustomerReviewTrip> CustomerReviewTrips { get; set; }

    public virtual DbSet<Models.Route> Routes { get; set; }

    public virtual DbSet<SellerTrip> SellerTrips { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<Trip> Trips { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Encrypt=false;Persist Security Info=False;Server=ADMIN\\DOTNETTEST;Initial Catalog=TripBooking;User ID=sa;Password=Admin@123;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GeneralParam>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_GeneralParam_1");
            entity.Property(e => e.ParamKey).HasColumnType("nvarchar(100)");
            entity.Property(e => e.ParamCode).HasColumnType("nvarchar(100)");
            entity.Property(e => e.ParamDescription).HasColumnType("nvarchar(500)");
        });

        modelBuilder.Entity<CustomerBookTrip>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_CustomerBookTrip_1");

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerBookTrips).HasConstraintName("FK_CustomerBookTrip_User");

            entity.HasOne(d => d.Trip).WithMany(p => p.CustomerBookTrips).HasConstraintName("FK_CustomerBookTrip_Trip");
        });

        modelBuilder.Entity<CustomerReviewTrip>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_UserReviewTrip");

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerReviewTrips).HasConstraintName("FK_UserReviewTrip_User");

            entity.HasOne(d => d.Trip).WithMany(p => p.CustomerReviewTrips)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UserReviewTrip_Trip");
        });

        modelBuilder.Entity<SellerTrip>(entity =>
        {
            entity.HasOne(d => d.Trip).WithMany(p => p.SellerTrips)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_SellerTrip_Trip");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.Property(e => e.CustomerBookTripId).ValueGeneratedNever();

            entity.HasOne(d => d.CustomerBookTrip).WithOne(p => p.Ticket).HasConstraintName("FK_Ticket_CustomerBookTrip");

            entity.HasOne(d=>d.GeneralParam).WithMany(p=>p.Tickets).HasForeignKey(t=>t.GeneralParamId).HasConstraintName("FK_Ticket_GeneralParam").OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Trip>(entity =>
        {
            entity.Property(e => e.DepartureTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.PlaceCount).HasDefaultValue(1);

            entity.HasOne(d => d.Driver).WithMany(p => p.Trips).HasConstraintName("FK_Trip_User");

            entity.HasOne(d => d.Route).WithMany(p => p.Trips).HasConstraintName("FK_Trip_Route");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3213E83F36DAC7FF");

            entity.Property(e => e.Active).HasDefaultValue(true);
            entity.Property(e => e.Avatar).HasDefaultValueSql("(NULL)");
            entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.DateModified).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email).HasDefaultValueSql("(NULL)");
            entity.Property(e => e.LastLogin).HasDefaultValueSql("(NULL)");
            entity.Property(e => e.Phone).HasDefaultValueSql("(NULL)");
            entity.Property(e => e.Type).HasDefaultValue("CUSTOMER");
            entity.Property(e => e.RowVersion).IsRowVersion();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
