using System;
using FDex.Application.Contracts.Persistence;
using FDex.Persistence.DbContexts;

namespace FDex.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FDexDbContext _context;
        private ITransactionRepository _transactionRepository;

        public UnitOfWork(FDexDbContext context)
        {
            _context = context;
        }

        public ITransactionRepository TransactionRepository => _transactionRepository ??= new TransactionRepository(_context);

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}

