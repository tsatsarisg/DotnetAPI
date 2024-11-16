using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.DTOS;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UserCompleteController : ControllerBase
{
    private DataContextDapper _dapper;
    
    public UserCompleteController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        Console.WriteLine(config.GetConnectionString("DefaultConnection"));
    }
    
    [HttpGet("TestConnection")]
    public DateTime TestConnection()
    {
        return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
    }
    
    [HttpGet("GetUsers/{userId}/{isActive}")]
    public IEnumerable<UserComplete> GetUsers(int userId, bool isActive)
    {
        var sql = @"EXEC TutorialAppSchema.spUsers_Get";
        var stringParameters = "";
        
        DynamicParameters sqlParameters = new DynamicParameters();
        
        if (userId != 0)
        {
            stringParameters += ", @UserId=@UserIdParameter";
            sqlParameters.Add("@UserIdParameter", userId, DbType.Int32 );
        } 
        if (isActive)
        {
            stringParameters += ", @Active=@ActiveParameter";
            sqlParameters.Add("@ActiveParameter", isActive, DbType.Boolean );
        }

        if (stringParameters.Length > 0)
        {
            sql += stringParameters.Substring(1);//, parameters.Length);
        }
        var users = _dapper.LoadData<UserComplete>(sql);
        return users;
    }
    
    [HttpPut("UpsertUser")]
    public IActionResult UpsertUser(UserComplete user)
    {
        string sql = @"EXEC TutorialAppSchema.spUser_Upsert
            @FirstName = '" + user.FirstName + 
                     "', @LastName = '" + user.LastName +
                     "', @Email = '" + user.Email + 
                     "', @Gender = '" + user.Gender + 
                     "', @Active = '" + user.Active + 
                     "', @JobTitle = '" + user.JobTitle + 
                     "', @Department = '" + user.Department + 
                     "', @Salary = '" + user.Salary + 
                     "', @UserId = " + user.UserId;

        if (_dapper.ExecuteSingle(sql))
        {
            return Ok();
        } 

        throw new Exception("Failed to Update User");
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @"TutorialAppSchema.spUser_Delete
            @UserId = " + userId.ToString();

        if (_dapper.ExecuteSingle(sql))
        {
            return Ok();
        } 

        throw new Exception("Failed to Delete User");
    }
}