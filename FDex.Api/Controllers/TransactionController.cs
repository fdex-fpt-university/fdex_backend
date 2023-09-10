using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FDex.Application.DTOs.Transaction;
using FDex.Application.Features.Transactions.Requests.Commands;
using FDex.Application.Features.Transactions.Requests.Queries;
using FDex.Application.Responses.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nethereum.RPC.Eth.DTOs;

namespace FDex.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly IMediator _mediator;
        public TransactionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<TransactionDTO>>> Get()
        {
            var transactions = await _mediator.Send(new GetTransactionsRequest());
            return Ok(transactions);
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<BaseCommandResponse>> Post([FromBody] AddTransactionDTO addTransactionDTO)
        {
            var command = new AddTransactionCommand { AddTransactionDTO = addTransactionDTO };
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}

