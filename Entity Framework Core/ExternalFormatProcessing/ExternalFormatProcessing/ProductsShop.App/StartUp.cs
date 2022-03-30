namespace ProductsShop.App
{
    using Newtonsoft.Json;
    using ProductsShop.Data;
    using ProductsShop.Models;
    using System.IO;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using System.Xml.Linq;

    public class StartUp   
    {
        public static void Main(string[] args)
        {
            DbTool dbTool = new DbTool();
            dbTool.Reset();
            Console.WriteLine(ImportUsersFromXml());
            Console.WriteLine(ImportCategoriesFromXml());
            Console.WriteLine(ImportProductsFromXml());
            ExportProductsInRange();
        }
        static void ExportProductsInRange()
        {
            using (var context = new ProductsShopDbContext())
            {
                var products = context.Products
                    .Where(p =>p.Buyer != null && p.Price >= 1000 && p.Price <= 2000)
                    .Select(p => new
                    {
                        p.Name,
                        p.Price,
                        BuyerFullName = p.Buyer.FirstName == null
                            ? p.Buyer.LastName
                            : $"{p.Buyer.FirstName} {p.Buyer.LastName}"
                    })
                    .ToArray();

                var xDoc = new XDocument(new XElement("products"));
                foreach (var p in products)
                {
                    xDoc.Root.Add(new XElement("product"),
                                    new XAttribute("name", p.Name),
                                    new XAttribute("price", p.Price),
                                    new XAttribute("buyer", p.BuyerFullName));
                }

                string xmlString = xDoc.ToString();
                string path = @"D:\C# DATABASES ADVANCED  - Entity Framework\ExternalFormatProcessing\ExternalFormatProcessing\ProductsShop.App\Files\productsInRange.xml";
                
                File.WriteAllText(path ,xmlString);
                context.SaveChanges();
            }
        }
        static string ImportProductsFromXml()
        {
            string path = @"D:\C# DATABASES ADVANCED  - Entity Framework\ExternalFormatProcessing\ExternalFormatProcessing\ProductsShop.App\Files\products.xml";
            var elements = ImportXml(path);
            var catProducts = new List<CategoryProduct>();

            using (var context = new ProductsShopDbContext())
            {
                var userIds = context.Users.Select(u => u.Id).ToArray();
                var categoryIds = context.Categories.Select(c => c.Id).ToArray();

                Random rnd = new Random();

                foreach (var p in elements)
                {
                    string name = p.Element("name").Value;
                    decimal price = decimal.Parse(p.Element("price").Value);

                    int sellerIndex = rnd.Next(0, userIds.Length);
                    int sellerId = userIds[sellerIndex];

                    var product = new Product()
                    {
                        Name = name,
                        Price = price,
                        SellerId = sellerId
                    };

                    int categoryIndex = rnd.Next(0, categoryIds.Length);
                    int categoryId = categoryIds[categoryIndex];

                    var category = new CategoryProduct()
                    {
                        Product = product,
                        CategoryId = categoryId
                    };

                    catProducts.Add(category);
                }

                context.AddRange(catProducts);
                context.SaveChanges();

                return $"{catProducts.Count} products were imported from file: {path}";
            }                      
        }
        static string ImportCategoriesFromXml()
        {
            string path = @"D:\C# DATABASES ADVANCED  - Entity Framework\ExternalFormatProcessing\ExternalFormatProcessing\ProductsShop.App\Files\categories.xml";
            var elements = ImportXml(path);

            var categories = new List<Category>();
            foreach (var e in elements)
            {
                var category = new Category()
                {
                    Name = e.Element("name").Value
                };

                categories.Add(category);
            }

            using (var context = new ProductsShopDbContext())
            {
                context.Categories.AddRange(categories);
                context.SaveChanges();
            }

            return $"{categories.Count} categories were imported from file: {path}";
        }
        static string ImportUsersFromXml()
        {
            string path = @"D:\C# DATABASES ADVANCED  - Entity Framework\ExternalFormatProcessing\ExternalFormatProcessing\ProductsShop.App\Files\users.xml";
            var elements = ImportXml(path);
            var users = new List<User>();

            foreach (var e in elements)
            {
                string firstName = e.Attribute("firstName")?.Value;
                string lastName = e.Attribute("lastName").Value;

                int? age = null;
                if (e.Attribute("age") != null)
                {
                    age = int.Parse(e.Attribute("age").Value);
                }

                var user = new User()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Age = age
                };

                users.Add(user);
            }

            using (var context = new ProductsShopDbContext())
            {
                context.Users.AddRange(users);
                context.SaveChanges();
            }

            return $"{users.Count} users were imported from file: {path}";
        }
        static void GetUsersAndProducts()
        {
            using (var context = new ProductsShopDbContext())
            {
                var users = context.Users
                    .Where(u => u.ProductsSold.Any())         
                    .Select(u => new
                    {                       
                        u.FirstName,
                        u.LastName,
                        Age = u.Age ?? 0,
                        SoldProducts = new
                        {
                            Count = u.ProductsSold.Count,
                            Products = u.ProductsSold.Select(ps=>new 
                            {
                                ps.Name,
                                ps.Price
                            })
                        }
                    })
                    .OrderByDescending(u => u.SoldProducts.Count)
                    .ThenBy(u => u.LastName)
                    .ToArray();

                string path = @"D:\C# DATABASES ADVANCED  - Entity Framework\ExternalFormatProcessing\ExternalFormatProcessing\ProductsShop.App\Files\UsersAndProducts.json";
               
                var jsonReady = new
                {
                    usersCount = users.Length,
                    users = users
                };

                var jsonString = JsonConvert.SerializeObject(jsonReady, Formatting.Indented, new JsonSerializerSettings()
                {
                    DefaultValueHandling = DefaultValueHandling.Ignore
                });
                File.WriteAllText(path, jsonString);

            }
        }
        static void GetCategoriesByCount()
        {
            using (var context = new ProductsShopDbContext())
            {
                var categories = context.Categories
                    .OrderBy(c => c.Name)
                    .Select(c => new
                    {
                        c.Name,
                        Count = c.Products.Count,
                        AveragePrice = c.Products
                               .Select(p=>p.Product.Price)
                               .Average(),
                        TotalPriceSum = c.Products
                               .Select(p=>p.Product.Price)
                               .Sum() 
                    })                    
                    .ToArray();

                string path = @"D:\C# DATABASES ADVANCED  - Entity Framework\ExternalFormatProcessing\ExternalFormatProcessing\ProductsShop.App\Files\CategoriesByProducts.json";
                var jsonString = JsonConvert.SerializeObject(categories, Formatting.Indented, new JsonSerializerSettings()
                {
                    DefaultValueHandling = DefaultValueHandling.Ignore
                });
                File.WriteAllText(path, jsonString);
            }
        }
        static void GetSuccessfullySoldProducts()
        {
            using (var context = new ProductsShopDbContext())
            {
                var products = context.Users
                    .Where(u => u.ProductsSold.Any(p=>p.BuyerId != null))
                    .OrderBy(u => u.LastName)
                    .ThenBy(u => u.FirstName)
                    .Select(u => new
                    {
                        u.FirstName,
                        u.LastName,
                        SoldProducts = u.ProductsSold.Select(p => new
                        {
                            p.Name,
                            p.Price,
                            BuyerFirstName = p.Buyer.FirstName,
                            BuyerLastName = p.Buyer.LastName
                        }).ToArray()
                    })
                    .ToArray();

                string path = @"D:\C# DATABASES ADVANCED  - Entity Framework\ExternalFormatProcessing\ExternalFormatProcessing\ProductsShop.App\Files\SoldProducts.json";
                var jsonString = JsonConvert.SerializeObject(products,Formatting.Indented,new JsonSerializerSettings() 
                {
                    DefaultValueHandling = DefaultValueHandling.Ignore
                });
                File.WriteAllText(path, jsonString);
            }
        }
        static void GetProductsInRange()
        {
            using (var context = new ProductsShopDbContext())
            {
                var products = context.Products
                    .Where(p => p.Price >= 500 && p.Price <= 1000)
                    .OrderBy(p=>p.Price)
                    .Select(p => new
                    {
                        p.Name,
                        p.Price,
                        Seller = $"{p.Seller.FirstName} {p.Seller.LastName}"
                    })
                    .ToArray();

                var jsonString = JsonConvert.SerializeObject(products,Formatting.Indented);
                string path = @"D:\C# DATABASES ADVANCED  - Entity Framework\ExternalFormatProcessing\ExternalFormatProcessing\ProductsShop.App\Files\PricesInRange.json";
                File.WriteAllText(path, jsonString);
            }
        }
        static void SetCategories()
        {
            using (var context = new ProductsShopDbContext())
            {
                var productIds = context.Products.AsNoTracking().Select(p => p.Id).ToArray();
                var categoryIds = context.Categories.AsNoTracking().Select(c => c.Id).ToArray();

                Random rnd = new Random();

                var categoryProducts = new List<CategoryProduct>();

                foreach (var p in productIds)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        int index = rnd.Next(0, categoryIds.Length);

                        while (categoryProducts.Any(cp=>cp.ProductId == p 
                            && cp.CategoryId == categoryIds[index]))
                        {
                            index = rnd.Next(0, categoryIds.Length);
                        }

                        var catProduct = new CategoryProduct()
                        {
                            ProductId = p,
                            CategoryId = categoryIds[index]
                        };

                        categoryProducts.Add(catProduct);
                    }
                }

                context.CategoryProducts.AddRange(categoryProducts);
                context.SaveChanges();
            }
            
        }
        static string ImportProductsFromJson()
        {
            string path = @"D:\C# DATABASES ADVANCED  - Entity Framework\ExternalFormatProcessing\ExternalFormatProcessing\ProductsShop.App\Files\products.json";

            Random rnd = new Random();

            Product[] products = ImportJson<Product>(path);
            
            using (var context = new ProductsShopDbContext())
            {               
                int[] userIds = context.Users
                    .Select(u => u.Id)
                    .ToArray();

                foreach (var p in products)
                {
                    int index = rnd.Next(0, userIds.Length);
                    int sellerId = userIds[index];

                    int? buyerId = sellerId;
                    while (buyerId == sellerId)
                    {
                        int buyerIndex = rnd.Next(0, userIds.Length);
                        buyerId = userIds[buyerIndex];
                    }
                    if (buyerId - sellerId < 5 && buyerId - sellerId > 0)
                    {
                        buyerId = null;
                    }
                    p.SellerId = sellerId;
                    p.BuyerId = buyerId;
                }

                context.Products.AddRange(products);
                context.SaveChanges();                
            }

            return $"{products.Length} products were imported from file: {path}";
        }
        static string ImportCategoriesFromJson()
        {
            using (var context = new ProductsShopDbContext())
            {
                string path = @"D:\C# DATABASES ADVANCED  - Entity Framework\ExternalFormatProcessing\ExternalFormatProcessing\ProductsShop.App\Files\categories.json";

                Category[] categories = ImportJson<Category>(path);

                context.Categories.AddRange(categories);
                context.SaveChanges();

                return $"{categories.Length} categories were imported from file: {path}";
            }           
        }
        static string ImportUsersFromJson()
        {
            using (var context = new ProductsShopDbContext())
            {
                string path = @"D:\C# DATABASES ADVANCED  - Entity Framework\ExternalFormatProcessing\ExternalFormatProcessing\ProductsShop.App\Files\users.json";

                User[] users = ImportJson<User>(path);

                context.Users.AddRange(users);
                context.SaveChanges();

                return $"{users.Length} users were imported from file: {path}";
            }
        }
        static T[] ImportJson<T>(string path)
        {
            string jsonString = File.ReadAllText(path);

            T[] objects = JsonConvert.DeserializeObject<T[]>(jsonString);

            return objects;
        }

        static IEnumerable<XElement> ImportXml(string path)
        {
            string xmlString = File.ReadAllText(path);

            var xmlDoc = XDocument.Parse(xmlString);

            var elements = xmlDoc.Root.Elements();

            return elements;
        }
    }
}
