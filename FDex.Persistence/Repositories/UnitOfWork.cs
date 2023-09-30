using System;
using FDex.Application.Contracts.Persistence;
using FDex.Persistence.DbContexts;

namespace FDex.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FDexDbContext _context;
        private ISwapRepository _swapRepository;
        private IUserRepository _userRepository;

        public UnitOfWork(FDexDbContext context)
        {
            _context = context;
        }

        public ISwapRepository SwapRepository => _swapRepository ??= new SwapRepository(_context);
        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context);

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

