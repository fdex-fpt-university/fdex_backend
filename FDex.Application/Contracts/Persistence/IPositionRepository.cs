using System;
using FDex.Domain.Entities;

namespace FDex.Application.Contracts.Persistence
{
	public interface IPositionRepository : IGenericRepository<Position>
	{
        Task<Position> GetPositionInDetails(string key);
    }
}

