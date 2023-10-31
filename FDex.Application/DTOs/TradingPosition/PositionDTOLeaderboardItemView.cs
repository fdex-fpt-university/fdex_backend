using System;
namespace FDex.Application.DTOs.TradingPosition
{
	public class PositionDTOLeaderboardItemView
	{
        public string Wallet { get; set; }
        public string IndexToken { get; set; }
        public bool Side { get; set; }
        public double Leverage { get; set; }
        public string Size { get; set; }
        public string ẺntyPrice { get; set; }
        public string PNL { get; set; }
    }
}

