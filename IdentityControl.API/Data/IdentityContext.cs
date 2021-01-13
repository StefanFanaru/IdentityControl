using System.Reflection;
using IdentityControl.API.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityControl.API.Data
{
    public class IdentityContext : IdentityDbContext<ApplicationUser>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            builder.Entity<ApplicationUser>().Property(x => x.Id)
                .IsRequired()
                .HasMaxLength(36);

            builder.Entity<ApplicationUser>().Property(x => x.BlogId)
                .IsRequired(false)
                .HasMaxLength(36);

            builder.Entity<ApplicationUser>().Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Entity<ApplicationUser>().Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Entity<ApplicationUser>().Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(50);

            builder.Entity<ApplicationUser>().Property(x => x.PictureUrl)
                .HasMaxLength(200);

            builder.Entity<ApplicationUser>().Property(x => x.PhoneNumber)
                .HasMaxLength(15);

            builder.Entity<ApplicationUser>().OwnsOne(o => o.Address, a =>
            {
                a.WithOwner();

                a.Property(s => s.ZipCode)
                    .HasMaxLength(18);

                a.Property(s => s.Street)
                    .HasMaxLength(180);

                a.Property(s => s.County)
                    .IsRequired()
                    .HasMaxLength(60);

                a.Property(s => s.Country)
                    .HasMaxLength(90)
                    .IsRequired();

                a.Property(s => s.City)
                    .HasMaxLength(100)
                    .IsRequired();
            });

            builder.Entity<ApplicationUser>().HasIndex(x => x.BlogId).IsUnique();
        }
    }
}