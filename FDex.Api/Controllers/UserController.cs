using System;
using FDex.Application.DTOs.Swap;
using FDex.Application.DTOs.User;
using FDex.Application.Features.Users.Requests.Commands;
using FDex.Application.Features.Users.Requests.Queries;
using FDex.Application.Models.Infrastructure;
using FDex.Application.Responses.User;
using FDex.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FDex.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<bool>> GetAccountStatus([FromQuery] string wallet)
        {
            bool accountStatus = await _mediator.Send(new GetAccountStatusRequest() { Wallet = wallet});
            return Ok(accountStatus);
        }

        [HttpPost("[action]")]
        public async Task<bool> PostReferredUser([FromBody] UpdateReferredUserCommand command) => await _mediator.Send(command);
        
        [HttpGet("[action]")]
        public async Task<ReferredUserQueryModel> GetReferredUsers([FromQuery] GetReferredUsersRequest query) => await _mediator.Send(query);

        [HttpGet("[action]")]
        public async Task<UserReferralInformationResponseModel> GetReferralLevelInformation([FromQuery] GetReferralLevelInformationRequest query) => await _mediator.Send(query);
        
        [HttpGet("[action]")]
        public async Task<Analytic> GetReferralSystemAnalytics()
        {
            Analytic analytic = await _mediator.Send(new GetReferralSystemAnalyticsRequest());
            return analytic;
        }

        [HttpGet("[action]")]
        public async Task<List<UserDTOLeaderboardItemView>> GetLeaderboard([FromQuery] GetLeaderboardRequest query) => await _mediator.Send(query);

        [HttpPost("[action]")]
        public async Task<bool> AddUser([FromBody] AddUserCommand command) => await _mediator.Send(command);

        [HttpPost("[action]")]
        public async Task<bool> RewardClaimedAndSigned([FromBody] RewardClaimedAndSignedCommand command) => await _mediator.Send(command);
    }
}

