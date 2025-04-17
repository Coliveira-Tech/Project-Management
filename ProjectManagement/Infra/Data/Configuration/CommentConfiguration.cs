using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Api.Infra.Data.Configuration
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable("Comment");
            builder.Property(e => e.Id).HasDefaultValueSql("NEWID()");
            builder.Property(e => e.Content).IsRequired();
            builder.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            builder.Property(e => e.TaskId).IsRequired();
            builder.Property(e => e.UserId).IsRequired();

            builder.HasKey(e => e.Id);

            builder.Navigation(e => e.Task).AutoInclude();
            builder.Navigation(e => e.User).AutoInclude();

            builder.HasOne(e => e.Task)
                .WithMany(e => e.Comments)
                .HasForeignKey(e => e.TaskId)
                .IsRequired();

            builder.HasOne(e => e.User)
                .WithMany(e => e.Comments)
                .HasForeignKey(e => e.UserId)
                .IsRequired();
        }
    }
}
