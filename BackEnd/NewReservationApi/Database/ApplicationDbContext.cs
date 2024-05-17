
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NewReservationApi.Enitities;
using System.Reflection.Emit;

namespace NewReservationApi.Database;

public class ApplicationDbContext : IdentityDbContext<Driver>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Family> Families { get; set; }
    public DbSet<Driver> Drivers { get; set; }
    public DbSet<Car> Cars { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdentityUserLogin<string>>()
           .HasKey(l => new { l.LoginProvider, l.ProviderKey });

        modelBuilder.Entity<IdentityUserRole<string>>()
                .HasKey(r => new { r.UserId, r.RoleId });

        modelBuilder.Entity<IdentityUserToken<string>>()
                .HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

        // Configure the one-to-many relationship between Family and Driver
        modelBuilder.Entity<Driver>(entity =>
        {
            entity.HasKey(d => d.Id);   

            entity.HasOne(d => d.Family)
                    .WithMany(f => f.Drivers)
                    .HasForeignKey(d => d.FamilyId)
                    .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure the one-to-many relationship between Family and Car
        modelBuilder.Entity<Car>(entity =>
        {
            entity.HasKey(d => d.Id);

            entity.HasOne(c => c.Family)
                    .WithMany(f => f.Cars)
                    .HasForeignKey(c => c.FamilyId)
                    .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure the relationships for the Reservation entity
        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(r => r.Id);

            entity.HasOne(r => r.Family)
                .WithMany(d => d.Reservations)
                .HasForeignKey(r => r.FamilyId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.Driver)
                .WithMany(f => f.Reservations)
                .HasForeignKey(r => r.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.Car)
                .WithMany(f => f.Reservations)
                .HasForeignKey(r => r.CarId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
