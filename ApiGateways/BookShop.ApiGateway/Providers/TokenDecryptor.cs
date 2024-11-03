using BookShop.ApiGateway.Contracts;
using BookShop.ApiGateway.Utils;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Net;

namespace BookShop.ApiGateway.Providers
{
    /// <summary>
    /// Token Decryptor to handle HTTP request and decrypt the encrypted token
    /// </summary>
    public class TokenDecryptor
    {
        private readonly ILogger<TokenDecryptor> _logger;
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _services;
        private readonly IContractManager _contractManager;

        public TokenDecryptor(RequestDelegate next, ILogger<TokenDecryptor> logger, IServiceProvider services)
        {
            _next = next;
            _logger = logger;
            _services = services;

            // Creating scope to get CommonService instance
            using var scope = _services.CreateScope();
            _contractManager = scope.ServiceProvider.GetRequiredService<IContractManager>();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                context.Request.EnableBuffering();
                var auth = context.Request.Headers?.Authorization;
                var token = auth?.FirstOrDefault(x => x != null && x.StartsWith("Bearer"))?.Split("Bearer ")[1];

                if (!string.IsNullOrWhiteSpace(token))
                {
                    var decryptedToken = Utilities.Decryptor(token);
                    context.Request.Headers.Authorization = new StringValues(new string[] { $"Bearer {decryptedToken}" });
                }

                // Continue down the Middleware pipeline, eventually returning to this class
                await _next(context);
            }
            catch (Exception ex)
            {
                // Sending generic exception failure to client request and capturing exception to logs
                var statusCode = (int)HttpStatusCode.InternalServerError;
                var result = JsonConvert.SerializeObject(
                    new
                    {
                        StatusCode = OAuthConstants.GeneralSystemFailureErrCode,
                        ErrorMessage = OAuthConstants.GeneralSystemFailureMsg
                    }
                );

                object error = new
                {
                    Request = await Utilities.FormatRequest(context.Request),
                    Response = $" Token Decryptor : {ex.Message}"
                };
                var req = context.Request;
                FileLogger.WriteLog(req.Path, req.Method, string.Empty, ex, error, _logger);
                await _contractManager.CommonService.HandleExceptionMessageAsync(context, result, statusCode).ConfigureAwait(false);
            }
        }
    }
}
