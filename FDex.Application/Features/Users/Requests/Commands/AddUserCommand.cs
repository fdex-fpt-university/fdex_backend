using System;
using MediatR;

namespace FDex.Application.Features.Users.Requests.Commands
{
	public class AddUserCommand : IRequest<bool>
    {
        public string Wallet { get; set; }
    }
}

