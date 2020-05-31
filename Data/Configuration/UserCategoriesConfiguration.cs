using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SACA.Models;

namespace SACA.Data.Mappings
{
    public class UserCategoriesConfiguration : IEntityTypeConfiguration<UserCategory>
    {
        public void Configure(EntityTypeBuilder<UserCategory> builder)
        {
            builder.HasKey(userCategory => new { userCategory.UserId, userCategory.CategoryId });

            builder.HasOne(userCategory => userCategory.User)
                .WithMany(user => user.UserCategories)
                .HasForeignKey(userCategory => userCategory.UserId)
                .IsRequired(required: true)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(userCategory => userCategory.Category)
                .WithMany(category => category.UserCategories)
                .HasForeignKey(userCategory => userCategory.CategoryId)
                .IsRequired(required: true)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
