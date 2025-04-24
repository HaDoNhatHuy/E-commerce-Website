using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web.Models;
using Web.Repository;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ContactController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ContactController(DataContext dataContext, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = dataContext;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var contact = _dataContext.Contacts.ToList();
            return View(contact);
        }
        public async Task<IActionResult> Edit(int Id)
        {
            var contact = await _dataContext.Contacts.FirstOrDefaultAsync();
            return View(contact);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ContactModel contact)
        {
            var exitsted_contact = await _dataContext.Contacts.FirstOrDefaultAsync();
            if (ModelState.IsValid)
            {                
                if (contact.LogoImageUpload != null)
                {
                    string fileUpload = Path.Combine(_webHostEnvironment.WebRootPath, "assets/images/contactImages/");
                    string pictureName = Guid.NewGuid().ToString() + "_" + contact.LogoImageUpload.FileName;
                    string filePath = Path.Combine(fileUpload, pictureName);

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await contact.LogoImageUpload.CopyToAsync(fs);
                    fs.Close();

                    exitsted_contact.LogoImage = pictureName;

                }
                exitsted_contact.Title = contact.Title;
                exitsted_contact.Description = contact.Description;
                exitsted_contact.Email = contact.Email;
                exitsted_contact.Phone = contact.Phone;
                exitsted_contact.Map = contact.Map;
                _dataContext.Update(exitsted_contact);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Cập nhật thông tin liên hệ thành công";
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
            }
            return View(contact);
        }
    }
}
