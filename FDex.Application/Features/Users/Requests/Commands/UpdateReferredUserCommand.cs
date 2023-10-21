using System;
using MediatR;

namespace FDex.Application.Features.Users.Requests.Commands
{
	public class UpdateReferredUserCommand : IRequest<bool>
	{
		public string ReferringUser { get; set; }
        public string ReferralUser { get; set; }

    }
}

