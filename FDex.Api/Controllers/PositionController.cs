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

        [HttpGet("[action]")]
        public async Task<List<PositionDTOViewOrder>> GetOrders([FromQuery] GetPositionOrdersRequest query) => await _mediator.Send(query);

        [HttpGet("[action]")]
        public async Task<List<PositionDTOView>> GetPositions([FromQuery] GetPositionsRequest query) => await _mediator.Send(query);

        [HttpGet("[action]")]
        public async Task<List<PositionDTOViewHistory>> GetHitories([FromQuery] GetPositionHistoriesRequest query) => await _mediator.Send(query);
    }
}

