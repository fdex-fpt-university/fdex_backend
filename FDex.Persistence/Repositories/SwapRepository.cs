using System;
using FDex.Application.Contracts.Persistence;
using FDex.Domain.Entities;
using FDex.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace FDex.Persistence.Repositories
{
    public class SwapRepository : GenericRepository<Swap>, ISwapRepository
	{
		public readonly FDexDbContext _context;
		public SwapRepository(FDexDbContext context) : base(context)
        {
			_context = context;
		}

        public async Task<List<Swap>> GetSwapsByCondition(string wallet, int page, int pageSize)
        {
            return await _context.Swaps
                .Where(s => s.Wallet.Equals(wallet))
                .OrderByDescending(e => e.Time)
                .Skip((page -1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}

