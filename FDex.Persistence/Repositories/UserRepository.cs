using System;
using System.Numerics;
using FDex.Application.Common.Models;
using FDex.Application.Contracts.Persistence;
using FDex.Application.DTOs.User;
using FDex.Domain.Entities;
using FDex.Domain.Enumerations;
using FDex.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using NBitcoin.Secp256k1;

namespace FDex.Persistence.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public readonly FDexDbContext _context;

        public UserRepository(FDexDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<object> GetDashboardItemDatas()
        {
            var totalUser = _context.Users;
            BigInteger accuredFees = 0;
            BigInteger accuredFeesChange = 0;
            BigInteger totalTradingVolumn = 0;
            BigInteger totalTradingVolumnChange = 0;
            BigInteger totalUserCount = totalUser.Count();
            BigInteger totalUserCountChange = await totalUser.Where(u => u.CreatedDate.Date > DateTime.Now.AddDays(-1).Date).CountAsync();
            List<Swap> swaps = _context.Swaps.ToList();
            List<AddLiquidity> addLiquidities = _context.AddLiquidities.ToList();
            List<Position> positions = _context.Positions.ToList();
            foreach( var pos in positions)
            {
                totalTradingVolumn += BigInteger.Parse(pos.TradingVolumn);
                if(pos.LastUpdatedDate.Date > DateTime.Now.AddDays(-1).Date)
                {
                    totalTradingVolumnChange += BigInteger.Parse(pos.TradingVolumn);
                }
            }
            foreach (var swap in swaps)
            {
                accuredFees += BigInteger.Parse(swap.Fee);
                if (swap.Time.Date > DateTime.Now.AddDays(-1).Date)
                {
                    accuredFeesChange += BigInteger.Parse(swap.Fee);
                }
            }
            foreach (var al in addLiquidities)
            {
                accuredFees += BigInteger.Parse(al.Fee);
                if (al.DateAdded.Date > DateTime.Now.AddDays(-1).Date)
                {
                    accuredFeesChange += BigInteger.Parse(al.Fee);
                }
            }
            var dashboardItemDatas = new
            {
                TotalUserCount = totalUserCount.ToString(),
                TotalUserCountChange = totalUserCountChange.ToString(),
                AccuredFees = accuredFees.ToString(),
                AccuredFeesChange = accuredFeesChange.ToString(),
                TotalTradingVolumn = totalTradingVolumn.ToString(),
                TotalTradingVolumnChange = totalTradingVolumnChange.ToString()
            };
            return dashboardItemDatas;
        }

        public async Task<List<UserDTOLeaderboardItemView>> GetLeaderboardItemsAsync(bool? isTradingVolumnAsc, bool? isAvgLeverageAsc, bool? isWinAsc, bool? isLossAsc, bool? isPNLwFeesAsc, int timeRange)
        {
            List<UserDTOLeaderboardItemView> response = new();
            var cutoffDate = DateTime.Now;
            switch (timeRange)
            {
                case 0:
                    cutoffDate = cutoffDate.AddHours(-24);
                    break;
                case 1:
                    cutoffDate = cutoffDate.AddDays(-7);
                    break;
                case 2:
                    cutoffDate = cutoffDate.AddMonths(-1);
                    break;
                default:
                    break;
            }
            var users = _context.Users
                .Where(user => _context.Positions
                    .Where(position => _context.PositionDetails
                        .Where(positionDetail => positionDetail.Time >= cutoffDate && positionDetail.PositionId == position.Id)
                        .Any())
                    .Any(position => position.Wallet == user.Wallet))
                .GroupBy(user => user.Wallet)
                .Select(userGroup => new UserDTOLeaderboardItemView
                {
                    Wallet = userGroup.Key,
                    TradingVol = userGroup.Sum(user => user.Positions
                        .SelectMany(position => position.PositionDetails)
                        .Where(positionDetail => positionDetail.Time >= cutoffDate)
                        .Sum(positionDetail => (int)BigInteger.Parse(positionDetail.SizeChanged))
                    ),
                    AvgLeverage = userGroup.Average(user => user.Positions
                        .Where(position => position.PositionDetails
                            .Any(positionDetail => positionDetail.Time >= cutoffDate)
                        )
                        .Average(position => position.Leverage)
                    ),
                    Win = userGroup.Sum(user => user.Positions
                        .SelectMany(position => position.PositionDetails)
                        .Where(positionDetail => positionDetail.Time >= cutoffDate && BigInteger.Parse(positionDetail.Pnl) > 0)
                        .Count()
                    ),
                    Loss = userGroup.Sum(user => user.Positions
                        .SelectMany(position => position.PositionDetails)
                        .Where(positionDetail => positionDetail.Time >= cutoffDate && BigInteger.Parse(positionDetail.Pnl) < 0)
                        .Count()
                    ),
                    PNLwFees = userGroup.Sum(user => user.Positions
                        .SelectMany(position => position.PositionDetails)
                        .Where(positionDetail => positionDetail.Time >= cutoffDate)
                        .Sum(positionDetail => (int)BigInteger.Parse(positionDetail.FeeValue))
                    )
                });

            return response;
        }
        public async Task<object> GetReferralAnalytics()
        {
            var levelCounts = new Dictionary<int, int>();
            levelCounts = await _context.Users
                .GroupBy(u => u.Level)
                .Select(u => new
                {
                    Level = u.Key ?? 0,
                    Count = u.Count()
                })
                .OrderByDescending(u => u.Level)
                .ToDictionaryAsync(
                    el => el.Level,
                    el => el.Count
                );
            object analytic = new
            {
                Level0 = levelCounts.GetValueOrDefault(0),
                Level1 = levelCounts.GetValueOrDefault(1),
                Level2 = levelCounts.GetValueOrDefault(2),
                Level3 = levelCounts.GetValueOrDefault(3),
            };
            return analytic;
        }

        public async Task<GetUserResponse> GetReferredUsers(string wallet, int page, int pageSize)
        {
            List<User> referredUsers = await _context.Users
                .Where(u => u.Wallet == wallet)
                .SelectMany(u => u.ReferredUsers)
                .OrderByDescending(e => e.ReferredUserDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            int referredUsersCount = await _context.Users.Where(u => u.Wallet == wallet).SelectMany(u => u.ReferredUsers).CountAsync();

            int numberOfPage = referredUsersCount % pageSize == 0 ? numberOfPage = referredUsersCount / pageSize : numberOfPage = referredUsersCount / pageSize + 1;

            var rs = new GetUserResponse()
            {
                Users = referredUsers,
                NumberOfPage = numberOfPage
            };

            return rs;
        }

        public async Task<List<User>> GetUsersInDetailsAsync()
        {
            List<User> users = await _context.Users
                .Include(u => u.Positions)
                .ThenInclude(p => p.PositionDetails)
                .ToListAsync();
            return users;
        }
    }
}