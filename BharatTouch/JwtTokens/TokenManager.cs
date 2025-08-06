using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Web;
using DataAccess;
using DataAccess.Models;
using DataAccess.ViewModels;
using Microsoft.IdentityModel.Tokens;
using BharatTouch.CommonHelper;

namespace BharatTouch.JwtTokens
{
    public class TokenManager
    {
        public static string GenerateJWTAuthetication(AdminModel model)//, string role)
        {
            var claims = new List<Claim>
            {
                new Claim("Email", model.EmailId.NullToString()),
                new Claim("UserName", model.FirstName.NullToString() + " " + model.LastName.NullToString()),
                new Claim("UserId", model.UserId.NullToString()),
                new Claim("DisplayName", model.Displayname.NullToString()),
                new Claim("IsAdmin", model.IsAdmin.NullToString()),
            };


            //claims.Add(new Claim(ClaimTypes.Role, role));


            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Convert.ToString(ConfigValues.JwtKey)));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtExpireMinutesSetting = ConfigValues.JwtExpireMinutesSetting;
            var jwtExpireDaysSetting = ConfigValues.JwtExpireDaysSetting;

           // DateTime expires;
            DateTime? expires = null;
            //if (!string.IsNullOrEmpty(jwtExpireMinutesSetting))
            //{
            //    // If JwtExpireMinutes is set, use it to calculate expiration time in minutes
            //    var jwtExpireMinutes = Convert.ToDouble(jwtExpireMinutesSetting);
            //    expires = DateTime.UtcNow.AddMinutes(jwtExpireMinutes);
            //}
            //else if (!string.IsNullOrEmpty(jwtExpireDaysSetting))
            //{
            //    // If JwtExpireDays is set, use it to calculate expiration time in days
            //    var jwtExpireDays = Convert.ToDouble(jwtExpireDaysSetting);
            //    expires = DateTime.UtcNow.AddDays(jwtExpireDays);
            //}
            //else
            //{
            //    // Default to one day expiration if neither JwtExpireMinutes nor JwtExpireDays are set
            //    expires = DateTime.UtcNow.AddDays(1);
            //}

            var token = new JwtSecurityToken(
                ConfigValues.JwtIssuer,
                ConfigValues.JwtAudience,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);


        }

        public static JwtTokenViewModel ValidateToken(string token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(ConfigValues.JwtKey);
            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,

                     //  Allow tokens without expiration
                    RequireExpirationTime = false,
                    ValidateLifetime = false
                };

                SecurityToken validatedToken;
                tokenHandler.ValidateToken(token, validationParameters, out validatedToken);


                // Corrected access to the validatedToken
                var jwtToken = (JwtSecurityToken)validatedToken;
                JwtTokenViewModel jwt = new JwtTokenViewModel
                {
                    UserId = jwtToken.Claims.First(claim => claim.Type == "UserId").Value.ToIntOrZero(),
                    EmailId = jwtToken.Claims.First(claim => claim.Type == "Email").Value.NullToString(),
                    UserName = jwtToken.Claims.First(claim => claim.Type == "UserName").Value.NullToString(),
                    DisplayName = jwtToken.Claims.First(claim => claim.Type == "DisplayName").Value.NullToString(),
                    IsAdmin = jwtToken.Claims.First(claim => claim.Type == "IsAdmin").Value.ToBoolean()
                };
                return jwt;
            }
            catch
            {
                return new JwtTokenViewModel();
            }
        }

        public static string GenerateJWTAuthetication_v2(AdminModel model)//, string role)
        {
            var claims = new List<Claim>
            {
                new Claim("Email", model.EmailId.NullToString()),
                new Claim("UserName", model.FirstName.NullToString() + " " + model.LastName.NullToString()),
                new Claim("UserId", model.UserId.NullToString()),
                new Claim("DisplayName", model.Displayname.NullToString()),
                new Claim("IsAdmin", model.IsAdmin.NullToString()),
            };


            //claims.Add(new Claim(ClaimTypes.Role, role));


            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(ConfigValues.JwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtExpireMinutesSetting = ConfigValues.JwtExpireMinutesSetting;
            var jwtExpireDaysSetting = ConfigValues.JwtExpireDaysSetting;

            DateTime expires;

            if (!string.IsNullOrEmpty(jwtExpireMinutesSetting))
            {
                // If JwtExpireMinutes is set, use it to calculate expiration time in minutes
                var jwtExpireMinutes = Convert.ToDouble(jwtExpireMinutesSetting);
                expires = DateTime.UtcNow.AddMinutes(jwtExpireMinutes);
            }
            else if (!string.IsNullOrEmpty(jwtExpireDaysSetting))
            {
                // If JwtExpireDays is set, use it to calculate expiration time in days
                var jwtExpireDays = Convert.ToDouble(jwtExpireDaysSetting);
                expires = DateTime.UtcNow.AddDays(jwtExpireDays);
            }
            else
            {
                // Default to one day expiration if neither JwtExpireMinutes nor JwtExpireDays are set
                expires = DateTime.UtcNow.AddDays(1);
            }

            var token = new JwtSecurityToken(
                ConfigValues.JwtIssuer,
                ConfigValues.JwtAudience,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);


        }

        public static string GetClaim(string type)
        {
            var principal = Thread.CurrentPrincipal as ClaimsPrincipal;
            return principal?.Claims.FirstOrDefault(c => c.Type == type)?.Value;
        }
    }
}