using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LetgoEcommerce.Data;
using LetgoEcommerce.Dtos;
using LetgoEcommerce.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using LetgoEcommerce.Dtos;

namespace LetgoEcommerce.Controllers
{
    [Produces("application/json")]
    [Route("api/Auth")]
    public class AuthController : Controller
    {
        private IAuthRepository _authRepository;
        private IConfiguration _configuration;

        public AuthController(IAuthRepository authRepository, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {




            if (await _authRepository.UserExists(user.email))
            {
                return Ok(new RegisterResult()
                {
                    status = false,
                    message = "Böyle bir kullanıcı zaten kayıtlı",
                    user = null

                });
            }

            if (!ModelState.IsValid)
            {
                return Ok(new RegisterResult()
                {
                    status = false,
                    message = "Geçersiz format",
                    user = null

                });
            }


            var createdUser = await _authRepository.Register(user);


            return Ok(new RegisterResult()
            {
                status = true,
                message = "Kullanıcı başarılı bir şekilde oluşturuldu",
                user = createdUser

            });
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] UserLoginDto userLoginDto)
        {
            var loginResult = await _authRepository.Login(userLoginDto.email, userLoginDto.password);

            if (loginResult.user == null)
            {
                return Ok(loginResult);
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Token").Value);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, loginResult.user.id.ToString()),
                    new Claim(ClaimTypes.Name, loginResult.user.name)
                }),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key)
                    , SecurityAlgorithms.HmacSha512Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            loginResult.message = tokenString;

            return Ok(loginResult);
        }


    }
}