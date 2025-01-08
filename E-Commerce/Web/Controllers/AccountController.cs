using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Web.Areas.Admin.Repository;
using Web.Models;
using Web.Models.ViewModels;
using Web.Repository;
namespace Web.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly DataContext _dataContext;
        public AccountController(UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager, DataContext dataContext, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dataContext = dataContext;
            _emailSender = emailSender;
        }
        public IActionResult Account(string returnUrl)
        {
            return View(new AccountViewModel
            {
                LoginViewModel = new LoginViewModel { ReturnUrl = returnUrl },
                RegisterViewModel = new RegisterViewModel()
            });
        }
        public IActionResult ForgetPassword(string returnUrl)
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SendMailForgotPass(AppUserModel user)
        {
            var checkMail = await _userManager.Users.FirstOrDefaultAsync(i => i.Email == user.Email);
            if (checkMail == null)
            {
                TempData["error"] = "Email không tìm thấy";
                return RedirectToAction("ForgetPassword", "Account");
            }
            else
            {
                string Token = Guid.NewGuid().ToString();
                checkMail.Token = Token;
                _dataContext.Update(checkMail);
                await _dataContext.SaveChangesAsync();

                var receiver = checkMail.Email;
                var subject = "Thay đổi password cho người dùng" + checkMail.Email;
                var message = "Click vào link để thay đổi mật khẩu" +
                    "<a href='" + $"{Request.Scheme}://{Request.Host}/Account/NewPassword?email="
                    + checkMail.Email + "&token=" + Token + "'</a>";
                await _emailSender.SendEmailAsync(receiver, subject, message);
            }
            TempData["success"] = "An email has been sent to your registered email address with password reset instructions.";
            return RedirectToAction("ForgetPassword", "Account");
        }
        public async Task<IActionResult> NewPassword(AppUserModel user, string token)
        {
            var checkUser = await _userManager.Users
                .Where(i => i.Email == user.Email && i.Token == user.Token)
                .FirstOrDefaultAsync();
            if (checkUser != null)
            {
                ViewBag.Email = checkUser.Email;
                ViewBag.Token = token;
            }
            else
            {
                TempData["error"] = "Email not found or Token is not right";
                return RedirectToAction("ForgetPassword", "Account");
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateNewPassword(AppUserModel user, string token)
        {
            var checkUser = await _userManager.Users
                .Where(i => i.Email == user.Email && i.Token == user.Token)
                .FirstOrDefaultAsync();
            if (checkUser != null)
            {
                string newToken = Guid.NewGuid().ToString();
                var passwordHasher = new PasswordHasher<AppUserModel>();
                var passwordHash = passwordHasher.HashPassword(checkUser, user.PasswordHash);
                checkUser.PasswordHash = passwordHash;
                checkUser.Token = newToken;
                await _userManager.UpdateAsync(checkUser);
                TempData["success"] = "Password Update Successfully";
                return RedirectToAction("Account");
            }
            else
            {
                TempData["error"] = "Email not found or Token is not right";
                return RedirectToAction("ForgetPassword", "Account");
            }
        }
        public async Task<IActionResult> History()
        {
            if ((bool)!User.Identity?.IsAuthenticated)
            {
                //User is not logged in, redirect to login
                return RedirectToAction("Account");
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
        public async Task<IActionResult> HistoryDetails(string OrderCode)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var orderDetails = await _dataContext.OrderDetails.Include(i => i.Product).Where(i => i.OrderCode == OrderCode).ToListAsync();
            var orderInfo = await _dataContext.Orders.Where(i => i.OrderCode == OrderCode && i.UserName == userEmail).FirstAsync();
            ViewBag.OrderInfo = orderInfo;
            ViewBag.ShippingFee = orderInfo.ShippingFee;
            ViewBag.OrderDate = orderInfo.CreatedDate;
            ViewBag.OrderCode = orderInfo.OrderCode;
            decimal discountValue = 0;
            var couponCookie = Request.Cookies["CouponTitle"];
            if (couponCookie != null)
            {
                var coupon = await _dataContext.Coupons.Where(i => i.Equals(couponCookie)).FirstOrDefaultAsync();
                discountValue = coupon.Discount;
            }
            ViewBag.DiscountValue = discountValue;
            return View(orderDetails);
        }
        public async Task<IActionResult> CancelOrder(string OrderCode)
        {
            if ((bool)!User.Identity?.IsAuthenticated)
            {
                //User is not logged in, redirect to login
                return RedirectToAction("Account");
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
        public async Task<IActionResult> AccountInfo()
        {
            if ((bool)!User.Identity?.IsAuthenticated)
            {
                //User is not logged in, redirect to login
                return RedirectToAction("Account");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var infomations = await _userManager.Users.Where(i => i.Id == userId).FirstOrDefaultAsync();
            return View(infomations);
        }
        public IActionResult UpdateAccount()
        {
            return View();
        }
        //[HttpPost]
        //public async Task<IActionResult> UpdateInfoAccount(AppUserModel appUserModel)
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var user = _userManager.Users.Where(i => i.Id == userId).FirstOrDefault();
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }
        //    else
        //    {

        //        var userUpdate = await _userManager.FindByIdAsync(user.Id);
        //        userUpdate.FullName = appUserModel.FullName;
        //        userUpdate.Email = appUserModel.Email;
        //        userUpdate.PhoneNumber = appUserModel.PhoneNumber;
        //        userUpdate.Address = appUserModel.Address;
        //        // Kiểm tra mật khẩu hiện tại
        //        if (!string.IsNullOrWhiteSpace(appUserModel.PasswordHash))
        //        {
        //            if (appUserModel.PasswordHash != user.PasswordHash)
        //            {
        //                TempData["error"] = "Mật khẩu hiện tại không chính xác.";
        //                return RedirectToAction("UpdateAccount", "Account");
        //            }
        //            // Kiểm tra New Password và Confirm Password
        //            if (!string.IsNullOrWhiteSpace(appUserModel.ConfirmPassword))
        //            {
        //                if (appUserModel.ConfirmPassword == user.PasswordHash)
        //                {
        //                    TempData["error"] = "Mật khẩu mới không được trùng với mật khẩu hiện tại.";
        //                    return RedirectToAction("UpdateAccount", "Account");
        //                }

        //                if (appUserModel.NewPassword != appUserModel.ConfirmPassword)
        //                {
        //                    TempData["error"] = "Xác nhận mật khẩu không khớp với mật khẩu mới.";
        //                    return RedirectToAction("UpdateAccount", "Account");
        //                }
        //            }
        //            else
        //            {
        //                var passwordHasher = new PasswordHasher<AppUserModel>();
        //                var passwordHash = passwordHasher.HashPassword(user, user.NewPassword);
        //                user.PasswordHash = passwordHash;
        //            }
        //        }
        //        await _userManager.UpdateAsync(userUpdate);
        //        await _dataContext.SaveChangesAsync();
        //        TempData["success"] = "Cập nhật tài khoản thành công";
        //        return RedirectToAction("UpdateAccount", "Account");
        //    }
        //}
        [HttpPost]
        public async Task<IActionResult> UpdateInfoAccount(AppUserModel appUserModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            // Cập nhật các thông tin không liên quan đến mật khẩu
            user.FullName = appUserModel.FullName;
            user.Email = appUserModel.Email;
            user.PhoneNumber = appUserModel.PhoneNumber;
            user.Address = appUserModel.Address;

            // Kiểm tra mật khẩu hiện tại nếu cần thay đổi mật khẩu
            if (!string.IsNullOrWhiteSpace(appUserModel.PasswordHash))
            {
                var passwordHasher = new PasswordHasher<IdentityUser>();

                // Kiểm tra mật khẩu hiện tại
                var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, appUserModel.PasswordHash);
                if (passwordVerificationResult == PasswordVerificationResult.Failed)
                {
                    TempData["error"] = "Mật khẩu hiện tại không chính xác.";
                    return RedirectToAction("UpdateAccount", "Account");
                }

                // Kiểm tra mật khẩu mới và xác nhận mật khẩu
                if (!string.IsNullOrWhiteSpace(appUserModel.NewPassword) && !string.IsNullOrWhiteSpace(appUserModel.ConfirmPassword))
                {
                    if (appUserModel.NewPassword == appUserModel.PasswordHash)
                    {
                        TempData["error"] = "Mật khẩu mới không được trùng với mật khẩu hiện tại.";
                        return RedirectToAction("UpdateAccount", "Account");
                    }

                    if (appUserModel.NewPassword != appUserModel.ConfirmPassword)
                    {
                        TempData["error"] = "Xác nhận mật khẩu không khớp với mật khẩu mới.";
                        return RedirectToAction("UpdateAccount", "Account");
                    }

                    // Hash mật khẩu mới
                    user.PasswordHash = passwordHasher.HashPassword(user, appUserModel.NewPassword);
                }
                else
                {
                    TempData["error"] = "Mật khẩu mới và xác nhận mật khẩu không được để trống.";
                    return RedirectToAction("UpdateAccount", "Account");
                }
            }

            // Cập nhật thông tin người dùng
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                TempData["error"] = "Có lỗi xảy ra khi cập nhật tài khoản.";
                return RedirectToAction("UpdateAccount", "Account");
            }

            TempData["success"] = "Cập nhật tài khoản thành công.";
            return RedirectToAction("UpdateAccount", "Account");
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