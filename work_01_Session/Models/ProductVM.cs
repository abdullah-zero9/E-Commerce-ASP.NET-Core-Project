namespace work_01_Session.Models
{
    public class ProductVM
    {
        public ProductVM()
        {
            this.SalesItems = new List<int>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public double Price { get; set; }
        public double Quantity { get; set; }
        public IFormFile ImagePath { get; set; }
        public string? Image { get; set; }
        public List<int> SalesItems { get; set; }
    }
}
