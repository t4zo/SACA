namespace SACA.Models.Dto
{
    public class ImageRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string Base64 { get; set; }
    }
}
