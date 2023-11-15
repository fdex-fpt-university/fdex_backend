using System;
using FDex.Application.Contracts.Persistence;
using FDex.Persistence.DbContexts;

namespace FDex.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly FDexDbContext _context;
        private bool _disposed;
        private ISwapRepository _swapRepository;
        private IUserRepository _userRepository;
        private IReporterRepository _reporterRepository;
        private ILiquidityRepository _liquidityRepository;
        private IPositionRepository _positionRepository;
        private IPositionDetailRepository _positionDetailRepository;
        private IRewardRepository _rewardRepository;

        public UnitOfWork(FDexDbContext context)
        {
            _context = context;
        }

        public ISwapRepository SwapRepository => _swapRepository ??= new SwapRepository(_context);
        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context);
        public IReporterRepository ReporterRepository => _reporterRepository ??= new ReporterRepository(_context);
        public ILiquidityRepository LiquidityRepository => _liquidityRepository ??= new LiquidityRepository(_context);
        public IPositionRepository PositionRepository => _positionRepository ??= new PositionRepository(_context);
        public IPositionDetailRepository PositionDetailRepository => _positionDetailRepository ??= new PositionDetailRepository(_context);
        public IRewardRepository RewardRepository => _rewardRepository ??= new RewardRepository(_context);

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            _context.Dispose();
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

