using System.Data;
using AutoMapper;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.DTOS;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetApi.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly DataContextDapper _dapper;
    private readonly AuthHelper _authHelper;
    private readonly ReusableSql _reusableSql;
    private readonly IMapper _mapper;

    public AuthController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        _authHelper = new AuthHelper(config);
        _reusableSql = new ReusableSql(config);
        _mapper = new Mapper(new MapperConfiguration(cfg => 
        {
            cfg.CreateMap<UserForRegistrationDto, UserComplete>();
        }));
    }

    [AllowAnonymous]
    [HttpPost("Register")]
    public IActionResult Register(UserForRegistrationDto userForRegistration)
    {
        if (userForRegistration.Password != userForRegistration.PasswordConfirm)
            throw new Exception("Passwords do not match!");
        var sqlCheckUserExists = "SELECT Email FROM TutorialAppSchema.Auth WHERE Email = '" +
                                 userForRegistration.Email + "'";

        IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);
        
        if (existingUsers.Count() != 0) throw new Exception("User with this email already exists!");
        
        UserForLoginDto userForSetPassword = new UserForLoginDto() {
            Email = userForRegistration.Email,
            Password = userForRegistration.Password
        };
        
        if (!_authHelper.SetPassword(userForSetPassword)) throw new Exception("Failed to register user.");
        
        UserComplete userComplete = _mapper.Map<UserComplete>(userForRegistration);
        userComplete.Active = true;

        if (_reusableSql.UpsertUser(userComplete))
        {
            return Ok();
        }
        throw new Exception("Failed to add user.");
    }

    [HttpPut("ResetPassword")]
    public IActionResult ResetPassword(UserForLoginDto userForSetPassword)
    {
        if (_authHelper.SetPassword(userForSetPassword))
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

        byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForConfirmation.PasswordSalt);

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
            {"token", _authHelper.CreateToken(userId)}
        });
    }

    [HttpGet("RefreshToken")]
    public string RefreshToken()
    {
        string userIdSql = @"
            SELECT UserId FROM TutorialAppSchema.Users WHERE UserId = '" +
            User.FindFirst("userId")?.Value + "'";
        
        int userId = _dapper.LoadDataSingle<int>(userIdSql);

        return _authHelper.CreateToken(userId);
    }

}