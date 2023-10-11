using System;
using System.Drawing;
using System.Numerics;
using FDex.Application.Enumerations;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace FDex.Application.DTOs.TradingPosition
{
    [Event("IncreasePosition")]
    public class IncreasePositionDTOBase : IEventDTO
	{
        [Parameter("bytes32", "key", 1, true)]
        public string Key { get; set; }

        [Parameter("address", "wallet", 2, false)]
        public string Wallet { get; set; }

        [Parameter("address", "collateralToken", 3, false)]
        public string CollateralToken { get; set; }

        [Parameter("address", "indexToken", 4, false)]
        public string IndexToken { get; set; }

        [Parameter("uint256", "collateralValue", 5, false)]
        public BigInteger CollateralValue { get; set; }

        [Parameter("uint256", "sizeChanged", 6, false)]
        public BigInteger SizeChanged { get; set; }

        [Parameter("Side", "side", 7, false)]
        public Side Side { get; set; }

        [Parameter("uint256", "indexPrice", 8, false)]
        public BigInteger IndexPrice { get; set; }

        [Parameter("uint256", "feeValue", 9, false)]
        public BigInteger FeeValue { get; set; }
    }
}

