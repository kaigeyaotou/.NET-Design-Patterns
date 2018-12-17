using System;
using Lunz.Data;
using Lunz.ProductCenter.Data;
using Lunz.ProductCenter.Health.Interface;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.Health.Health
{
    public class DBHealthChecker : IHealthChecker
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DBHealthChecker> _logger;
        private readonly IAmbientDatabaseLocator _databaseLocator;
        private readonly IDatabaseScopeFactory _databaseScopeFactory;

        public DBHealthChecker(
            IMediator mediator,
            ILogger<DBHealthChecker> logger,
            IAmbientDatabaseLocator databaseLocator,
            IDatabaseScopeFactory databaseScopeFactory)
        {
            _mediator = mediator;
            _logger = logger;
            _databaseLocator = databaseLocator;
            _databaseScopeFactory = databaseScopeFactory;
        }

        public string Message { get; set; }

        public bool DoCheck()
        {
            bool isOK = false;
            try
            {
                using (var scope = _databaseScopeFactory.CreateWithTransaction())
                {
                    var db = _databaseLocator.GetProductCenterManagementDatabase();
                    isOK = db.State == System.Data.ConnectionState.Open;
                }
            }
            catch (Exception e)
            {
                Message = $"fatil to check db,msg is = {e.Message},stacktrace = {e.StackTrace}";
                _logger.LogCritical(Message);
            }

            return isOK;
        }
    }
}
