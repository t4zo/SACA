using SACA.Models.Identity;
using System.Text.Json.Serialization;

namespace SACA.Models
{
    public class Image : BaseEntity
    {
        public int CategoryId { get; set; }

        [JsonIgnore]
        public Category Category { get; set; }

        public int? UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string Name { get; set; }
        public string Url { get; set; }
        public string FullyQualifiedPublicUrl { get; set; }
    }
}