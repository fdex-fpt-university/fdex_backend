using System;

namespace FDex.Domain.Entities
{
	public class User
	{
        public string Wallet { get; set; }
        public string? ReferralCode { get; set; }
        public string? ReferredUserOf { get; set; }
        public DateTime CreatedDate { get; set; }

        public ICollection<Swap>? Swaps { get; set; }
        public ICollection<User>? ReferredUsers { get; set; }
    }
}

