using System;
using FDex.Application.Contracts.Persistence;
using FDex.Application.Models.Infrastructure;
using FDex.Domain.Entities;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace FDex.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
	{
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IServiceProvider _serviceProvider;
        public JobController(IRecurringJobManager recurringJobManager, IServiceProvider serviceProvider)
		{
            _recurringJobManager = recurringJobManager;
            _serviceProvider = serviceProvider;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult> StartReferralRewardMonthly()
        {

            var jobId = Guid.NewGuid().ToString();
            _recurringJobManager.AddOrUpdate(jobId, () => StartReferralReward(jobId), Cron.Monthly);
            return Ok($"Job Id: {jobId} started monthly...");
        }

        [HttpGet("[action]")]
        public async Task<ActionResult> StopReferralRewardMonthly()
        {
            return Ok();
        }

        [NonAction]
        public async Task StartReferralReward(string jobId)
        {
            Console.WriteLine($"[DEV-INF] Job Id: {jobId} is starting ...");
            await using var scope = _serviceProvider.CreateAsyncScope();
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            Analytic analytic = await _unitOfWork.UserRepository.GetReferralAnalytics();
            var rewardPerLevel = 10000;
            var level1RewardAmount = 0;
            var level2RewardAmount = 0;
            var level3RewardAmount = 0;
            if (analytic.Level1 > 0)
            {
                level1RewardAmount = rewardPerLevel / analytic.Level1;
            }
            if(analytic.Level2 > 0)
            {
                level2RewardAmount = rewardPerLevel / analytic.Level2;
            }
            if(analytic.Level3 > 0)
            {
                level3RewardAmount = rewardPerLevel / analytic.Level3;
            }
            var users = await _unitOfWork.UserRepository.GetAllAsync();
            foreach (var user in users)
            {
                switch (user.Level)
                {
                    case 0:
                        break;
                    case 1:
                        Reward rewardLevel1 = new Reward()
                        {
                            Id = Guid.NewGuid(),
                            Wallet = user.Wallet,
                            Amount = level1RewardAmount.ToString()
                        };
                        await _unitOfWork.RewardRepository.AddAsync(rewardLevel1);
                        break;
                    case 2:
                        Reward rewardLevel2 = new Reward()
                        {
                            Id = Guid.NewGuid(),
                            Wallet = user.Wallet,
                            Amount = level2RewardAmount.ToString()
                        };
                        await _unitOfWork.RewardRepository.AddAsync(rewardLevel2);
                        break;
                    case 3:
                        Reward rewardLevel3 = new Reward()
                        {
                            Id = Guid.NewGuid(),
                            Wallet = user.Wallet,
                            Amount = level3RewardAmount.ToString()
                        };
                        await _unitOfWork.RewardRepository.AddAsync(rewardLevel3);
                        break;
                }
            }
            await _unitOfWork.SaveAsync();
            _unitOfWork.Dispose();
        }
    }
}

