using System;
using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Draekien.CleanVerticalSlice.Common.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ApiControllerBase : ControllerBase
    {
        private IMediator? _mediator;

        /// <summary>
        /// Returns an instance of <see cref="IMediator"/>
        /// </summary>
        protected IMediator Mediator => (_mediator ??= HttpContext.RequestServices.GetService<IMediator>()) ?? throw new InvalidOperationException("Mediator is not registered");
    }
}
