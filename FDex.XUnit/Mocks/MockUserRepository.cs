using System;
using FDex.Application.Contracts.Persistence;
using FDex.Domain.Entities;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDex.XUnit.Mocks
{
	public static class MockUserRepository
	{
        public static Mock<IUserRepository> GetUserRepository()
        {
            var users = new List<User>
            {
                new User
                {
                    Wallet = "User Wallet Test 1"
                },
                new User
                {
                    Wallet = "User Wallet Test 2"
                },
                new User
                {
                    Wallet = "User Wallet Test 3"
                },
            };

            var mockRepo = new Mock<IUserRepository>();

            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

            mockRepo.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync((User user) =>
            {
                users.Add(user);
                return user;
            });

            return mockRepo;

        }
    }
}

