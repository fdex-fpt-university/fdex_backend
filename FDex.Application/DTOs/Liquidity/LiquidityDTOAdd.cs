using System;
namespace FDex.Application.DTOs.Liquidity
{
	public partial class LiquidityDTOAdd : LiquidityDTOBase
	{
        public string TxnHash { get; set; }
        public DateTime DateAdded { get; set; }
    }
}

