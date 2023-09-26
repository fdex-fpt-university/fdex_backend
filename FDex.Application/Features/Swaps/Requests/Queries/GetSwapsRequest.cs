using System;
using FDex.Application.DTOs.Swap;
using MediatR;

namespace FDex.Application.Features.Swaps.Requests.Queries
{
	public class GetSwapsRequest : IRequest<List<SwapDTO>>
	{
	}
}