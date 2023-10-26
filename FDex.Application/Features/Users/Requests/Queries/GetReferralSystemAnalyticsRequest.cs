using System;
using System.ComponentModel.DataAnnotations;
using FDex.Domain.Entities;
using MediatR;

namespace FDex.Application.Features.Users.Requests.Queries
{
	public class GetReferralSystemAnalyticsRequest : IRequest<UserLevelAnalytic>
	{
	}
}

