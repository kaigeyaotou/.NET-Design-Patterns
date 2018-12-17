using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Lunz.ProductCenter.Health
{
    public abstract class ControllerBase : Controller
    {
        protected ControllerBase(IMediator mediator, ILogger logger)
        {
            Mediator = mediator;
            Logger = logger;
        }

        protected ILogger Logger { get; }

        protected IMediator Mediator { get; }

        public override BadRequestObjectResult BadRequest(ModelStateDictionary modelState)
        {
            return BadRequest(ModelState.Select(x => x.Value).SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
        }
    }
}