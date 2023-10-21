using System;
using FDex.Application.DTOs.TradingPosition;
using MediatR;

namespace FDex.Application.Features.Positions.Requests.Queries
{
	public class GetPositionHistoriesRequest : IRequest<List<PositionDTOViewHistory>>
    {
        public string Wallet { get; set; }
    }
}

