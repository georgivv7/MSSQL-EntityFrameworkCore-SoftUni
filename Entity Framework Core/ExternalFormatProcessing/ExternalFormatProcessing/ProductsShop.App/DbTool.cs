namespace ProductsShop.App
{
    using ProductsShop.Data;
    public class DbTool
    {
        public void Reset() 
        {
            using (var context = new ProductsShopDbContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
        }
    }
}
