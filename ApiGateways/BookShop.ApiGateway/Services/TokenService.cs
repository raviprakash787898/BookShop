using BookShop.ApiGateway.Contracts;
using BookShop.ApiGateway.Models;
using BookShop.ApiGateway.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace BookShop.ApiGateway.Services
{
    public class TokenService : ITokenService
    {
        private readonly AppSettings _appSettings;
        public TokenService(AppSettings appSettings) 
        {
            _appSettings = appSettings;
        }

        public async Task<IActionResult> ValidateAndGenerateToken(AuthenticationRequest req)
        {
            if (req.UserName != "RPUser" || req.Password != "Hello@1234")
            {
                return Utilities.ResponseContent(JsonConvert.SerializeObject(new
                {
                    error = OAuthConstants.Invalid_Credentials,
                    error_description = OAuthConstants.NoUserFoundMsg
                }), (int)HttpStatusCode.BadRequest);
            }

            var result = GenerateToken(req);
            return result;
        }

        private IActionResult GenerateToken(AuthenticationRequest usr)
        {
            var serverError = Utilities.ResponseContent(JsonConvert.SerializeObject(new
            {
                error = OAuthConstants.ServerError,
                error_description = OAuthConstants.GeneralSystemFailureMsg
            }), (int)HttpStatusCode.InternalServerError);

            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings?.JwtToken?.Key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var jwtTokenHandler = new JwtSecurityTokenHandler();

                ClaimsIdentity oAuthIdentity = new ClaimsIdentity("Bearer");
                oAuthIdentity.AddClaim(new Claim(ClaimTypes.Name, usr.UserName));

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = oAuthIdentity,
                    Issuer = _appSettings.JwtToken.Issuer,
                    Audience = _appSettings.JwtToken.Issuer,
                    Expires = DateTime.UtcNow.AddHours(Convert.ToInt32(_appSettings.JwtToken.expHrs)),
                    SigningCredentials = credentials
                };

                var token = jwtTokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = jwtTokenHandler.WriteToken(token);
                var encryptedToken = Utilities.Encryptor(jwtToken);

                return Utilities.ResponseContent(JsonConvert.SerializeObject(new AuthenticationResponse()
                {
                    access_token = encryptedToken,
                    token_type = OAuthConstants.Bearer,
                    expires_in = (int)Convert.ToDateTime(tokenDescriptor.Expires).Subtract(DateTime.UtcNow).TotalSeconds,
                    userName = usr.UserName,
                    issued = DateTime.UtcNow.AddHours(0).ToString("R"),
                    expires = DateTime.UtcNow.AddHours(Convert.ToInt32(_appSettings.JwtToken.expHrs)).ToString("R")
                }), (int)HttpStatusCode.OK);
            }
            catch (Exception)
            {
                return serverError;
            }
        }
    }
}
