using System;
using System.Numerics;

namespace FDex.Application.DTOs.User
{
	public class UserDTOLeaderboardItemView
	{
		public string Wallet { get; set; }
		public string TradingVol { get; set; }
		public double AvgLeverage { get; set; }
		public int Win { get; set; }
		public int Loss { get; set; }
		public string PNLwFees { get; set; }
	}
}

