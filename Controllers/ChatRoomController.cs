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

namespace AutomateBussiness.Controllers
{

    public class ChatRoomController : Controller
    {
        private readonly UserManager<AccountViewModel> userManager;
        private readonly SignInManager<AccountViewModel> signInManager;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly AutomateBussinessContext _context;
        private readonly IConfiguration _config;
        public ChatRoomController(UserManager<AccountViewModel> userManager,
            SignInManager<AccountViewModel> signInManager, IHubContext<ChatHub> hubContext,
            AutomateBussinessContext context, IConfiguration config)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _hubContext = hubContext;
            _context = context;
            _config = config;
        }

        [Authorize]
        [Route("chat")]
        public IActionResult Index()
        {
            Random random = new Random();
            List<DataPoint> dataPoints1 = new List<DataPoint>();
            List<DataPoint> dataPoints2 = new List<DataPoint>();

            int updateInterval = 500;

            // initial value
            double yValue1 = 0;
            double yValue2 = 0;
            double time;

            DateTime dateNow = DateTime.Now;
            DateTime date = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, 9, 30, 00);
            time = ((DateTimeOffset)date).ToUnixTimeSeconds() * 1000;
            //addData(1);

            ViewBag.DataPoints1 = JsonConvert.SerializeObject(dataPoints1);
            ViewBag.DataPoints2 = JsonConvert.SerializeObject(dataPoints2);
            ViewBag.YValue1 = yValue1;
            ViewBag.YValue2 = yValue2;
            ViewBag.Time = time;
            ViewBag.UpdateInterval = updateInterval;

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

            void addData(int count)
            {
                double deltaY1, deltaY2;
                for (int i = 0; i < count; i++)
                {
                    time += updateInterval;
                    deltaY1 = .5 + random.NextDouble() * (-.5 - .5);
                    deltaY2 = .5 + random.NextDouble() * (-.5 - .5);

                    // adding random value and rounding it to two digits.
                    yValue1 = Math.Round((yValue1 + deltaY1) * 100) / 100;
                    yValue2 = Math.Round((yValue2 + deltaY2) * 100) / 100;

                    // pushing the new values
                    dataPoints1.Add(new DataPoint(time, yValue1));
                    dataPoints2.Add(new DataPoint(time, yValue2));
                }
            }
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
                new Claim(ClaimTypes.Role,"Machine"),
                new Claim("FactoryName",user.FactoryName)

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

        [Authorize]
        public IActionResult Streamming()
        {
            return View();
        }
    }
}