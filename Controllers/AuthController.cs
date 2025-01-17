using AuthApplication.Dto;
using AuthApplication.Helpers;
using AuthApplication.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthContext authContext;
        private readonly IMapper _mapper;
        private readonly JwtService jwtService;

        public AuthController(AuthContext authContext, IMapper mapper, JwtService jwtService)
        {
            this.authContext = authContext;
            _mapper = mapper;
            this.jwtService = jwtService;
        }

        [HttpGet]

        public IActionResult Get()
        {
            var users = authContext.Users.ToList();
            var response = new { data = users };
            if (users == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(response);
        }

        [HttpPost("register")]

        public IActionResult Register(RegisterDto registerDto)
        {
            var existingUser = authContext.Users.FirstOrDefault(u => u.Email == registerDto.Email);
            if (existingUser != null)
            {
                return BadRequest("Email already exists");
            }
            //var user = new User
            //{
            //    Name = registerDto.Name,
            //    Email = registerDto.Email,
            //    Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password)
            //};
            var user = _mapper.Map<User>(registerDto);
            user.Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
            authContext.Users.Add(user);
            authContext.SaveChanges();
            return Ok(user);
        }

        [HttpPost("login")]

        public IActionResult Login(LoginDto loginDto)
        {
            var user = authContext.Users.FirstOrDefault(u => u.Email == loginDto.Email);
            if (user == null)
            {
                return BadRequest("Invalid Credentials");
            }
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            {
                return BadRequest("Invalid Credentials");
            }
            //return Ok("Login successful");

            var jwt = jwtService.Generate(user.Id);

            Response.Cookies.Append(key:"jwt", value:jwt, new CookieOptions
            {
                HttpOnly = true
            });
            return Ok(new { message = "Success" });
        }

        [HttpDelete("{id:int}")]

        public IActionResult DeleteAuth(int id)
        {
            var deleteUser = authContext.Users.Find(id);
            if (deleteUser == null)
            {
                return NotFound();
            }
            authContext.Users.Remove(deleteUser);
            authContext.SaveChanges();
            return Ok("Success");
        }
    }
}
