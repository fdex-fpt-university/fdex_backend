using System;
using System.Numerics;
using System.Text;
using AutoMapper;
using FDex.Application.Contracts.Persistence;
using FDex.Application.DTOs.Liquidity;
using FDex.Application.DTOs.Reporter;
using FDex.Application.DTOs.Swap;
using FDex.Application.DTOs.TradingPosition;
using FDex.Application.Enumerations;
using FDex.Domain.Entities;
using FDex.Domain.Enumerations;
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
        private const string COMMON_CONTRACT = "Common";
        private const string REPORTER_CONTRACT = "Reporter";

        bool isFirstParam = true;
        private BigInteger _currentReporterBlockNumber = 34002213;
        private BigInteger _currentCommonBlockNumber = 34115291;
        private BigInteger _limitBlockNumber = 9999;
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
            var poolAddress = "0x713B1c99A5871b6Ea58C890305DD7066FC01988b";
            var oracleAddress = "0x1E16D408a6ae4E2a867cd33F15cb7E17441139c1";
            var increasePositionEventHandler = _web3.Eth.GetEvent<FDexIncreaPositionDTO>(poolAddress);
            var decreasePositionEventHandler = _web3.Eth.GetEvent<FDexDecreaPositionDTO>(poolAddress);
            var openPositionEventHandler = _web3.Eth.GetEvent<FDexOpenPositionDTO>(poolAddress);
            var closePositionEventHandler = _web3.Eth.GetEvent<FDexClosePositionDTO>(poolAddress);
            var liquidatePositionEventHandler = _web3.Eth.GetEvent<LiquidatePositionDTO>(poolAddress);
            var swapEventHandler = _web3.Eth.GetEvent<SwapDTO>(poolAddress);
            var addLiquidityEventHandler = _web3.Eth.GetEvent<AddLiquidityDTO>(poolAddress);
            var reporterAddedEventHandler = _web3.Eth.GetEvent<ReporterAddedDTO>(oracleAddress);
            var reporterRemovedEventHandler = _web3.Eth.GetEvent<ReporterRemovedDTO>(oracleAddress);
            var reporterPostedEventHandler = _web3.Eth.GetEvent<ReporterPostedDTO>(oracleAddress);
            while (!stoppingToken.IsCancellationRequested && _currentReporterBlockNumber <= latestBlockNumber || _currentCommonBlockNumber <= latestBlockNumber)
            {
                BlockParameter startCommonBlock = HandleBlockParameter(COMMON_CONTRACT);
                BlockParameter endCommonBlock = HandleBlockParameter(COMMON_CONTRACT);
                BlockParameter startReporterBlock = HandleBlockParameter(REPORTER_CONTRACT);
                BlockParameter endReporterBlock = HandleBlockParameter(REPORTER_CONTRACT);

                var filterAllSwapEvents = swapEventHandler.CreateFilterInput(startCommonBlock, endCommonBlock);
                var filterAllAddLiquidity = addLiquidityEventHandler.CreateFilterInput(startCommonBlock, endCommonBlock);
                var filterAllReporterAdded = reporterAddedEventHandler.CreateFilterInput(startReporterBlock, endReporterBlock);
                var filterAllReporterRemoved = reporterRemovedEventHandler.CreateFilterInput(startReporterBlock, endReporterBlock);
                var filterAllReporterPosted = reporterPostedEventHandler.CreateFilterInput(startReporterBlock, endReporterBlock);
                var filterAllIncreasePosition = increasePositionEventHandler.CreateFilterInput(startCommonBlock, endCommonBlock);
                var filterAllDecreasePosition = decreasePositionEventHandler.CreateFilterInput(startCommonBlock, endCommonBlock);
                var filterAllOpenPosition = openPositionEventHandler.CreateFilterInput(startCommonBlock, endCommonBlock);
                var filterAllClosePosition = closePositionEventHandler.CreateFilterInput(startCommonBlock, endCommonBlock);
                var filterAllLiquidatePosition = liquidatePositionEventHandler.CreateFilterInput(startCommonBlock, endCommonBlock);

                //if (_currentReporterBlockNumber <= latestBlockNumber)
                //{
                //    var reporterAddedEvents = await reporterAddedEventHandler.GetAllChangesAsync(filterAllReporterAdded);
                //    var reporterRemovedEvents = await reporterRemovedEventHandler.GetAllChangesAsync(filterAllReporterRemoved);
                //    var reporterPostedEvents = await reporterPostedEventHandler.GetAllChangesAsync(filterAllReporterPosted);
                //    foreach (var log in reporterAddedEvents)
                //    {
                //        var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                //        var foundReporterAdded = await _unitOfWork.ReporterRepository.FindAsync(log.Event.Wallet);
                //        if (foundReporterAdded == null)
                //        {
                //            await _unitOfWork.ReporterRepository.AddAsync(new Reporter { Wallet = log.Event.Wallet, ReportCount = 0 });
                //        }
                //        await _unitOfWork.SaveAsync();
                //        _unitOfWork.Dispose();
                //    }

                //    foreach (var log in reporterRemovedEvents)
                //    {
                //        var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                //        var foundReporterRemoved = await _unitOfWork.ReporterRepository.FindAsync(log.Event.Wallet);
                //        if (foundReporterRemoved != null)
                //        {
                //            _unitOfWork.ReporterRepository.Remove(foundReporterRemoved);
                //        }
                //        await _unitOfWork.SaveAsync();
                //        _unitOfWork.Dispose();
                //    }

                //    foreach (var log in reporterPostedEvents)
                //    {
                //        var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                //        var foundReporterPosted = await _unitOfWork.ReporterRepository.FindAsync(log.Event.Wallet);
                //        if (foundReporterPosted != null)
                //        {
                //            Reporter postingReporter = await _unitOfWork.ReporterRepository.FindAsync(log.Event.Wallet);
                //            postingReporter.ReportCount += 1;
                //            postingReporter.LastReportedDate = DateTime.Now;
                //            _unitOfWork.ReporterRepository.Update(postingReporter);
                //        }
                //        await _unitOfWork.SaveAsync();
                //        _unitOfWork.Dispose();
                //    }
                //}

                if (_currentCommonBlockNumber <= latestBlockNumber)
                {
                    var swapEvents = await swapEventHandler.GetAllChangesAsync(filterAllSwapEvents);
                    var addLiquidityEvents = await addLiquidityEventHandler.GetAllChangesAsync(filterAllAddLiquidity);
                    var increasePositionEvents = await increasePositionEventHandler.GetAllChangesAsync(filterAllIncreasePosition);
                    var decreasePositionEvents = await decreasePositionEventHandler.GetAllChangesAsync(filterAllDecreasePosition);
                    var openPositionEvents = await openPositionEventHandler.GetAllChangesAsync(filterAllOpenPosition);
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

                    foreach (var log in openPositionEvents)
                    {
                        Console.WriteLine("[DEV-INF] Decoding an open position event ...");
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
                        string key = Encoding.UTF8.GetString(log.Event.Key);
                        Position foundPosition = await _unitOfWork.PositionRepository.GetPositionInDetails(key);
                        if (foundPosition == null)
                        {
                            Position pos = new()
                            {
                                Id = Guid.NewGuid(),
                                Key = key,
                                Wallet = log.Event.Wallet,
                                CollateralToken = log.Event.CollateralToken,
                                IndexToken = log.Event.IndexToken,
                                Side = log.Event.Side == '1',
                            };
                            await _unitOfWork.PositionRepository.AddAsync(pos);
                            PositionDetail posd = new()
                            {
                                Id = Guid.NewGuid(),
                                PositionId = pos.Id,
                                CollateralValue = log.Event.CollateralValue.ToString(),
                                IndexPrice = log.Event.IndexPrice.ToString(),
                                PositionState = PositionState.Open,
                                SizeChanged = log.Event.SizeChanged.ToString(),
                                FeeValue = log.Event.FeeValue.ToString(),
                                Time = DateTime.Now
                            };
                            await _unitOfWork.PositionDetailRepository.AddAsync(posd);
                        }
                        await _unitOfWork.SaveAsync();
                        _unitOfWork.Dispose();
                    }

                    foreach (var log in increasePositionEvents)
                    {
                        Console.WriteLine("[DEV-INF] Decoding an increase position event ...");
                        var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        string key = Encoding.UTF8.GetString(log.Event.Key);
                        Position foundPosition = await _unitOfWork.PositionRepository.GetPositionInDetails(key);
                        PositionDetail posd = new()
                        {
                            Id = Guid.NewGuid(),
                            PositionId = foundPosition.Id,
                            CollateralValue = log.Event.CollateralValue.ToString(),
                            IndexPrice = log.Event.IndexPrice.ToString(),
                            PositionState = PositionState.Increase,
                            SizeChanged = log.Event.SizeChanged.ToString(),
                            FeeValue = log.Event.FeeValue.ToString(),
                            Time = DateTime.Now
                        };
                        await _unitOfWork.PositionDetailRepository.AddAsync(posd);
                        await _unitOfWork.SaveAsync();
                        _unitOfWork.Dispose();
                    }

                    foreach (var log in decreasePositionEvents)
                    {
                        Console.WriteLine("[DEV-INF] Decoding a decrease position event ...");
                        var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        string key = Encoding.UTF8.GetString(log.Event.Key);
                        Position foundPosition = await _unitOfWork.PositionRepository.GetPositionInDetails(key);
                        PositionDetail posd = new()
                        {
                            Id = Guid.NewGuid(),
                            PositionId = foundPosition.Id,
                            CollateralValue = (~log.Event.CollateralChanged + 1).ToString(),
                            IndexPrice = log.Event.IndexPrice.ToString(),
                            PositionState = PositionState.Decrease,
                            SizeChanged = log.Event.SizeChanged.ToString(),
                            FeeValue = log.Event.FeeValue.ToString(),
                            Time = DateTime.Now
                        };
                        await _unitOfWork.PositionDetailRepository.AddAsync(posd);
                        await _unitOfWork.SaveAsync();
                        _unitOfWork.Dispose();
                    }

                    foreach (var log in closePositionEvents)
                    {
                        Console.WriteLine("[DEV-INF] Decoding a close position event ...");
                        var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        string key = Encoding.UTF8.GetString(log.Event.Key);
                        Position foundPosition = await _unitOfWork.PositionRepository.GetPositionInDetails(key);
                        PositionDetail posd = new()
                        {
                            Id = Guid.NewGuid(),
                            PositionId = foundPosition.Id,
                            CollateralValue = (~log.Event.CollateralValue + 1).ToString(),
                            PositionState = PositionState.Close,
                            SizeChanged = log.Event.Size.ToString(),
                            Time = DateTime.Now
                        };
                        await _unitOfWork.PositionDetailRepository.AddAsync(posd);
                        await _unitOfWork.SaveAsync();
                        _unitOfWork.Dispose();
                    }

                    foreach (var log in liquidatePositionEvents)
                    {
                        Console.WriteLine("[DEV-INF] Decoding a liquidate position event ...");
                        var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        string key = Encoding.UTF8.GetString(log.Event.Key);
                        Position foundPosition = await _unitOfWork.PositionRepository.GetPositionInDetails(key);
                        PositionDetail posd = new()
                        {
                            Id = Guid.NewGuid(),
                            PositionId = foundPosition.Id,
                            CollateralValue = (~log.Event.CollateralValue + 1).ToString(),
                            PositionState = PositionState.Liquidate,
                            SizeChanged = log.Event.Size.ToString(),
                            Pnl = log.Event.Pnl.Sig == 1 ? log.Event.Pnl.Abs.ToString() : (~log.Event.Pnl.Abs + 1).ToString(),
                            Time = DateTime.Now
                        };
                        await _unitOfWork.PositionDetailRepository.AddAsync(posd);
                        await _unitOfWork.SaveAsync();
                        _unitOfWork.Dispose();
                    }
                }
            }
        }

        private BlockParameter HandleBlockParameter(string contract)
        {
            if (isFirstParam)
            {
                if (contract.Equals(COMMON_CONTRACT))
                {
                    _currentCommonBlockNumber += 1;
                    isFirstParam = !isFirstParam;
                    return new BlockParameter(new HexBigInteger(_currentCommonBlockNumber));
                }
                else
                {
                    _currentReporterBlockNumber += 1;
                    isFirstParam = !isFirstParam;
                    return new BlockParameter(new HexBigInteger(_currentReporterBlockNumber));
                }
                
            }
            if (contract.Equals(COMMON_CONTRACT))
            {
                _currentCommonBlockNumber += _limitBlockNumber;
                isFirstParam = !isFirstParam;
                return new BlockParameter(new HexBigInteger(_currentCommonBlockNumber));
            }
            else
            {
                _currentReporterBlockNumber += _limitBlockNumber;
                isFirstParam = !isFirstParam;
                return new BlockParameter(new HexBigInteger(_currentReporterBlockNumber));
            }
        }
    }
}

