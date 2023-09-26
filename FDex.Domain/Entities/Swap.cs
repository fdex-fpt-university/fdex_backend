using System;
using System.Numerics;

namespace FDex.Domain.Entities
{
	public class Swap
	{
        public Guid Id { get; set; }
        public string Sender { get; set; }
        public string TokenIn { get; set; }
        public string TokenOut { get; set; }
        public BigInteger AmountIn { get; set; }
        public BigInteger AmountOut { get; set; }
        public BigInteger Fee { get; set; }
        public DateTime Time { get; set; }
    }
}

