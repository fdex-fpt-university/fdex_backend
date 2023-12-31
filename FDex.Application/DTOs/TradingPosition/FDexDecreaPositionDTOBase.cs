﻿using System;
using System.Numerics;
using FDex.Application.Enumerations;
using FDex.Application.Models.Application;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace FDex.Application.DTOs.TradingPosition
{
    [Event("FDexDecreaPosition")]
    public class FDexDecreaPositionDTOBase : IEventDTO
    {
        [Parameter("bytes32", "key", 1, true)]
        public virtual byte[] Key { get; set; }

        [Parameter("address", "wallet", 2, false)]
        public virtual string Wallet { get; set; }

        [Parameter("address", "collateralToken", 3, false)]
        public virtual string CollateralToken { get; set; }

        [Parameter("address", "indexToken", 4, false)]
        public virtual string IndexToken { get; set; }

        [Parameter("uint256", "collateralChanged", 5, false)]
        public virtual BigInteger CollateralChanged { get; set; }

        [Parameter("uint256", "sizeChanged", 6, false)]
        public virtual BigInteger SizeChanged { get; set; }

        [Parameter("uint8", "side", 7, false)]
        public virtual byte Side { get; set; }

        [Parameter("uint256", "indexPrice", 8, false)]
        public virtual BigInteger IndexPrice { get; set; }

        [Parameter("tuple", "pnl", 9, false)]
        public virtual SignedInt Pnl { get; set; }

        [Parameter("uint256", "feeValue", 10, false)]
        public virtual BigInteger FeeValue { get; set; }

        [Parameter("uint256", "size", 11, false)]
        public virtual BigInteger Size { get; set; }

        [Parameter("uint256", "collateralValue", 12, false)]
        public virtual BigInteger CollateralValue { get; set; }

        [Parameter("uint256", "entryPrice", 13, false)]
        public virtual BigInteger EntryPrice { get; set; }

        [Parameter("uint256", "entryInterestRate", 14, false)]
        public virtual BigInteger EntryInterestRate { get; set; }

        [Parameter("uint256", "reserveAmount", 15, false)]
        public virtual BigInteger ReserveAmount { get; set; }
    }
}

