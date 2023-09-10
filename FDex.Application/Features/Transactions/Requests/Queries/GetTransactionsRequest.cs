using System;
using FDex.Application.DTOs.Transaction;
using MediatR;

namespace FDex.Application.Features.Transactions.Requests.Queries
{
	public class GetTransactionsRequest : IRequest<List<TransactionDTO>>
	{
	}
}

