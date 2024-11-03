using BookShop.ApiGateway.Contracts;
using BookShop.ApiGateway.Utils;
using System.Security.Claims;

namespace BookShop.ApiGateway.Services
{
    public class CommonService : ICommonService
    {
        public CommonService() { }

        /// <summary>Handles the authorization header.</summary>
        /// <param name="context">The context.</param>
        public void HandleAuthorizationHeader(HttpContext context)
        {
            // Adding UsrId to the Authorized request
            var claim = context.User.Claims;
            var usrName = claim.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value?.ToString();

            if (!string.IsNullOrWhiteSpace(usrName))
            {
                context.Request.Headers.Append(OAuthConstants.UserID, usrName);
            }
        }

        /// <summary>
        /// Handles any application or unwanted error
        /// </summary>
        /// <param name="context">Accepts the HttpContext</param>
        /// <param name="exception">Accepts the exception</param>
        /// <returns>Return Json , with StatusCode - 500 and ErrorMsg</returns>
        public Task HandleExceptionMessageAsync(HttpContext context, string result, int statusCode)
        {
            context.Response.ContentType = Constants.APIContentType;
            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsync(result);
        }
    }
}
