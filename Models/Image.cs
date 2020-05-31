namespace SACA.Models
{
    public class Image : BaseEntity
    {
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
