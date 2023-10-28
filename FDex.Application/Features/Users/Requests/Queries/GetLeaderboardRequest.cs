using System;
using System.ComponentModel.DataAnnotations;
using FDex.Application.DTOs.User;
using MediatR;

namespace FDex.Application.Features.Users.Requests.Queries
{
	public class GetLeaderboardRequest : IRequest<List<UserDTOLeaderboardItemView>>
	{
        public bool? IsTradingVolumnAsc { get; set; }
        public bool? IsAvgLeverageAsc { get; set; }
        public bool? IsWinAsc { get; set; }
        public bool? IsLossAsc { get; set; }
		public bool? IsPNLwFeesAsc { get; set; }
        [Required]
        public int TimeRange { get; set; }
    }
}

