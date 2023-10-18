using System;
using System.Numerics;
using AutoMapper;
using FDex.Application.Contracts.Persistence;
using FDex.Application.DTOs.Liquidity;
using FDex.Application.DTOs.Reporter;
using FDex.Application.DTOs.Swap;
using FDex.Application.DTOs.TradingPosition;
using FDex.Application.Enumerations;
using FDex.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.Model;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace FDex.Application.Services
{
    public class EventDataSeedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMapper _mapper;
        private readonly Web3 _web3;

        bool isFirstParam = true;
        private static BigInteger _currentReporterBlockNumber = 33502213;
        private static BigInteger _currentCommonBlockNumber = 34115291;
        private static BigInteger _limitBlockNumber = 9999;
        const string RPC_URL = "https://bsc.getblock.io/c9c2311d-f632-47b1-ae8f-7cde9cd02fba/testnet/";

        public EventDataSeedService(IMapper mapper, IServiceProvider serviceProvider)
        {
            ClientBase.ConnectionTimeout = TimeSpan.FromDays(1);
            _serviceProvider = serviceProvider;
            _mapper = mapper;
            _web3 = new(RPC_URL);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var latestBlockNumber = await _web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
            var poolAddress = "0x9Fca52B0E21AdfF52563D179b1593149109593b5";
            var oracleAddress = "0x1E16D408a6ae4E2a867cd33F15cb7E17441139c1";
            var increasePositionEventHandler = _web3.Eth.GetEvent<IncreasePositionDTO>(poolAddress);
            var decreasePositionEventHandler = _web3.Eth.GetEvent<DecreasePositionDTO>(poolAddress);
            var updatePositionEventHandler = _web3.Eth.GetEvent<UpdatePositionDTO>(poolAddress);
            var closePositionEventHandler = _web3.Eth.GetEvent<ClosePositionDTO>(poolAddress);
            var liquidatePositionEventHandler = _web3.Eth.GetEvent<LiquidatePositionDTO>(poolAddress);
            var swapEventHandler = _web3.Eth.GetEvent<SwapDTO>(poolAddress);
            var addLiquidityEventHandler = _web3.Eth.GetEvent<AddLiquidityDTO>(poolAddress);
            var reporterAddedEventHandler = _web3.Eth.GetEvent<ReporterAddedDTO>(oracleAddress);
            var reporterRemovedEventHandler = _web3.Eth.GetEvent<ReporterRemovedDTO>(oracleAddress);
            var reporterPostedEventHandler = _web3.Eth.GetEvent<ReporterPostedDTO>(oracleAddress);
            while (!stoppingToken.IsCancellationRequested && _currentReporterBlockNumber <= latestBlockNumber || _currentCommonBlockNumber <= latestBlockNumber)
            {
                BlockParameter startCommonBlock = HandleBlockParameter(_currentCommonBlockNumber);
                BlockParameter endCommonBlock = HandleBlockParameter(_currentCommonBlockNumber);
                BlockParameter startReporterBlock = HandleBlockParameter(_currentReporterBlockNumber);
                BlockParameter endReporterBlock = HandleBlockParameter(_currentReporterBlockNumber);

                var filterAllSwapEvents = swapEventHandler.CreateFilterInput(startCommonBlock, endCommonBlock);
                var filterAllAddLiquidity = addLiquidityEventHandler.CreateFilterInput(startCommonBlock, endCommonBlock);
                var filterAllReporterAdded = reporterAddedEventHandler.CreateFilterInput(startReporterBlock, endReporterBlock);
                var filterAllReporterRemoved = reporterRemovedEventHandler.CreateFilterInput(startReporterBlock, endReporterBlock);
                var filterAllReporterPosted = reporterPostedEventHandler.CreateFilterInput(startReporterBlock, endReporterBlock);
                var filterAllIncreasePosition = increasePositionEventHandler.CreateFilterInput(startCommonBlock, endCommonBlock);
                var filterAllDecreasePosition = decreasePositionEventHandler.CreateFilterInput(startCommonBlock, endCommonBlock);
                var filterAllUpdatePosition = updatePositionEventHandler.CreateFilterInput(startCommonBlock, endCommonBlock);
                var filterAllClosePosition = closePositionEventHandler.CreateFilterInput(startCommonBlock, endCommonBlock);
                var filterAllLiquidatePosition = liquidatePositionEventHandler.CreateFilterInput(startCommonBlock, endCommonBlock);

                if (_currentReporterBlockNumber <= latestBlockNumber)
                {
                    var reporterAddedEvents = await reporterAddedEventHandler.GetAllChangesAsync(filterAllReporterAdded);
                    var reporterRemovedEvents = await reporterRemovedEventHandler.GetAllChangesAsync(filterAllReporterRemoved);
                    var reporterPostedEvents = await reporterPostedEventHandler.GetAllChangesAsync(filterAllReporterPosted);
                    foreach (var log in reporterAddedEvents)
                    {
                        var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        var foundReporterAdded = await _unitOfWork.ReporterRepository.FindAsync(log.Event.Wallet);
                        if (foundReporterAdded == null)
                        {
                            await _unitOfWork.ReporterRepository.AddAsync(new Reporter { Wallet = log.Event.Wallet, ReportCount = 0 });
                        }
                        await _unitOfWork.SaveAsync();
                        _unitOfWork.Dispose();
                    }

                    foreach (var log in reporterRemovedEvents)
                    {
                        var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        var foundReporterRemoved = await _unitOfWork.ReporterRepository.FindAsync(log.Event.Wallet);
                        if (foundReporterRemoved != null)
                        {
                            _unitOfWork.ReporterRepository.Remove(foundReporterRemoved);
                        }
                        await _unitOfWork.SaveAsync();
                        _unitOfWork.Dispose();
                    }

                    foreach (var log in reporterPostedEvents)
                    {
                        var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        var foundReporterPosted = await _unitOfWork.ReporterRepository.FindAsync(log.Event.Wallet);
                        if (foundReporterPosted != null)
                        {
                            Reporter postingReporter = await _unitOfWork.ReporterRepository.FindAsync(log.Event.Wallet);
                            postingReporter.ReportCount += 1;
                            postingReporter.LastReportedDate = DateTime.Now;
                            _unitOfWork.ReporterRepository.Update(postingReporter);
                        }
                        await _unitOfWork.SaveAsync();
                        _unitOfWork.Dispose();
                    }
                }

                if (_currentCommonBlockNumber <= latestBlockNumber)
                {
                    var swapEvents = await swapEventHandler.GetAllChangesAsync(filterAllSwapEvents);
                    var addLiquidityEvents = await addLiquidityEventHandler.GetAllChangesAsync(filterAllAddLiquidity);
                    var increasePositionEvents = await increasePositionEventHandler.GetAllChangesAsync(filterAllIncreasePosition);
                    var decreasePositionEvents = await decreasePositionEventHandler.GetAllChangesAsync(filterAllDecreasePosition);
                    var updatePositionEvents = await updatePositionEventHandler.GetAllChangesAsync(filterAllUpdatePosition);
                    var closePositionEvents = await closePositionEventHandler.GetAllChangesAsync(filterAllClosePosition);
                    var liquidatePositionEvents = await liquidatePositionEventHandler.GetAllChangesAsync(filterAllLiquidatePosition);

                    foreach (var log in swapEvents)
                    {
                        var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
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
                                Fee = log.Event.Fee * log.Event.MarkPrice,
                                Time = DateTime.Now
                            };
                            Swap swap = _mapper.Map<Swap>(rawSwap);
                            await _unitOfWork.SwapRepository.AddAsync(swap);
                        }
                        await _unitOfWork.SaveAsync();
                        _unitOfWork.Dispose();
                    }

                    foreach (var log in addLiquidityEvents)
                    {
                        var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
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
                        var foundAddLiquidity = await _unitOfWork.AddLiquidityRepository.FindAsync(log.Log.TransactionHash);
                        if (foundAddLiquidity == null)
                        {
                            AddLiquidityDTOAdd rawAddLiquidity = new()
                            {
                                TxnHash = log.Log.TransactionHash,
                                Wallet = log.Event.Wallet,
                                Asset = log.Event.Asset,
                                Amount = log.Event.Amount,
                                Fee = log.Event.Fee * log.Event.MarkPriceIn,
                                DateAdded = DateTime.Now
                            };
                            AddLiquidity addLiquidity = _mapper.Map<AddLiquidity>(rawAddLiquidity);
                            await _unitOfWork.AddLiquidityRepository.AddAsync(addLiquidity);
                        }
                        await _unitOfWork.SaveAsync();
                        _unitOfWork.Dispose();
                    }


                    foreach (var log in increasePositionEvents)
                    {
                    }

                    foreach (var log in decreasePositionEvents)
                    {
                    }

                    foreach (var log in updatePositionEvents)
                    {
                    }

                    foreach (var log in closePositionEvents)
                    {
                    }

                    foreach (var log in liquidatePositionEvents)
                    {
                    }
                }
            }
        }

        private BlockParameter HandleBlockParameter(BigInteger blockNumber)
        {
            if (isFirstParam)
            {
                blockNumber += 1;
                isFirstParam = !isFirstParam;
                return new BlockParameter(new HexBigInteger(blockNumber));
            }
            blockNumber += _limitBlockNumber;
            return new BlockParameter(new HexBigInteger(blockNumber));
        }
    }
}

