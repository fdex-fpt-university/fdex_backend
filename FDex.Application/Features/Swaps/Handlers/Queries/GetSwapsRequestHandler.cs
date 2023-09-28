using System;
using AutoMapper;
using FDex.Application.Contracts.Persistence;
using FDex.Application.DTOs.Swap;
using FDex.Application.Features.Swaps.Requests.Queries;
using FDex.Domain.Entities;
using MediatR;

namespace FDex.Application.Features.Swaps.Handlers.Queries
{
	public class GetSwapsRequestHandler : IRequestHandler<GetSwapsRequest, List<SwapDTOView>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetSwapsRequestHandler(IUnitOfWork unitOfWork, IMapper mapper)
		{
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<SwapDTOView>> Handle(GetSwapsRequest request, CancellationToken cancellationToken)
        {
            var swaps = await _unitOfWork.SwapRepository.GetAllAsync();
            List<SwapDTOView> swapDTOs = _mapper.Map<List<SwapDTOView>>(swaps);
            return swapDTOs;
        }
    }
}

