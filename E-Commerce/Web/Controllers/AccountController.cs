using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Models.ViewModels;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        public AccountController(UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Account(string returnUrl)
        {
            return View(new AccountViewModel
            {
                LoginViewModel = new LoginViewModel { ReturnUrl = returnUrl },
                RegisterViewModel = new RegisterViewModel()
            });
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
    }
}