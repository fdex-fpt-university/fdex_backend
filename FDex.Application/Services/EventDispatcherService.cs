using System;
using AutoMapper;
using FDex.Application.Contracts.Persistence;
using FDex.Application.DTOs.Swap;
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

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async void GetLogsTokenSwapObservableSubscription() {
            using (var client = new StreamingWebSocketClient("wss://bsc-testnet.publicnode.com"))
            {
                var filterTransfers = Event<SwapDTO>.GetEventABI().CreateFilterInput();
                var subscription = new EthLogsObservableSubscription(client);
                subscription.GetSubscriptionDataResponsesAsObservable().Subscribe(log =>
                {
                    try
                    {
                        var decoded = Event<SwapDTO>.DecodeEvent(log);
                        if (decoded != null)
                        {
                            SwapDTOAdd swapDTOAdd = new()
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
                });
                await client.StartAsync();

                // begin receiving subscription data
                // data will be received on a background thread
                await subscription.SubscribeAsync(filterTransfers);
                while (true)
                {
                    await Task.Delay(TimeSpan.FromDays(1));
                }
            }

        }

    }
}

