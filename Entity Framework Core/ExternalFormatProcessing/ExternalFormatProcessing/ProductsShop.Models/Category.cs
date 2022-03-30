namespace ProductsShop.Models
{
    using System.Collections.Generic;
    public class Category
    {
        public Category()
        {
            Products = new HashSet<CategoryProduct>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<CategoryProduct> Products { get; set; }  
    }
}
