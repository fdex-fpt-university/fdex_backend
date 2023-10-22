using System;
using FDex.Application.Contracts.Persistence;
using FDex.Application.Features.Users.Requests.Queries;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FDex.Application.Features.Users.Handlers.Queries
{
    public class GetReferredUsersRequestHandler : IRequestHandler<GetReferredUsersRequest, List<object>>
	{
        private readonly IServiceProvider _serviceProvider;

        public GetReferredUsersRequestHandler(IServiceProvider serviceProvider)
		{
            _serviceProvider = serviceProvider;
		}

        public async Task<List<object>> Handle(GetReferredUsersRequest request, CancellationToken cancellationToken)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var users = await _unitOfWork.UserRepository.GetReferredUsers(request.Wallet);
            _unitOfWork.Dispose();
            List<object> response = new();
            if(users != null)
            {
                foreach(var user in users)
                {
                    object refUser = new
                    {
                        Wallet = user.Wallet,
                        ReferralPoint = user.TradePoint
                    };
                    response.Add(refUser);
                }
            }
            return response;
        }
    }
}

