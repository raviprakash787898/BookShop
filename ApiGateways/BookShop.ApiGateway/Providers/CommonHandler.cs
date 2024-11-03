using BookShop.ApiGateway.Contracts;
using BookShop.ApiGateway.Utils;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace BookShop.ApiGateway.Providers
{
    /// <summary>
    /// Common Middleware to handle Authorization and passing it to the request and Logs Exception/Information in a file
    /// </summary>
    public class CommonHandler
    {
        private readonly ILogger<CommonHandler> _logger;
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _services;
        private readonly IContractManager _contractManager;

        public CommonHandler(RequestDelegate next, ILogger<CommonHandler> logger, IServiceProvider services)
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
                var builder = new StringBuilder();
                var request = await Utilities.FormatRequest(context.Request);
                builder.AppendLine("").Append("Request: ").AppendLine(request);

                // Coping pointer to the original response body stream
                var originalBodyStream = context.Response.Body;

                // Create a new memory stream
                using var responseBody = new MemoryStream();

                // and use that for the temporary response body
                context.Response.Body = responseBody;

                // Handling Authorization for request and adding UsrId to the Authorized request
                _contractManager.CommonService.HandleAuthorizationHeader(context);


                // Continue down the Middleware pipeline, eventually returning to this class
                await _next(context);


                // Format the response from the server
                var response = await Utilities.FormatResponse(context.Request, context.Response, true);
                builder.Append("Response: ").AppendLine(response);
                FileLogger.WriteInformationLog(builder.ToString(), _logger);

                // Copy the contents of the new memory stream (which contains the response) to the original stream, which is then returned to the client.
                await responseBody.CopyToAsync(originalBodyStream);
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
                    Response = JsonConvert.SerializeObject(new { StatusCode = statusCode, ErrorMessage = ex.Message })
                };
                var req = context.Request;
                FileLogger.WriteLog(req.Path, req.Method, string.Empty, ex, error, _logger);
                await _contractManager.CommonService.HandleExceptionMessageAsync(context, result, statusCode).ConfigureAwait(false);
            }
        }
    }
}
