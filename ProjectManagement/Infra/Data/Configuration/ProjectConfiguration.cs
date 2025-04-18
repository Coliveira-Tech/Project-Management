using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Api.Infra.Data.Configuration
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("Project");
            builder.Property(e => e.Id).HasDefaultValueSql("NEWID()");
            builder.Property(e => e.Name).IsRequired();
            builder.Property(e => e.Description).IsRequired();
            builder.Property(e => e.OwnerId).IsRequired();

            builder.HasKey(e => e.Id);

            builder.Navigation(e => e.Tasks).AutoInclude();
            builder.Navigation(e => e.Owner).AutoInclude();

            builder.HasMany(e => e.Tasks)
                .WithOne(e => e.Project)
                .HasForeignKey(e => e.ProjectId)
                .IsRequired();

            builder.HasOne(e => e.Owner)
                .WithMany(e => e.Projects)
                .HasForeignKey(e => e.OwnerId)
                .IsRequired();
        }
    }
}
