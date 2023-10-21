using System;
using AutoMapper;
using FDex.Application.Contracts.Persistence;
using FDex.Application.DTOs.TradingPosition;
using FDex.Application.Features.Positions.Requests.Queries;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FDex.Application.Features.Positions.Handlers.Queries
{
    public class PositionsRequestHandler : IRequestHandler<GetPositionsRequest, List<PositionDTOView>>
    {
        private readonly IServiceProvider _serviceProvider;

        public PositionsRequestHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<List<PositionDTOView>> Handle(GetPositionsRequest request, CancellationToken cancellationToken)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var _mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
            var positions = await _unitOfWork.PositionRepository.GetPositionsInDetails(request.Wallet);
            _unitOfWork.Dispose();
            List<PositionDTOView> positionDTOs = _mapper.Map<List<PositionDTOView>>(positions);
            return positionDTOs;
        }
    }
}

