using FDex.Domain.Enumerations;
using System;
namespace FDex.Application.DTOs.TradingPosition
{
    public class PositionDTOViewOrder
    {
        public string CollateralToken { get; set; }
        public string IndexToken { get; set; }
        public string Size { get; set; }
        public string EntryPrice { get; set; }
        public bool Side { get; set; }
    }
}

