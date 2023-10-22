using System;
using System.ComponentModel.DataAnnotations;
using FDex.Application.DTOs.TradingPosition;
using MediatR;

namespace FDex.Application.Features.Positions.Requests.Queries
{
	public class GetPositionsRequest : IRequest<List<PositionDTOView>>
    {
        [Required]
        public string Wallet { get; set; }
    }
}

