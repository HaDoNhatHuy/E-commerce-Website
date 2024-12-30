using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Web.Models;
using Web.Repository;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Banner")]
    //[Authorize(Roles = " Admin")]
    public class BannerController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _environment;
        public BannerController(DataContext dataContext, IWebHostEnvironment environment)
        {
            _dataContext = dataContext;
            _environment = environment;
        }
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Banners.OrderByDescending(i => i.Id).ToListAsync());
        }
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BannerModel banner)
        {

            if (ModelState.IsValid)
            {
                var newBanner = new BannerModel();
                newBanner.Name = banner.Name;
                newBanner.Description = banner.Description;
                newBanner.Status = banner.Status;
                if (banner.ImageUpload != null)
                {
                    string fileUpload = Path.Combine(_environment.WebRootPath, "assets/images/bannerImages/");
                    string pictureName = Guid.NewGuid().ToString() + "_" + banner.ImageUpload.FileName;
                    string filePath = Path.Combine(fileUpload, pictureName);

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await banner.ImageUpload.CopyToAsync(fs);
                    fs.Close();

                    newBanner.Image = pictureName;
                }
                _dataContext.Banners.Add(newBanner);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm Banner thành công";
                return RedirectToAction("Index");
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
                    return BadRequest(errorMessage);
                }
                return View(banner);
            }
        }
        [Route("Edit")]
        public async Task<IActionResult> Edit(int Id)
        {
            var existBanner = await _dataContext.Banners.FindAsync(Id);
            return View(existBanner);
        }
        [HttpPost]
        [Route("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BannerModel banner)
        {
            var existBanner = await _dataContext.Banners.FindAsync(banner.Id);
            if (banner == null && !ModelState.IsValid)
            {
                return NotFound();
            }
            existBanner.Name = banner.Name;
            existBanner.Description = banner.Description;
            existBanner.Status = banner.Status;
            if (existBanner.ImageUpload != null)
            {
                string fileUpload = Path.Combine(_environment.WebRootPath, "assets/images/bannerImages/");
                string pictureName = Guid.NewGuid().ToString() + "_" + banner.ImageUpload.FileName;
                string filePath = Path.Combine(fileUpload, pictureName);

                FileStream fs = new FileStream(filePath, FileMode.Create);
                await banner.ImageUpload.CopyToAsync(fs);
                fs.Close();

                existBanner.Image = pictureName;
            }
            TempData["success"] = "Cập nhật Banner thành công";
            _dataContext.Banners.Update(existBanner);
            await _dataContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult> Delete(int Id)
        {
            var banner = await _dataContext.Banners.FindAsync(Id);
            if (banner == null)
            {
                return NotFound();
            }
            string fileUpload = Path.Combine(_environment.WebRootPath, "assets/images/bannerImages/");
            string pictureName = Guid.NewGuid().ToString() + "_" + banner.Image;
            if (System.IO.File.Exists(pictureName))
            {
                System.IO.File.Delete(pictureName);
            }
            _dataContext.Remove(banner);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Xóa Banner thành công";
            return RedirectToAction("Index");
        }
    }
}
