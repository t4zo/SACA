using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SACA.Models;
using SACA.Utilities;

namespace SACA.Data.Mappings
{
    public class ImageConfiguration : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.Property(c => c.Id)
                .HasIdentityOptions(startValue: Constants.DatabaseIdStartValue);

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

            builder.HasKey(image => image.Id);
        }
    }
}
