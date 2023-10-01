using System;
namespace FDex.Domain.Entities
{
	public class Reporter
	{
        public string Wallet { get; set; }
        public int ReportCount { get; set; }
        public DateTime LastReportedDate { get; set; }
    }
}

