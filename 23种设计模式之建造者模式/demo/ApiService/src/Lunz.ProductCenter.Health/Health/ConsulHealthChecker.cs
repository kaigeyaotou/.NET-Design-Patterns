using System;
using System.Collections.Generic;
using System.Text;
using Consul;
using Lunz.ProductCenter.Health.Interface;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.Health.Health
{
    public class ConsulHealthChecker : IHealthChecker
    {
        private readonly ConsulClient _client;
        private readonly ILogger<ConsulHealthChecker> _logger;

        public ConsulHealthChecker(ConsulClient client, ILogger<ConsulHealthChecker> logger)
        {
            _client = client;
            _logger = logger;
        }

        public string Message { get; set; }

        public bool DoCheck()
        {
            bool isOK = false;
            try
            {
                var leader = _client.Status.Leader().GetAwaiter().GetResult();
                if (!string.IsNullOrEmpty(leader))
                {
                    isOK = true;
                }
            }
            catch (Exception e)
            {
                Message = $"fatil to check consul,msg is = {e.Message},stacktrace = {e.StackTrace}";
                _logger.LogCritical(Message);
            }

            return isOK;
        }
    }
}
