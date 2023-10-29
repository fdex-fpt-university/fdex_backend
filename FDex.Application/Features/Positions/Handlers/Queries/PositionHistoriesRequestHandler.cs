using System;
using AutoMapper;
using FDex.Application.Contracts.Persistence;
using FDex.Application.DTOs.TradingPosition;
using FDex.Application.Features.Positions.Requests.Queries;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FDex.Application.Features.Positions.Handlers.Queries
{
    public class PositionHistoriesRequestHandler : IRequestHandler<GetPositionHistoriesRequest, List<PositionDTOViewHistory>>
    {
        private readonly IServiceProvider _serviceProvider;

        public PositionHistoriesRequestHandler(IServiceProvider serviceProvider)
		{
            _serviceProvider = serviceProvider;
		}

        public async Task<List<PositionDTOViewHistory>> Handle(GetPositionHistoriesRequest request, CancellationToken cancellationToken)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var histories = await _unitOfWork.PositionRepository.GetPositionHistoriesInDetails(request.Wallet);
            _unitOfWork.Dispose();
            return histories;
        }
    }
}

