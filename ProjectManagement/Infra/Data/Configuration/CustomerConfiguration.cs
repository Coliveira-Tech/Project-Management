using ProjectManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace ProjectManagement.Api.Infra.Data.Configuration
{
    //[ExcludeFromCodeCoverage]
    //public class CustomerConfiguration: IEntityTypeConfiguration<Customer>
    //{
    //    public void Configure(EntityTypeBuilder<Customer> builder)
    //    {
    //        builder.ToTable("Customer");
    //        builder.Property(e => e.Id).HasDefaultValueSql("NEWID()");
    //        builder.Property(e => e.Name).IsRequired();
    //        builder.Property(e => e.Email).IsRequired();
    //        builder.Property(e => e.Phone).IsRequired();
    //        builder.Property(e => e.Address).IsRequired();
    //        builder.Property(e => e.Document).IsRequired();

    //        builder.HasKey(e => e.Id);

    //        builder.Navigation(e => e.Orders).AutoInclude();

    //        builder.HasMany(e => e.Orders)
    //            .WithOne(e => e.Customer)
    //            .HasForeignKey(e => e.CustomerId)
    //            .IsRequired();
    //    }
    //}
}
