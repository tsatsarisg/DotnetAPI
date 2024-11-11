using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.DTOS;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UserEFController : ControllerBase
{
    private readonly IUserRepository _userRepository;   
    private readonly IMapper _mapper;
    
    public UserEFController(IConfiguration config, IUserRepository userRepository)
    {
        _userRepository = userRepository;
        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserDto, User>();
        }));
    }
    
    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
        return _userRepository.GetUsers();
    }
    
    [HttpGet("GetSingleUser/{userId}")]
    public User GetSingleUser(int userId)
    {
        User user = _userRepository.GetSingleUser(userId);
        return user;
    }


    [HttpPut("EditUser")]
    public ActionResult<User> EditUser(User user)
    {
        User? userDb =  _userRepository.GetSingleUser(user.UserId);
            
        if (userDb != null)
        {
            userDb.Active = user.Active;
            userDb.FirstName = user.FirstName;
            userDb.LastName = user.LastName;
            userDb.Email = user.Email;
            userDb.Gender = user.Gender;
            if (_userRepository.SaveChanges())
            {
                return Ok();
            } 

            throw new Exception("Failed to Update User");
        }
        
        throw new Exception("Failed to Get User");
    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(UserDto user)
    {
        User userDb = _mapper.Map<User>(user);
        
        _userRepository.AddEntity(userDb);
        if (_userRepository.SaveChanges())
        {
            return Ok();
        } 

        throw new Exception("Failed to Add User");
    }
    
    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        User? userDb =  _userRepository.GetSingleUser(userId);
            
        if (userDb != null)
        {
            _userRepository.RemoveEntity(userDb);
            _userRepository.RemoveEntity(userDb);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            } 

            throw new Exception("Failed to Delete User");
        }
        
        throw new Exception("Failed to Get User");
    }
}

