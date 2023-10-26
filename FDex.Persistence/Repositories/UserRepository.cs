using System;
using System.Numerics;
using FDex.Application.Contracts.Persistence;
using FDex.Domain.Entities;
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

        public async Task<UserLevelAnalytic> GetReferralAnalytics()
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
            UserLevelAnalytic analytic = new()
            {
                Level0 = levelCounts.GetValueOrDefault(0),
                Level1 = levelCounts.GetValueOrDefault(1),
                Level2 = levelCounts.GetValueOrDefault(2),
                Level3 = levelCounts.GetValueOrDefault(3),
            };
            return analytic;
        }

        public async Task<List<User>> GetReferredUsers(string wallet, int page, int pageSize)
        {
            List<User> referredUsers = await _context.Users
                .Where(u => u.Wallet == wallet)
                .SelectMany(u => u.ReferredUsers)
                .OrderByDescending(e => e.ReferredUserDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return referredUsers;
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