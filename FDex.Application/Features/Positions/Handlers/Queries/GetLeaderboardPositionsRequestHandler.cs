using System;
using FDex.Application.Contracts.Persistence;
using FDex.Application.DTOs.TradingPosition;
using FDex.Application.Features.Positions.Requests.Queries;
using FDex.Application.Responses.Positions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FDex.Application.Features.Positions.Handlers.Queries
{
    public class GetLeaderboardPositionsRequestHandler : IRequestHandler<GetLeaderboardPositionsRequest, PositionLeaderboardResponseModel>
	{
		private readonly IServiceProvider _serviceProvider;
		public GetLeaderboardPositionsRequestHandler(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

        public async Task<PositionLeaderboardResponseModel> Handle(GetLeaderboardPositionsRequest request, CancellationToken cancellationToken)
        {
            PositionLeaderboardResponseModel response = new() { Id = Guid.NewGuid() };
            await using var scope = _serviceProvider.CreateAsyncScope();
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var positions = await _unitOfWork.PositionRepository.GetLeaderboardPositionsAsync(request.IsLeverageAsc, request.IsSizeAsc, request.IsPNLAsc);
            _unitOfWork.Dispose();
            if(positions != null)
            {
                response.IsSuccess = true;
                response.Message = "Request Sucessful!";
                response.Positions = positions;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = "Request Failed!";
                response.Errors = new()
                {
                    $"Coudn't found any positions"
                };
            }
            return response;
        }
    }
}

