using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using AutomateBussiness.Hubs;
using AutomateBussiness.Models;

namespace AutomateBussiness.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IHubContext<ChatHub> _hubContext;

        public AccountController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, IHubContext<ChatHub> hubContext)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _hubContext = hubContext;
        }

        [HttpGet]
        public IActionResult Register()
        {
            
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserAccount model)
        {
            if (ModelState.IsValid)
            {
                // Copy data from RegisterViewModel to IdentityUser
                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                // Store user data in AspNetUsers database table
                var result = await userManager.CreateAsync(user, model.Password);

                // If user is successfully created, sign-in the user using
                // SignInManager and redirect to index action of HomeController
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("index", "home");
                }

                // If there are any errors, add them to the ModelState object
                // which will be displayed by the validation summary tag helper
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
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
                var result = await signInManager.PasswordSignInAsync(
                    model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    await _hubContext.Clients.Group("SignalR Users").SendAsync("ReceiveMessage", model.Email, $"-- Login success on : {DateTime.Now}");
                    
                    return RedirectToAction("Dashboard", "home");
                }
                await _hubContext.Clients.Group("SignalR Users").SendAsync("ReceiveMessage", model.Email, $"-- Login fail on : {DateTime.Now}");
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }



            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            return RedirectToAction("index", "home");
        }
    }
}