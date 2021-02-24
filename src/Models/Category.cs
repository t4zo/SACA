using SACA.Models.Identity;
using System.Collections.Generic;

namespace SACA.Models
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }

        public ICollection<ApplicationUser> ApplicationUsers { get; set; }
        public ICollection<Image> Images { get; set; }
    }
}