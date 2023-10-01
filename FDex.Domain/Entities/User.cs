using System;
namespace FDex.Domain.Entities
{
	public class User
	{
        public string Wallet { get; set; }
        public string? ReferalToken { get; set; }
        public uint? ReferalCode { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

