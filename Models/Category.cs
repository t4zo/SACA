using System.Collections.Generic;

namespace SACA.Models
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string IconName { get; set; }

        public ICollection<UserCategory> UserCategories { get; set; }
        public ICollection<Image> Images { get; set; }
    }
}