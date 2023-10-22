using System;
using FDex.Domain.Entities;

namespace FDex.Application.Contracts.Persistence
{
	public interface IUserRepository : IGenericRepository<User>
	{
        Task<object> GetDashboardItemDatas();
        Task<List<User>> GetReferredUsers(string wallet);
        Task<object> GetReferralAnalytics();
    }
}

