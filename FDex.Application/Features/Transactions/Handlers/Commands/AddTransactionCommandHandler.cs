using System;
using AutoMapper;
using FDex.Application.Contracts.Persistence;
using FDex.Application.DTOs.Transaction;
using FDex.Application.Features.Transactions.Requests.Commands;
using FDex.Application.Responses.Transaction;
using FDex.Domain.Entities;
using MediatR;
using Nethereum.Web3;
using Org.BouncyCastle.Crypto;

namespace FDex.Application.Features.Transactions.Handlers.Commands
{
	public class AddTransactionCommandHandler : IRequestHandler<AddTransactionCommand, AddTransactionCommandResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AddTransactionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<AddTransactionCommandResponse> Handle(AddTransactionCommand request, CancellationToken cancellationToken)
        {
            //Listen to transaction event

            //const string eventName = "SwapCompleted";
            //string ethereumRpcUrl = "https://data-seed-prebsc-1-s1.binance.org:8545/";
            //Web3 web3 = new Web3(ethereumRpcUrl);
            //string swapContractABI = "[\n    {\n      \"inputs\": [\n        {\n          \"internalType\": \"address\",\n          \"name\": \"_pool\",\n          \"type\": \"address\"\n        },\n        {\n          \"internalType\": \"address\",\n          \"name\": \"_weth\",\n          \"type\": \"address\"\n        },\n        {\n          \"internalType\": \"address\",\n          \"name\": \"_lpToken\",\n          \"type\": \"address\"\n        }\n      ],\n      \"stateMutability\": \"nonpayable\",\n      \"type\": \"constructor\"\n    },\n    {\n      \"inputs\": [\n        {\n          \"internalType\": \"address\",\n          \"name\": \"_token\",\n          \"type\": \"address\"\n        },\n        {\n          \"internalType\": \"uint256\",\n          \"name\": \"_amountIn\",\n          \"type\": \"uint256\"\n        },\n        {\n          \"internalType\": \"uint256\",\n          \"name\": \"_minLpAmount\",\n          \"type\": \"uint256\"\n        }\n      ],\n      \"name\": \"addLiquidity\",\n      \"outputs\": [],\n      \"stateMutability\": \"payable\",\n      \"type\": \"function\"\n    },\n    {\n      \"inputs\": [],\n      \"name\": \"lpToken\",\n      \"outputs\": [\n        {\n          \"internalType\": \"contract ILPToken\",\n          \"name\": \"\",\n          \"type\": \"address\"\n        }\n      ],\n      \"stateMutability\": \"view\",\n      \"type\": \"function\"\n    },\n    {\n      \"inputs\": [],\n      \"name\": \"pool\",\n      \"outputs\": [\n        {\n          \"internalType\": \"contract IPool\",\n          \"name\": \"\",\n          \"type\": \"address\"\n        }\n      ],\n      \"stateMutability\": \"view\",\n      \"type\": \"function\"\n    },\n    {\n      \"inputs\": [\n        {\n          \"internalType\": \"address\",\n          \"name\": \"_tokenOut\",\n          \"type\": \"address\"\n        },\n        {\n          \"internalType\": \"uint256\",\n          \"name\": \"_lpAmount\",\n          \"type\": \"uint256\"\n        },\n        {\n          \"internalType\": \"uint256\",\n          \"name\": \"_minOut\",\n          \"type\": \"uint256\"\n        }\n      ],\n      \"name\": \"removeLiquidity\",\n      \"outputs\": [],\n      \"stateMutability\": \"payable\",\n      \"type\": \"function\"\n    },\n    {\n      \"inputs\": [\n        {\n          \"internalType\": \"address\",\n          \"name\": \"_fromToken\",\n          \"type\": \"address\"\n        },\n        {\n          \"internalType\": \"address\",\n          \"name\": \"_toToken\",\n          \"type\": \"address\"\n        },\n        {\n          \"internalType\": \"uint256\",\n          \"name\": \"_amountIn\",\n          \"type\": \"uint256\"\n        },\n        {\n          \"internalType\": \"uint256\",\n          \"name\": \"_minOut\",\n          \"type\": \"uint256\"\n        }\n      ],\n      \"name\": \"swap\",\n      \"outputs\": [],\n      \"stateMutability\": \"payable\",\n      \"type\": \"function\"\n    },\n    {\n      \"inputs\": [],\n      \"name\": \"weth\",\n      \"outputs\": [\n        {\n          \"internalType\": \"contract IWETH\",\n          \"name\": \"\",\n          \"type\": \"address\"\n        }\n      ],\n      \"stateMutability\": \"view\",\n      \"type\": \"function\"\n    },\n    {\n      \"stateMutability\": \"payable\",\n      \"type\": \"receive\"\n    }\n  ]";
            //string contractAddress = "0x65870ceCFB6725939C1321E055224eA405504A52";
            //var contract = web3.Eth.GetContract(swapContractABI, contractAddress);
            ////var contract = web3.Eth.GetContract(SwapContract.ABI, contractAddress);


            //var filterInput = contract.GetEvent(eventName).CreateFilterInput();

            //var events = await web3.Eth.Filters.GetLogs.SendRequestAsync(filterInput);

            //foreach (var evt in events)
            //{
            //    Console.WriteLine($"Transaction Hash: {evt.TransactionHash}");
            //    Console.WriteLine($"SwapCompleted Event Data: {evt.Data}");
            //}

            AddTransactionCommandResponse response = new();
            Transaction transaction = _mapper.Map<Transaction>(request.AddTransactionDTO);
            transaction = await _unitOfWork.TransactionRepository.AddAsync(transaction);
            await _unitOfWork.Save();
            response.IsSuccess = true;
            response.Message = "Creation Successful!";
            response.Id = transaction.Id;
            response.AddTransactionDTO = request.AddTransactionDTO;
            return response;
        }
    }
}

