using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.DTOS;
using DotnetAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace DotnetApi.Controllers;

public class AuthController(IConfiguration config) : ControllerBase
{
    private readonly DataContextDapper _dapper = new(config);
    private readonly IConfiguration _config = config;
    private readonly AuthHelper authHelper =  new AuthHelper(config);
    
    [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(UserForRegistrationDto userForRegistration)
        {
            if (userForRegistration.Password == userForRegistration.PasswordConfirm)
            {
                string sqlCheckUserExists = "SELECT Email FROM TutorialAppSchema.Auth WHERE Email = '" +
                    userForRegistration.Email + "'";

                IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);
                if (existingUsers.Count() == 0)
                {
                    UserForLoginDto userForSetPassword = new UserForLoginDto() {
                        Email = userForRegistration.Email,
                        Password = userForRegistration.Password
                    };
                    if (authHelper.SetPassword(userForSetPassword))
                    {
                        
                        string sqlAddUser = @"EXEC TutorialAppSchema.spUser_Upsert
                            @FirstName = '" + userForRegistration.FirstName + 
                            "', @LastName = '" + userForRegistration.LastName +
                            "', @Email = '" + userForRegistration.Email + 
                            "', @Gender = '" + userForRegistration.Gender + 
                            "', @Active = 1" + 
                            ", @JobTitle = '" + userForRegistration.JobTitle + 
                            "', @Department = '" + userForRegistration.Department + 
                            "', @Salary = '" + userForRegistration.Salary + "'";
                        // string sqlAddUser = @"
                        //     INSERT INTO TutorialAppSchema.Users(
                        //         [FirstName],
                        //         [LastName],
                        //         [Email],
                        //         [Gender],
                        //         [Active]
                        //     ) VALUES (" +
                        //         "'" + userForRegistration.FirstName + 
                        //         "', '" + userForRegistration.LastName +
                        //         "', '" + userForRegistration.Email + 
                        //         "', '" + userForRegistration.Gender + 
                        //         "', 1)";
                        if (_dapper.ExecuteSingle(sqlAddUser))
                        {
                            return Ok();
                        }
                        throw new Exception("Failed to add user.");
                    }
                    throw new Exception("Failed to register user.");
                }
                throw new Exception("User with this email already exists!");
            }
            throw new Exception("Passwords do not match!");
        }
        
    [HttpPut("ResetPassword")]
    public IActionResult ResetPassword(UserForLoginDto userForSetPassword)
    {
        if (authHelper.SetPassword(userForSetPassword))
        {
            return Ok();
        }
        throw new Exception("Failed to update password!");
    }
    
    [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDto userForLogin)
        {
            string sqlForHashAndSalt = @"EXEC TutorialAppSchema.spLoginConfirmation_Get 
                @Email = @EmailParam";

            DynamicParameters sqlParameters = new DynamicParameters();

            // SqlParameter emailParameter = new SqlParameter("@EmailParam", SqlDbType.VarChar);
            // emailParameter.Value = userForLogin.Email;
            // sqlParameters.Add(emailParameter);

            sqlParameters.Add("@EmailParam", userForLogin.Email, DbType.String);

            UserForLoginConfirmationDto userForConfirmation = _dapper
                .LoadDataSingleWithParameters<UserForLoginConfirmationDto>(sqlForHashAndSalt, sqlParameters);

            byte[] passwordHash = authHelper.GetPasswordHash(userForLogin.Password, userForConfirmation.PasswordSalt);

            // if (passwordHash == userForConfirmation.PasswordHash) // Won't work

            for (int index = 0; index < passwordHash.Length; index++)
            {
                if (passwordHash[index] != userForConfirmation.PasswordHash[index]){
                    return StatusCode(401, "Incorrect password!");
                }
            }

            string userIdSql = @"
                SELECT UserId FROM TutorialAppSchema.Users WHERE Email = '" +
                userForLogin.Email + "'";

            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return Ok(new Dictionary<string, string> {
                {"token", authHelper.CreateToken(userId)}
            });
        }

        [HttpGet("RefreshToken")]
        public string RefreshToken()
        {
            string userIdSql = @"
                SELECT UserId FROM TutorialAppSchema.Users WHERE UserId = '" +
                User.FindFirst("userId")?.Value + "'";
            
            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return authHelper.CreateToken(userId);
        }
    
}