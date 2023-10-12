using System;
using System.Numerics;
using FDex.Application.Enumerations;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace FDex.Application.DTOs.TradingPosition
{
	[Event("LiquidatePosition")]
	public class LiquidatePositionDTOBase : IEventDTO
	{
        [Parameter("bytes32", "key", 1, true)]
        public virtual string Key { get; set; }

        [Parameter("address", "account", 2, false)]
        public virtual string Account { get; set; }

        [Parameter("address", "collateralToken", 3, false)]
        public virtual string CollateralToken { get; set; }

        [Parameter("address", "indexToken", 4, false)]
        public virtual string IndexToken { get; set; }

        [Parameter("Side", "side", 5, false)]
        public virtual Side Side { get; set; }

        [Parameter("uint256", "size", 6, false)]
        public virtual BigInteger Size { get; set; }

        [Parameter("uint256", "collateralValue", 7, false)]
        public virtual BigInteger CollateralValue { get; set; }

        [Parameter("uint256", "reserveAmount", 8, false)]
        public virtual BigInteger ReserveAmount { get; set; }

        [Parameter("uint256", "indexPrice", 9, false)]
        public virtual BigInteger IndexPrice { get; set; }

        [Parameter("SignedInt", "pnl", 10, false)]
        public virtual int Pnl { get; set; }

        [Parameter("uint256", "feeValue", 11, false)]
        public virtual BigInteger FeeValue { get; set; }

    }
}

