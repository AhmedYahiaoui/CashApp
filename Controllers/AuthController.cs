using CashApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;

namespace CashApp.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]/[action]")]
    public class AuthController : Controller
    {
        private readonly CashAppContext _database;
        private readonly IConfiguration _config;
        private readonly string FromAddress = "brainyconceptdev@gmail.com";
        private readonly string SiteName = "http://localhost/";
        private readonly IHttpContextAccessor _httpContextAccessor;


        public AuthController(CashAppContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _database = context;
            _config = configuration;
            _httpContextAccessor = httpContextAccessor;

        }


        private string CreateToken(int id)
        {
            DateTime issuedAt = DateTime.UtcNow;
            DateTime expires = DateTime.UtcNow.AddHours(12);
            var tokenHandler = new JwtSecurityTokenHandler();

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                new[]
                {
                new Claim(ClaimTypes.Name, id.ToString())
            });
            var securityKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(_config["Jwt:Key"]));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            //create the jwt
            var token =
                    tokenHandler.CreateJwtSecurityToken(
                        issuer: _config["Jwt:Issuer"],
                        audience: _config["Jwt:Issuer"],
                        subject: claimsIdentity,
                        notBefore: issuedAt,
                        expires: expires,
                        signingCredentials: signingCredentials
                        );
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login([FromBody] User user)
        {
            Console.WriteLine("Login");
            Debug.WriteLine("Login");

            try
            {
                User result = _database.Users.Where(x => x.Email == user.Email).FirstOrDefault();
                if (result == null)
                {
                    return NotFound("User Not Found");
                }
                if (BC.Verify(user.Password, result.Password) == false)
                {
                    return NotFound("Wrong password");
                }
                else
                {
                    return Ok(new { User = result, Token = CreateToken(result.Id) });
                }
            }
            catch
            {
                return BadRequest("Failed");
            }
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAccount([FromBody] User user)
        {
            Console.WriteLine("CreateAccount");
            Debug.WriteLine("CreateAccount");
            try
            {
                if (String.IsNullOrEmpty(user.Email) || String.IsNullOrEmpty(user.Username))
                    return NotFound(" Username And Mail can't be empty ");
                
                if (String.IsNullOrEmpty(user.Password))
                    return NotFound(" Password can't be empty ");

                if (user.Password.Length < 4)
                    return NotFound(" Password must be more then 4 caratcres ");

                User searchMail = _database.Users.Where(x => x.Email == user.Email).FirstOrDefault();
                User searchUsername = _database.Users.Where(x => x.Username == user.Username).FirstOrDefault();


                if (searchMail != null)
                    return NotFound("Mail already used");
                
                if (searchUsername != null)
                    return NotFound("Username already used");
                

                try
                {
                    user.CreatedAt = DateTime.Now;
                    user.Password = BC.HashPassword(user.Password, BC.GenerateSalt(12));

                    _database.Users.Add(user);
                    _database.SaveChanges();

                    string generatedGuid = Guid.NewGuid().ToString("N");
                    string toAddress = user.Email;

                    const string subject = "Confirmation Mail";
                    string body = @"Hi " + user.Username + " \n Your account creation is succesful to activate your account please open this link " + SiteName + "/Confirmation/ActivateAccount?confirmation_token=" + generatedGuid;

                    var smtpClient = new SmtpClient("smtp.gmail.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential(FromAddress, "Qsdfgh456"),
                        EnableSsl = true,
                    };

                    smtpClient.Send(FromAddress, toAddress, subject, body);
                    return Ok("Succses");
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Create and send mail" + BadRequest());
                    return BadRequest(e);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(" error =======>  " + BadRequest());
                return BadRequest(e);
            }
        }


    }
}
