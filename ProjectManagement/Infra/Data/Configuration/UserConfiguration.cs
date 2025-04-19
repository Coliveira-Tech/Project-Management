using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Enums;
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

            builder.HasData(UserSeed());
        }

        private static User[] UserSeed()
        {
            return
            [
                new User
                {
                    Id = Guid.Parse("96a51fca-20ab-4bfc-be2f-b37e79632d06"),
                    Name = "Linus Torvalds",
                    Email = "linus@linux.org",
                    Password = "#ViTeam#35",
                    Role = UserRole.Administrator
                },
                new User
                {
                    Id = Guid.Parse("c6551a2c-e187-49c0-b929-4bba525b14ae"),
                    Name = "Stive Jobs",
                    Email = "steve@apple.com",
                    Password = "@MacAttack22!",
                    Role = UserRole.Manager
                },
                new User
                {
                    Id = Guid.Parse("3485751c-4840-41fa-bfce-8054d8756cee"),
                    Name = "Bill Gates",
                    Email = "bill@microsoft.com",
                    Password = "123456",
                    Role = UserRole.Guest
                }
            ];
        }
    }
}