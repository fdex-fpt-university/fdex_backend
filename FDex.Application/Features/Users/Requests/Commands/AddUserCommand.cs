using System;
using FDex.Application.Responses.Users;
using MediatR;

namespace FDex.Application.Features.Users.Requests.Commands
{
	public class AddUserCommand : IRequest<AddUserCommandResponseModel>
    {
        public string Wallet { get; set; }
    }
}

