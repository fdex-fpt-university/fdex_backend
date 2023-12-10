using System;
using FDex.Application.Contracts.Persistence;
using FDex.Application.Features.Users.Requests.Commands;
using FDex.Application.Responses.Users;
using FDex.Domain.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FDex.Application.Features.Users.Handlers.Commands
{
    public class AddUserCommandHandler : IRequestHandler<AddUserCommand, AddUserCommandResponseModel>
    {
        private readonly IServiceProvider _serviceProvider;

        public AddUserCommandHandler(IServiceProvider serviceProvider)
		{
            _serviceProvider = serviceProvider;
        }

        public async Task<AddUserCommandResponseModel> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            AddUserCommandResponseModel response = new() { Id = Guid.NewGuid() };
            if (string.IsNullOrEmpty(request.Wallet))
            {
                response.IsSuccess = false;
                response.Message = "Command Failed!";
                response.Errors = new()
                {
                    $"Your wallet: \"{request.Wallet}\" is empty!"
                };
            }
            else
            {
                await using var scope = _serviceProvider.CreateAsyncScope();
                var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var user = await _unitOfWork.UserRepository.FindAsync(request.Wallet);
                if (user != null)
                {
                    response.IsSuccess = false;
                    response.Message = "Command Failed!";
                    response.Errors = new()
                    {
                        $"Your wallet: \"{request.Wallet}\" is already exist!"
                    };
                }
                else
                {
                    var newUser = new User()
                    {
                        Wallet = request.Wallet,
                        CreatedDate = DateTime.Now
                    };
                    await _unitOfWork.UserRepository.AddAsync(newUser);
                    await _unitOfWork.SaveAsync();
                    _unitOfWork.Dispose();
                    response.IsSuccess = true;
                    response.Message = "Command Successed!";
                }
            }
            return response;
        }
    }
}

