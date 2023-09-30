﻿using System;
namespace FDex.Application.Contracts.Persistence
{
	public interface IUnitOfWork
	{
        ISwapRepository SwapRepository { get; }
        IUserRepository UserRepository { get; }

        Task Save();
    }
}

