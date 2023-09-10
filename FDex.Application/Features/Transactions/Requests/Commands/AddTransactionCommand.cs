using System;
using FDex.Application.DTOs.Transaction;
using FDex.Application.Responses.Transaction;
using MediatR;

namespace FDex.Application.Features.Transactions.Requests.Commands
{
	public class AddTransactionCommand : IRequest<AddTransactionCommandResponse>
	{
		public AddTransactionDTO AddTransactionDTO { get; set; }
    }
}

