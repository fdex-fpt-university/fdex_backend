﻿using FDex.Domain.Entities;
using FDex.Domain.Enumerations;
using FDex.Domain.ValueObjects;
using System;
namespace FDex.Application.DTOs.TradingPosition
{
	public class PositionDTOView
	{
        public string CollateralToken { get; set; }
        public string IndexToken { get; set; }
        public string Size { get; set; }
        public string EntryPrice { get; set; }
        public bool Side { get; set; }
    }
}

