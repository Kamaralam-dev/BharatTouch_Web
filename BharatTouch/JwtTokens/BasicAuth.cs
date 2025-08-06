using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using Microsoft.IdentityModel.Tokens;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Net;
using BharatTouch.CommonHelper;

namespace BharatTouch.JwtTokens
{
    public class AuthorizationAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            // Extract the JWT token from the request headers
            var authHeader = actionContext.Request.Headers.Authorization;
            if (authHeader == null || authHeader.Scheme != "Bearer" || string.IsNullOrEmpty(authHeader.Parameter))
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                return;
            }

            var token = authHeader.Parameter;

            // Validate the JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Convert.ToString(ConfigValues.JwtKey));
            var tokenValidationParameters = new TokenValidationParameters
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

            try
            {
                // Validate token
                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out validatedToken);

                //if (validatedToken.ValidTo < DateTime.UtcNow)
                //{
                //    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "Token has expired.");
                //    return;
                //}

                var jwtToken = (JwtSecurityToken)validatedToken;
                var emailId = jwtToken.Claims.First(claim => claim.Type == "Email").Value;
                var loggedUserId = jwtToken.Claims.First(claim => claim.Type == "UserId").Value;
                var loggedUserDisplayName = jwtToken.Claims.First(claim => claim.Type == "DisplayName").Value;

                // Set the principal for the current request context
                Thread.CurrentPrincipal = principal;
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.User = principal;
                }
            }
            catch (SecurityTokenException exe)
            {
                // Token validation failed
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                return;
            }

            base.OnAuthorization(actionContext);
        }
    }
}