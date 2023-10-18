using System;
using Azure;
using FDex.Application.Contracts.Persistence;
using FDex.Domain.Entities;
using FDex.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace FDex.Persistence.Repositories
{
	public class PositionRepository : GenericRepository<Position>, IPositionRepository
	{
		private readonly FDexDbContext _context;
		public PositionRepository(FDexDbContext context) : base(context)
		{
			_context = context;
		}

        public async Task<Position> GetPositionInDetails(string key)
        {
			return await _context.Positions
				.Include(p => p.PositionDetails)
                .FirstOrDefaultAsync(p => p.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
        }
    }
}
