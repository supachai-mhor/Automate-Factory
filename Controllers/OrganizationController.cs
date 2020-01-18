using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutomateBussiness.Data;
using AutomateBussiness.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AutomateBussiness.Controllers
{
    [Authorize]
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
            try
            {
                var All_Organizations = from m in _context.Organizations
                                       select m;
            
                List<OrganizationModel> Organization = await All_Organizations.ToListAsync();
           
                if (Organization == null)
                {
                    return NotFound();
                }
                //convert object to json string.
                string json = JsonConvert.SerializeObject(Organization);

                string path = @"D:\ASP NET Project\Automate-Factory\wwwroot\json\jsondata.json";
               // string path = @"~/json/jsondata.json";
                //export data to json file. 
                using (TextWriter tw = new StreamWriter(path))
                {
                    tw.WriteLine(json);
                };

                return View(Organization);

            }catch(Exception ex)
                {
                    return View("Error" + ex.ToString());
                }
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

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,name,position,photo,phone,address,email,parent,work_quality,initiative,cooperative")] OrganizationModel Organization)
        {
            if (ModelState.IsValid)
            {
                _context.Add(Organization);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(Organization);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Organization = await _context.Organizations.FindAsync(id);
            if (Organization == null)
            {
                return NotFound();
            }
            return View(Organization);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,name,position,photo,phone,address,email,parent,work_quality,initiative,cooperative")] OrganizationModel Organization)
        {
            if (id != Organization.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(Organization);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrganizationExists(Organization.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(Organization);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Organization = await _context.Organizations
                .FirstOrDefaultAsync(m => m.id == id);
            if (Organization == null)
            {
                return NotFound();
            }

            return View(Organization);
        }

        private bool OrganizationExists(int id)
        {
            return _context.Organizations.Any(e => e.id == id);
        }



    }
}