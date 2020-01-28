using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutomateBussiness.Models;


using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AutomateBussiness.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AccountViewModel> userManager;
        private readonly SignInManager<AccountViewModel> signInManager;
        public HomeController(ILogger<HomeController> logger, UserManager<AccountViewModel> userManager,
            SignInManager<AccountViewModel> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Dashboard");
            }
            else
            {
                return View();
            }
            
        }
        [Authorize]//(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Dashboard()
        {
            
            return View();
        }

       
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
              

    }
}
