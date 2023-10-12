using System;
using System.Drawing;
using System.Numerics;

namespace FDex.Domain.Entities
{
	public class Position
	{
		public string Key { get; set; }
		public string Wallet { get; set; }
		public string CollateralToken { get; set; }
		public string IndexToken { get; set; }
        public BigInteger CollateralValue { get; set; }
        public BigInteger SizeChanged { get; set; }
        public string Side { get; set; }
        public BigInteger IndexPrice { get; set; }
        public BigInteger FeeValue { get; set; }
        public DateTime OpenTime { get; set; }
        public DateTime CloseTime { get; set; }
    }
}

