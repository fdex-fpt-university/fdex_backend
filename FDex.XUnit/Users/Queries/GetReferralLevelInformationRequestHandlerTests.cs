using System;
using FDex.Application.Contracts.Persistence;
using FDex.Application.Features.Users.Handlers.Queries;
using FDex.XUnit.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace FDex.XUnit.Users.Queries
{
	public class GetReferralLevelInformationRequestHandlerTests
    {
        private GetReferralLevelInformationRequestHandler _handler;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly IServiceProvider _serviceProvider;

        public GetReferralLevelInformationRequestHandlerTests()
		{
            _mockUserRepository = MockUserRepository.GetUserRepository();
            _mockUnitOfWork = MockUnitOfWork.GetUnitOfWork();
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped(sp => _mockUnitOfWork.Object);
            serviceCollection.AddScoped(sp => _mockUserRepository.Object);
            _serviceProvider = serviceCollection.BuildServiceProvider();
            _handler = new GetReferralLevelInformationRequestHandler(_serviceProvider);
        }

        [Fact]
        public async Task Correct_Information()
		{
            await using var scope = _serviceProvider.CreateAsyncScope();
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            _unitOfWork.Dispose();
        }

        [Fact]
        public async Task Incorrect_Information()
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            _unitOfWork.Dispose();
        }
    }
}

