using System;
using FDex.Application.Contracts.Persistence;
using FDex.Application.Features.Users.Requests.Commands;
using FDex.Domain.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FDex.Application.Features.Users.Handlers.Commands
{
    public class AddUserCommandHandler : IRequestHandler<AddUserCommand, bool>
    {
        private readonly IServiceProvider _serviceProvider;

        public AddUserCommandHandler(IServiceProvider serviceProvider)
		{
            _serviceProvider = serviceProvider;
        }

        public async Task<bool> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var user = await _unitOfWork.UserRepository.FindAsync(request.Wallet);
            if (user != null)
            {
                return false;
            }
            var newUser = new User()
            {
                Wallet = request.Wallet,
                CreatedDate = DateTime.Now
            };
            await _unitOfWork.UserRepository.AddAsync(newUser);
            await _unitOfWork.SaveAsync();
            _unitOfWork.Dispose();
            return true;
        }
    }
}

