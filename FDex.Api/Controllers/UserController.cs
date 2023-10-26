using System;
using FDex.Application.DTOs.Swap;
using FDex.Application.DTOs.User;
using FDex.Application.Features.Users.Requests.Commands;
using FDex.Application.Features.Users.Requests.Queries;
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
        public async Task<bool> PostReferredUser([FromBody] UpdateReferredUserCommand command)
        {
            command = new UpdateReferredUserCommand() { ReferringUser = command.ReferringUser, ReferralUser = command.ReferralUser };
            await _mediator.Send(command);
            return true;
        }

        [HttpGet("[action]")]
        public async Task<ReferredUserQueryModel> GetReferredUsers([FromQuery] GetReferredUsersRequest query) => await _mediator.Send(query);

        [HttpGet("[action]")]
        public async Task<object> GetReferralLevelInformation(string wallet)
        {
            object info = await _mediator.Send(new GetReferralLevelInformationRequest() { Wallet = wallet });
            return info;
        }

        [HttpGet("[action]")]
        public async Task<UserLevelAnalytic> GetReferralSystemAnalytics()
        {
            UserLevelAnalytic info = await _mediator.Send(new GetReferralSystemAnalyticsRequest());
            return info;
        }

        [HttpGet("[action]")]
        public async Task<List<UserDTOLeaderboardItemView>> GetLeaderboard([FromQuery] GetLeaderboardRequest query) => await _mediator.Send(query);

        [HttpPost("[action]")]
        public async Task<bool> AddUser([FromBody] AddUserCommand command)
        {
            command = new AddUserCommand() { Wallet = command.Wallet};
            await _mediator.Send(command);
            return true;
        }
    }
}

