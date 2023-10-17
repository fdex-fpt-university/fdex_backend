using System;
using AutoMapper;
using FDex.Application.Contracts.Persistence;
using FDex.Application.DTOs.Swap;
using FDex.Application.Features.Swaps.Requests.Queries;
using FDex.Domain.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FDex.Application.Features.Swaps.Handlers.Queries
{
	public class GetSwapsRequestHandler : IRequestHandler<GetSwapsRequest, List<SwapDTOView>>
    {
        private readonly IServiceProvider _serviceProvider;

        public GetSwapsRequestHandler(IServiceProvider serviceProvider)
		{
            _serviceProvider = serviceProvider;
        }

        public async Task<List<SwapDTOView>> Handle(GetSwapsRequest request, CancellationToken cancellationToken)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var _mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
            var swaps = await _unitOfWork.SwapRepository.GetSwapsByCondition(request.Wallet, request.Page, request.PageSize);
            _unitOfWork.Dispose();
            List<SwapDTOView> swapDTOs = _mapper.Map<List<SwapDTOView>>(swaps);
            return swapDTOs;
        }
    }
}

