using ApiAuth.Models;
using ApiAuth.Repositories;
using ApiAuth.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ApiAuth.Controllers
{
    [ApiController]
    [Route("v1")]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>> AuthenticateAsync(
            [FromBody] User model
        )
        {
            var user = UserRepository.Get(
                model.Username,
                model.Password
            );

            if (user is null)
            {
                return NotFound(new { message = "Incorrect user or password" });
            }

            var token = TokenService.GenerateToken(user);

            user.Password = string.Empty;

            return Ok(new { user, token });
        }

        [HttpPost]
        [Route("loginWithRefreshToken")]
        public async Task<ActionResult<dynamic>> AuthenticateWithRefreshAsync(
            [FromBody] User model
        )
        {
            var user = UserRepository.Get(
                model.Username,
                model.Password
            );

            if (user is null)
            {
                return NotFound(new { message = "Incorrect user or password" });
            }

            var token = TokenService.GenerateToken(user);
            var refreshToken = TokenService.GenerateRefreshToken();
            TokenService.AddRefreshToken(user.Username, refreshToken);

            user.Password = string.Empty;

            var result = new
            {
                user,
                refreshToken,
                token
            };

            return Ok(result);
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<ActionResult<dynamic>> Refresh(
            string token,
            string refreshToken
        )
        {
            var principal = TokenService.GetPrincipalFromExpiredToken(token);
            var username = principal.Identity.Name;
            var savedRefreshToken = TokenService.GetRefreshToken(username);
            if (savedRefreshToken != refreshToken)
            {
                throw new SecurityTokenException("Invalid refresh token");
            }
            var newJwtToken = TokenService.GenerateToken(principal.Claims);
            var newRefreshToken = TokenService.GenerateRefreshToken();
            TokenService.DeleteRefreshToken(
                username,
                refreshToken
            );
            TokenService.AddRefreshToken(
                username,
                newRefreshToken
            );

            return new ObjectResult(new 
            {
                newJwtToken,
                newRefreshToken
            });
        }
    }
}
