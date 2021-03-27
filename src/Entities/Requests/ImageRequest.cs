namespace SACA.Entities.Requests
{
    public class ImageRequest : BaseEntity
    {
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string Base64 { get; set; }
    }
}