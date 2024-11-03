using Microsoft.IdentityModel.Tokens;

namespace BookShop.ApiGateway.Contracts
{
    public interface IContractManager
    {
        ITokenService TokenService { get; }
        ICommonService CommonService { get; }
    }
}
