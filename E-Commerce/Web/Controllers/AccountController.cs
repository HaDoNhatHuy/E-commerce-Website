using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Web.Models;
using Web.Models.ViewModels;
using Web.Repository;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly DataContext _dataContext;
        public AccountController(UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager, DataContext dataContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dataContext = dataContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult Account(string returnUrl)
        {
            return View(new AccountViewModel
            {
                LoginViewModel = new LoginViewModel { ReturnUrl = returnUrl },
                RegisterViewModel = new RegisterViewModel()
            });
        }
        public async Task<IActionResult> History()
        {
            if ((bool)!User.Identity?.IsAuthenticated)
            {
                //User is not logged in, redirect to login
                RedirectToAction("Login", "Acount");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var Orders = await _dataContext.Orders
                .Where(i => i.UserName == userEmail)
                .OrderByDescending(i => i.Id)
                .ToListAsync();
            ViewBag.Email = userEmail;
            return View(Orders);
        }
        public async Task<IActionResult> HistoryDetails()
        {
            return View();
        }
        public async Task<IActionResult> CancelOrder(string OrderCode)
        {
            if ((bool)!User.Identity?.IsAuthenticated)
            {
                //User is not logged in, redirect to login
                RedirectToAction("Login", "Acount");
            }
            try
            {
                var order = await _dataContext.Orders.Where(i => i.OrderCode == OrderCode).FirstAsync();
                order.Status = 3;
                if (order != null)
                {
                    _dataContext.Orders.Update(order);
                    await _dataContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return RedirectToAction("History", "Account");
        }
        [HttpPost]
        public async Task<IActionResult> Login(AccountViewModel loginVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(loginVM.LoginViewModel.UserName, loginVM.LoginViewModel.Password, false, false);
                if (result.Succeeded)
                {
                    return Redirect(loginVM.LoginViewModel.ReturnUrl ?? "/");
                }
                ModelState.AddModelError("", "Invalid username or password.");
            }

            // Re-populate the view model with register data
            var model = new AccountViewModel
            {
                LoginViewModel = loginVM.LoginViewModel,
                RegisterViewModel = new RegisterViewModel()
            };

            return View("Account", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AccountViewModel registerVM)
        {
            if (ModelState.IsValid)
            {
                var newUser = new AppUserModel { UserName = registerVM.RegisterViewModel.UserName, Email = registerVM.RegisterViewModel.Email };
                var result = await _userManager.CreateAsync(newUser, registerVM.RegisterViewModel.Password);

                if (result.Succeeded)
                {
                    TempData["success"] = "Registration successful!";
                    return RedirectToAction("Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Re-populate the view model with login data
            var model = new AccountViewModel
            {
                LoginViewModel = new LoginViewModel(),
                RegisterViewModel = registerVM.RegisterViewModel
            };

            return View("Account", model);
        }
        public async Task<IActionResult> Logout(string returnUrl = "/")
        {
            await _signInManager.SignOutAsync();
            return Redirect(returnUrl);
        }
    }
}