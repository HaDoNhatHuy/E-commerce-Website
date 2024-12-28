using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Models;
using Web.Models.ViewModels;
using Web.Repository;

namespace Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _environment;

        public ProductController(DataContext context, IWebHostEnvironment environment)
        {
            _dataContext = context;
            _environment = environment;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Details(int Id)
        {
            if (Id == null)
                return RedirectToAction("Index");
            var productById = _dataContext.Products
                .Include("Category")
                .Include("Brand")
                .Include(i => i.Ratings)
                .Where(i => i.Id == Id).FirstOrDefault();
            //var productById = await _dataContext.Products.Where(p => p.Id == Id).FirstOrDefaultAsync();

            //Related Product
            var relatedProducts = await _dataContext.Products
                .Where(i => i.CategoryId == productById.CategoryId && i.Id != productById.Id)
                .Take(4)
                .ToListAsync();
            ViewBag.RelatedProducts = relatedProducts;

            var listComments = await _dataContext.Ratings
                .Where(i => i.ProductId == Id)
                .Take(2)
                .ToListAsync();
            //ViewBag.ListComments = listComments;

            var productViewModel = new ProductDetailsViewModel
            {
                ProductDetails = productById,
                //RatingDetails = productById.Ratings
                ListComments = listComments
            };
            return View(productViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Search(string searchItem)
        {
            var products = await _dataContext.Products
                .Where(i => i.Name.Contains(searchItem) || i.Description.Contains(searchItem))
                .ToListAsync();

            ViewBag.Keyword = searchItem;
            return View(products);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rating(RatingModel rate)
        {
            if (ModelState.IsValid)
            {
                var newRating = new RatingModel
                {
                    ProductId = rate.ProductId,
                    Name = rate.Name,
                    Email = rate.Email,
                    Comment = rate.Comment,
                    Rating = rate.Rating, //đánh giá sao
                };
                if (rate.ImageUpload != null)
                {
                    string fileUpload = Path.Combine(_environment.WebRootPath, "assets/images/ratingImages/");
                    string imageName = Guid.NewGuid().ToString() + "_" + rate.ImageUpload.FileName;
                    string filePath = Path.Combine(fileUpload, imageName);

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await rate.ImageUpload.CopyToAsync(fs);
                    fs.Close();

                    newRating.Image = imageName;
                }

                _dataContext.Ratings.Add(newRating);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm đánh giá thành công";
                return Redirect(Request.Headers["Referer"]);
            }
            else
            {
                TempData["error"] = "Model có một vài thứ đang bị lỗi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                    string errorMessage = string.Join("\n", errors);
                    return RedirectToAction("Details", new { Id = rate.ProductId });
                }
            }
            return Redirect(Request.Headers["Referer"]);
        }

    }
}
