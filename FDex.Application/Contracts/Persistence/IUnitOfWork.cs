using System;
namespace FDex.Application.Contracts.Persistence
{
	public interface IUnitOfWork
	{
        ISwapRepository SwapRepository { get; }
        IUserRepository UserRepository { get; }
        IReporterRepository ReporterRepository { get; }
        IAddLiquidityRepository AddLiquidityRepository { get; }
        IPositionRepository PositionRepository { get; }
        IPositionDetailRepository PositionDetailRepository { get; }

        Task SaveAsync();
        void Dispose();
    }
}

