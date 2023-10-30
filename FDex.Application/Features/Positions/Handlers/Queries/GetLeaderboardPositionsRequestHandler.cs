using System;
using FDex.Application.Contracts.Persistence;
using FDex.Application.DTOs.TradingPosition;
using FDex.Application.Features.Positions.Requests.Queries;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FDex.Application.Features.Positions.Handlers.Queries
{
    public class GetLeaderboardPositionsRequestHandler : IRequestHandler<GetLeaderboardPositionsRequest, List<PositionDTOLeaderboardItemView>>
	{
		private readonly IServiceProvider _serviceProvider;
		public GetLeaderboardPositionsRequestHandler(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

        public async Task<List<PositionDTOLeaderboardItemView>> Handle(GetLeaderboardPositionsRequest request, CancellationToken cancellationToken)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var positions = await _unitOfWork.PositionRepository.GetLeaderboardPositionsAsync(request.IsLeverageAsc, request.IsSizeAsc, request.IsPNLAsc);
            _unitOfWork.Dispose();
            return positions;
        }
    }
}

