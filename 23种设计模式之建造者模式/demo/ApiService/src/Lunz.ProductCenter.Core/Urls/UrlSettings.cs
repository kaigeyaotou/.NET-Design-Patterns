using Lunz.Configuration;

namespace Lunz.ProductCenter.Core.Urls
{
    public class UrlSettings : IUrlSettings
    {
        private readonly IConfigurationManager _configuration;
        private readonly string _appBaseUrl;

        public UrlSettings(IConfigurationManager configuration)
        {
            _configuration = configuration;
            _appBaseUrl = _configuration.AppSettings.OrderManagement?.AppBaseURL;
        }

        public string OrderUrlTemplate
            => $"{_appBaseUrl}{_configuration.AppSettings.OrderManagement?.AppOrderURL}";
    }
}