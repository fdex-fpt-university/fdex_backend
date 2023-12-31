﻿using System;
using AutoMapper;
using FDex.Application.Contracts.Persistence;
using FDex.Application.DTOs.User;
using FDex.Application.Features.Users.Requests.Queries;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FDex.Application.Features.Users.Handlers.Queries
{
    public class GetLeaderboardRequestHandler : IRequestHandler<GetLeaderboardRequest, List<UserDTOLeaderboardItemView>>
	{
		private readonly IServiceProvider _serviceProvider;
		public GetLeaderboardRequestHandler(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

        public async Task<List<UserDTOLeaderboardItemView>> Handle(GetLeaderboardRequest request, CancellationToken cancellationToken)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var _mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
            var items = await _unitOfWork.UserRepository.GetLeaderboardItemsAsync(request.IsTradingVolumnAsc, request.IsAvgLeverageAsc, request.IsWinAsc, request.IsLossAsc, request.IsPNLwFeesAsc,request.TimeRange);
            _unitOfWork.Dispose();
            return items;
        }
    }
}

