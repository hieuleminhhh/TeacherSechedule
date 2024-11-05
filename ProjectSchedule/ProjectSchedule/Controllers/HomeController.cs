using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectSchedule.Authenticate;
using ProjectSchedule.Dto;
using ProjectSchedule.Models;

namespace ProjectSchedule.Controllers
{
    [ApiController]
    [Route("api")]
    public class HomeController : Controller
    {
        private readonly ProjectPRN231Context _context;
        private readonly JwtTokenGenerator _jwtTokenGenerator;
        public HomeController(ProjectPRN231Context context, JwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
            _jwtTokenGenerator = jwtTokenGenerator;
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Invalid request");
            }

            var teacher = await _context.Teachers
                .Where(t => t.Email == model.Email && t.Password == model.Password)
                .FirstOrDefaultAsync();

            if (teacher == null)
            {
                return Unauthorized("Invalid email or password");
            }
            var token = _jwtTokenGenerator.GenerateJwtToken(teacher);
            return Ok(new
            {
                Message = "Login successful",
                token,
                teacher.TeacherId,
                teacher.Email,
                teacher.Role
            });
        }
        [HttpPost("Register")]
        public IActionResult Register([FromBody] Register model)
        {
            if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Invalid request");
            }

            if (string.IsNullOrEmpty(model.FirstName) || string.IsNullOrEmpty(model.LastName) || string.IsNullOrEmpty(model.PhoneNumber))
            {
                return BadRequest("All fields must be filled");
            }

            if (!long.TryParse(model.PhoneNumber, out _))
            {
                return BadRequest("Invalid phone number");
            }

            if (!IsValidEmail(model.Email))
            {
                return BadRequest("Invalid email format");
            }

            var existingUser = _context.Teachers.FirstOrDefault(e => e.Email.Equals(model.Email));

            if (existingUser != null)
            {
                return BadRequest("Email is already in use");
            }

            var newUser = new Teacher
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                Password = model.Password,
                Role = model.Role
            };

            _context.Teachers.Add(newUser);
            _context.SaveChanges();

            var token = _jwtTokenGenerator.GenerateJwtToken(newUser);
            return Ok(new { Message = "Registration successful", Token = token });
        }


        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        [HttpGet("GetUserById/{id}")]
        public IActionResult GetUserById(int id)
        {
            var a = _context.Teachers.Where(t => t.TeacherId == id).FirstOrDefault();
            if(a == null)
            {
                return NotFound("Not found user");
            }
            return Ok(a);
        }
        [HttpGet("GetUserByEmail")]
        public IActionResult GetUserByEmail()
        {
            var a = _context.Teachers.Select(a => new {a.TeacherId, a.Email}).ToList();

            if (a == null)
            {
                return NotFound("Not found user");
            }
            return Ok(a);
        }
    }
}
