using Microsoft.AspNetCore.Mvc;
using Shopping.Models;
using Shopping.Data;
using Shopping.Dtos;

namespace Shopping.Controllers;


[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    DataContextEF _entityFramework;

    public UserController(IConfiguration config)
    {
        _entityFramework = new(config);
    }

    [HttpGet("GetUsers")]
    public IEnumerable<User>? GetUsers(){
        
        return _entityFramework.Users;
    }

    [HttpGet("GetUser/{userId}")]
    public User? GetUsers(int userId){
        return _entityFramework.Users?
                    .Where(u => u.UserId == userId).First();
    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAdd user){
        User toDbUser = new(){
            UserId = 0,
            Email = user.Email,
            Name = user.Name,
            CreatedTime = DateTime.Now
        };
        _entityFramework.Add(toDbUser);

        if(_entityFramework.SaveChanges() > 0)
            return Ok();
    
        throw new Exception("Falied to add User");
    }

    
}
