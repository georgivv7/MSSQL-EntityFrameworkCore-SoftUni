namespace ProductsShop.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using ProductsShop.Models;
    internal class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(250);

            builder.HasOne(p => p.Seller)
                .WithMany(s => s.ProductsSold)
                .HasForeignKey(p => p.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Buyer)
               .WithMany(b => b.ProductsBought)
               .HasForeignKey(p => p.BuyerId)
               .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
