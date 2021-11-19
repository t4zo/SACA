namespace SACA.Entities.Responses
{
    public class ImageResponse
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}