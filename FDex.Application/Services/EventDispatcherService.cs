using System;
using AutoMapper;
using FDex.Application.Contracts.Persistence;
using FDex.Application.DTOs.Swap;
using FDex.Domain.Entities;
using Microsoft.Extensions.Hosting;
using Nethereum.Contracts;
using Nethereum.JsonRpc.WebSocketStreamingClient;
using Nethereum.RPC.Reactive.Eth.Subscriptions;

namespace FDex.Application.Services
{
	public class EventDispatcherService : IHostedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

		public EventDispatcherService(IUnitOfWork unitOfWork, IMapper mapper)
		{
            _unitOfWork = unitOfWork;
            _mapper = mapper;
		}

        //protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    while (!stoppingToken.IsCancellationRequested)
        //    {
        //        //await GetLogsTokenSwapObservableSubscription(stoppingToken);
        //        //using (var client = new StreamingWebSocketClient("wss://bsc-testnet.publicnode.com"))
        //        //{
        //        //    var filterTransfers = Event<SwapDTO>.GetEventABI().CreateFilterInput();
        //        //    var subscription = new EthLogsObservableSubscription(client);
        //        //    subscription.GetSubscriptionDataResponsesAsObservable().Subscribe(async log =>
        //        //    {
        //        //        SwapDTOAdd swapDTOAdd = new();
        //        //        try
        //        //        {
        //        //            var decoded = Event<SwapDTO>.DecodeEvent(log);
        //        //            if (decoded != null)
        //        //            {
        //        //                swapDTOAdd = new()
        //        //                {
        //        //                    Sender = decoded.Event.Sender,
        //        //                    TokenIn = decoded.Event.TokenIn,
        //        //                    TokenOut = decoded.Event.TokenOut,
        //        //                    AmountIn = decoded.Event.AmountIn,
        //        //                    AmountOut = decoded.Event.AmountOut,
        //        //                    Fee = decoded.Event.Fee
        //        //                };
        //        //            }
        //        //            else
        //        //            {
        //        //                Console.WriteLine("Found not standard swap log");
        //        //            }
        //        //        }
        //        //        catch (Exception ex)
        //        //        {
        //        //            Console.WriteLine("Log Address: " + log.Address + " is not a standard swap log:", ex.Message);
        //        //        }
        //        //        Swap swap = _mapper.Map<Swap>(swapDTOAdd);
        //        //        await _unitOfWork.SwapRepository.AddAsync(swap);
        //        //        await _unitOfWork.Save();

        //        //    });
        //        //    await client.StartAsync();
        //        //    await subscription.SubscribeAsync(filterTransfers);
        //        //    while (true)
        //        //    {
        //        //        await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        //        //    }
        //        //}
        //    }
        //}


        public async Task GetLogsTokenSwapObservableSubscription()
        {
            using (var client = new StreamingWebSocketClient("wss://bsc-testnet.publicnode.com"))
            {
                var filterTransfers = Event<SwapDTO>.GetEventABI().CreateFilterInput();
                var subscription = new EthLogsObservableSubscription(client);
                subscription.GetSubscriptionDataResponsesAsObservable().Subscribe(async log =>
                {
                    SwapDTOAdd swapDTOAdd = new();
                    try
                    {
                        var decoded = Event<SwapDTO>.DecodeEvent(log);
                        if (decoded != null)
                        {
                            swapDTOAdd = new()
                            {
                                Sender = decoded.Event.Sender,
                                TokenIn = decoded.Event.TokenIn,
                                TokenOut = decoded.Event.TokenOut,
                                AmountIn = decoded.Event.AmountIn,
                                AmountOut = decoded.Event.AmountOut,
                                Fee = decoded.Event.Fee
                            };
                        }
                        else
                        {
                            Console.WriteLine("Found not standard swap log");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Log Address: " + log.Address + " is not a standard swap log:", ex.Message);
                    }
                    Swap swap = _mapper.Map<Swap>(swapDTOAdd);
                    await _unitOfWork.SwapRepository.AddAsync(swap);
                    await _unitOfWork.Save();
                });
                await client.StartAsync();
                await subscription.SubscribeAsync(filterTransfers);
                while (true)
                {
                    await Task.Delay(TimeSpan.FromDays(1));
                }
            }

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

