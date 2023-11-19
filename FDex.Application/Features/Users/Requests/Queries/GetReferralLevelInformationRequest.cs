using System;
using FDex.Application.Responses.User;
using MediatR;

namespace FDex.Application.Features.Users.Requests.Queries
{
	public class GetReferralLevelInformationRequest : IRequest<UserReferralInformationResponseModel>
	{
		public string Wallet { get; set; }
	}
}

