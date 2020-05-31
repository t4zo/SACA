using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SACA.Models;
using SACA.Utilities;

namespace SACA.Data.Mappings
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(category => category.Id).HasIdentityOptions(startValue: Constants.DatabaseIdStartValue);

            builder.HasKey(category => category.Id);
        }
    }
}
