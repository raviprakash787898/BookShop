using BookShop.ApiGateway.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.ApiGateway.Contracts
{
    public interface ITokenService
    {
        Task<IActionResult> ValidateAndGenerateToken(AuthenticationRequest req);
    }
}
