using System;
using System.Numerics;
using EntityFrameworkCore.Triggered;
using FDex.Application.Contracts.Persistence;
using FDex.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace FDex.Persistence.Triggers
{
    public class UserPointTrigger : IAfterSaveAsyncTrigger<PositionDetail>
    {
        private readonly IServiceProvider _serviceProvider;
        private BigInteger dec = BigInteger.Pow(10, 30);

        public UserPointTrigger(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

        public async Task AfterSaveAsync(ITriggerContext<PositionDetail> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType == ChangeType.Added || context.ChangeType == ChangeType.Modified)
            {
                await using var scope = _serviceProvider.CreateAsyncScope();
                var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                BigInteger point = 0;
                if (context.Entity.FeeValue != null)
                {
                    point = BigInteger.Parse(context.Entity.FeeValue) / dec; 
                }
                var position = await _unitOfWork.PositionRepository.FindAsync(context.Entity.PositionId);
                var user = await _unitOfWork.UserRepository.FindAsync(position.Wallet);
                var refUser = await _unitOfWork.UserRepository.FindAsync(user.ReferredUserOf);
                user.TradePoint = user.TradePoint != null ? user.TradePoint += (decimal) point : user.TradePoint = (decimal) point;
                _unitOfWork.UserRepository.Update(user);
                if(refUser != null)
                {
                    refUser.ReferralPoint = refUser.ReferralPoint != null ? refUser.ReferralPoint += (decimal)point : user.ReferralPoint = (decimal)point;
                    _unitOfWork.UserRepository.Update(refUser);
                }
                await _unitOfWork.SaveAsync();
                _unitOfWork.Dispose();
            }
        }
    }
}

