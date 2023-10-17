using System;
using AutoMapper;
using FDex.Application.Contracts.Persistence;
using FDex.Application.DTOs.Swap;
using FDex.Application.Features.DashboardItems.Requests.Queries;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FDex.Application.Features.DashboardItems.Handlers.Queries
{
    public class GetDashboardItemDataRequestHandler : IRequestHandler<GetDashboardItemDataRequest, object>
	{
        private readonly IServiceProvider _serviceProvider;

        public GetDashboardItemDataRequestHandler(IServiceProvider serviceProvider)
		{
            _serviceProvider = serviceProvider;
        }

        public async Task<object> Handle(GetDashboardItemDataRequest request, CancellationToken cancellationToken)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            object dashboardItemDatas = await _unitOfWork.UserRepository.GetDashboardItemDatas();
            _unitOfWork.Dispose();
            return dashboardItemDatas;
        }
    }
}

