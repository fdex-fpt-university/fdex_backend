using System;
using MediatR;

namespace FDex.Application.Features.Users.Requests.Queries
{
	public class GetReferralLevelInformationRequest : IRequest<object>
	{
		public string Wallet { get; set; }
	}
}

