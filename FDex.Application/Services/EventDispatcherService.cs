using System;
using System.Collections;
using System.Numerics;
using System.Text;
using AutoMapper;
using FDex.Application.Contracts.Persistence;
using FDex.Application.DTOs.Liquidity;
using FDex.Application.DTOs.Reporter;
using FDex.Application.DTOs.Swap;
using FDex.Application.DTOs.TradingPosition;
using FDex.Domain.Entities;
using FDex.Domain.Enumerations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nethereum.ABI.Model;
using Nethereum.Contracts;
using Nethereum.JsonRpc.Client;
using Nethereum.JsonRpc.WebSocketStreamingClient;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Reactive.Eth.Subscriptions;

namespace FDex.Application.Services
{
    public class EventDispatcherService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMapper _mapper;
        private readonly List<string> _wss;
        private int currentWssIndex;

        public EventDispatcherService(IServiceProvider serviceProvider, IMapper mapper)
        {
            _serviceProvider = serviceProvider;
            _mapper = mapper;
            _wss = new()
            {
                "wss://bsc-testnet.publicnode.com",
                "wss://bsc.getblock.io/c9c2311d-f632-47b1-ae8f-7cde9cd02fba/testnet/",
                "wss://bsc.getblock.io/034447a0-2d47-427d-a049-27bca272aeb0/testnet/",
                "wss://bsc-testnet.public.blastapi.io",
                "wss://sly-long-cherry.bsc-testnet.quiknode.pro/4ac0090884736ecd32a595fe2ec55910ca239cdb/"

            };
            currentWssIndex = 0;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await using var scope = _serviceProvider.CreateAsyncScope();
                try
                {
                    StreamingWebSocketClient client = new(HandleWebsocketString());
                    NewFilterInput swapFilterInput = Event<SwapDTO>.GetEventABI().CreateFilterInput();
                    NewFilterInput addLiquidityFilterInput = Event<AddLiquidityDTO>.GetEventABI().CreateFilterInput();
                    NewFilterInput reporterAddedFilterInput = Event<ReporterAddedDTO>.GetEventABI().CreateFilterInput();
                    NewFilterInput reporterRemovedFilterInput = Event<ReporterRemovedDTO>.GetEventABI().CreateFilterInput();
                    NewFilterInput reporterPostedFilterInput = Event<ReporterPostedDTO>.GetEventABI().CreateFilterInput();
                    NewFilterInput openPositionFilterInput = Event<FDexOpenPositionDTO>.GetEventABI().CreateFilterInput();
                    NewFilterInput increasePositionFilterInput = Event<FDexIncreaPositionDTO>.GetEventABI().CreateFilterInput();
                    NewFilterInput decreasePositionFilterInput = Event<FDexDecreaPositionDTO>.GetEventABI().CreateFilterInput();
                    NewFilterInput closePositionFilterInput = Event<FDexClosePositionDTO>.GetEventABI().CreateFilterInput();
                    NewFilterInput liquidatePositionFilterInput = Event<LiquidatePositionDTO>.GetEventABI().CreateFilterInput();

                    EthLogsObservableSubscription swapSubscription = new(client);
                    EthLogsObservableSubscription addLiquiditySubscription = new(client);
                    EthLogsObservableSubscription reporterAddedSubscription = new(client);
                    EthLogsObservableSubscription reporterRemovedSubscription = new(client);
                    EthLogsObservableSubscription reporterPostedSubscription = new(client);
                    EthLogsObservableSubscription openPositionSubscription = new(client);
                    EthLogsObservableSubscription increasePositionSubscription = new(client);
                    EthLogsObservableSubscription decreasePositionSubscription = new(client);
                    EthLogsObservableSubscription closePositionSubscription = new(client);
                    EthLogsObservableSubscription liquidatePositionSubscription = new(client);

                    swapSubscription.GetSubscriptionDataResponsesAsObservable().Subscribe(async log =>
                    {
                        Console.WriteLine("[DEV-INF] Decoding a swap event ...");
                        var decodedSwap = Event<SwapDTO>.DecodeEvent(log);
                        var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        SwapDTOAdd swapDTOAdd = new()
                        {
                            TxnHash = decodedSwap.Log.TransactionHash,
                            Wallet = decodedSwap.Event.Wallet,
                            TokenIn = decodedSwap.Event.TokenIn,
                            TokenOut = decodedSwap.Event.TokenOut,
                            AmountIn = decodedSwap.Event.AmountIn,
                            AmountOut = decodedSwap.Event.AmountOut,
                            Fee = decodedSwap.Event.Fee * decodedSwap.Event.MarkPrice,
                            Time = DateTime.Now
                        };
                        Swap swap = _mapper.Map<Swap>(swapDTOAdd);
                        await _unitOfWork.SwapRepository.AddAsync(swap);
                        var foundUser = await _unitOfWork.UserRepository.FindAsync(decodedSwap.Event.Wallet);
                        if (foundUser == null)
                        {
                            User user = new()
                            {
                                Wallet = decodedSwap.Event.Wallet,
                                CreatedDate = DateTime.Now
                            };
                            await _unitOfWork.UserRepository.AddAsync(user);
                        }
                        await _unitOfWork.SaveAsync();
                        _unitOfWork.Dispose();
                    });

                    reporterAddedSubscription.GetSubscriptionDataResponsesAsObservable().Subscribe(async log =>
                    {
                        Console.WriteLine("[DEV-INF] Decoding a reporter added event ...");
                        var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        var decodedReporterAdded = Event<ReporterAddedDTO>.DecodeEvent(log);
                        var foundReporterAdded = await _unitOfWork.ReporterRepository.FindAsync(decodedReporterAdded.Event.Wallet);
                        if (foundReporterAdded == null)
                        {
                            await _unitOfWork.ReporterRepository.AddAsync(new Reporter { Wallet = decodedReporterAdded.Event.Wallet, ReportCount = 0 });
                        }
                        await _unitOfWork.SaveAsync();
                        _unitOfWork.Dispose();
                    });

                    reporterRemovedSubscription.GetSubscriptionDataResponsesAsObservable().Subscribe(async log =>
                    {
                        Console.WriteLine("[DEV-INF] Decoding a reporter removed event ...");
                        var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        var decodedReporterRemoved = Event<ReporterRemovedDTO>.DecodeEvent(log);
                        var foundReporterRemoved = await _unitOfWork.ReporterRepository.FindAsync(decodedReporterRemoved.Event.Wallet);
                        if (foundReporterRemoved != null)
                        {
                            _unitOfWork.ReporterRepository.Remove(foundReporterRemoved);
                        }
                        await _unitOfWork.SaveAsync();
                        _unitOfWork.Dispose();
                    });

                    reporterPostedSubscription.GetSubscriptionDataResponsesAsObservable().Subscribe(async log =>
                    {
                        Console.WriteLine("[DEV-INF] Decoding a reporter posted event ...");
                        var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        var decodedReporterPosted = Event<ReporterPostedDTO>.DecodeEvent(log);
                        var foundReporterPosted = await _unitOfWork.ReporterRepository.FindAsync(decodedReporterPosted.Event.Wallet);
                        if (foundReporterPosted != null)
                        {
                            Reporter postingReporter = await _unitOfWork.ReporterRepository.FindAsync(decodedReporterPosted.Event.Wallet);
                            postingReporter.ReportCount += 1;
                            postingReporter.LastReportedDate = DateTime.Now;
                            _unitOfWork.ReporterRepository.Update(postingReporter);
                        }
                        await _unitOfWork.SaveAsync();
                        _unitOfWork.Dispose();
                    });

                    addLiquiditySubscription.GetSubscriptionDataResponsesAsObservable().Subscribe(async log =>
                    {
                        Console.WriteLine("[DEV-INF] Decoding an add liquidity event ...");
                        var decodedAddLiquidity = Event<AddLiquidityDTO>.DecodeEvent(log);
                        var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        var foundUser = await _unitOfWork.UserRepository.FindAsync(decodedAddLiquidity.Event.Wallet);
                        if (foundUser == null)
                        {
                            User user = new()
                            {
                                Wallet = decodedAddLiquidity.Event.Wallet,
                                CreatedDate = DateTime.Now
                            };
                            await _unitOfWork.UserRepository.AddAsync(user);
                        }
                        AddLiquidityDTOAdd addLiquidityDTOAdd = new()
                        {
                            TxnHash = decodedAddLiquidity.Log.TransactionHash,
                            Wallet = decodedAddLiquidity.Event.Wallet,
                            Asset = decodedAddLiquidity.Event.Asset,
                            Amount = decodedAddLiquidity.Event.Amount,
                            Fee = decodedAddLiquidity.Event.Fee * decodedAddLiquidity.Event.MarkPriceIn,
                            DateAdded = DateTime.Now
                        };
                        AddLiquidity addLiquidity = _mapper.Map<AddLiquidity>(addLiquidityDTOAdd);
                        await _unitOfWork.AddLiquidityRepository.AddAsync(addLiquidity);
                        await _unitOfWork.SaveAsync();
                        _unitOfWork.Dispose();
                    });

                    openPositionSubscription.GetSubscriptionDataResponsesAsObservable().Subscribe(async log =>
                    {
                        Console.WriteLine("[DEV-INF] Decoding an open position event ...");
                        var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        var decodedOpenPosition = Event<FDexOpenPositionDTO>.DecodeEvent(log);
                        var foundUser = await _unitOfWork.UserRepository.FindAsync(decodedOpenPosition.Event.Wallet);
                        if (foundUser == null)
                        {
                            User user = new()
                            {
                                Wallet = decodedOpenPosition.Event.Wallet,
                                CreatedDate = DateTime.Now
                            };
                            await _unitOfWork.UserRepository.AddAsync(user);
                        }
                        string key = BitConverter.ToString(decodedOpenPosition.Event.Key).Replace("-", "");
                        Position foundPosition = await _unitOfWork.PositionRepository.GetPositionInDetails(key);
                        if (foundPosition == null)
                        {
                            Position pos = new()
                            {
                                Id = Guid.NewGuid(),
                                Key = key,
                                Wallet = decodedOpenPosition.Event.Wallet,
                                CollateralToken = decodedOpenPosition.Event.CollateralToken,
                                IndexToken = decodedOpenPosition.Event.IndexToken,
                                Side = decodedOpenPosition.Event.Side == '1',
                                Size = decodedOpenPosition.Event.Size.ToString(),
                                Leverage = (int)(decodedOpenPosition.Event.Size / decodedOpenPosition.Event.CollateralValue)
                            };
                            await _unitOfWork.PositionRepository.AddAsync(pos);
                            PositionDetail posd = new()
                            {
                                Id = Guid.NewGuid(),
                                PositionId = pos.Id,
                                CollateralValue = decodedOpenPosition.Event.CollateralValue.ToString(),
                                EntryPrice = decodedOpenPosition.Event.EntryPrice.ToString(),
                                IndexPrice = decodedOpenPosition.Event.IndexPrice.ToString(),
                                ReserveAmount = decodedOpenPosition.Event.ReserveAmount.ToString(),
                                PositionState = PositionState.Open,
                                SizeChanged = decodedOpenPosition.Event.SizeChanged.ToString(),
                                FeeValue = decodedOpenPosition.Event.FeeValue.ToString(),
                                Time = DateTime.Now
                            };
                            await _unitOfWork.PositionDetailRepository.AddAsync(posd);
                        }
                        else
                        {
                            PositionDetail posd = new()
                            {
                                Id = Guid.NewGuid(),
                                PositionId = foundPosition.Id,
                                CollateralValue = decodedOpenPosition.Event.CollateralValue.ToString(),
                                EntryPrice = decodedOpenPosition.Event.EntryPrice.ToString(),
                                IndexPrice = decodedOpenPosition.Event.IndexPrice.ToString(),
                                ReserveAmount = decodedOpenPosition.Event.ReserveAmount.ToString(),
                                PositionState = PositionState.Open,
                                SizeChanged = decodedOpenPosition.Event.SizeChanged.ToString(),
                                FeeValue = decodedOpenPosition.Event.FeeValue.ToString(),
                                Time = DateTime.Now
                            };
                            await _unitOfWork.PositionDetailRepository.AddAsync(posd);
                        }
                        await _unitOfWork.SaveAsync();
                        _unitOfWork.Dispose();
                    });

                    increasePositionSubscription.GetSubscriptionDataResponsesAsObservable().Subscribe(async log =>
                    {
                        Console.WriteLine("[DEV-INF] Decoding an increase position event ...");
                        var decodedIncreasePosition = Event<FDexIncreaPositionDTO>.DecodeEvent(log);
                        var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        string key = BitConverter.ToString(decodedIncreasePosition.Event.Key).Replace("-", "");
                        Position foundPosition = await _unitOfWork.PositionRepository.GetPositionInDetails(key);
                        PositionDetail posd = new()
                        {
                            Id = Guid.NewGuid(),
                            PositionId = foundPosition.Id,
                            CollateralValue = decodedIncreasePosition.Event.CollateralValue.ToString(),
                            EntryPrice = decodedIncreasePosition.Event.EntryPrice.ToString(),
                            IndexPrice = decodedIncreasePosition.Event.IndexPrice.ToString(),
                            ReserveAmount = decodedIncreasePosition.Event.ReserveAmount.ToString(),
                            PositionState = PositionState.Increase,
                            SizeChanged = decodedIncreasePosition.Event.SizeChanged.ToString(),
                            FeeValue = decodedIncreasePosition.Event.FeeValue.ToString(),
                            Time = DateTime.Now
                        };
                        await _unitOfWork.PositionDetailRepository.AddAsync(posd);

                        await _unitOfWork.SaveAsync();
                        _unitOfWork.Dispose();
                    });

                    decreasePositionSubscription.GetSubscriptionDataResponsesAsObservable().Subscribe(async log =>
                    {
                        Console.WriteLine("[DEV-INF] Decoding a decrease position event ...");
                        var decodedDecreasePosition = Event<FDexDecreaPositionDTO>.DecodeEvent(log);
                        var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        string key = BitConverter.ToString(decodedDecreasePosition.Event.Key).Replace("-", "");
                        Position foundPosition = await _unitOfWork.PositionRepository.GetPositionInDetails(key);

                        PositionDetail posd = new()
                        {
                            Id = Guid.NewGuid(),
                            PositionId = foundPosition.Id,
                            CollateralValue = (~decodedDecreasePosition.Event.CollateralChanged + 1).ToString(),
                            EntryPrice = decodedDecreasePosition.Event.EntryPrice.ToString(),
                            IndexPrice = decodedDecreasePosition.Event.IndexPrice.ToString(),
                            ReserveAmount = decodedDecreasePosition.Event.ReserveAmount.ToString(),
                            PositionState = PositionState.Decrease,
                            SizeChanged = decodedDecreasePosition.Event.SizeChanged.ToString(),
                            FeeValue = decodedDecreasePosition.Event.FeeValue.ToString(),
                            Time = DateTime.Now
                        };
                        await _unitOfWork.PositionDetailRepository.AddAsync(posd);

                        await _unitOfWork.SaveAsync();
                        _unitOfWork.Dispose();
                    });


                    closePositionSubscription.GetSubscriptionDataResponsesAsObservable().Subscribe(async log =>
                    {
                        Console.WriteLine("[DEV-INF] Decoding a close position event ...");
                        var decodedClosePosition = Event<FDexClosePositionDTO>.DecodeEvent(log);
                        var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        string key = BitConverter.ToString(decodedClosePosition.Event.Key).Replace("-", "");
                        Position foundPosition = await _unitOfWork.PositionRepository.GetPositionInDetails(key);

                        PositionDetail posd = new()
                        {
                            Id = Guid.NewGuid(),
                            PositionId = foundPosition.Id,
                            CollateralValue = (~decodedClosePosition.Event.CollateralValue + 1).ToString(),
                            EntryPrice = decodedClosePosition.Event.EntryPrice.ToString(),
                            IndexPrice = decodedClosePosition.Event.IndexPrice.ToString(),
                            ReserveAmount = decodedClosePosition.Event.ReserveAmount.ToString(),
                            PositionState = PositionState.Close,
                            SizeChanged = decodedClosePosition.Event.Size.ToString(),
                            Pnl = decodedClosePosition.Event.Pnl.Sig == 1 ? decodedClosePosition.Event.Pnl.Abs.ToString() : (~decodedClosePosition.Event.Pnl.Abs + 1).ToString(),
                            Time = DateTime.Now
                        };
                        await _unitOfWork.PositionDetailRepository.AddAsync(posd);

                        await _unitOfWork.SaveAsync();
                        _unitOfWork.Dispose();
                    });

                    liquidatePositionSubscription.GetSubscriptionDataResponsesAsObservable().Subscribe(async log =>
                    {
                        Console.WriteLine("[DEV-INF] Decoding a liquidate position event ...");
                        var decodedLiquidatePosition = Event<LiquidatePositionDTO>.DecodeEvent(log);
                        var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        string key = BitConverter.ToString(decodedLiquidatePosition.Event.Key).Replace("-", "");
                        Position foundPosition = await _unitOfWork.PositionRepository.GetPositionInDetails(key);

                        PositionDetail posd = new()
                        {
                            Id = Guid.NewGuid(),
                            PositionId = foundPosition.Id,
                            CollateralValue = (~decodedLiquidatePosition.Event.CollateralValue + 1).ToString(),
                            EntryPrice = null,
                            IndexPrice = decodedLiquidatePosition.Event.IndexPrice.ToString(),
                            ReserveAmount = decodedLiquidatePosition.Event.ReserveAmount.ToString(),
                            PositionState = PositionState.Liquidate,
                            SizeChanged = decodedLiquidatePosition.Event.Size.ToString(),
                            Pnl = decodedLiquidatePosition.Event.Pnl.Sig == 1 ? decodedLiquidatePosition.Event.Pnl.Abs.ToString() : (~decodedLiquidatePosition.Event.Pnl.Abs + 1).ToString(),
                            Time = DateTime.Now
                        };
                        await _unitOfWork.PositionDetailRepository.AddAsync(posd);

                        await _unitOfWork.SaveAsync();
                        _unitOfWork.Dispose();
                    });

                    await client.StartAsync();
                    await swapSubscription.SubscribeAsync(swapFilterInput);
                    await reporterAddedSubscription.SubscribeAsync(reporterAddedFilterInput);
                    await reporterRemovedSubscription.SubscribeAsync(reporterRemovedFilterInput);
                    await reporterPostedSubscription.SubscribeAsync(reporterPostedFilterInput);
                    await increasePositionSubscription.SubscribeAsync(increasePositionFilterInput);
                    await decreasePositionSubscription.SubscribeAsync(decreasePositionFilterInput);
                    await openPositionSubscription.SubscribeAsync(openPositionFilterInput);
                    await closePositionSubscription.SubscribeAsync(closePositionFilterInput);
                    await liquidatePositionSubscription.SubscribeAsync(liquidatePositionFilterInput);
                    while (true)
                    {
                        Console.WriteLine("[DEV-INF] Client state: " + client.WebSocketState);
                        if (client.WebSocketState == System.Net.WebSockets.WebSocketState.Aborted || client.WebSocketState == System.Net.WebSockets.WebSocketState.Closed)
                        {
                            //restart client
                            Console.WriteLine($"[DEV-INF] Client {client.WebSocketState}, restarting ...");
                            await swapSubscription.UnsubscribeAsync();
                            await reporterAddedSubscription.UnsubscribeAsync();
                            await reporterRemovedSubscription.UnsubscribeAsync();
                            await reporterPostedSubscription.UnsubscribeAsync();
                            await openPositionSubscription.UnsubscribeAsync();
                            await increasePositionSubscription.UnsubscribeAsync();
                            await decreasePositionSubscription.UnsubscribeAsync();
                            await closePositionSubscription.UnsubscribeAsync();
                            await liquidatePositionSubscription.UnsubscribeAsync();
                            await client.StopAsync();
                            await client.StartAsync();
                            await swapSubscription.SubscribeAsync(swapFilterInput);
                            await reporterAddedSubscription.SubscribeAsync(reporterAddedFilterInput);
                            await reporterRemovedSubscription.SubscribeAsync(reporterRemovedFilterInput);
                            await reporterPostedSubscription.SubscribeAsync(reporterPostedFilterInput);
                            await openPositionSubscription.SubscribeAsync(openPositionFilterInput);
                            await increasePositionSubscription.SubscribeAsync(increasePositionFilterInput);
                            await decreasePositionSubscription.SubscribeAsync(decreasePositionFilterInput);
                            await closePositionSubscription.SubscribeAsync(closePositionFilterInput);
                            await liquidatePositionSubscription.SubscribeAsync(liquidatePositionFilterInput);
                        }
                        await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DEV-ERR] WebSocket error: {ex.Message}, switching ...");
                }
            }
        }

        private string HandleWebsocketString()
        {
            string wss = _wss[currentWssIndex];
            currentWssIndex = (currentWssIndex + 1) % _wss.Count;
            return wss;
        }
    }
}