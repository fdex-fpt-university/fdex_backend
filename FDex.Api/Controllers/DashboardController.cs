using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FDex.Application.DTOs.Swap;
using FDex.Application.Features.DashboardItems.Requests.Queries;
using FDex.Application.Features.Swaps.Requests.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FDex.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator _mediator;
        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("DashboardItemData")]
        public async Task<ActionResult<object>> Get()
        {
            object datas = await _mediator.Send(new GetDashboardItemDataRequest());
            return Ok(datas);
        }
    }
}

