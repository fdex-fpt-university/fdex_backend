using System;
using AutoMapper;
using FDex.Application.Contracts.Persistence;
using FDex.Application.Features.Users.Handlers.Commands;
using FDex.Application.Features.Users.Requests.Commands;
using FDex.Application.Responses.Common;
using FDex.Domain.Entities;
using FDex.XUnit.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Moq;
using Shouldly;

namespace FDex.XUnit.Users.Commands
{
	public class AddUserCommandHandlerTests
    {
        private readonly User _user;
        private AddUserCommandHandler _handler;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IServiceProvider> _mockServiceProvider;
        private IReadOnlyList<User> _users;

        public AddUserCommandHandlerTests()
        {
            _user = new User
            {
                Wallet = "User Wallet Test X"
            };
            _mockUserRepository = MockUserRepository.GetUserRepository();
            _mockUnitOfWork = MockUnitOfWork.GetUnitOfWork();
            _mockServiceProvider = ServiceProviderFactory.GetServiceProvider(
                (typeof(IUserRepository), _mockUserRepository.Object),
                (typeof(IUnitOfWork), _mockUnitOfWork.Object));
            _handler = new AddUserCommandHandler(_mockServiceProvider.Object);
        }

        [Fact]
        public async Task Valid_User_Added()
        {
            await using var scope = _mockServiceProvider.Object.CreateAsyncScope();
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            _users = await _unitOfWork.UserRepository.GetAllAsync();
            var userCount = _users.Count();
            var result = await _handler.Handle(new AddUserCommand() { Wallet = _user.Wallet }, CancellationToken.None);
            _users = await _unitOfWork.UserRepository.GetAllAsync();
            _unitOfWork.Dispose();
            _users.Count.ShouldBe(userCount + 1);
            result.ShouldBeOfType<BaseCommandResponse>();
        }

        [Fact]
        public async Task InValid_LeaveType_Added()
        {
            await using var scope = _mockServiceProvider.Object.CreateAsyncScope();
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            _user.Wallet = null;
            _users = await _unitOfWork.UserRepository.GetAllAsync();
            var userCount = _users.Count();
            var result = await _handler.Handle(new AddUserCommand() { Wallet = _user.Wallet }, CancellationToken.None);
            _users = await _unitOfWork.UserRepository.GetAllAsync();
            _unitOfWork.Dispose();
            _users.Count.ShouldBe(userCount);
            result.ShouldBeOfType<BaseCommandResponse>();

        }
    }
}

