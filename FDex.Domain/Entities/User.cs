using System;
namespace FDex.Domain.Entities
{
	public class User
	{
        public string Wallet { get; set; }
        public string? ReferalCode { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

