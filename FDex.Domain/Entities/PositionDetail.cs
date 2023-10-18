using System;
using FDex.Domain.Enumerations;
using FDex.Domain.ValueObjects;

namespace FDex.Domain.Entities
{
	public class PositionDetail
	{
        public Guid PositionId { get; set; }
        public string CollateralValue { get; set; }
        public string IndexPrice { get; set; }
        public string EntryPrice { get; set; }
        public EventType EventType { get; set; }
        public string SizeChanged { get; set; }
        public string FeeValue { get; set; }
        public string? EntryInterestRate { get; set; }
        public int? Pnl { get; set; }
        public DateTime Time { get; set; }

        public Position Position { get; set; }

    }
}

