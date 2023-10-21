using System;
using FDex.Application.DTOs.TradingPosition;
using MediatR;

namespace FDex.Application.Features.Positions.Requests.Queries
{
	public class GetPositionsRequest : IRequest<List<PositionDTOView>>
    {
        public string Wallet { get; set; }
    }
}

