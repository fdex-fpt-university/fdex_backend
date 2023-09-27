using System;
using AutoMapper;
using FDex.Application.Contracts.Persistence;
using FDex.Application.DTOs.Swap;
using FDex.Application.Features.Swaps.Requests.Queries;
using FDex.Domain.Entities;
using MediatR;

namespace FDex.Application.Features.Swaps.Handlers.Queries
{
	public class GetSwapsRequestHandler : IRequestHandler<GetSwapsRequest, List<SwapDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetSwapsRequestHandler(IUnitOfWork unitOfWork, IMapper mapper)
		{
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<SwapDTO>> Handle(GetSwapsRequest request, CancellationToken cancellationToken)
        {
            var swaps = await _unitOfWork.SwapRepository.GetAllAsync();
            return _mapper.Map<List<SwapDTO>>(swaps);
        }
    }
}

