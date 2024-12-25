﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Drawing2D;
using Web.Models;
using Web.Repository;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin,Author")]
    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;
        public BrandController(DataContext context)
        {
            _dataContext = context;
        }
        public IActionResult Index()
        {
            return View(_dataContext.Brands.OrderByDescending(i => i.Id).ToList());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BrandModel brand)
        {
            if (ModelState.IsValid)
            {
                //code thêm dữ liệu
                brand.Slug = brand.Name.Replace(" ", "-");
                var slug = await _dataContext.Brands.FirstOrDefaultAsync(i => i.Slug == brand.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Thương hiệu đã có trong Database");
                    return View(brand);
                }
                _dataContext.Add(brand);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm thương hiệu thành công";
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
            return View(brand);
        }
        public async Task<IActionResult> Edit(int Id)
        {
            BrandModel brand = await _dataContext.Brands.FindAsync(Id);
            return View(brand);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id, BrandModel brand)
        {
            if (ModelState.IsValid)
            {
                //code thêm dữ liệu
                brand.Slug = brand.Name.Replace(" ", "-");
                var slug = await _dataContext.Brands.FirstOrDefaultAsync(i => i.Slug == brand.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "thương hiệu đã có trong Database");
                    return View(brand);
                }
                _dataContext.Update(brand);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Cập nhật thương hiệu thành công";
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
            return View(brand);
        }
        public async Task<IActionResult> Delete(int Id)
        {
            BrandModel brand = await _dataContext.Brands.FindAsync(Id);
            _dataContext.Brands.Remove(brand);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Đã xóa thương hiệu";
            return RedirectToAction("Index");
        }
    }
}
