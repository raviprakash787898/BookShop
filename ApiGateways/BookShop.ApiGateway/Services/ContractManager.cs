using BookShop.ApiGateway.Contracts;
using BookShop.ApiGateway.Models;
using Microsoft.Extensions.Options;

namespace BookShop.ApiGateway.Services
{
    public sealed class ContractManager : IContractManager
    {
        private readonly AppSettings _settings;
        private readonly Lazy<ITokenService> _tokenService;
        private readonly Lazy<ICommonService> _commonService;

        public ContractManager(IConfiguration config, IOptions<AppSettings> settings)
        {
            // Getting AppSettings value and injecting to all service layer
            _settings = settings.Value;
            _tokenService = new Lazy<ITokenService>(() => new TokenService(_settings));
            _commonService = new Lazy<ICommonService>(() => new CommonService());
        }

        public ITokenService TokenService => _tokenService.Value;
        public ICommonService CommonService => _commonService.Value;
    }
}
