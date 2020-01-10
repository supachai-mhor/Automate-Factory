using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MvcMovie.Data;
using MvcMovie.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace MvcMovie.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        private readonly IConfiguration _config;
        private readonly MvcMovieContext _dbContext;
        public TokenController(IConfiguration config, MvcMovieContext dbContext)
        {
            _config = config;
            _dbContext = dbContext;
        }

        [HttpPost("/api/adminJob")]
        public async Task<IActionResult> CreateUser(UserAccount model)
        {
            model.HashedPassword = EncryptPassword(model.HashedPassword);
            _dbContext.UserAccounts.Add(model);
            await _dbContext.SaveChangesAsync();
            return Ok(model);
        }

        [HttpPost("/api/login")]
        public IActionResult LoginWithToken([FromBody]LoginCredential login)
        {
            IActionResult response = Unauthorized();

            if (login != null)
            {
                var user = Authenticate(login);

                if (user != null)
                {
                    var tokenString = BuildToken(user);
                    response = Ok(new { Token = tokenString });
                }

            }
            return response;
        }

        private string BuildToken(UserAccount user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF32.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.UserName),
        new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
        new Claim(ClaimTypes.Version, "1.0"),
        new Claim(ClaimTypes.Role, "USER")
    };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims,
              notBefore: DateTime.UtcNow,
              expires: DateTime.UtcNow.AddDays(60),
              signingCredentials: creds);

           return new JwtSecurityTokenHandler().WriteToken(token);
          
        }

        private UserAccount Authenticate(LoginCredential login)
        {
            var hashPassword = EncryptPassword(login.ClientSecret);
            var client = _dbContext.UserAccounts.FirstOrDefault
                (c => c.UserName == login.ClientId && c.HashedPassword == hashPassword);
            return client;
        }
        private string EncryptPassword(string clearPassword)
        {
            byte[] clearPasswordByte = Encoding.UTF8.GetBytes(clearPassword);
            SHA256 mySHA256 = SHA256.Create();
            byte[] hashValue = mySHA256.ComputeHash(clearPasswordByte);

            string encryptedPassword = Convert.ToBase64String(hashValue);
            return encryptedPassword;
        }
        public class LoginCredential
        {
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
        }

    }
}
