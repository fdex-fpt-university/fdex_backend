using FDex.Domain.Enumerations;
using FDex.Domain.ValueObjects;
using System;
namespace FDex.Application.DTOs.TradingPosition
{
    public class PositionDTOViewHistory
    {
        public Guid Id { get; set; }
        public string Wallet { get; set; }
        public string CollateralToken { get; set; }
        public string IndexToken { get; set; }
        public string Size { get; set; }
        public bool Side { get; set; }
        public SignedInt Pnl { get; set; }
    }
}

