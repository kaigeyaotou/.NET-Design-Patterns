using System;
using System.Collections.Generic;
using System.Text;
using Lunz.ProductCenter.Health.Health;
using Lunz.ProductCenter.Health.Interface;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.Health
{
    [Route("api/v1/[controller]")]
    public class CheckController : ControllerBase
    {
        private readonly IEnumerable<IHealthChecker> _healthCheckers;

        public CheckController(
            IMediator mediator,
            ILogger<CheckController> logger,
            IEnumerable<IHealthChecker> healthCheckers)
            : base(mediator, logger)
        {
            _healthCheckers = healthCheckers;
        }

        [HttpGet("/health")]
        public IActionResult Healthe()
        {
            foreach (var item in _healthCheckers)
            {
                if (!item.DoCheck())
                {
                    throw new Exception(item.Message);
                }
            }

            return Ok();
        }
    }
}
