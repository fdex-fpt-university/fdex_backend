﻿using System;
using FDex.Application.DTOs.Swap;
using MediatR;

namespace FDex.Application.Features.Swaps.Requests.Queries
{
	public class GetSwapsRequest : IRequest<List<SwapDTOView>>
	{
		public string Wallet { get; set; }
		public int Page { get; set; }
		public int PageSize { get; set; }
	}
}