using aChatBotApi.Jwt;
using AutoMapper;
using FinancialBot.BL.DTOs;
using FinancialBot.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace aChatBotApi.Controllers
{
    
    [Route("api/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<Users> _userManager;
        private readonly IMapper _mapper;
        private readonly JwtClaim _jwt;

        public AccountController(
            UserManager<Users> userManager,
            IMapper mapper,
            JwtClaim jwt)
        {
            _userManager = userManager;
            _mapper = mapper;
            _jwt = jwt;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationDto dataUser)
        {
            if (dataUser == null || !ModelState.IsValid)
                return BadRequest();

            var user = _mapper.Map<Users>(dataUser);
            var result = await _userManager.CreateAsync(user, dataUser.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new RegisterResponseDto { Errors = errors });
            }

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            var user = await _userManager.FindByNameAsync(login.UserName);

            if (user == null || !await _userManager.CheckPasswordAsync(user, login.Password))
                return Unauthorized(new LoginReponseDto { ErrorMessage = "Invalid Authentication" });

            var signingCredentials = _jwt.GetSigningCredentials();
            var claims = _jwt.GetClaims(user);
            var tokenOptions = _jwt.GenerateTokenOptions(signingCredentials, claims);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return Ok(new LoginReponseDto { IsSuccess = true, Token = token });
        }
    }
}