using System;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace FDex.XUnit.Mocks
{
    public static class ServiceProviderFactory
    {
        public static Mock<IServiceProvider> GetServiceProvider(params (Type @interface, object service)[] services)
        {
            var scopedServiceProviderMock = new Mock<IServiceProvider>();
            foreach (var (@interface, service) in services)
                scopedServiceProviderMock.Setup(s => s.GetService(@interface))
                    .Returns(service);

            var serviceScopeMock = new Mock<IServiceScope>();
            serviceScopeMock.SetupGet(x => x.ServiceProvider)
                .Returns(scopedServiceProviderMock.Object);

            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            serviceScopeFactoryMock.Setup(x => x.CreateScope())
                .Returns(serviceScopeMock.Object);

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(s => s.GetService(typeof(IServiceScopeFactory)))
                .Returns(serviceScopeFactoryMock.Object);

            foreach (var (@interface, service) in services)
            {
                serviceProviderMock.Setup(s => s.GetService(@interface))
                    .Returns(service);

                serviceProviderMock.As<ISupportRequiredService>()
                   .Setup(m => m.GetRequiredService(@interface))
                   .Returns(service);
            }
            return serviceProviderMock;
        }
    }
}

