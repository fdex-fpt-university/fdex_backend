using System;
using AutoMapper;
using FDex.Application.Contracts.Persistence;
using FDex.Application.Features.Users.Requests.Queries;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FDex.Application.Features.Users.Handlers.Queries
{
	public class GetAccountStatusRequestHandler : IRequestHandler<GetAccountStatusRequest, bool>
	{
        private readonly IServiceProvider _serviceProvider;

        public GetAccountStatusRequestHandler(IServiceProvider serviceProvider)
		{
            _serviceProvider = serviceProvider;
		}

        public async Task<bool> Handle(GetAccountStatusRequest request, CancellationToken cancellationToken)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var account = await _unitOfWork.UserRepository.FindAsync(request.Wallet);
            _unitOfWork.Dispose();
            return !String.IsNullOrEmpty(account.ReferredUserOf);
        }
    }
}

