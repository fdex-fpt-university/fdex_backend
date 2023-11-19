using System;
using FDex.Application.DTOs.TradingPosition;
using FDex.Application.Responses.Common;

namespace FDex.Application.Responses.Positions
{
	public class PositionLeaderboardResponseModel : BaseResponseModel
	{
        public List<PositionDTOLeaderboardItemView> Positions { get; set; }
    }
}

