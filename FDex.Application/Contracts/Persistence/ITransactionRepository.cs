using System;
using FDex.Domain.Entities;

namespace FDex.Application.Contracts.Persistence
{
	public interface ITransactionRepository : IGenericRepository<Transaction>
	{
	}
}

