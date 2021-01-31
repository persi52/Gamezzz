using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gamezzz.Models;
using Gamezzz.ViewModelsFolder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Gamezzz.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> userManager;   //test@test.com Test123!
        private readonly SignInManager<AppUser> signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
          
            return RedirectToAction("index", "home");
        }

        [HttpGet]
        public IActionResult AccountSettings()
        {
            return View("AccountSettings", "Account");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser { UserName = model.Email, Email = model.Email };
                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // await signInManager.SignInAsync(user, isPersistent: false);
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

                    var confirmationLink = Url.Action("ConfirmEmail", "Account",
                    new { userId = user.Id, token = token }, Request.Scheme);

                    Console.WriteLine(confirmationLink);

                    return View("PleaseConfirm","Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }      


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                //IdentityUser signedUser = await userManager.FindByEmailAsync(model.Email);
                //var result = await signInManager.PasswordSignInAsync(signedUser.Email, model.Password, model.RememberMe, false);
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);   

                if (result.Succeeded)              
                return RedirectToAction("index", "home");
                else if(result.IsLockedOut)
                ModelState.AddModelError(string.Empty, "Account is not confirmed");
                else
                ModelState.AddModelError(string.Empty, "Invalid login attempt");
               
            }
            return View(model);
        }

        [HttpGet]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("index", "home");
            }

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return View("Error");
            }

            var result = await userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
                return View();

            return View("Error");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);

                if(user == null)
                {
                    return RedirectToAction("Login");
                }

                var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                if (!result.Succeeded)
                {
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }

                await signInManager.RefreshSignInAsync(user);
                return View("ChangePasswordConfirmation");
            }

            return View(model);
        }

        public IActionResult Profile()
        {
            return View();
        }
    }
}