using System;
using EntityFrameworkCore.Triggered;
using FDex.Application.Contracts.Persistence;
using FDex.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace FDex.Persistence.Triggers
{
	public class UserLevelTrigger : IAfterSaveAsyncTrigger<User>
    {
        private readonly IServiceProvider _serviceProvider;
		public UserLevelTrigger(IServiceProvider serviceProvider)
		{
            _serviceProvider = serviceProvider;
		}

        public async Task AfterSaveAsync(ITriggerContext<User> context, CancellationToken cancellationToken)
        {
            if (context.Entity.TradePoint.HasValue && context.Entity.ReferralPoint.HasValue)
            {
                await using var scope = _serviceProvider.CreateAsyncScope();
                var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                if (context.Entity.TradePoint >= 1000 && context.Entity.ReferralPoint >= 1000)
                {
                    context.Entity.Level = 1;
                }
                else if (context.Entity.TradePoint >= 2000 && context.Entity.ReferralPoint >= 2000)
                {
                    context.Entity.Level = 2;
                }
                else if (context.Entity.TradePoint >= 3000 && context.Entity.ReferralPoint >= 3000)
                {
                    context.Entity.Level = 3;
                }    
                else
                {
                    context.Entity.Level = 0;
                }
                await _unitOfWork.SaveAsync();
                _unitOfWork.Dispose();
            }
        }
    }
}

