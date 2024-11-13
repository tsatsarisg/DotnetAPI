using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Helpers;

public class AuthHelper
{
    private readonly IConfiguration _config;
    public AuthHelper(IConfiguration config)
    {
        _config = config;
    }
    
    public byte[] GetPasswordHash(string password, byte[] passwordSalt)
    {
        string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value +
                                        Convert.ToBase64String(passwordSalt);

        return KeyDerivation.Pbkdf2(
            password: password,
            salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 1000000,
            numBytesRequested: 256 / 8
        );
    }

    public string CreateToken(int userId)
    {
        var claims = new Claim[]
        {
            new Claim("userId", userId.ToString())
        };

        var tokenString = _config.GetSection("AppSettings:TokenKey").Value;
        
        var tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenString ?? ""));
        
        var credentials = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha256Signature);
        var descriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = credentials,
            Expires = DateTime.Now.AddHours(1),
        };
        
        var handler = new JwtSecurityTokenHandler();
        
        var token = handler.CreateToken(descriptor);
        
        return handler.WriteToken(token);
    }
}