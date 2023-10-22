using System;
using System.ComponentModel.DataAnnotations;
using FDex.Application.DTOs.TradingPosition;
using MediatR;

namespace FDex.Application.Features.Positions.Requests.Queries
{
	public class GetPositionOrdersRequest : IRequest<List<PositionDTOViewOrder>>
	{
        [Required]
        public string Wallet { get; set; }
    }
}

