using System;
using AutoMapper;
using FDex.Application.Contracts.Persistence;
using FDex.Application.DTOs.Swap;
using FDex.Application.DTOs.TradingPosition;
using FDex.Application.Features.Positions.Requests.Queries;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FDex.Application.Features.Positions.Handlers.Queries
{
    public class PositionOrdersRequestHandler : IRequestHandler<GetPositionOrdersRequest, List<PositionDTOViewOrder>>
	{
        private readonly IServiceProvider _serviceProvider;

        public PositionOrdersRequestHandler(IServiceProvider serviceProvider)
		{
            _serviceProvider = serviceProvider;
		}

        public async Task<List<PositionDTOViewOrder>> Handle(GetPositionOrdersRequest request, CancellationToken cancellationToken)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var _mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
            var positions = await _unitOfWork.PositionRepository.GetPositionOrdersInDetails(request.Wallet);
            _unitOfWork.Dispose();
            List<PositionDTOViewOrder> orderDTOs = _mapper.Map<List<PositionDTOViewOrder>>(positions);
            return orderDTOs;
        }
    }
}

