﻿using System;
using FDex.Domain.Entities;

namespace FDex.Application.Contracts.Persistence
{
	public interface IUserRepository : IGenericRepository<User>
	{
        Task<object> GetDashboardItemDatas();
        Task<Dictionary<int, List<User>>> GetReferredUsers(string wallet, int page, int pageSize);
        Task<UserLevelAnalytic> GetReferralAnalytics();
        Task<List<User>> GetUsersInDetailsAsync();
    }
}

