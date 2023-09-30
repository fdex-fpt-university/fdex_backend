using System;
using AutoMapper;
using FDex.Application.Contracts.Persistence;
using FDex.Application.DTOs.Swap;
using FDex.Application.Features.DashboardItems.Requests.Queries;
using MediatR;

namespace FDex.Application.Features.DashboardItems.Handlers.Queries
{
    public class GetDashboardItemDataRequestHandler : IRequestHandler<GetDashboardItemDataRequest, object>
	{
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetDashboardItemDataRequestHandler(IUnitOfWork unitOfWork, IMapper mapper)
		{
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<object> Handle(GetDashboardItemDataRequest request, CancellationToken cancellationToken)
        {
            object dashboardItemDatas = await _unitOfWork.UserRepository.GetDashboardItemDatas();
            return dashboardItemDatas;
        }
    }
}

