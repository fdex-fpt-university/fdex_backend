using System;
using FDex.Application.Contracts.Persistence;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDex.XUnit.Mocks
{
	public static class MockUnitOfWork
	{
        public static Mock<IUnitOfWork> GetUnitOfWork()
        {
            var mockUow = new Mock<IUnitOfWork>();
            var mockUserRepo = MockUserRepository.GetUserRepository();
            mockUow.Setup(r => r.UserRepository).Returns(mockUserRepo.Object);
            return mockUow;
        }
    }
}

