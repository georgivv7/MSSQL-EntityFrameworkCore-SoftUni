namespace P01_BillsPaymentSystem.Data.EntityConfig
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using P01_BillsPaymentSystem.Data.Models;

    class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
    {
        public void Configure(EntityTypeBuilder<BankAccount> builder)
        {
            builder.HasKey(b => b.BankAccountId);

            builder.Property(b => b.BankName)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(50);

            builder.Property(b => b.SWIFTCode)
                .IsUnicode(false)
                .IsRequired()
                .HasMaxLength(20);

            builder.Ignore(ba => ba.PaymentMethodId);
        }
    }
}
