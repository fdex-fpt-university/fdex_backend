using System;
using FDex.Application.Contracts.Persistence;
using FDex.Domain.Entities;
using FDex.Persistence.DbContexts;

namespace FDex.Persistence.Repositories
{
	public class RewardRepository : GenericRepository<Reward>, IRewardRepository
    {
        public readonly FDexDbContext _context;
        public RewardRepository(FDexDbContext context) : base(context)
		{
			_context = context;
		}
	}
}

