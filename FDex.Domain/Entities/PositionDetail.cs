using System;
namespace FDex.Domain.Entities
{
	public class PositionDetail
	{
        public Guid PositionId { get; set; }
        public string CollateralValue { get; set; }
        public string IndexPrice { get; set; }
        public int UpdateType { get; set; }
        public string SizeChanged { get; set; }
        public string FeeValue { get; set; }
        public DateTime Time { get; set; }

        public Position Position { get; set; }

    }
}

