using System;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace FDex.Application.Features.Users.Requests.Queries
{
	public class GetReferredUsersRequest : IRequest<Dictionary<int,List<object>>>
	{
		[Required]
		public string Wallet { get; set; }
		[Required]
		public int Page { get; set; }
		[Required]
		public int PageSize { get; set; }
	}
}

