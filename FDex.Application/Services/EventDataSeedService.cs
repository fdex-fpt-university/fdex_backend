using System;
using System.Numerics;
using AutoMapper;
using FDex.Application.Contracts.Persistence;
using FDex.Application.DTOs.Swap;
using FDex.Domain.Entities;
using Microsoft.Extensions.Hosting;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace FDex.Application.Services
{
    public class EventDataSeedService : BackgroundService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly Web3 _web3;

        bool isFirstParam = true;
        private static BigInteger _currentBlockNumber = 33768909;
        private static BigInteger _limitBlockNumber = 9999;


        const string RPC_URL = "https://sly-long-cherry.bsc-testnet.quiknode.pro/4ac0090884736ecd32a595fe2ec55910ca239cdb/";

        public EventDataSeedService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            ClientBase.ConnectionTimeout = TimeSpan.FromDays(1);
            _web3 = new(RPC_URL);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var latestBlockNumber = await _web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
            var contractAddress = "0x02109586C4dCEf32367786D9DEF4306d18b063C7";
            var swapEventHandler = _web3.Eth.GetEvent<SwapDTO>(contractAddress);
            while (!stoppingToken.IsCancellationRequested && _currentBlockNumber < latestBlockNumber)
            {

                var filterAllSwapEvents = swapEventHandler.CreateFilterInput(HandleBlockParameter(), HandleBlockParameter());
                var swapEvents = await swapEventHandler.GetAllChangesAsync(filterAllSwapEvents);
                foreach (var log in swapEvents)
                {
                    var foundUser = await _unitOfWork.UserRepository.FindAsync(log.Event.Wallet);
                    if (foundUser == null)
                    {
                        User user = new()
                        {
                            Wallet = log.Event.Wallet,
                            CreatedDate = DateTime.Now
                        };
                        await _unitOfWork.UserRepository.AddAsync(user);
                    }
                    var foundSwap = await _unitOfWork.SwapRepository.FindAsync(log.Log.TransactionHash);
                    if (foundSwap == null)
                    {
                        SwapDTOAdd rawSwap = new()
                        {
                            TxnHash = log.Log.TransactionHash,
                            Wallet = log.Event.Wallet,
                            TokenIn = log.Event.TokenIn,
                            TokenOut = log.Event.TokenOut,
                            AmountIn = log.Event.AmountIn,
                            AmountOut = log.Event.AmountOut,
                            Fee = log.Event.Fee * log.Event.MarkPrice
                        };
                        Swap swap = _mapper.Map<Swap>(rawSwap);
                        await _unitOfWork.SwapRepository.AddAsync(swap);
                    }
                    await _unitOfWork.Save();
                }
            }
        }

        private BlockParameter HandleBlockParameter()
        {
            if (isFirstParam)
            {
                _currentBlockNumber += 1;
                isFirstParam = !isFirstParam;
                return new BlockParameter(new HexBigInteger(_currentBlockNumber));
            }
            _currentBlockNumber += _limitBlockNumber;
            return new BlockParameter(new HexBigInteger(_currentBlockNumber));
        }
    }
}

