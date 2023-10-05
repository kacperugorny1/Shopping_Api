using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Shopping.Data;
using Shopping.Models;
using Shopping.Dtos;
using Shopping.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace Shopping.Controllers{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    
    public class AuthController : ControllerBase{
        private readonly DataContextEF _entityFramework;
        private readonly AuthHelper _authHelper;
        public AuthController(IConfiguration config){
            _entityFramework = new(config);
            _authHelper = new(config);
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(RegistrationForm Form){
            if(!(Form.Password == Form.PasswordConfirm && Form.Password.Length >= 8))
                throw new Exception("Passwords do not match or < 8 chars");   
            
            if(Form.Email == "")
                throw new Exception("Specify an Email");

            //Check user if exists
            IEnumerable<Auth> existingUsers = _entityFramework.Auth.Where(u => u.Email == Form.Email);
            if(existingUsers.Count() != 0)
                throw new Exception("User with this email already exists");
            
            byte[] passwordSalt = new byte[128 / 8];
            using(RandomNumberGenerator rng = RandomNumberGenerator.Create()){
                rng.GetNonZeroBytes(passwordSalt);    
            }

            //Hashing password
            byte[] passwordHash = _authHelper.GetPasswordHash(Form.Password, passwordSalt);

            Auth newUserAuth = new(){
                Email = Form.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };
            User newUser = new(){
                Email = Form.Email,
                UserId = 0,
                Name = Form.Name,
                CreatedTime = DateTime.Now
            };
            _entityFramework.Add(newUserAuth);
            if(_entityFramework.SaveChanges() > 0)
            {
                _entityFramework.Add(newUser);
                if(_entityFramework.SaveChanges() > 0)
                    return Ok();
                throw new Exception("Falied to Add user");
            }
            throw new Exception("Falied to register user");
        }
        
        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(LoginForm Form){
            Auth? DbInfo = _entityFramework.Auth?.SingleOrDefault(u => u.Email == Form.Email);
            if(DbInfo == null)
                return StatusCode(401, "Wrong credentials");

            byte[] passwordHash = _authHelper.GetPasswordHash(Form.Password, DbInfo.PasswordSalt);


            //Dont work
            //if(passwordHash == DbInfo.PasswordHash)
            
            
            // both works but convert looks cleaner but probably slower
            // for(int i = 0; i < passwordHash.Length; ++i){
            //     if(passwordHash[i] != DbInfo.PasswordHash[i])
            //         return StatusCode(401 ,"Wrong Password");
            // }

            int userId = _entityFramework.Users.Where(u => u.Email == Form.Email).First().UserId;

            if(Convert.ToBase64String(passwordHash) == Convert.ToBase64String(DbInfo.PasswordHash)){
                //new code - cookie creation next 10 lines
                dynamic token = _authHelper.CreateToken(userId);
                HttpContext.Response.Cookies.Append("token", token,
                new CookieOptions{
                   Expires = DateTime.Now.AddDays(7),
                   HttpOnly = true,
                   Secure = true,
                   IsEssential = true,
                   SameSite = SameSiteMode.None
                });
                // this can be just Ok() but for now i left this
                return Ok(new Dictionary<string, string>{
                    {"token", token}
                });
            }
            
            return StatusCode(401 ,"Wrong Credentials");

            
        }

        [HttpGet("RefreshToken")]
        public string RefreshToken(){
            
            int userId = _entityFramework.Users.Where(u => u.UserId == Int32.Parse(User.FindFirst("userId").Value)).First().UserId;

            return _authHelper.CreateToken(userId);
        }

        [HttpGet("Logout")]
        public IActionResult Logout(){
            HttpContext.Response.Cookies.Append("token", "",
                new CookieOptions{
                   Expires = DateTime.Now.AddDays(7),
                   HttpOnly = true,
                   Secure = true,
                   IsEssential = true,
                   SameSite = SameSiteMode.None
                });

            return Ok();
        }

        
        [HttpGet("IsLogged")]
        public IActionResult IsLogged(){
            return Ok(0);
        } 

    }
}



// just generating jwt no cookie
// [AllowAnonymous]
//         [HttpPost("Login")]
//         public IActionResult Login(LoginForm Form){
//             Auth? DbInfo = _entityFramework.Auth?.SingleOrDefault(u => u.Email == Form.Email);
//             if(DbInfo == null)
//                 return StatusCode(401, "Wrong credentials");

//             byte[] passwordHash = _authHelper.GetPasswordHash(Form.Password, DbInfo.PasswordSalt);


//             //Dont work
//             //if(passwordHash == DbInfo.PasswordHash)
            
            
//             // both works but convert looks cleaner but probably slower
//             // for(int i = 0; i < passwordHash.Length; ++i){
//             //     if(passwordHash[i] != DbInfo.PasswordHash[i])
//             //         return StatusCode(401 ,"Wrong Password");
//             // }

//             int userId = _entityFramework.Users.Where(u => u.Email == Form.Email).First().UserId;

//             if(Convert.ToBase64String(passwordHash) == Convert.ToBase64String(DbInfo.PasswordHash))
//                 return Ok(new Dictionary<string, string>{
//                     {"token", _authHelper.CreateToken(userId)}
//                 });
            
//             return StatusCode(401 ,"Wrong Credentials");

            
//         }

//         [HttpGet("RefreshToken")]
//         public string RefreshToken(){
            
//             int userId = _entityFramework.Users.Where(u => u.UserId == Int32.Parse(User.FindFirst("userId").Value)).First().UserId;

//             return _authHelper.CreateToken(userId);
//         }

       

//     }