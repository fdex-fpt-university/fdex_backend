using System;
using FDex.Application.Contracts.Persistence;
using FDex.Application.Features.Users.Requests.Queries;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FDex.Application.Features.Users.Handlers.Queries
{
    public class GetReferralLevelInformationRequestHandler : IRequestHandler<GetReferralLevelInformationRequest, object>
	{
		private readonly IServiceProvider _serviceProvider;
		public GetReferralLevelInformationRequestHandler(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

        public async Task<object> Handle(GetReferralLevelInformationRequest request, CancellationToken cancellationToken)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var user = await _unitOfWork.UserRepository.FindAsync(request.Wallet);
            _unitOfWork.Dispose();
            object response = new
            {
                TradePoint = user.TradePoint,
                ReferralPoint = user.ReferralPoint,
                Level = user.Level
            };
            return response;
        }
    }
}

