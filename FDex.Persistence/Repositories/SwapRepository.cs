using System;
using FDex.Application.Contracts.Persistence;
using FDex.Domain.Entities;
using FDex.Persistence.DbContexts;

namespace FDex.Persistence.Repositories
{
	public class SwapRepository : GenericRepository<Swap>, ISwapRepository
	{
		public readonly FDexDbContext _context;
		public SwapRepository(FDexDbContext context) : base(context)
        {
			_context = context;
		}
	}
}

