using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using usersDemo.Data;
using usersDemo.Models;
using usersDemo.Services;

namespace usersDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class userController : ControllerBase
    {
        private readonly UserDbContext _context;
        
       
        public userController(UserDbContext context)
        {
            _context = context;
            
        }



        [HttpGet("getUser"), Authorize()]
        public async Task<IActionResult> getUser()
        {
            var users = await _context.Users.Select(u => new
            {
                Id = u.UserID,
                Username = u.Username,
                FirstName = u.FirstName,
                LastName = u.LastName
            }).ToListAsync();

            return Ok(users);
        }


        
        [HttpPut("editUser/{id}"), Authorize(Roles = "Admin")]

        public async Task<IActionResult> updateUser(int id, [FromBody] UpdateUserRequest req)
        {
 

            var getUser = await _context.Users.FindAsync(id);
            
            if(getUser == null)
            {
                return NotFound();
            }

            if(string.IsNullOrEmpty(req.FirstName) || string.IsNullOrEmpty(req.LastName))
            {
                ModelState.AddModelError("", "Firstname and Lastname are require!");
                return BadRequest();
            }

            getUser.FirstName = req.FirstName;
            getUser.LastName = req.LastName;
            getUser.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(new { Message = $"Firstname: {getUser.FirstName} ,Lastname: {getUser.LastName}, update success." });

            
        }


        [HttpPost("register")]
        public async Task<IActionResult> addUser([FromBody]  User user)
        {
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password) ||
                string.IsNullOrEmpty(user.FirstName) || string.IsNullOrEmpty(user.LastName))
            {
                ModelState.AddModelError("", "All fields are required!");
                return BadRequest(ModelState);
            }

            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
            {
                ModelState.AddModelError("Username", "Username is already taken.");
                return BadRequest(ModelState);
            }

            (string hash, string salt) = passwordHasherService.HashPassword(user.Password);
            user.Password = hash;
            user.Salt = salt;



            _context.Users.Add(user);
            await _context.SaveChangesAsync();


            return Ok(new { Message = "Registration successful" });
        }

        [HttpDelete("deleteUser/{id}"), Authorize(Roles = "Admin")]

        public async Task<IActionResult> deleteUser(int id)
        {
            var getUser = await _context.Users.FindAsync(id);
            if (getUser == null) {
                return NotFound();
            }

            _context.Users.Remove(getUser);
            await _context.SaveChangesAsync();
            return Ok(new {message = "Delete success"});


        }

        [HttpPost("login")]

        public async Task<IActionResult> login([FromBody] login req)
        {
            if(string.IsNullOrEmpty(req.Username) || string.IsNullOrEmpty(req.Password)) 
            {
                ModelState.AddModelError("Username", "Username is required!");
                return BadRequest(ModelState);
            }

            var getUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == req.Username);

            if (getUser == null)
            {
                ModelState.AddModelError("Username", "Username not found!");
                return NotFound(ModelState);
            }

            if (!passwordHasherService.VerifyPassword(req.Password, getUser.Salt, getUser.Password))
            {
                ModelState.AddModelError("Password", "Invalid password");
                return BadRequest(ModelState);
            }


            var token = GenerateJwtToken(getUser);


            return Ok(new {Message = "login success", Token = token});


        }


        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role), 
            };


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("userDemoSecretKeyFor"));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1), 
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }





    }
}
