using System;
namespace FDex.Application.DTOs.Transaction
{
	public class AddTransactionDTO
	{
        public string From { get; set; }
        public string To { get; set; }
        public string Address { get; set; }
        public string Receive { get; set; }
        public DateTime Time { get; set; }
        public bool Status { get; set; }
    }
}

