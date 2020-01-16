using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutomateBussiness.Data;
using AutomateBussiness.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AutomateBussiness.Controllers
{
    public class OrganizationController : Controller
    {
        public List<OrganizationModel> Organization { get; set; }
        private readonly AutomateBussinessContext _context;
        public object Claimtype { get; private set; }

        public OrganizationController(AutomateBussinessContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            var All_Organizations = from m in _context.Organizations
                         select m;

            List<OrganizationModel> Organization = await All_Organizations.ToListAsync();
                       
            if (Organization == null)
            {
                return NotFound();
            }
            ViewBag.JsonData = JsonConvert.SerializeObject(Organization);
            return View(Organization);
        }

        [HttpPost]
        public async Task<List<OrganizationModel>> getOrgData()
        {
            var All_Organizations = from m in _context.Organizations
                                    select m;

            List<OrganizationModel> Organization = await All_Organizations.ToListAsync();

            if (Organization == null)
            {
                return null;
            }

            return Organization;
        }
             

    }
}