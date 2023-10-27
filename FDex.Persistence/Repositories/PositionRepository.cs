using System;
using Azure;
using FDex.Application.Contracts.Persistence;
using FDex.Domain.Entities;
using FDex.Domain.Enumerations;
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
            List<Position> response = new();
            var positions = await _context.Positions
                .Where(x => x.Wallet == wallet)
                .Include(c => c.PositionDetails)
                .ToListAsync();
            foreach (var pos in positions)
            {
                var positionDetails = pos.PositionDetails
                    .OrderByDescending(pd => pd.Time)
                    .ToList();
                foreach(var posd in positionDetails)
                {
                    if(posd.PositionState == PositionState.Close || posd.PositionState == PositionState.Liquidate || posd.PositionState == PositionState.Decrease)
                    {
                        response.Add(pos);
                    }
                }
            }
            return response;
        }

        public async Task<Position> GetPositionInDetails(string key)
        {
            return await _context.Positions
                .Include(p => p.PositionDetails)
                .FirstOrDefaultAsync(p => p.Key.Equals(key));
        }

        public async Task<List<Position>> GetPositionOrdersInDetails(string wallet)
        {
            List<Position> response = new();
            var positions = await _context.Positions
                .Where(x => x.Wallet == wallet)
                .Include(x => x.PositionDetails)
                .ToListAsync();
            foreach (var pos in positions)
            {
                var latestPositionDetail = pos.PositionDetails
                    .OrderByDescending(pd => pd.Time)
                    .FirstOrDefault();
                if (latestPositionDetail.PositionState == PositionState.Order)
                {
                    response.Add(pos);
                }
            }
            return response;
        }

        public async Task<List<Position>> GetPositionsInDetails(string wallet)
        {
            List<Position> response = new();
            var positions = await _context.Positions
                .Where(x => x.Wallet == wallet)
                .Include(c => c.PositionDetails)
                .ToListAsync();
            foreach(var pos in positions)
            {
                var latestPositionDetail = pos.PositionDetails
                    .OrderByDescending(pd => pd.Time)
                    .FirstOrDefault();
                if(latestPositionDetail.PositionState == PositionState.Open || latestPositionDetail.PositionState == PositionState.Increase || latestPositionDetail.PositionState == PositionState.Decrease)
                {
                    response.Add(pos);
                }
            }
            return response;
        }
    }
}
