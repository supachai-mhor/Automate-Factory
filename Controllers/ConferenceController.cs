using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutomateBussiness.Data;
using AutomateBussiness.Hubs;
using AutomateBussiness.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AutomateBussiness.Controllers
{
    [Authorize]
     public class ConferenceController : Controller
    {
        private readonly UserManager<AccountViewModel> userManager;
        private readonly SignInManager<AccountViewModel> signInManager;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly AutomateBussinessContext _context;
        private readonly IConfiguration _config;
        public ConferenceController(UserManager<AccountViewModel> userManager,
            SignInManager<AccountViewModel> signInManager, IHubContext<ChatHub> hubContext,
            AutomateBussinessContext context, IConfiguration config)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _hubContext = hubContext;
            _context = context;
            _config = config;
        }
        public IActionResult Index()
        {
            var facName = userManager.Users.Where(m => m.UserName == User.Identity.Name).First().FactoryName;

            var user = new AccountViewModel
            {
                UserName = User.Identity.Name,
                Email = User.Identity.Name,
                FactoryName = facName
            };

            var tokenString = BuildToken(user);
            ViewBag.token = tokenString;

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
                new Claim("FactoryName",user.FactoryName),
                new Claim("MachineName","Viewer")

                //new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
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