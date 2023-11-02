using System;
using System.Drawing;
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
using Nethereum.BlockchainProcessing.BlockStorage.Entities;
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
        private BigInteger _latestBlockNumber = 0;
        private BigInteger _currentReporterBlockNumber = 34002213;
        private BigInteger _currentCommonBlockNumber = 34597299;
        private BigInteger _limitBlockNumber = 9999;
        const string RPC_URL = "https://bsc.getblock.io/6d5630b5-f63b-4830-bc17-90d9c7ada49e/testnet/";

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
            _latestBlockNumber = await _web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
            var poolAddress = "0x8B8DC0A49f0F401f575f8DC3AA3641BbBCca9194";
            var oracleAddress = "0x1E16D408a6ae4E2a867cd33F15cb7E17441139c1";
            var increasePositionEventHandler = _web3.Eth.GetEvent<FDexIncreaPositionDTO>(poolAddress);
            var decreasePositionEventHandler = _web3.Eth.GetEvent<FDexDecreaPositionDTO>(poolAddress);
            var openPositionEventHandler = _web3.Eth.GetEvent<FDexOpenPositionDTO>(poolAddress);
            var closePositionEventHandler = _web3.Eth.GetEvent<FDexClosePositionDTO>(poolAddress);
            var liquidatePositionEventHandler = _web3.Eth.GetEvent<LiquidatePositionDTO>(poolAddress);
            var swapEventHandler = _web3.Eth.GetEvent<SwapDTO>(poolAddress);
            var liquidityEventHandler = _web3.Eth.GetEvent<LiquidityDTO>(poolAddress);
            var reporterAddedEventHandler = _web3.Eth.GetEvent<ReporterAddedDTO>(oracleAddress);
            var reporterRemovedEventHandler = _web3.Eth.GetEvent<ReporterRemovedDTO>(oracleAddress);
            var reporterPostedEventHandler = _web3.Eth.GetEvent<ReporterPostedDTO>(oracleAddress);
            while (_currentReporterBlockNumber <= _latestBlockNumber || _currentCommonBlockNumber <= _latestBlockNumber)
            {
                BlockParameter startCommonBlock = HandleBlockParameter(COMMON_CONTRACT);
                BlockParameter endCommonBlock = HandleBlockParameter(COMMON_CONTRACT);
                BlockParameter startReporterBlock = HandleBlockParameter(REPORTER_CONTRACT);
                BlockParameter endReporterBlock = HandleBlockParameter(REPORTER_CONTRACT);

                var filterAllSwapEvents = swapEventHandler.CreateFilterInput(startCommonBlock, endCommonBlock);
                var filterAllLiquidity = liquidityEventHandler.CreateFilterInput(startCommonBlock, endCommonBlock);
                var filterAllReporterAdded = reporterAddedEventHandler.CreateFilterInput(startReporterBlock, endReporterBlock);
                var filterAllReporterRemoved = reporterRemovedEventHandler.CreateFilterInput(startReporterBlock, endReporterBlock);
                var filterAllReporterPosted = reporterPostedEventHandler.CreateFilterInput(startReporterBlock, endReporterBlock);
                var filterAllIncreasePosition = increasePositionEventHandler.CreateFilterInput(startCommonBlock, endCommonBlock);
                var filterAllDecreasePosition = decreasePositionEventHandler.CreateFilterInput(startCommonBlock, endCommonBlock);
                var filterAllOpenPosition = openPositionEventHandler.CreateFilterInput(startCommonBlock, endCommonBlock);
                var filterAllClosePosition = closePositionEventHandler.CreateFilterInput(startCommonBlock, endCommonBlock);
                var filterAllLiquidatePosition = liquidatePositionEventHandler.CreateFilterInput(startCommonBlock, endCommonBlock);

                if (_currentReporterBlockNumber <= _latestBlockNumber)
                {
                    var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var reporterAddedEvents = await reporterAddedEventHandler.GetAllChangesAsync(filterAllReporterAdded);
                    var reporterRemovedEvents = await reporterRemovedEventHandler.GetAllChangesAsync(filterAllReporterRemoved);
                    var reporterPostedEvents = await reporterPostedEventHandler.GetAllChangesAsync(filterAllReporterPosted);
                    foreach (var log in reporterAddedEvents)
                    {
                        var foundReporterAdded = await _unitOfWork.ReporterRepository.FindAsync(log.Event.Wallet);
                        if (foundReporterAdded == null)
                        {
                            await _unitOfWork.ReporterRepository.AddAsync(new Reporter { Wallet = log.Event.Wallet, ReportCount = 0 });
                        }
                    }

                    foreach (var log in reporterRemovedEvents)
                    {
                        var foundReporterRemoved = await _unitOfWork.ReporterRepository.FindAsync(log.Event.Wallet);
                        if (foundReporterRemoved != null)
                        {
                            _unitOfWork.ReporterRepository.Remove(foundReporterRemoved);
                        }
                    }

                    foreach (var log in reporterPostedEvents)
                    {
                        var foundReporterPosted = await _unitOfWork.ReporterRepository.FindAsync(log.Event.Wallet);
                        //var receipt = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(log.Log.TransactionHash);
                        //var block = await _web3.Eth.Blocks.GetBlockWithTransactionsByHash.SendRequestAsync(receipt.BlockHash);
                        if (foundReporterPosted != null)
                        {
                            Reporter postingReporter = await _unitOfWork.ReporterRepository.FindAsync(log.Event.Wallet);
                            postingReporter.ReportCount += 1;
                            postingReporter.LastReportedDate = DateTime.Now;
                            //postingReporter.LastReportedDate = DateTimeOffset.FromUnixTimeSeconds((long)block.Timestamp.Value).LocalDateTime;
                            _unitOfWork.ReporterRepository.Update(postingReporter);
                        }
                    }
                    await _unitOfWork.SaveAsync();
                    _unitOfWork.Dispose();
                }

                if (_currentCommonBlockNumber <= _latestBlockNumber)
                {
                    var swapEvents = await swapEventHandler.GetAllChangesAsync(filterAllSwapEvents);
                    var liquidityEvents = await liquidityEventHandler.GetAllChangesAsync(filterAllLiquidity);
                    var increasePositionEvents = await increasePositionEventHandler.GetAllChangesAsync(filterAllIncreasePosition);
                    var decreasePositionEvents = await decreasePositionEventHandler.GetAllChangesAsync(filterAllDecreasePosition);
                    var openPositionEvents = await openPositionEventHandler.GetAllChangesAsync(filterAllOpenPosition);
                    var closePositionEvents = await closePositionEventHandler.GetAllChangesAsync(filterAllClosePosition);
                    var liquidatePositionEvents = await liquidatePositionEventHandler.GetAllChangesAsync(filterAllLiquidatePosition);

                    foreach (var log in swapEvents)
                    {
                        var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        var foundUser = await _unitOfWork.UserRepository.FindAsync(log.Event.Wallet);
                        var receipt = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(log.Log.TransactionHash);
                        var block = await _web3.Eth.Blocks.GetBlockWithTransactionsByHash.SendRequestAsync(receipt.BlockHash);
                        if (foundUser == null)
                        {
                            User user = new()
                            {
                                Wallet = log.Event.Wallet,
                                CreatedDate = DateTimeOffset.FromUnixTimeSeconds((long)block.Timestamp.Value).LocalDateTime
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
                                Time = DateTimeOffset.FromUnixTimeSeconds((long)block.Timestamp.Value).LocalDateTime
                            };
                            Swap swap = _mapper.Map<Swap>(rawSwap);
                            await _unitOfWork.SwapRepository.AddAsync(swap);
                        }
                        await _unitOfWork.SaveAsync();
                        _unitOfWork.Dispose();
                    }

                    foreach (var log in liquidityEvents)
                    {
                        var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        var foundUser = await _unitOfWork.UserRepository.FindAsync(log.Event.Wallet);
                        var receipt = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(log.Log.TransactionHash);
                        var block = await _web3.Eth.Blocks.GetBlockWithTransactionsByHash.SendRequestAsync(receipt.BlockHash);
                        if (foundUser == null)
                        {
                            User user = new()
                            {
                                Wallet = log.Event.Wallet,
                                CreatedDate = DateTimeOffset.FromUnixTimeSeconds((long)block.Timestamp.Value).LocalDateTime
                            };
                            await _unitOfWork.UserRepository.AddAsync(user);
                        }
                        var foundLiquidity = await _unitOfWork.LiquidityRepository.FindAsync(log.Log.TransactionHash);
                        if (foundLiquidity == null)
                        {
                            LiquidityDTOAdd rawLiquidity = new()
                            {
                                TxnHash = log.Log.TransactionHash,
                                Wallet = log.Event.Wallet,
                                Asset = log.Event.Asset,
                                Amount = log.Event.Amount,
                                Fee = log.Event.Fee * log.Event.MarkPriceIn,
                                DateAdded = DateTimeOffset.FromUnixTimeSeconds((long)block.Timestamp.Value).LocalDateTime
                            };
                            Liquidity liquidity = _mapper.Map<Liquidity>(rawLiquidity);
                            await _unitOfWork.LiquidityRepository.AddAsync(liquidity);
                        }
                        await _unitOfWork.SaveAsync();
                        _unitOfWork.Dispose();
                    }

                    foreach (var log in openPositionEvents)
                    {
                        try
                        {
                            Console.WriteLine("[DEV-INF] Decoding an open position event ...");
                            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                            var foundUser = await _unitOfWork.UserRepository.FindAsync(log.Event.Wallet);
                            var receipt = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(log.Log.TransactionHash);
                            var block = await _web3.Eth.Blocks.GetBlockWithTransactionsByHash.SendRequestAsync(receipt.BlockHash);
                            if (foundUser == null)
                            {
                                User user = new()
                                {
                                    Wallet = log.Event.Wallet,
                                    CreatedDate = DateTimeOffset.FromUnixTimeSeconds((long)block.Timestamp.Value).LocalDateTime
                                };
                                await _unitOfWork.UserRepository.AddAsync(user);
                            }
                            string key = "0x" + BitConverter.ToString(log.Event.Key).Replace("-", "");
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
                                    Size = log.Event.Size.ToString(),
                                    Side = log.Event.Side == '1',
                                    Leverage = (int)(log.Event.Size / log.Event.CollateralValue),
                                    TradingVolumn = log.Event.SizeChanged.ToString(),
                                    LastUpdatedDate = DateTimeOffset.FromUnixTimeSeconds((long)block.Timestamp.Value).LocalDateTime
                                };
                                await _unitOfWork.PositionRepository.AddAsync(pos);
                                PositionDetail posd = new()
                                {
                                    Id = Guid.NewGuid(),
                                    PositionId = pos.Id,
                                    CollateralValue = log.Event.CollateralValue.ToString(),
                                    EntryPrice = log.Event.EntryPrice.ToString(),
                                    IndexPrice = log.Event.IndexPrice.ToString(),
                                    ReserveAmount = log.Event.ReserveAmount.ToString(),
                                    PositionState = PositionState.Open,
                                    SizeChanged = log.Event.SizeChanged.ToString(),
                                    FeeValue = log.Event.FeeValue.ToString(),
                                    Time = DateTimeOffset.FromUnixTimeSeconds((long)block.Timestamp.Value).LocalDateTime
                                };
                                await _unitOfWork.PositionDetailRepository.AddAsync(posd);
                            }
                            else
                            {
                                PositionDetail posd = new()
                                {
                                    Id = Guid.NewGuid(),
                                    PositionId = foundPosition.Id,
                                    CollateralValue = log.Event.CollateralValue.ToString(),
                                    EntryPrice = log.Event.EntryPrice.ToString(),
                                    IndexPrice = log.Event.IndexPrice.ToString(),
                                    ReserveAmount = log.Event.ReserveAmount.ToString(),
                                    PositionState = PositionState.Open,
                                    SizeChanged = log.Event.SizeChanged.ToString(),
                                    FeeValue = log.Event.FeeValue.ToString(),
                                    Time = DateTimeOffset.FromUnixTimeSeconds((long)block.Timestamp.Value).LocalDateTime
                                };
                                await _unitOfWork.PositionDetailRepository.AddAsync(posd);
                                foundPosition.TradingVolumn = (BigInteger.Parse(foundPosition.TradingVolumn) + log.Event.Size).ToString();
                                _unitOfWork.PositionRepository.Update(foundPosition);
                            }

                            await _unitOfWork.SaveAsync();
                            _unitOfWork.Dispose();
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine("[DEV-ERR] Exception: " + e.Message);
                        }
                    }

                    foreach (var log in increasePositionEvents)
                    {
                        Console.WriteLine("[DEV-INF] Decoding an increase position event ...");
                        var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        string key = "0x" + BitConverter.ToString(log.Event.Key).Replace("-", "");
                        Position foundPosition = await _unitOfWork.PositionRepository.GetPositionInDetails(key);
                        var receipt = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(log.Log.TransactionHash);
                        var block = await _web3.Eth.Blocks.GetBlockWithTransactionsByHash.SendRequestAsync(receipt.BlockHash);
                        PositionDetail posd = new()
                        {
                            Id = Guid.NewGuid(),
                            PositionId = foundPosition.Id,
                            CollateralValue = log.Event.CollateralValue.ToString(),
                            EntryPrice = log.Event.EntryPrice.ToString(),
                            IndexPrice = log.Event.IndexPrice.ToString(),
                            ReserveAmount = log.Event.ReserveAmount.ToString(),
                            PositionState = PositionState.Increase,
                            SizeChanged = log.Event.SizeChanged.ToString(),
                            FeeValue = log.Event.FeeValue.ToString(),
                            Time = DateTimeOffset.FromUnixTimeSeconds((long)block.Timestamp.Value).LocalDateTime
                        };
                        foundPosition.LastUpdatedDate = DateTimeOffset.FromUnixTimeSeconds((long)block.Timestamp.Value).LocalDateTime;
                        foundPosition.Size = log.Event.Size.ToString();
                        foundPosition.TradingVolumn = (BigInteger.Parse(foundPosition.TradingVolumn) + log.Event.SizeChanged).ToString();
                        _unitOfWork.PositionRepository.Update(foundPosition);
                        await _unitOfWork.PositionDetailRepository.AddAsync(posd);
                        await _unitOfWork.SaveAsync();
                        _unitOfWork.Dispose();
                    }

                    foreach (var log in decreasePositionEvents)
                    {
                        Console.WriteLine("[DEV-INF] Decoding a decrease position event ...");
                        var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        string key = "0x" + BitConverter.ToString(log.Event.Key).Replace("-", "");
                        Position foundPosition = await _unitOfWork.PositionRepository.GetPositionInDetails(key);
                        var receipt = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(log.Log.TransactionHash);
                        var block = await _web3.Eth.Blocks.GetBlockWithTransactionsByHash.SendRequestAsync(receipt.BlockHash);
                        PositionDetail posd = new()
                        {
                            Id = Guid.NewGuid(),
                            PositionId = foundPosition.Id,
                            CollateralValue = log.Event.CollateralValue.ToString(),
                            EntryPrice = log.Event.EntryPrice.ToString(),
                            IndexPrice = log.Event.IndexPrice.ToString(),
                            ReserveAmount = log.Event.ReserveAmount.ToString(),
                            PositionState = PositionState.Decrease,
                            SizeChanged = log.Event.SizeChanged.ToString(),
                            FeeValue = log.Event.FeeValue.ToString(),
                            Time = DateTimeOffset.FromUnixTimeSeconds((long)block.Timestamp.Value).LocalDateTime
                        };
                        foundPosition.LastUpdatedDate = DateTimeOffset.FromUnixTimeSeconds((long)block.Timestamp.Value).LocalDateTime;
                        foundPosition.Size = log.Event.Size.ToString();
                        _unitOfWork.PositionRepository.Update(foundPosition);
                        await _unitOfWork.PositionDetailRepository.AddAsync(posd);
                        await _unitOfWork.SaveAsync();
                        _unitOfWork.Dispose();
                    }

                    foreach (var log in closePositionEvents)
                    {
                        Console.WriteLine("[DEV-INF] Decoding a close position event ...");
                        var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        string key = "0x" + BitConverter.ToString(log.Event.Key).Replace("-", "");
                        Position foundPosition = await _unitOfWork.PositionRepository.GetPositionInDetails(key);
                        var receipt = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(log.Log.TransactionHash);
                        var block = await _web3.Eth.Blocks.GetBlockWithTransactionsByHash.SendRequestAsync(receipt.BlockHash);
                        PositionDetail posd = new()
                        {
                            Id = Guid.NewGuid(),
                            PositionId = foundPosition.Id,
                            CollateralValue = log.Event.CollateralValue.ToString(),
                            EntryPrice = log.Event.EntryPrice.ToString(),
                            IndexPrice = log.Event.IndexPrice.ToString(),
                            ReserveAmount = log.Event.ReserveAmount.ToString(),
                            PositionState = PositionState.Close,
                            SizeChanged = log.Event.Size.ToString(),
                            Pnl = log.Event.Pnl.Sig == 1 ? log.Event.Pnl.Abs.ToString() : (~log.Event.Pnl.Abs + 1).ToString(),
                            Time = DateTimeOffset.FromUnixTimeSeconds((long)block.Timestamp.Value).LocalDateTime
                        };
                        foundPosition.LastUpdatedDate = DateTimeOffset.FromUnixTimeSeconds((long)block.Timestamp.Value).LocalDateTime;
                        foundPosition.Size = "0";
                        _unitOfWork.PositionRepository.Update(foundPosition);
                        await _unitOfWork.PositionDetailRepository.AddAsync(posd);
                        await _unitOfWork.SaveAsync();
                        _unitOfWork.Dispose();
                    }

                    foreach (var log in liquidatePositionEvents)
                    {
                        Console.WriteLine("[DEV-INF] Decoding a liquidate position event ...");
                        var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        string key = "0x" + BitConverter.ToString(log.Event.Key).Replace("-", "");
                        Position foundPosition = await _unitOfWork.PositionRepository.GetPositionInDetails(key);
                        var receipt = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(log.Log.TransactionHash);
                        var block = await _web3.Eth.Blocks.GetBlockWithTransactionsByHash.SendRequestAsync(receipt.BlockHash);
                        PositionDetail posd = new()
                        {
                            Id = Guid.NewGuid(),
                            PositionId = foundPosition.Id,
                            CollateralValue = log.Event.CollateralValue.ToString(),
                            EntryPrice = null,
                            IndexPrice = log.Event.IndexPrice.ToString(),
                            ReserveAmount = log.Event.ReserveAmount.ToString(),
                            PositionState = PositionState.Liquidate,
                            SizeChanged = log.Event.Size.ToString(),
                            Pnl = log.Event.Pnl.Sig == 1 ? log.Event.Pnl.Abs.ToString() : (~log.Event.Pnl.Abs + 1).ToString(),
                            Time = DateTimeOffset.FromUnixTimeSeconds((long)block.Timestamp.Value).LocalDateTime
                        };
                        foundPosition.LastUpdatedDate = DateTimeOffset.FromUnixTimeSeconds((long)block.Timestamp.Value).LocalDateTime;
                        foundPosition.Size = "0";
                        _unitOfWork.PositionRepository.Update(foundPosition);
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
                if (_latestBlockNumber - _currentCommonBlockNumber < _limitBlockNumber)
                {
                    _currentCommonBlockNumber += _latestBlockNumber - _currentCommonBlockNumber;
                }
                else
                {
                    _currentCommonBlockNumber += _limitBlockNumber;
                }
                isFirstParam = !isFirstParam;
                return new BlockParameter(new HexBigInteger(_currentCommonBlockNumber));
            }
            else
            {
                if (_latestBlockNumber - _currentReporterBlockNumber < _limitBlockNumber)
                {
                    _currentReporterBlockNumber += _latestBlockNumber - _currentReporterBlockNumber;
                }
                else
                {
                    _currentReporterBlockNumber += _limitBlockNumber;
                }
                isFirstParam = !isFirstParam;
                return new BlockParameter(new HexBigInteger(_currentReporterBlockNumber));
            }
        }
    }
}

