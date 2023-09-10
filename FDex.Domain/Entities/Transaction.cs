using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FDex.Domain.Entities
{
	public class Transaction
	{
        public Guid Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Address { get; set; }
        public string Receive { get; set; }
        public DateTime Time { get; set; }
        public bool Status { get; set; }
    }
}

