using System;
using System.Numerics;
using FDex.Application.Enumerations;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace FDex.Application.DTOs.TradingPosition
{
    [Event("FDexOpenPosition")]
    public class FDexOpenPositionDTOBase : IEventDTO
    {
        [Parameter("bytes32", "key", 1, true)]
        public virtual byte[] Key { get; set; }

        [Parameter("uint256", "size", 2, false)]
        public virtual BigInteger Size { get; set; }

        [Parameter("uint256", "collateralValue", 3, false)]
        public virtual BigInteger CollateralValue { get; set; }

        [Parameter("uint256", "entryPrice", 4, false)]
        public virtual BigInteger EntryPrice { get; set; }

        [Parameter("uint256", "entryInterestRate", 5, false)]
        public virtual BigInteger EntryInterestRate { get; set; }

        [Parameter("uint256", "reserveAmount", 6, false)]
        public virtual BigInteger ReserveAmount { get; set; }

        [Parameter("uint256", "indexPrice", 7, false)]
        public virtual BigInteger IndexPrice { get; set; }

        [Parameter("address", "wallet", 8, false)]
        public virtual string Wallet { get; set; }

        [Parameter("address", "collateralToken", 9, false)]
        public virtual string CollateralToken { get; set; }

        [Parameter("address", "indexToken", 10, false)]
        public virtual string IndexToken { get; set; }

        [Parameter("uint256", "sizeChanged", 11, false)]
        public virtual BigInteger SizeChanged { get; set; }

        [Parameter("uint8", "side", 12, false)]
        public virtual byte Side { get; set; }

        [Parameter("uint256", "feeValue", 13, false)]
        public virtual BigInteger FeeValue { get; set; }
    }
}

