using FDex.Domain.Enumerations;
using System;
namespace FDex.Application.DTOs.TradingPosition
{
    public class PositionDTOViewOrder
    {
        public Guid Id { get; set; }
        public string Wallet { get; set; }
        public string Key { get; set; }
        public string CollateralToken { get; set; }
        public string IndexToken { get; set; }
        public string? Size { get; set; }
        public bool Side { get; set; }
        public ICollection<PositionDetailViewOrderDto>? PositionDetails { get; set; }
    }

    public class PositionDetailViewOrderDto
    {
        public Guid Id { get; set; }
        public Guid PositionId { get; set; }
        public string CollateralValue { get; set; }
        public string? IndexPrice { get; set; }
        public string? EntryPrice { get; set; }
        public PositionState PositionState { get; set; }
        public string SizeChanged { get; set; }
        public string? FeeValue { get; set; }
        public string? EntryInterestRate { get; set; }
        public string? Pnl { get; set; }
        public DateTime Time { get; set; }
    }
}

