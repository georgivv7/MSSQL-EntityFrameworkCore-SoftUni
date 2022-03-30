namespace Instagraph.Data.Configurations
{
    using Instagraph.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasAlternateKey(e => e.Username);
            builder.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasOne(u => u.ProfilePicture)
                .WithMany(p => p.Users)
                .HasForeignKey(u => u.ProfilePictureId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
