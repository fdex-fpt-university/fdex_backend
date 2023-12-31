﻿using System;

namespace FDex.Domain.Entities
{
	public class User
	{
        public string Wallet { get; set; }
        public string? ReferredUserOf { get; set; }
        public DateTime? ReferredUserDate { get; set; }
        public decimal? TradePoint { get; set; }
        public decimal? ReferralPoint { get; set; }
        public int? Level { get; set; }
        public DateTime CreatedDate { get; set; }

        public ICollection<Swap>? Swaps { get; set; }
        public ICollection<Reward>? Rewards { get; set; }
        public ICollection<Liquidity>? Liquidities { get; set; }
        public ICollection<Position>? Positions { get; set; }
        public ICollection<User>? ReferredUsers { get; set; }
    }
}

