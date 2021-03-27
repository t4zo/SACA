using SACA.Entities;

namespace SACA.Data.Seed.Models
{
    public class CategoriesSeed : EntitySeed<Category>
    {
        public CategoriesSeed(ApplicationDbContext context) : base(context, "SACA.Data.Seed.Json.00Categories.json")
        {
        }
    }
}