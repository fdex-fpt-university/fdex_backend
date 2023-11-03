using System;
using FDex.Application.Contracts.Persistence;
using Moq;

namespace FDex.XUnit.Mocks
{
	public static class MockServiceProvider
	{
        public static Mock<IServiceProvider> GetServiceProvider()
        {
            var mockSP = new Mock<IServiceProvider>();
            return mockSP;
        }
    }
}

