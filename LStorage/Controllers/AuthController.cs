using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LStorage.Auth;
using LStorage.Dao;
using LStorage.Extension;
using LStorage.Models.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;
using LStorage.Models.Dto;
using Microsoft.AspNetCore.Authorization;

namespace LStorage.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class AuthController : ControllerBase
{
    private IUserRepository _userRepository;

    public AuthController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateUser([FromBody]SingUpData signUpData, CancellationToken token)
    {
        var user = await _userRepository.GetUserByEmail(signUpData.Email,token);
        if (user != null)
        {
            return BadRequest(new { errorText = "User with this email already exists" });
        }

        var newUser = new User
        {
            Email = signUpData.Email,
            Name = signUpData.Name,
            Password = signUpData.Password.Hash(),
            Role = signUpData.Role,
            CreatedAt = DateTime.Now
        };
        await _userRepository.SaveUser(newUser, token);
        return Ok();
    }
    
    
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdatePassword([FromBody]UpdatePass updatePasswordDto, CancellationToken token)
    {
        var user = await _userRepository.GetUserByEmail(updatePasswordDto.Email,token);
        if (user == null)
        {
            return BadRequest(new { errorText = "User with this email not found" });
        }

        user.Password = updatePasswordDto.Password.Hash();
        await _userRepository.UpdateUser(user, token);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> SignIn(SingInData singUpData, CancellationToken token)
    {
        var identity = await GetIdentity(singUpData.Email, singUpData.Password, token);
        if (identity == null)
        {
            return BadRequest(new { errorText = "Invalid username or password." });
        }

        var now = DateTime.UtcNow;
        // создаем JWT-токен
        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            notBefore: now,
            claims: identity.Claims,
            expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256));
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        var response = new ResultToken()
        {
            Token = encodedJwt,
            Name = identity.Name
        };
        var serialize = JsonSerializer.Serialize(response);
        return Ok(serialize);
    }

    [HttpGet]
    [Authorize]
    public IActionResult Refresh()
    {
        var role = User.Claims.FirstOrDefault(c => c.Type == ClaimsIdentity.DefaultRoleClaimType)?.Value;
        var identityName = User.Identity.Name;
        var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var now = DateTime.UtcNow;
        // создаем JWT-токен
        var claims = new List<Claim>
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, identityName),
            new Claim( ClaimTypes.Email, email),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, role)
        };
        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            notBefore: now,
            claims: claims,
            expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256));
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        var response = new ResultToken()
        {
            Token = encodedJwt,
            Name = identityName
        };
        var serialize = JsonSerializer.Serialize(response);
        return Ok(serialize);
    }

    private async Task<ClaimsIdentity> GetIdentity(string Email, string password, CancellationToken cancellationToken)
    {
        var person = await _userRepository.GetUserByEmail(Email, cancellationToken);
        if (person != null && password.Verify(person.Password))
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, person.Name),
                new Claim(ClaimTypes.Email, person.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, person.Role)
            };
            ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }

        // если пользователя не найдено
        return null;
    }
    
    [HttpPost]
    [Authorize(Roles = "admin")]
    private async Task DeleteUser([FromBody]string email, CancellationToken token)
    {
        var user = await _userRepository.GetUserByEmail(email, token);
        if (user != null)
        {
           await _userRepository.DeleteUser(user);
        }
    }
    
    [HttpGet]
    [Authorize(Roles = "admin")]
    public Task<IEnumerable<UserDto>> GetUsers(CancellationToken token)
    {
        return _userRepository.GetUsers(token);
    }
}