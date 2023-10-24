using System;
using FDex.Domain.Entities;

namespace FDex.Application.Contracts.Persistence
{
	public interface IPositionRepository : IGenericRepository<Position>
	{
        Task<Position> GetPositionInDetails(string key);

        Task<List<Position>> GetPositionOrdersInDetails(string wallet);

        Task<List<Position>> GetPositionHistoriesInDetails(string wallet);

        Task<List<Position>> GetPositionsInDetails(string wallet);
    }
}

