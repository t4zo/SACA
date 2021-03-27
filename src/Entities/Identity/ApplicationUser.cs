using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace SACA.Entities.Identity
{
    public class ApplicationUser : IdentityUser<int>
    {
        public ICollection<Category> Categories { get; set; }
        public ICollection<Image> Images { get; set; }
    }
}