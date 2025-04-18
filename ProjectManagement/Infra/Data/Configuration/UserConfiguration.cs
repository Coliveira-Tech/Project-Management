using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace ProjectManagement.Api.Infra.Data.Configuration
{
    [ExcludeFromCodeCoverage]
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("User");
            builder.Property(e => e.Id).HasDefaultValueSql("NEWID()");
            builder.Property(e => e.Name).IsRequired();
            builder.Property(e => e.Email).IsRequired();
            builder.Property(e => e.Password).IsRequired();
            builder.Property(e => e.Role).IsRequired();

            builder.HasKey(e => e.Id);

            builder.HasMany(e => e.Projects)
                .WithOne(e => e.Owner)
                .HasForeignKey(e => e.OwnerId)
                .IsRequired();

            builder.HasMany(e => e.Tasks)
                .WithOne(e => e.AssignedUser)
                .HasForeignKey(e => e.AssignedUserId)
                .IsRequired(false);
        }
    }
}
