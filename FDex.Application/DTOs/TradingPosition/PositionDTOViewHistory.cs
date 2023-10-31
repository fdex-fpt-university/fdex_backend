using FDex.Domain.Enumerations;
using FDex.Domain.ValueObjects;
using System;
namespace FDex.Application.DTOs.TradingPosition
{
    public class PositionDTOViewHistory
    {
        public string CollateralToken { get; set; }
        public string IndexToken { get; set; }
        public string Size { get; set; }
        public string EntryPrice { get; set; }
        public bool Side { get; set; }
        public string Pnl { get; set; }
        public DateTime Time { get; set; }
    }
}

