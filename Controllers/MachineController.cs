using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AutomateBussiness.Controllers
{
    public class MachineController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}