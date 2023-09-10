using System;
using AutoMapper;
using FDex.Application.Contracts.Persistence;
using FDex.Application.DTOs.Transaction;
using FDex.Application.Features.Transactions.Requests.Queries;
using FDex.Domain.Entities;
using MediatR;

namespace FDex.Application.Features.Transactions.Handlers.Queries
{
	public class GetTransactionsRequestHandler : IRequestHandler<GetTransactionsRequest, List<TransactionDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetTransactionsRequestHandler(IUnitOfWork unitOfWork, IMapper mapper)
		{
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<TransactionDTO>> Handle(GetTransactionsRequest request, CancellationToken cancellationToken)
        {
            var transactions = await _unitOfWork.TransactionRepository.GetAllAsync();
            return _mapper.Map<List<TransactionDTO>>(transactions);
        }
    }
}

