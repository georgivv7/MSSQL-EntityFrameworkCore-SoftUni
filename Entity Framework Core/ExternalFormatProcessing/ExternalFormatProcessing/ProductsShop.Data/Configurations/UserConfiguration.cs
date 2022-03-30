namespace ProductsShop.Data.Configurations  
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using ProductsShop.Models;
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.FirstName)
                .IsRequired(false)
                .IsUnicode(true)
                .HasMaxLength(30);

            builder.Property(u => u.LastName)
                .IsRequired(true)
                .IsUnicode(true)
                .HasMaxLength(30);
        }
    }
}