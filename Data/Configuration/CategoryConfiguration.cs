using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SACA.Constants;
using SACA.Models;

namespace SACA.Data.Mappings
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasIdentityOptions(startValue: AuthorizationConstants.Database.StartValueId);
        }
    }
}
