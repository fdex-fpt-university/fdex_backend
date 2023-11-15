using System;
using System.Numerics;
using FDex.Application.Common.Models;
using FDex.Application.Contracts.Persistence;
using FDex.Application.DTOs.User;
using FDex.Application.Models.Infrastructure;
using FDex.Domain.Entities;
using FDex.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

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
            List<Liquidity> liquidities = _context.Liquidities.ToList();
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
            foreach (var al in liquidities)
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

        public async Task<List<UserDTOLeaderboardItemView>> GetLeaderboardItemsAsync(bool? isTradingVolumeAsc, bool? isAvgLeverageAsc, bool? isWinAsc, bool? isLossAsc, bool? isPNLwFeesAsc, int timeRange)
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
            var users = await _context.Users.Include(u => u.Positions).ThenInclude(p => p.PositionDetails).ToListAsync();
            foreach (var user in users)
            {
                var responseItem = new UserDTOLeaderboardItemView()
                {
                    Wallet = user.Wallet,
                    TradingVol = BigInteger.Zero.ToString(),
                    AvgLeverage = double.NegativeZero,
                    Win = 0,
                    Loss = 0,
                    PNLwFees = BigInteger.Zero.ToString()
                };
                var positions = user.Positions;
                if (positions != null)
                {
                    foreach(var position in positions)
                    {
                        if (position.LastUpdatedDate > cutoffDate)
                        {
                            responseItem.TradingVol = (BigInteger.Parse(responseItem.TradingVol) + BigInteger.Parse(position.TradingVolumn)).ToString();
                            responseItem.AvgLeverage += position.Leverage;
                            var positionDetails = position.PositionDetails;
                            foreach (var positionDetail in positionDetails)
                            {
                                var pnl = positionDetail.Pnl;
                                if (pnl != null)
                                {
                                    responseItem.PNLwFees = (BigInteger.Parse(responseItem.PNLwFees) + BigInteger.Parse(pnl)).ToString();
                                    if (BigInteger.Parse(pnl) >= 0)
                                    {
                                        responseItem.Win += 1;
                                    }
                                    else
                                    {
                                        responseItem.Loss += 1;
                                    }
                                }
                            }
                        }
                    }
                    responseItem.AvgLeverage /= positions.Count();
                    responseItem.AvgLeverage = Math.Round(responseItem.AvgLeverage, 2);
                    if(responseItem.AvgLeverage == null)
                    {
                        responseItem.AvgLeverage = 0;
                    }
                }
                response.Add(responseItem);
            }
            response = isTradingVolumeAsc.HasValue ? (isTradingVolumeAsc.Value ? response.OrderBy(item => BigInteger.Parse(item.TradingVol)).ToList() : response.OrderByDescending(item => BigInteger.Parse(item.TradingVol)).ToList()) : response;
            response = isAvgLeverageAsc.HasValue ? (isAvgLeverageAsc.Value ? response.OrderBy(item => item.AvgLeverage).ToList() : response.OrderByDescending(item => item.AvgLeverage).ToList()) : response;
            response = isWinAsc.HasValue ? (isWinAsc.Value ? response.OrderBy(item => item.Win).ToList() : response.OrderByDescending(item => item.Win).ToList()) : response;
            response = isLossAsc.HasValue ? (isLossAsc.Value ? response.OrderBy(item => item.Loss).ToList() : response.OrderByDescending(item => item.Loss).ToList()) : response;
            response = isPNLwFeesAsc.HasValue ? (isPNLwFeesAsc.Value ? response.OrderBy(item => BigInteger.Parse(item.PNLwFees)).ToList() : response.OrderByDescending(item => BigInteger.Parse(item.PNLwFees)).ToList()) : response;
            return response;
        }
        public async Task<Analytic> GetReferralAnalytics()
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
            Analytic analytic = new Analytic
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

        public async Task<string> GetRewardAsync(string wallet)
        {
            decimal totalRewardAmount = 0;
            List<Reward> rewards = await _context.Rewards.Where(r => r.Wallet.Equals(wallet)).ToListAsync();
            foreach(var reward in rewards)
            {
                if (!reward.ClaimedDate.HasValue)
                {
                    totalRewardAmount += decimal.Parse(reward.Amount);
                }
            }
            return totalRewardAmount.ToString();
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