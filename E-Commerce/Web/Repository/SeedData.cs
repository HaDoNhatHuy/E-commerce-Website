using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Repository
{
    public class SeedData
    {
        public static void SeedingData(DataContext _context)
        {
            _context.Database.Migrate();
            if (!_context.Products.Any())
            {
                CategoryModel Laptop = new CategoryModel { Name = "Laptop", Slug = "laptop", Description = "Laptop is necessary for everone", Status = 1 };
                CategoryModel Phone = new CategoryModel { Name = "Phone", Slug = "phone", Description = "Phone is necessary for everone", Status = 1 };
                BrandModel apple = new BrandModel { Name = "Apple", Slug = "apple", Description = "Apple is large brand in the world", Status = 1 };
                BrandModel samsung = new BrandModel { Name = "Samsung", Slug = "samsung", Description = "Samsung is large brand in the world", Status = 1 };
                _context.Products.AddRange(
                    new ProductModel { Name = "Macbook", Slug = "Macbook", Description = "Macbook is an American Brand", Picture = "1.jpg", Category = Laptop, Brand = apple, Price = 123456 },
                    new ProductModel { Name = "Samsung", Slug = "Samsung", Description = "Samsung is an Korean Brand", Picture = "1.jpg", Category = Phone, Brand = samsung, Price = 123456 }
                );
                _context.SaveChanges();
            }
        }
    }
}
