using SACA.Entities.Identity;
using System.Text.Json.Serialization;

namespace SACA.Entities
{
    public class Category : IEntity
    {
        //public Category()
        //{
        //    ApplicationUsers = new HashSet<ApplicationUser>();
        //    Images = new HashSet<Image>();
        //}

        public int Id { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        public ICollection<ApplicationUser> ApplicationUsers { get; set; }
        public ICollection<Image> Images { get; set; }
    }
}