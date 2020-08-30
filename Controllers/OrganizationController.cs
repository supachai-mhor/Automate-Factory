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
using Newtonsoft.Json;

namespace AutomateBussiness.Controllers
{
    [Authorize]
    public class OrganizationController : Controller
    {
        private readonly UserManager<AccountViewModel> userManager;
        private readonly SignInManager<AccountViewModel> signInManager;
        private readonly AutomateBussinessContext _context;
        private readonly IWebHostEnvironment hostingEnvironment;
        public object Claimtype { get; private set; }
        private List<OrganizationViewModel> Organization { get; set; }
        public string facID = "";
        public OrganizationController(AutomateBussinessContext context,
            UserManager<AccountViewModel> userManager,
            SignInManager<AccountViewModel> signInManager,
            IWebHostEnvironment hostingEnvironment)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.hostingEnvironment = hostingEnvironment;
            _context = context;

        }
        private  void getFacID()
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
                var All_Organizations = from m in _context.OrganizationTable where m.factoryID == facID
                                        orderby m.parent
                                       select m;
                //All_Organizations = All_Organizations.Where(x => x.factoryID == facID);

                List<OrganizationViewModel> Organization = await All_Organizations.ToListAsync();
           
                //if (Organization == null)
                //{
                //    return NotFound();
                //}
                //convert object to json string.
                //string json = JsonConvert.SerializeObject(Organization);

                //string path = @"D:\ASP NET Project\Automate-Factory\wwwroot\json\jsondata.json";
               // string path = @"~/json/jsondata.json";
                //export data to json file. 
                //using (TextWriter tw = new StreamWriter(path))
                //{
                //    tw.WriteLine(json);
                //};

                return View(Organization);

            }catch(Exception ex)
                {
                    return View("Error" + ex.ToString());
                }
            }

        [HttpPost]
        public async Task<List<OrganizationViewModel>> getOrgData()
        {
            getFacID();
            var All_Organizations = from m in _context.OrganizationTable
                                    where m.factoryID == facID
                                    orderby m.parent
                                    select m;
            //All_Organizations = All_Organizations.Where(x => x.factoryID == facID);

            List<OrganizationViewModel> Organization = await All_Organizations.ToListAsync();

            if (Organization == null)
            {
                return null;
            }

            return Organization;
        }

        public async Task<IActionResult> Create()
        {
            getFacID();
            // Use LINQ to get list of genres.
            IQueryable<string> ListLeader = from m in _context.OrganizationTable
                                            where m.factoryID == facID
                                            orderby m.name
                                            select m.name;

            var LeaderModel = new SelectList(await ListLeader.Distinct().ToListAsync());
            ViewBag.LeaderModel = LeaderModel;
            return View();
        }
        private async Task<string> getParentID(string nameParent)
        {
               var All_Organizations = from m in _context.OrganizationTable
                                    where (m.factoryID == facID && m.name == nameParent)
                                    select m;

           return  All_Organizations.First().id.ToString();
                       
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( OrganizationCreateViewModel organization)
        {
            if (ModelState.IsValid)
            {
                //add new user also
                getFacID();
                var factory = _context.FactoryTable.Where(m => m.id == facID);
                if (factory.Count() == 1)
                {

                    // Copy data from RegisterViewModel to IdentityUser
                    var user = new AccountViewModel
                    {
                        UserName = organization.email,
                        Email = organization.email,
                        PhoneNumber = organization.phone,
                        factoryID = factory.First().id
                    };

                    // Store user data in AspNetUsers database table
                    var result = await userManager.CreateAsync(user, organization.defaultPassword);

                    // If user is successfully created, sign-in the user using
                    // SignInManager and redirect to index action of HomeController
                    if (result.Succeeded)
                    {
                        if (organization.parent != null)
                        {
                            organization.parent = await getParentID(organization.parent);
                        }
                        string uniqueFileName = null;
                        // has selected an image to upload.
                        if (organization.photo != null)
                        {
                            // The image must be uploaded to the images folder in wwwroot
                            // To get the path of the wwwroot folder we are using the inject
                            // HostingEnvironment service provided by ASP.NET Core
                            string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "Img\\Emp");
                            // To make sure the file name is unique we are appending a new
                            // GUID value and and an underscore to the file name
                            uniqueFileName = Guid.NewGuid().ToString() + "_" + organization.photo.FileName;
                            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                            // Use CopyTo() method provided by IFormFile interface to
                            // copy the file to wwwroot/images folder
                            organization.photo.CopyTo(new FileStream(filePath, FileMode.Create));
                        }
                        else
                        {
                            uniqueFileName = "noimg.png";
                        }
                        OrganizationViewModel newEmp = new OrganizationViewModel
                        {
                            name = organization.name,
                            position = organization.position,
                            photo = uniqueFileName,
                            phone = organization.phone,
                            address = organization.address,
                            email = organization.email,
                            parent = organization.parent,
                            work_quality = organization.work_quality,
                            initiative = organization.initiative,
                            cooperative = organization.cooperative,
                            factoryID = facID,
                            defaultPassword = organization.defaultPassword,
                            nickName = organization.nickName,
                            accessType = organization.accessType
                        };

                        _context.OrganizationTable.Add(newEmp);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("Email", "can't add this email because already has in system ");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Not found your company id");
                }

            }
            return View(Organization);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            getFacID();
            // Use LINQ to get list of genres.
            IQueryable<string> ListLeader = from m in _context.OrganizationTable
                                            where m.factoryID == facID
                                            orderby m.name
                                            select m.name;

            var LeaderModel = new SelectList(await ListLeader.Distinct().ToListAsync());
            ViewBag.LeaderModel = LeaderModel;

            var Organization = await _context.OrganizationTable.FindAsync(id);
            if (Organization == null)
            {
                return NotFound();
            }
            return View(Organization);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,name,position,photo,phone,address,email,parent,work_quality,initiative,cooperative,defaultPassword,accessType")] OrganizationViewModel Organization)
        {
            if (id != Organization.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                getFacID();

                if (Organization.parent != null)
                {
                    Organization.parent = await getParentID(Organization.parent);
                }
                Organization.factoryID = facID;

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

            var Organization = await _context.OrganizationTable
                .FirstOrDefaultAsync(m => m.id == id);
            if (Organization == null)
            {
                return NotFound();
            }

            return View(Organization);
        }

        private bool OrganizationExists(int id)
        {
            return _context.OrganizationTable.Any(e => e.id == id);
        }
    }
}