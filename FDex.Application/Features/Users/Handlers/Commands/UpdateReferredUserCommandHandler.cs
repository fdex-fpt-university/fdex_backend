﻿using System;
using AutoMapper;
using FDex.Application.Contracts.Persistence;
using FDex.Application.Features.Users.Requests.Commands;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FDex.Application.Features.Users.Handlers.Commands
{
	public class UpdateReferredUserCommandHandler : IRequestHandler<UpdateReferredUserCommand>
    {
        private readonly IServiceProvider _serviceProvider;

        public UpdateReferredUserCommandHandler(IServiceProvider serviceProvider)
		{
            _serviceProvider = serviceProvider;
		}

        public async Task Handle(UpdateReferredUserCommand request, CancellationToken cancellationToken)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var referringUser = await _unitOfWork.UserRepository.FindAsync(request.ReferringUser);
            referringUser.ReferredUserOf = request.ReferralUser;
            referringUser.Level = 0;
            _unitOfWork.UserRepository.Update(referringUser);
            _unitOfWork.Dispose();
        }
    }
}

