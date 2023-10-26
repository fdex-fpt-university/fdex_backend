using System;
using System.ComponentModel.DataAnnotations;
using FDex.Application.DTOs.User;
using MediatR;

namespace FDex.Application.Features.Users.Requests.Queries
{
	public class GetReferredUsersRequest : IRequest<ReferredUserQueryModel>
	{
		[Required]
		public string Wallet { get; set; }
		[Required]
		public int Page { get; set; }
		[Required]
		public int PageSize { get; set; }
	}
}

