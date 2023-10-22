using System;
using FDex.Application.Contracts.Persistence;
using FDex.Application.Features.Users.Requests.Queries;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FDex.Application.Features.Users.Handlers.Queries
{
	public class GetReferralSystemAnalyticsRequestHandler : IRequestHandler<GetReferralSystemAnalyticsRequest, object>
	{
		private readonly IServiceProvider _serviceProvider;

		public GetReferralSystemAnalyticsRequestHandler(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

        public async Task<object> Handle(GetReferralSystemAnalyticsRequest request, CancellationToken cancellationToken)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var analytic = await _unitOfWork.UserRepository.GetReferralAnalytics();
            _unitOfWork.Dispose();
            return analytic;
        }
    }
}

