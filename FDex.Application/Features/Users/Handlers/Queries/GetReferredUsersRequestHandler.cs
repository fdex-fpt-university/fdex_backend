using System;
using FDex.Application.Contracts.Persistence;
using FDex.Application.Features.Users.Requests.Queries;
using FDex.Domain.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FDex.Application.Features.Users.Handlers.Queries
{
    public class GetReferredUsersRequestHandler : IRequestHandler<GetReferredUsersRequest, Dictionary<int,List<object>>>
	{
        private readonly IServiceProvider _serviceProvider;

        public GetReferredUsersRequestHandler(IServiceProvider serviceProvider)
		{
            _serviceProvider = serviceProvider;
		}

        public async Task<Dictionary<int, List<object>>> Handle(GetReferredUsersRequest request, CancellationToken cancellationToken)
        {
            Dictionary<int, List<object>> response = new();
            await using var scope = _serviceProvider.CreateAsyncScope();
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            Dictionary<int, List<User>> usersWithNumberOfPage = await _unitOfWork.UserRepository.GetReferredUsers(request.Wallet, request.Page, request.PageSize);
            _unitOfWork.Dispose();
            response[usersWithNumberOfPage.First().Key] = new List<object>();
            var rawUsers = usersWithNumberOfPage.First().Value;
            if(rawUsers != null)
            {
                foreach(var user in rawUsers)
                {
                    object refUser = new
                    {
                        Wallet = user.Wallet,
                        ReferralPoint = user.TradePoint,
                        ReferredDate = DateTime.Now
                    };
                    response.First().Value.Add(refUser);
                }
            }
            return response;
        }
    }
}

