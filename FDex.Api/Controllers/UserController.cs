using System;
using FDex.Application.DTOs.Swap;
using FDex.Application.Features.Users.Requests.Commands;
using FDex.Application.Features.Users.Requests.Queries;
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
    }
}

