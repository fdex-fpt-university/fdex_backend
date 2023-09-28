using System;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace FDex.Application.DTOs.Swap
{
	public class SwapDTOView
	{
        public string TokenIn { get; set; }
        public string TokenOut { get; set; }
        public string AmountIn { get; set; }
        public string AmountOut { get; set; }
        public DateTime Time { get; set; }
    }
}

