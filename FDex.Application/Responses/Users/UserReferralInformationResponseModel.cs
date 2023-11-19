using System;
using FDex.Application.Responses.Common;

namespace FDex.Application.Responses.Users
{
	public class UserReferralInformationResponseModel : BaseResponseModel
    {
        public decimal? TradePoint { get; set; }
        public decimal? ReferralPoint { get; set; }
        public int? Level { get; set; }
    }
}

