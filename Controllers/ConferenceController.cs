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
using AutomateBussiness.Models.ConferenceModels;
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
        public int facID = 0;
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
        public async Task<IActionResult> Index()
        {
            var facID= userManager.Users.Where(m => m.UserName == User.Identity.Name).First().factoryID;

            var user = new AccountViewModel
            {
                UserName = User.Identity.Name,
                Email = User.Identity.Name,
                factoryID = facID
            };

            var tokenString =  await BuildToken(user);
            ViewBag.token = tokenString;

            //IQueryable<string> genreQuery = from m in _context.Movie
            //                                orderby m.Genre
            //                                select m.Genre;

            //var movies = from m in _context.Movie
            //             select m;

            //if (!string.IsNullOrEmpty(searchString))
            //{
            //    movies = movies.Where(s => s.Title.Contains(searchString));
            //}

            //if (!string.IsNullOrEmpty(movieGenre))
            //{
            //    movies = movies.Where(x => x.Genre == movieGenre);
            //}

            //var movieGenreVM = new MovieGenreViewModel
            //{
            //    Genres = new SelectList(await genreQuery.Distinct().ToListAsync()),
            //    Movies = await movies.ToListAsync()
            //};

            // find current user
            OrganizationViewModel _user = _context.OrganizationTable.Where(r => r.email == user.Email).First();

            // find chat Relationships
            IEnumerable<Relationship> _relationships = _context.RelationshipsTable.Where(r => r.requestId == user.Id || r.responedId == user.Id);

            //get all my groups
            var genreGroupId = from r in _relationships where r.relationType == RelationType.groups
                                            select r.responedId;

            // find my contacts groups
            IEnumerable<ChatGroups> _chatGroups=null;
            if (genreGroupId != null)
            {
                _chatGroups = _context.ChatGroupsTable.Where(g => genreGroupId.Contains(g.groupID)).ToList();
            }

            //get all my machines
            var genreMachineId = from r in _relationships
                               where r.relationType == RelationType.machines
                               select r.responedId;

            // find all my contacts machines
            IEnumerable<MachineViewModel> _machine = null;
            if (genreMachineId != null)
            {
                _machine = _context.MachineTable.Where(g => genreMachineId.Contains(g.machineHashID)).ToList();
            }


            // find chat History
            IEnumerable<ChatHistorys> _charHistory = _context.ChatHistorysTable.Where(r => r.senderId == user.Id || r.receiverId == user.Id);

            _relationships = _relationships.Where(r => r.relationType != RelationType.machines && r.relationType != RelationType.groups);

            var model = new ConferenceViewModel
            {
                user = _user,
                machines = _machine,
                chatGroups = _chatGroups,
                chatHistorys = _charHistory,
                relationships = _relationships
            };

            return View(model);
        }

        private async Task<string> BuildToken(AccountViewModel user)
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

            var encodeToken =  new JwtSecurityTokenHandler().WriteToken(token);

            return encodeToken;

        }
    }
}