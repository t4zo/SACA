using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SACA.Constants;
using SACA.Models;

namespace SACA.Data.Mappings
{
    public class ImageConfiguration : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.HasKey(image => image.Id);

            builder.Property(c => c.Id)
                .HasIdentityOptions(startValue: AuthorizationConstants.Database.StartValueId);

            builder.HasOne(image => image.Category)
                .WithMany(category => category.Images)
                .HasForeignKey(image => image.CategoryId)
                .IsRequired(required: false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(image => image.User)
                .WithMany(user => user.Images)
                .HasForeignKey(image => image.UserId)
                .IsRequired(required: false)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
