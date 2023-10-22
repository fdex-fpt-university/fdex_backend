using System;
using MediatR;

namespace FDex.Application.Features.Users.Requests.Queries
{
	public class GetReferredUsersRequest : IRequest<List<object>>
	{
		public string Wallet { get; set; }
	}
}

