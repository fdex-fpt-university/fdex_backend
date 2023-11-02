using System;
using FDex.Application.Contracts.Persistence;
using FDex.Domain.Entities;
using FDex.Persistence.DbContexts;

namespace FDex.Persistence.Repositories
{
	public class LiquidityRepository : GenericRepository<Liquidity>, ILiquidityRepository
	{
        public readonly FDexDbContext _context;
        public LiquidityRepository(FDexDbContext context) : base(context)
		{
			_context = context;
		}
	}
}

