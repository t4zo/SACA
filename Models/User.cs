using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace SACA.Models
{
    public class User : IdentityUser<int>
    {
        public ICollection<UserCategory> UserCategories { get; set; }
        public ICollection<Image> Images { get; set; }
    }
}