using System;
using FDex.Domain.Entities;

namespace FDex.Application.Contracts.Persistence
{
	public interface ISwapRepository : IGenericRepository<Swap>
	{
		Task<List<Swap>> GetSwapsByCondition(string wallet, int page, int pageSize);
	}
}

