using System;
using FDex.Domain.Enumerations;
using FDex.Domain.ValueObjects;

namespace FDex.Domain.Entities
{
	public class PositionDetail
	{
        public Guid Id { get; set; }
        public Guid PositionId { get; set; }
        public string CollateralValue { get; set; }
        public string? EntryPrice { get; set; }
        public string? IndexPrice { get; set; }
        public string SizeChanged { get; set; }
        public string? ReserveAmount { get; set; }
        public string? FeeValue { get; set; }
        public string? EntryInterestRate { get; set; }
        public string? Pnl { get; set; }
        public PositionState PositionState { get; set; }
        public DateTime Time { get; set; }

        public Position Position { get; set; }

    }
}

