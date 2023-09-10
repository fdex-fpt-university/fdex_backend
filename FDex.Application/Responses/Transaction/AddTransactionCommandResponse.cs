using System;
using FDex.Application.DTOs.Transaction;
using FDex.Application.Responses.Common;

namespace FDex.Application.Responses.Transaction
{
	public class AddTransactionCommandResponse : BaseCommandResponse
    {
		public AddTransactionDTO AddTransactionDTO { get; set; }
	}
}

