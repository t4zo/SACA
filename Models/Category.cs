using System.Collections.Generic;
using SACA.Models.Identity;

namespace SACA.Models
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string IconName { get; set; }

        public ICollection<ApplicationUser> ApplicationUsers { get; set; }
        public ICollection<Image> Images { get; set; }
    }
}