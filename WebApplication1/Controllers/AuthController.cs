using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
namespace WebApplication1.Controllers
{
	[ApiController]
	[Route("api/[Controller]")]
	public class AuthController : ControllerBase
	{
		private readonly IConfiguration _configuration;
		public AuthController(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		[HttpPost("login")]
		public IActionResult Login([FromBody] LoginDTO dto) 
		{
			Claim[] claims = null;
			if (dto.Username == "admin" && dto.Password == "123")
			{
				claims = new[]
			   {
					new Claim(ClaimTypes.Name, dto.Username),
					new Claim(ClaimTypes.Role, "Admin")
				};
			}
			else if (dto.Username == "test" && dto.Password == "123")
			{
				claims = new[]
			   {
					new Claim(ClaimTypes.Name, dto.Username),
					new Claim(ClaimTypes.Role, "Test")
				};
			}

			if(claims == null)
			{
				return Unauthorized();
			}				

			var key = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(
					_configuration["Jwt:Key"]!
				)
			);

			var credentials = new SigningCredentials(
				key,
				SecurityAlgorithms.HmacSha256
			);

			var token = new JwtSecurityToken(
				issuer: _configuration["Jwt:Issuer"],
				audience: _configuration["Jwt:Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddHours(1),
				signingCredentials: credentials
			);

			var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

			return Ok(new LoginResponseDTO { Token = tokenString });
		}
	}
}
