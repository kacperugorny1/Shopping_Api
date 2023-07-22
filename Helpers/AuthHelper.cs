


using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;

namespace Shopping.Helpers{

    public class AuthHelper{
        private IConfiguration _config;
        public AuthHelper(IConfiguration IConfig){
            _config = IConfig;
        }
        public byte[] GetPasswordHash(string password, byte[] passwordSalt){
            string passwordSaltPlusString = _config.GetSection("PasswordKey").Value + 
                    Convert.ToBase64String(passwordSalt);

            return KeyDerivation.Pbkdf2(
                    password: password,
                    salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8
                );
            }

        public string CreateToken(int userId){
            string? tokenKeyString = _config.GetSection("TokenKey").Value;
            Claim[] claims = new Claim[]{
                new Claim("userId", userId.ToString())
            };
            SymmetricSecurityKey tokenKey = new(
                Encoding.UTF8.GetBytes(
                    tokenKeyString != null? tokenKeyString:""));

            SigningCredentials credentials = new(tokenKey,SecurityAlgorithms.HmacSha512Signature);
 
            SecurityTokenDescriptor descriptor = new(){
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = credentials,
                Expires = DateTime.Now.AddDays(1)
            };

            JwtSecurityTokenHandler tokenHandler = new();
            
            SecurityToken token = tokenHandler.CreateToken(descriptor);

            return tokenHandler.WriteToken(token);
        }

    }

}