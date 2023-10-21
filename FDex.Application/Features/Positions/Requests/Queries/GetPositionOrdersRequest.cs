using System;
using FDex.Application.DTOs.TradingPosition;
using MediatR;

namespace FDex.Application.Features.Positions.Requests.Queries
{
	public class GetPositionOrdersRequest : IRequest<List<PositionDTOViewOrder>>
	{
        public string Wallet { get; set; }
    }
}

