namespace API.DTOs
{
    public class DetailedProductDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        //public string ProductType { get; set; }
        public int ProductCategoryId { get; set; }
        public string LanguageCode { get; set; }
    }
}