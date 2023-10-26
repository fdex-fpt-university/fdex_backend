using System;
using AutoMapper;
using FDex.Application.Contracts.Persistence;
using FDex.Application.DTOs.User;
using FDex.Application.Features.Users.Requests.Queries;
using FDex.Domain.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FDex.Application.Features.Users.Handlers.Queries
{
    public class GetReferredUsersRequestHandler : IRequestHandler<GetReferredUsersRequest, ReferredUserQueryModel>
    {
        private readonly IServiceProvider _serviceProvider;

        public GetReferredUsersRequestHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<ReferredUserQueryModel> Handle(GetReferredUsersRequest request, CancellationToken cancellationToken)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var _mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

            var users = await _unitOfWork.UserRepository.GetReferredUsers(request.Wallet, request.Page, request.PageSize);

            int numberOfPage = 0;

            if (users.Count % request.PageSize == 0)
            {
                numberOfPage = users.Count / request.PageSize;
            }
            else
            {
                numberOfPage = users.Count / request.PageSize + 1;
            }

            var usersMapped = _mapper.Map<List<UserDto>>(users);

            return new ReferredUserQueryModel()
            {
                NumberOfPage = numberOfPage,
                Users = usersMapped
            };
        }
    }
}

