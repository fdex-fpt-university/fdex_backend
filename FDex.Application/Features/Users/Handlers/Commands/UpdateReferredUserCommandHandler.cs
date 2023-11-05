using System;
using AutoMapper;
using FDex.Application.Contracts.Persistence;
using FDex.Application.Features.Users.Requests.Commands;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FDex.Application.Features.Users.Handlers.Commands
{
    public class UpdateReferredUserCommandHandler : IRequestHandler<UpdateReferredUserCommand, bool>
    {
        private readonly IServiceProvider _serviceProvider;

        public UpdateReferredUserCommandHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<bool> Handle(UpdateReferredUserCommand request, CancellationToken cancellationToken)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var referringUser = await _unitOfWork.UserRepository.FindAsync(request.ReferringUser);
            var referralUser = await _unitOfWork.UserRepository.FindAsync(request.ReferralUser);
            if (referralUser == null || referringUser == null)
            {
                return false;
            }
            // Update referral user
            referringUser.ReferredUserOf = request.ReferralUser;
            referringUser.ReferredUserDate = DateTime.Now;
            _unitOfWork.UserRepository.Update(referringUser);
            await _unitOfWork.SaveAsync();
            _unitOfWork.Dispose();
            return true;
        }
    }
}

