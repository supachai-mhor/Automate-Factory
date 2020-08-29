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
using AutomateBussiness.Data;

namespace AutomateBussiness.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AccountViewModel> userManager;
        private readonly SignInManager<AccountViewModel> signInManager;
        private readonly AutomateBussinessContext _context;

        public string facName = "";
        public HomeController(ILogger<HomeController> logger, UserManager<AccountViewModel> userManager,
            SignInManager<AccountViewModel> signInManager, AutomateBussinessContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _logger = logger;
            _context = context;
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
        private void getFacName()
        {
            var facId = userManager.Users.Where(m => m.UserName == User.Identity.Name).First().factoryID;
            var factory = _context.FactoryTable.Where(m => m.id == facId);
            if (factory.Count() > 0)
            {
                facName = factory.First().factoryName;
            }
        }
        [Authorize]//(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Dashboard()
        {

            getFacName();
            ViewBag.factoryName = facName;

            return View();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Desktop()
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
