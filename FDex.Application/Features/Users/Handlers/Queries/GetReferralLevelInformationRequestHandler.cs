using System;
using FDex.Application.Contracts.Persistence;
using FDex.Application.Features.Users.Requests.Queries;
using FDex.Application.Responses.User;
using FDex.Domain.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FDex.Application.Features.Users.Handlers.Queries
{
    public class GetReferralLevelInformationRequestHandler : IRequestHandler<GetReferralLevelInformationRequest, UserReferralInformationResponseModel>
	{
		private readonly IServiceProvider _serviceProvider;
		public GetReferralLevelInformationRequestHandler(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

        public async Task<UserReferralInformationResponseModel> Handle(GetReferralLevelInformationRequest request, CancellationToken cancellationToken)
        {
            UserReferralInformationResponseModel response = new() { Id = Guid.NewGuid()};
            await using var scope = _serviceProvider.CreateAsyncScope();
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            User user = await _unitOfWork.UserRepository.FindAsync(request.Wallet);
            _unitOfWork.Dispose();
            if (user != null)
            {
                response.IsSuccess = true;
                response.Message = "Request Successed!";
                response.TradePoint = user.TradePoint;
                response.ReferralPoint = user.ReferralPoint;
                response.Level = user.Level;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = "Request Failed!";
                response.Errors = new()
                {
                    $"Coudn't found user {request.Wallet}"
                };
            }
            return response;
        }
    }
}

