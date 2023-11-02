using System;
using System.Numerics;

namespace FDex.Domain.Entities
{
	public class Liquidity
	{
        public string TxnHash { get; set; }
        public string Wallet { get; set; }
        public string Asset { get; set; }
        public string Amount { get; set; }
        public string Fee { get; set; }
        public string MarkPriceIn { get; set; }
        public DateTime DateAdded { get; set; }

        public User User { get; set; }
    }
}

