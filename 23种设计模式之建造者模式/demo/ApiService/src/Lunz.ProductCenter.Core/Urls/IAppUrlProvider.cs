using System;

namespace Lunz.ProductCenter.Core.Urls
{
    public interface IAppUrlProvider
    {
        string OrderPageUrl(Guid orderId);
    }
}