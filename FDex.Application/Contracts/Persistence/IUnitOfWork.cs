using System;
namespace FDex.Application.Contracts.Persistence
{
	public interface IUnitOfWork
	{
        ISwapRepository SwapRepository { get; }
        IUserRepository UserRepository { get; }
        IReporterRepository ReporterRepository { get; }
        ILiquidityRepository LiquidityRepository { get; }
        IPositionRepository PositionRepository { get; }
        IPositionDetailRepository PositionDetailRepository { get; }
        IRewardRepository RewardRepository { get; }

        Task SaveAsync();
        void Dispose();
    }
}

