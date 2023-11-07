using System;
namespace FDex.Domain.Entities
{
	public class Reward
	{
		public Guid Id { get; set; }
		public string Wallet { get; set; }
		public string Amount { get; set; }
		public DateTime? ClaimedDate { get; set; }

		public User User { get; set; }
	}
}

