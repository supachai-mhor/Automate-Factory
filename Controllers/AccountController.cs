using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using AutomateBussiness.Hubs;
using AutomateBussiness.Models;
using AutomateBussiness.Data;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AutomateBussiness.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AccountViewModel> userManager;
        private readonly SignInManager<AccountViewModel> signInManager;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly AutomateBussinessContext _context;
        private readonly IConfiguration _config;
        public string facID = "";
        public AccountController(UserManager<AccountViewModel> userManager,
            SignInManager<AccountViewModel> signInManager, IHubContext<ChatHub> hubContext, 
            AutomateBussinessContext context, IConfiguration config)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _hubContext = hubContext;
            _context = context;
            _config = config;
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
                var factory =   _context.FactoryTable.Where(m => m.factoryName == model.FactoryName);
                if (factory.Count()==0)
                {
                    var newFactory = new FactoryViewModel
                    {
                        id= Guid.NewGuid().ToString() + Guid.NewGuid().ToString(),
                        foundDate = DateTime.Now,
                        factoryName = model.FactoryName,
                        
                    };
                    _context.Add(newFactory);
                    await _context.SaveChangesAsync();
                        
                    // Copy data from RegisterViewModel to IdentityUser
                    var user = new AccountViewModel
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        factoryID = newFactory.id
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
                    foreach (var error in result.Errors)
                    {

                        // If there are any errors, add them to the ModelState object
                        // which will be displayed by the validation summary tag helper
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                }
                else
                {

                    // If there are any errors, add them to the ModelState object
                    // which will be displayed by the validation summary tag helper
                    ModelState.AddModelError(string.Empty, "This business name already used.");
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
                    //get user by email
                    //var user = userManager.Users.SingleOrDefault(x => x.Email == model.Email);
                    //get user from user manager
                    //var facName = userManager.Users.Where(m => m.UserName == User.Identity.Name).First().FactoryName;
                    //var userFromManager = await userManager.GetUserAsync(user);
                    //var externalAccessToken = await userManager.GetAuthenticationTokenAsync(user, "Microsoft", "access_token");

                    //var userId = userManager.Users.Where(m => m.UserName == User.Identity.Name).First().Id;
                    //var ident = await userManager.GetAuthenticationTokenAsync(User.Identity, "Test", "access_token");
                    var tokenString = BuildToken(model);
                    if (tokenString != null)
                    {
                        Console.WriteLine(tokenString);
                        await _hubContext.Clients.Group("SignalR Users").SendAsync("ReceiveMessage", model.Email, $"-- Login success on : {DateTime.Now}");
                        return RedirectToAction("Dashboard", "home");
                    }          
                }
                await _hubContext.Clients.Group("SignalR Users").SendAsync("ReceiveMessage", model.Email, $"-- Login fail on : {DateTime.Now}");
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }

            return View(model);
        }

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[HttpGet]
        //[Route("admin/gettoken")]
        //public IEnumerable<string> GetData()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //[HttpGet]
        //[Route("admin/token")]
        //public string GetToken(string email)
        //{
        //    var claims = new List<Claim>
        //    {
        //        new Claim(JwtRegisteredClaimNames.Sub, email),
        //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        //    };

        //    var key = new SymmetricSecurityKey(Encoding.UTF32.GetBytes(_config["Jwt:Key"]));
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //    var token = new JwtSecurityToken(
        //        issuer: _config["Jwt:Issuer"],
        //        audience: _config["Jwt:Issuer"],
        //        claims: claims,
        //        expires: DateTime.UtcNow.AddDays(60),
        //        signingCredentials: creds

        //    );

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}
  
        private string BuildToken(LoginViewModel user)
        {
            var facName = userManager.Users.Where(m => m.UserName == user.Email).First().factoryID;
            var factory = _context.FactoryTable.Where(m => m.id == facName);
            if (factory.Count() > 0)
            {
                facID = factory.First().id;
                var key = new SymmetricSecurityKey(Encoding.UTF32.GetBytes(_config["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub,user.Email),
                    new Claim(JwtRegisteredClaimNames.Email,user.Email),
                    new Claim(ClaimTypes.Role,"User"),
                    new Claim("FactoryID",facID),
                    new Claim("MachineID","Viewer")

                    //new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    //new Claim(JwtRegisteredClaimNames.Email, user.Email)
                    //new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
                };

                var token = new JwtSecurityToken(
                    issuer :"https://localhost:44377/",
                    audience : "https://localhost:44377/",
                    claims,
                    notBefore : DateTime.UtcNow,
                    expires : DateTime.UtcNow.AddDays(60),
                    signingCredentials : creds);

                var encodeToken = new JwtSecurityTokenHandler().WriteToken(token);

                return encodeToken;
            }
            else
            {
                return null;
            }
            
                        
            //var key = new SymmetricSecurityKey(Encoding.UTF32.GetBytes(_config["Jwt:Key"]));
            //var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //var claims = new List<Claim>
            //{
            //new Claim(ClaimTypes.NameIdentifier, user.Email),
            //new Claim(ClaimTypes.Email, user.Email)
            //};

            //var token = new JwtSecurityToken(_config["Jwt:Issuer"],
            //  _config["Jwt:Issuer"],
            //  claims,
            //  notBefore: DateTime.UtcNow,
            //  expires: DateTime.UtcNow.AddDays(60),
            //  signingCredentials: creds);

            //return new JwtSecurityTokenHandler().WriteToken(token);

        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            return RedirectToAction("index", "home");
        }
    }
}