using SACA.Entities.Identity;
using System.Text.Json.Serialization;

namespace SACA.Entities
{
    public class Image : IEntity
    {
        //public Image()
        //{
        //    Category = new Category();
        //    User = new ApplicationUser();
        //}

        public int Id { get; set; }

        public int CategoryId { get; set; }

        [JsonIgnore]
        public Category Category { get; set; }

        public int? UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string Name { get; set; }
        public string Url { get; set; }
    }
}