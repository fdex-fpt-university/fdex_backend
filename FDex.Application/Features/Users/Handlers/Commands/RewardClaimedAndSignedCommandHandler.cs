using System;
using FDex.Application.Contracts.Persistence;
using FDex.Application.Features.Users.Requests.Commands;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FDex.Application.Features.Users.Handlers.Commands
{
    public class RewardClaimedAndSignedCommandHandler : IRequestHandler<RewardClaimedAndSignedCommand, bool>
	{
        private readonly IServiceProvider _serviceProvider;

        public RewardClaimedAndSignedCommandHandler(IServiceProvider serviceProvider)
		{
            _serviceProvider = serviceProvider;
		}

        public async Task<bool> Handle(RewardClaimedAndSignedCommand request, CancellationToken cancellationToken)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            
            _unitOfWork.Dispose();
            return true;
        }
    }
}

