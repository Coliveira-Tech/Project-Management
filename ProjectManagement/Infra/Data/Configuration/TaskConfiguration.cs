using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectManagement.Api.Infra.Data.Configuration
{
    public class TaskConfiguration: IEntityTypeConfiguration<Domain.Entities.Task>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Task> builder)
        {
            builder.ToTable("Task");
            builder.Property(e => e.Id).HasDefaultValueSql("NEWID()");
            builder.Property(e => e.Title).IsRequired();
            builder.Property(e => e.Description).IsRequired();
            builder.Property(e => e.Status).IsRequired();
            builder.Property(e => e.Priority).IsRequired();
            builder.Property(e => e.DueDate).IsRequired();
            builder.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            builder.Property(e => e.UpdatedAt).IsRequired();
            builder.Property(e => e.ProjectId).IsRequired();
            builder.Property(e => e.AssignedUserId);
            
            builder.HasKey(e => e.Id);

            builder.Navigation(e => e.AssignedUser).AutoInclude();
            builder.Navigation(e => e.Comments).AutoInclude();

            builder.HasOne(e => e.Project)
                .WithMany(e => e.Tasks)
                .HasForeignKey(e => e.ProjectId)
                .IsRequired();

            builder.HasOne(e => e.AssignedUser)
                .WithMany(e => e.Tasks)
                .HasForeignKey(e => e.AssignedUserId)
                .IsRequired(false);

            builder.HasMany(e => e.Comments)
                .WithOne(e => e.Task)
                .HasForeignKey(e => e.TaskId)
                .IsRequired(false);
        }
    }
}
