using System;
using System.Numerics;
using Azure;
using FDex.Application.Contracts.Persistence;
using FDex.Application.DTOs.TradingPosition;
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

        public async Task<List<PositionDTOLeaderboardItemView>> GetLeaderboardPositionsAsync(bool? isLeverageAsc, bool? isSizeAsc, bool? isPNLAsc)
        {
            List<PositionDTOLeaderboardItemView> response = new();
            var positions = await _context.Positions.Include(p => p.PositionDetails).Include(p => p.User).ToListAsync();
            foreach(var position in positions)
            {
                List<PositionDetail> positionAggregates = new();
                var responseItem = new PositionDTOLeaderboardItemView()
                {
                    Wallet = position.User.Wallet,
                    IndexToken = position.IndexToken,
                    Side = position.Side,
                    Leverage = position.Leverage,
                    Size = position.Size,
                    EntryPrice = "",
                    PNL = "",
                    Time = DateTime.Now
                };
                var positionDetails = position.PositionDetails.OrderBy(pd => pd.Time);
                foreach(var positionDetail in positionDetails)
                {

                }
                response.Add(responseItem);
            }
            response = isLeverageAsc.HasValue ? (isLeverageAsc.Value ? response.OrderBy(item => item.Leverage).ToList() : response.OrderByDescending(item => item.Leverage).ToList()) : response;
            response = isSizeAsc.HasValue ? (isSizeAsc.Value ? response.OrderBy(item => BigInteger.Parse(item.Size)).ToList() : response.OrderByDescending(item => BigInteger.Parse(item.Size)).ToList()) : response;
            response = isPNLAsc.HasValue ? (isPNLAsc.Value ? response.OrderBy(item => BigInteger.Parse(item.PNL)).ToList() : response.OrderByDescending(item => BigInteger.Parse(item.PNL)).ToList()) : response;
            return response;
        }

        public async Task<List<PositionDTOViewHistory>> GetPositionHistoriesInDetails(string wallet)
        {
            List<PositionDTOViewHistory> response = new();
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
                    if(posd.PositionState == PositionState.Decrease || posd.PositionState == PositionState.Close || posd.PositionState == PositionState.Liquidate)
                    {
                        response.Add(new PositionDTOViewHistory()
                        {
                            CollateralToken = pos.CollateralToken,
                            IndexToken = pos.IndexToken,
                            EntryPrice = posd.EntryPrice,
                            Side = pos.Side,
                            Size = posd.SizeChanged,
                            Pnl = posd.Pnl,
                            Time = posd.Time
                        });
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
