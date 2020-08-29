using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutomateBussiness.Models;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using AutomateBussiness.Data;
using Microsoft.AspNetCore.SignalR;
using AutomateBussiness.Hubs;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutomateBussiness.Controllers
{
    public class WebRTCController : Controller
    {
        private readonly UserManager<AccountViewModel> userManager;
        private readonly SignInManager<AccountViewModel> signInManager;
        private readonly IHubContext<AutomateHub> _hubContext;
        private readonly AutomateBussinessContext _context;
        private readonly IConfiguration _config;
        public WebRTCController(UserManager<AccountViewModel> userManager,
    SignInManager<AccountViewModel> signInManager, IHubContext<AutomateHub> hubContext,
    AutomateBussinessContext context, IConfiguration config)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _hubContext = hubContext;
            _context = context;
            _config = config;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {

            var facID =  userManager.Users.Where(m => m.UserName == User.Identity.Name).First().factoryID;
            var user = new AccountViewModel
            {
                UserName = User.Identity.Name,
                Email = User.Identity.Name,
                factoryID = facID
            };

            var tokenString = BuildToken(user);
            ViewBag.token = tokenString;

            return View();
        }
        [Authorize]
        public IActionResult MachineRTC()
        {
            return View();
        }
        private string BuildToken(AccountViewModel user)
        {

            var key = new SymmetricSecurityKey(Encoding.UTF32.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub,user.Email),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim(ClaimTypes.Role,"User"),
                new Claim("FactoryID",user.factoryID),
                new Claim("MachineID","Viewer"),
                new Claim("MachineName","Viewer")

            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Issuer"],
                claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds);

            var encodeToken = new JwtSecurityTokenHandler().WriteToken(token);

            return encodeToken;

        }
    }
}