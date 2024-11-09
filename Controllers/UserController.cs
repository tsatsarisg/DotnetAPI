using DotnetAPI.Data;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private DataContextDapper _dapper;
    
    public UserController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        Console.WriteLine(config.GetConnectionString("DefaultConnection"));
    }
    
    [HttpGet("TestConnection")]
    public DateTime TestConnection()
    {
        return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
    }
    
    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
        var sql = @"SELECT [UserId],
            [FirstName],
            [LastName],
            [Email],
            [Gender],
            [Active] FROM TutorialAppSchema.Users";
        
        var users = _dapper.LoadData<User>(sql);
        return users;
    }
    
    [HttpGet("GetSingleUser/{userId}")]
    public User GetSingleUser(string userId)
    {
        string sql = @"
            SELECT [UserId],
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active] 
            FROM TutorialAppSchema.Users
                WHERE UserId = " + userId.ToString(); 
        User user = _dapper.LoadDataSingle<User>(sql);
        return user;
    }


    [HttpPut("EditUser")]
    public ActionResult<User> EditUser(User user)
    {
        string sql = @"
        UPDATE TutorialAppSchema.Users
            SET [FirstName] = '" + user.FirstName + 
                     "', [LastName] = '" + user.LastName +
                     "', [Email] = '" + user.Email + 
                     "', [Gender] = '" + user.Gender + 
                     "', [Active] = '" + user.Active + 
                     "' WHERE UserId = " + user.UserId;
        if (_dapper.ExecuteSingle(sql))
        {
            return Ok();
        }
        
        throw new Exception("Failed to update user");
    }

    [HttpPost]
    public ActionResult<User> AddUser()
    {
        return Ok();
    }
}