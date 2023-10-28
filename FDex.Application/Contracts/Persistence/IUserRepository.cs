using System;
using FDex.Application.Common.Models;
using FDex.Application.DTOs.User;
using FDex.Domain.Entities;

namespace FDex.Application.Contracts.Persistence
{
	public interface IUserRepository : IGenericRepository<User>
	{
        Task<object> GetDashboardItemDatas();
        Task<GetUserResponse> GetReferredUsers(string wallet, int page, int pageSize);
        Task<object> GetReferralAnalytics();
        Task<List<User>> GetUsersInDetailsAsync();
        Task<List<UserDTOLeaderboardItemView>> GetLeaderboardItemsAsync(bool? isTradingVolumnAsc, bool? isAvgLeverageAsc, bool? isWinAsc, bool? isLossAsc, bool? isPNLwFeesAsc, int timeRange);
    }
}

