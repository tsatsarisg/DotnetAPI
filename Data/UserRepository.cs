using DotnetAPI.Models;

namespace DotnetAPI.Data;

public class UserRepository: IUserRepository
{
    private DataContextEF _entityFramework;
    
    public UserRepository(IConfiguration config)
    {
        _entityFramework = new DataContextEF(config);
    }
    
    public bool SaveChanges()
    {
        return _entityFramework.SaveChanges() > 0;
    }

    public void AddEntity<T>(T enityToAdd)
    {
        if (enityToAdd != null)
        { 
            _entityFramework.Add(enityToAdd);
        }
    }
    
    public void RemoveEntity<T>(T enityToAdd)
    {
        if (enityToAdd != null)
        { 
            _entityFramework.Remove(enityToAdd);
        }
    }
    
    public IEnumerable<User> GetUsers() => _entityFramework.Users.ToList();

    public User GetSingleUser(int userId)
    {
        return _entityFramework.Users
            .FirstOrDefault(u => u.UserId == userId);
    }
}