﻿using System;
using AutoMapper;
using FDex.Application.Contracts.Persistence;
using FDex.Application.DTOs.Liquidity;
using FDex.Application.DTOs.Reporter;
using FDex.Application.DTOs.Swap;
using FDex.Application.Enumerations;
using FDex.Domain.Entities;
using Microsoft.Extensions.Hosting;
using Nethereum.Contracts;
using Nethereum.JsonRpc.Client;
using Nethereum.JsonRpc.WebSocketStreamingClient;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Reactive.Eth.Subscriptions;
using Org.BouncyCastle.Asn1.Ocsp;

namespace FDex.Application.Services
{
    public class EventDispatcherService : BackgroundService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly List<string> _wss;
        private int currentWssIndex;

        public EventDispatcherService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
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
                try
                {
                    StreamingWebSocketClient client = new(HandleWebsocketString());
                    NewFilterInput swapFilterInput = Event<SwapDTO>.GetEventABI().CreateFilterInput();
                    NewFilterInput addLiquidityFilterInput = Event<AddLiquidityDTO>.GetEventABI().CreateFilterInput();
                    NewFilterInput reporterAddedFilterInput = Event<ReporterAddedDTO>.GetEventABI().CreateFilterInput();
                    NewFilterInput reporterRemovedFilterInput = Event<ReporterRemovedDTO>.GetEventABI().CreateFilterInput();
                    NewFilterInput reporterPostedFilterInput = Event<ReporterPostedDTO>.GetEventABI().CreateFilterInput();
                    EthLogsObservableSubscription subscription = new(client);
                    var sub = subscription.GetSubscriptionDataResponsesAsObservable();
                    sub.Subscribe(async log =>
                    {
                        try
                        {
                            var decodedSwap = Event<SwapDTO>.DecodeEvent(log);
                            var decodedAddLiquidity = Event<AddLiquidityDTO>.DecodeEvent(log);
                            var decodedReporterAdded = Event<ReporterAddedDTO>.DecodeEvent(log);
                            var decodedReporterRemoved = Event<ReporterRemovedDTO>.DecodeEvent(log);
                            var decodedReporterPosted = Event<ReporterPostedDTO>.DecodeEvent(log);
                            if (decodedSwap != null)
                            {
                                Console.WriteLine("[DEV-INF] Decoding a swap event ...");
                                await StoreSwapEventAsync(decodedSwap);
                                await StoreUserAsync(decodedSwap.Event.Wallet);
                            }
                            else if (decodedAddLiquidity != null)
                            {
                                Console.WriteLine("[DEV-INF] Decoding an add liquidity event ...");
                                await StoreUserAsync(decodedAddLiquidity.Event.Wallet);
                                await StoreAddLiquidityEvent(decodedAddLiquidity);
                            }
                            else if (decodedReporterAdded != null)
                            {
                                Console.WriteLine("[DEV-INF] Decoding an reporter added event ...");
                                await HandleReporterEvent(decodedReporterAdded.Event.Wallet, ReporterEventType.Added);
                            }
                            else if (decodedReporterRemoved != null)
                            {
                                Console.WriteLine("[DEV-INF] Decoding an reporter removed event ...");
                                await HandleReporterEvent(decodedReporterRemoved.Event.Wallet, ReporterEventType.Removed);
                            }
                            else if (decodedReporterPosted != null)
                            {
                                Console.WriteLine("[DEV-INF] Decoding an reporter posted event ...");
                                await HandleReporterEvent(decodedReporterPosted.Event.Wallet, ReporterEventType.Posted);
                            }
                            else
                            {
                                Console.WriteLine("[DEV-INF] Found not standard event log");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("[DEV-ERR] Log Address: " + log.Address + " is not a standard swap log:", ex.Message);
                        }
                    });
                    await client.StartAsync();
                    await subscription.SubscribeAsync(swapFilterInput);
                    while (true)
                    {
                        Console.WriteLine("[DEV-INF] Client state: " + client.WebSocketState);
                        if (client.WebSocketState == System.Net.WebSockets.WebSocketState.Aborted)
                        {
                            //restart client
                            Console.WriteLine("[DEV-INF] Client aborted, restarting ...");
                            await subscription.UnsubscribeAsync();
                            await client.StopAsync();
                            await client.StartAsync();
                            await subscription.SubscribeAsync(swapFilterInput);
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

        private async Task StoreAddLiquidityEvent(EventLog<AddLiquidityDTO> decodedAddLiquidity)
        {
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
        }

        private string HandleWebsocketString()
        {
            string wss = _wss[currentWssIndex];
            currentWssIndex = (currentWssIndex + 1) % _wss.Count;
            return wss;
        }

        private async Task HandleReporterEvent(string wallet, ReporterEventType reporterEvent)
        {
            switch (reporterEvent)
            {
                case ReporterEventType.Added:
                    var foundReporterAdded = await _unitOfWork.ReporterRepository.FindAsync(wallet);
                    if (foundReporterAdded == null)
                    {
                        await _unitOfWork.ReporterRepository.AddAsync(new Reporter { Wallet = wallet, ReportCount = 0 });
                    }
                    break;
                case ReporterEventType.Removed:
                    var foundReporterRemoved = await _unitOfWork.ReporterRepository.FindAsync(wallet);
                    if (foundReporterRemoved != null)
                    {
                        _unitOfWork.ReporterRepository.Remove(foundReporterRemoved);
                    }
                    break;
                case ReporterEventType.Posted:
                    var foundReporterPosted = await _unitOfWork.ReporterRepository.FindAsync(wallet);
                    if (foundReporterPosted != null)
                    {
                        Reporter postingReporter = await _unitOfWork.ReporterRepository.FindAsync(wallet);
                        postingReporter.ReportCount += 1;
                        postingReporter.LastReportedDate = DateTime.Now;
                        _unitOfWork.ReporterRepository.Update(postingReporter);
                    }
                    break;
            }
            await _unitOfWork.SaveAsync();
        }

        private async Task StoreUserAsync(string wallet)
        {
            var foundUser = await _unitOfWork.UserRepository.FindAsync(wallet);
            if (foundUser == null)
            {
                User user = new()
                {
                    Wallet = wallet,
                    CreatedDate = DateTime.Now
                };
                await _unitOfWork.UserRepository.AddAsync(user);
                await _unitOfWork.SaveAsync();
            }
        }

        private async Task StoreSwapEventAsync(EventLog<SwapDTO> decodedSwap)
        {
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
            await _unitOfWork.SaveAsync();
        }
    }
}

