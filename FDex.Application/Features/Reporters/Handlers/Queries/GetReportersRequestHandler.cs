using System;
using AutoMapper;
using FDex.Application.Contracts.Persistence;
using FDex.Application.DTOs.Reporter;
using FDex.Application.DTOs.Swap;
using FDex.Application.Features.Reporters.Requests.Queries;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FDex.Application.Features.Reporters.Handlers.Queries
{
	public class GetReportersRequestHandler : IRequestHandler<GetReportersRequest, List<ReporterDTOView>>
    {
        private readonly IServiceProvider _serviceProvider;

        public GetReportersRequestHandler(IServiceProvider serviceProvider)
		{
            _serviceProvider = serviceProvider;
        }

        public async Task<List<ReporterDTOView>> Handle(GetReportersRequest request, CancellationToken cancellationToken)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var _mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
            var reporters = await _unitOfWork.ReporterRepository.GetAllAsync();
            _unitOfWork.Dispose();
            List<ReporterDTOView> reporterDTOs = _mapper.Map<List<ReporterDTOView>>(reporters);
            return reporterDTOs;
        }
    }
}

