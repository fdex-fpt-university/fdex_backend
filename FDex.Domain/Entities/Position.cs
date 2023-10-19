using System;
using System.Drawing;
using System.Numerics;

namespace FDex.Domain.Entities
{
	public class Position
	{
        public Guid Id { get; set; }
		public string Wallet { get; set; }
		public string Key { get; set; }
		public string CollateralToken { get; set; }
		public string IndexToken { get; set; }
		public string? Size { get; set; }
        public bool Side { get; set; }

        public User User { get; set; }
        public ICollection<PositionDetail>? PositionDetails { get; set; }
    }
}

