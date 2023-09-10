using System;
using FDex.Application.Contracts.Persistence;
using FDex.Domain.Entities;
using FDex.Persistence.DbContexts;

namespace FDex.Persistence.Repositories
{
	public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
	{
		public readonly FDexDbContext _context;
		public TransactionRepository(FDexDbContext context) : base(context)
        {
			_context = context;
		}
	}
}

