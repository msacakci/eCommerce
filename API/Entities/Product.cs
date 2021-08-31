using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
        public string ProductType { get; set; }

        //public int Price { get; set; }
    }
}