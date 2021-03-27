namespace SACA.Entities.Responses
{
    public class ImageResponse : BaseEntity
    {
        public int CategoryId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string FullyQualifiedPublicUrl { get; set; }
    }
}