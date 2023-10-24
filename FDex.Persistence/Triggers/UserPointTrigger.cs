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
            //if (context.ChangeType == ChangeType.Added || context.ChangeType == ChangeType.Modified)
            //{
            //    await using var scope = _serviceProvider.CreateAsyncScope();
            //    var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            //    BigInteger point = BigInteger.Parse(context.Entity.FeeValue) / dec;
            //    var user = await _unitOfWork.UserRepository.FindAsync(context.Entity.Position.Wallet);
            //    var refUser = await _unitOfWork.UserRepository.FindAsync(user.ReferredUserOf);
            //    Console.WriteLine(point);
            //    //user.TradePoint += (int)point;
            //    //refUser.ReferralPoint += (int)point;
            //    _unitOfWork.UserRepository.Update(user);
            //    _unitOfWork.UserRepository.Update(refUser);
            //    await _unitOfWork.SaveAsync();
            //    _unitOfWork.Dispose();
            //}
        }
    }
}

