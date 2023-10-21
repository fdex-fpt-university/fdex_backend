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

        public async Task<List<Position>> GetPositionHistoriesInDetails(string wallet)
        {
            var positions = await _context.Positions.Where(x => x.Wallet == wallet).Include(c => c.PositionDetails).ToListAsync();
            return positions;
        }

        public async Task<Position> GetPositionInDetails(string key)
        {
            return await _context.Positions
                .Include(p => p.PositionDetails)
                .FirstOrDefaultAsync(p => p.Key.ToLower().Equals(key.ToLower()));
        }

        public async Task<List<Position>> GetPositionOrdersInDetails(string wallet)
        {
            var positions = await _context.Positions.Include(x => x.PositionDetails).ToListAsync();
            return positions;
        }

        public async Task<List<Position>> GetPositionsInDetails(string wallet)
        {
            var positions = await _context.Positions.Where(x => x.Wallet == wallet).Include(c => c.PositionDetails).ToListAsync();
            return positions;
        }
    }
}
