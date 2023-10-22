using System;
using FDex.Application.Contracts.Persistence;
using FDex.Domain.Entities;
using FDex.Persistence.DbContexts;

namespace FDex.Persistence.Repositories
{
	public class PositionDetailRepository : GenericRepository<PositionDetail>, IPositionDetailRepository
	{
		private readonly FDexDbContext _context;
		public PositionDetailRepository(FDexDbContext context) : base(context)
		{
			_context = context;
		}
	}
}

