namespace BookShop.ApiGateway.Contracts
{
    public interface ICommonService
    {
        void HandleAuthorizationHeader(HttpContext context);
        Task HandleExceptionMessageAsync(HttpContext context, string result, int statusCode);
    }
}
