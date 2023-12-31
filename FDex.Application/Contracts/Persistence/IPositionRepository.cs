﻿using System;
using FDex.Application.DTOs.TradingPosition;
using FDex.Domain.Entities;

namespace FDex.Application.Contracts.Persistence
{
	public interface IPositionRepository : IGenericRepository<Position>
	{
        Task<Position> GetPositionInDetails(string key);

        Task<List<Position>> GetPositionOrdersInDetails(string wallet);

        Task<List<PositionDTOViewHistory>> GetPositionHistoriesInDetails(string wallet);

        Task<List<Position>> GetPositionsInDetails(string wallet);

        Task<List<PositionDTOLeaderboardItemView>> GetLeaderboardPositionsAsync(bool? isLeverageAsc, bool? isSizeAsc, bool? isPNLAsc);
    }
}

