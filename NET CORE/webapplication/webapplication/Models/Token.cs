using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace webapplication.Models
{
    public class Token
    {
        public Token(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public Boolean ValidateJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            string PasswordKeyJWT = Configuration.GetSection("MySettings").GetSection("PasswordKeyJWT").Value;
           // var key = Encoding.ASCII.GetBytes(Configuration.GetSection("MySettings").GetSection("PasswordKeyJWT").Value);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                   
                    //// set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    //ClockSkew = TimeSpan.Zero

                    ValidateIssuer = true,
                    ValidateAudience = true,
                    //ValidateLifetime = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = Configuration.GetSection("MySettings").GetSection("APIUrlServerJWT").Value, //"http://localhost:5000", 
                    ValidAudience = Configuration.GetSection("MySettings").GetSection("APIUrlServerJWT").Value,//"http://localhost:5000",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(PasswordKeyJWT))

                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var jti = jwtToken.Claims.First(claim => claim.Type == "canvas_userid").Value;
                //var accountId = jwtToken.Claims.First(x => x.Type == "name").Value;


            }
            catch(Exception ex)
            {
                // return null if validation fails
                return false;
            }

            return true;
        }

        public TokenData GetDataFromToken(string token)
        {
            TokenData TokenData = new TokenData();
            var tokenHandler = new JwtSecurityTokenHandler();
            string PasswordKeyJWT = Configuration.GetSection("MySettings").GetSection("PasswordKeyJWT").Value;
            // var key = Encoding.ASCII.GetBytes(Configuration.GetSection("MySettings").GetSection("PasswordKeyJWT").Value);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {

                    //// set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    //ClockSkew = TimeSpan.Zero

                    ValidateIssuer = true,
                    ValidateAudience = true,
                    //ValidateLifetime = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = Configuration.GetSection("MySettings").GetSection("APIUrlServerJWT").Value, //"http://localhost:5000", 
                    ValidAudience = Configuration.GetSection("MySettings").GetSection("APIUrlServerJWT").Value,//"http://localhost:5000",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(PasswordKeyJWT))

                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                var canvas_userid = jwtToken.Claims.First(claim => claim.Type == "canvas_userid").Value;
                var user_id = jwtToken.Claims.First(claim => claim.Type == "user_id").Value;
                var user_nombre = jwtToken.Claims.First(claim => claim.Type == "user_nombre").Value;

                //var accountId = jwtToken.Claims.First(x => x.Type == "name").Value;
                TokenData.canvas_userid = canvas_userid;
                TokenData.user_id = user_id;
                TokenData.user_nombre = user_nombre;
            }
            catch (Exception ex)
            {
                // return null if validation fails
                
            }

            return TokenData;
        }
    }
}
