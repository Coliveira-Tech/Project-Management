using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Api.Infra.Data.Configuration
{
    public class TaskHistoryConfiguration : IEntityTypeConfiguration<TaskHistory>
    {
        public void Configure(EntityTypeBuilder<TaskHistory> builder)
        {
            builder.ToTable("TaskHistory");
            builder.Property(e => e.Id).HasDefaultValueSql("NEWID()");
            builder.Property(e => e.Timestamp).HasDefaultValueSql("GETDATE()");
            builder.Property(e => e.Field).IsRequired();
            builder.Property(e => e.OldValue).IsRequired();
            builder.Property(e => e.NewValue).IsRequired();
            builder.Property(e => e.TaskId).IsRequired();

            builder.HasKey(e => e.Id);

            builder.Navigation(e => e.Task).AutoInclude();
            builder.Navigation(e => e.User).AutoInclude();

            builder.HasOne(e => e.Task)
                .WithMany(e => e.TaskHistory)
                .HasForeignKey(e => e.TaskId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(e => e.User)
                .WithMany(e => e.TaskHistory)
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
