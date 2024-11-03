using BookShop.ApiGateway.Contracts;
using BookShop.ApiGateway.Models;
using BookShop.ApiGateway.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace BookShop.ApiGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IContractManager _contractManager;
        private readonly ILogger<TokenController> _logger;

        public TokenController(IContractManager contractManager, ILogger<TokenController> logger)
        {
            _contractManager = contractManager;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Token([FromBody] AuthenticationRequest authRequest)
        {
            try
            {
                _logger.LogInformation("Token request initiated..." + DateTime.Now.ToString());
                if (authRequest == null || string.IsNullOrWhiteSpace(authRequest.UserName) || string.IsNullOrWhiteSpace(authRequest.Password))
                {
                    return new ContentResult()
                    {
                        ContentType = Constants.APIContentType,
                        StatusCode = (int)HttpStatusCode.Forbidden,
                        Content = JsonConvert.SerializeObject(new
                        {
                            error = OAuthConstants.InvalidDataDescription,
                            error_description = OAuthConstants.MissingRequiredFieldsFailureMessage
                        })
                    };
                }

                return await _contractManager.TokenService.ValidateAndGenerateToken(authRequest);
            }
            catch (Exception)
            {
                _logger.LogInformation("Token request failed..." + DateTime.Now.ToString());
                throw;
            }
        }
    }
}
