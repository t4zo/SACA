using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace SACA.Models.Identity
{
    public class ApplicationUser : IdentityUser<int>
    {
        public ICollection<Category> Categories { get; set; }
        public ICollection<Image> Images { get; set; }
    }
}
