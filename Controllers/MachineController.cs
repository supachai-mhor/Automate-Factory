using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutomateBussiness.Data;
using AutomateBussiness.Models;
using Microsoft.AspNetCore.Authorization;
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

        public string facID = "";
        public MachineController(AutomateBussinessContext context,
            UserManager<AccountViewModel> userManager,
            SignInManager<AccountViewModel> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
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


            return View();
        }
    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,name,plant,process,line,description,vendor,supervisor,installed_date,machineImage")] MachineViewModel machine)
        {
            if (ModelState.IsValid)
            {
                getFacID();
                machine.factoryID = facID;
                machine.machineHashID = Guid.NewGuid().ToString() + Guid.NewGuid().ToString();
                var findsupervisorEmail =  _context.OrganizationTable.Where(m => m.factoryID == facID && m.email==machine.supervisor);
                if (findsupervisorEmail.Count() > 0)
                {
                    _context.Add(machine);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Not found your supervisor email");
                    return View(machine);
                }

            }
            return View(machine);
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
                machine.machineHashID = Guid.NewGuid().ToString() + DateTime.Now.ToString();
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