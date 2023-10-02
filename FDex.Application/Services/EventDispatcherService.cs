using System;
using AutoMapper;
using FDex.Application.Contracts.Persistence;
using FDex.Application.DTOs.Liquidity;
using FDex.Application.DTOs.Swap;
using FDex.Domain.Entities;
using Microsoft.Extensions.Hosting;
using Nethereum.Contracts;
using Nethereum.JsonRpc.Client;
using Nethereum.JsonRpc.WebSocketStreamingClient;
using Nethereum.RPC.Reactive.Eth.Subscriptions;
using Org.BouncyCastle.Asn1.Ocsp;

namespace FDex.Application.Services
{
    public class EventDispatcherService : BackgroundService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EventDispatcherService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var client = new StreamingWebSocketClient("wss://bsc-testnet.publicnode.com");
                var swapFilterInput = Event<SwapDTO>.GetEventABI().CreateFilterInput();
                var addLiquidityFilterInput = Event<AddLiquidityDTO>.GetEventABI().CreateFilterInput();
                var subscription = new EthLogsObservableSubscription(client);
                var sub = subscription.GetSubscriptionDataResponsesAsObservable();
                sub.Subscribe(async log =>
                {
                    try
                    {
                        var decodedSwap = Event<SwapDTO>.DecodeEvent(log);
                        var decodedAddLiquidity = Event<AddLiquidityDTO>.DecodeEvent(log);
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
                try
                {
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
                    Console.WriteLine($"[DEV-ERR] WebSocket error: {ex.Message}");
                }
            }
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
                await _unitOfWork.Save();
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
                Fee = decodedSwap.Event.Fee * decodedSwap.Event.MarkPrice
            };
            Swap swap = _mapper.Map<Swap>(swapDTOAdd);
            await _unitOfWork.SwapRepository.AddAsync(swap);
            await _unitOfWork.Save();
        }
    }
}

