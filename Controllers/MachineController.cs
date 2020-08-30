using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutomateBussiness.Data;
using AutomateBussiness.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutomateBussiness.Controllers
{
    [Authorize]
    public class MachineController : Controller
    {
        private readonly UserManager<AccountViewModel> userManager;
        private readonly SignInManager<AccountViewModel> signInManager;
        private readonly AutomateBussinessContext _context;
        private readonly IWebHostEnvironment hostingEnvironment;

        public string facID = "";
        public MachineController(AutomateBussinessContext context,
            UserManager<AccountViewModel> userManager,
            SignInManager<AccountViewModel> signInManager, IWebHostEnvironment hostingEnvironment)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.hostingEnvironment = hostingEnvironment;
            _context = context;

        }
        private void getFacID()
        {
            var facName = userManager.Users.Where(m => m.UserName == User.Identity.Name).First().factoryID;
            var factory = _context.FactoryTable.Where(m => m.id == facName);
            if (factory.Count() > 0)
            {
                facID = factory.First().id;
            }
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                getFacID();
                var allMachine = from m in _context.MachineTable where m.factoryID == facID
                                 select m;

                List<MachineViewModel> machine = await allMachine.ToListAsync();

                return View(machine);

            }
            catch (Exception ex)
            {
                return View("Error" + ex.ToString());
            }
        }
        public async Task<IActionResult> Create()
        {
            getFacID();
            // Use LINQ to get list of genres.
            IQueryable<string> ListPlant = from m in _context.MachineTable
                                            where m.factoryID == facID
                                            orderby m.name
                                            select m.plant;
            IQueryable<string> ListProcess = from m in _context.MachineTable
                                           where m.factoryID == facID
                                           orderby m.name
                                           select m.process;
            IQueryable<string> ListLine = from m in _context.MachineTable
                                           where m.factoryID == facID
                                           orderby m.name
                                           select m.line;

            var ListPlantModel = new SelectList(await ListPlant.Distinct().ToListAsync());
            ViewBag.ListPlantModel = ListPlantModel;
            var ListProcessModel = new SelectList(await ListProcess.Distinct().ToListAsync());
            ViewBag.ListProcessModel = ListProcessModel;
            var ListLineModel = new SelectList(await ListLine.Distinct().ToListAsync());
            ViewBag.ListLineModel = ListLineModel;

            ViewBag.FactoryID = facID;

            return View();
        }
    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( MachineCreateViewModel machine)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                // has selected an image to upload.
                if (machine.machineImage != null)
                {
                    // The image must be uploaded to the images folder in wwwroot
                    // To get the path of the wwwroot folder we are using the inject
                    // HostingEnvironment service provided by ASP.NET Core
                    string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "Img\\MC");
                    // To make sure the file name is unique we are appending a new
                    // GUID value and and an underscore to the file name
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + machine.machineImage.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    // Use CopyTo() method provided by IFormFile interface to
                    // copy the file to wwwroot/images folder
                    machine.machineImage.CopyTo(new FileStream(filePath, FileMode.Create));
                }
                else
                {
                    uniqueFileName = "MC1.jpg";
                }

                getFacID();

                MachineViewModel newMachine = new MachineViewModel
                {
                   name = machine.name,
                   plant = machine.plant,
                   process = machine.process,
                   line = machine.line,
                   description = machine.description,
                   vendor = machine.vendor,
                   supervisor = machine.supervisor,
                   installed_date = machine.installed_date,
                   factoryID = facID,
                   machineImage = uniqueFileName,
                   machineHashID = Guid.NewGuid().ToString()
                };

                   _context.Add(newMachine);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
            }
            else
            {
                 ModelState.AddModelError(string.Empty, "Not found your supervisor email");
                 return View(machine);
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var machine = await _context.MachineTable.FindAsync(id);
            if (machine == null)
            {
                return NotFound();
            }
            return View(machine);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,name,plant,process,line,description,vendor,supervisor,installed_date,machineImage")] MachineViewModel machine)
        {
            if (id != machine.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                getFacID();
                machine.factoryID = facID;
                machine.machineHashID = Guid.NewGuid().ToString();
                var findsupervisorEmail = _context.OrganizationTable.Where(m => m.factoryID == facID && m.email == machine.supervisor);
                if (findsupervisorEmail.Count() > 0)
                {
                    try
                    {
                        _context.Update(machine);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        ModelState.AddModelError(string.Empty, "Can't edit machine");
                        return View(machine);
                    }

                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Not found your supervisor email");
                    return View(machine);
                }
                
            }
            return View(machine);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var machine = await _context.MachineTable
                .FirstOrDefaultAsync(m => m.id == id);
            if (machine == null)
            {
                return NotFound();
            }

            return View(machine);
        }
    }
}