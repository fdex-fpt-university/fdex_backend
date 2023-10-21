using System;
using FDex.Application.DTOs.Reporter;
using FDex.Application.DTOs.TradingPosition;
using FDex.Application.Features.Positions.Requests.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FDex.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PositionController : ControllerBase
	{
        private readonly IMediator _mediator;
        public PositionController(IMediator mediator)
		{
            _mediator = mediator;
		}

        [HttpGet("GetOrders")]
        public async Task<ActionResult<List<PositionDTOViewOrder>>> Get([FromQuery] string wallet)
        {
            var orders = await _mediator.Send(new GetPositionOrdersRequest() { Wallet = wallet});
            return Ok(orders);
        }

        //[HttpGet("GetPositions")]
        //[HttpGet("GetHitories")]
    }
}

