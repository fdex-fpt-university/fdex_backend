using System;
namespace FDex.Application.Contracts.Persistence
{
	public interface IUnitOfWork
	{
        ITransactionRepository TransactionRepository { get; }

        Task Save();
    }
}

