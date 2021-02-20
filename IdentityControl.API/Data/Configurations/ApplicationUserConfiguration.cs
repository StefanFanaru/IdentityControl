using IdentityControl.API.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityControl.API.Data.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(x => x.Id)
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.PictureUrl)
                .HasMaxLength(200);

            builder.Property(x => x.PhoneNumber)
                .HasMaxLength(15);

            builder.OwnsOne(o => o.Address, a =>
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
        }
    }
}