using System;
using FDex.Application.DTOs.TradingPosition;
using MediatR;

namespace FDex.Application.Features.Positions.Requests.Queries
{
	public class GetLeaderboardPositionsRequest : IRequest<List<PositionDTOLeaderboardItemView>>
	{
		public bool? IsLeverageAsc { get; set; }
        public bool? IsSizeAsc { get; set; }
        public bool? IsPNLAsc { get; set; }
    }
}

