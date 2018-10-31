
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NBitcoin;
using Stratis.Bitcoin.Base;
using Stratis.Bitcoin.Configuration;
using Stratis.Bitcoin.Consensus;
using Stratis.Bitcoin.Controllers;
using Stratis.Bitcoin.Controllers.Models;
using Stratis.Bitcoin.Features.Consensus.CoinViews;
using Stratis.Bitcoin.Features.Consensus.Rules.CommonRules;
using Stratis.Bitcoin.Features.Miner.Interfaces;
using Stratis.Bitcoin.Features.Miner.Staking;
using Stratis.Bitcoin.Features.PeerFlooding.Models;
using Stratis.Bitcoin.Features.Wallet;
using Stratis.Bitcoin.Features.Wallet.Models;
using Stratis.Bitcoin.Interfaces;
using Stratis.Bitcoin.Mining;
using Stratis.Bitcoin.Utilities;
using Stratis.Bitcoin.Utilities.Extensions;
using Script = NBitcoin.Script;

namespace Stratis.Bitcoin.Features.PeerFlooding.Controllers
{
    [Route("api/[controller]")]
    public class PeerFloodingController : Controller
    {
        private readonly ILogger logger;
        private readonly string walletName;
        private readonly string password;
        private readonly int totalruns;

        public PeerFloodingController(ILoggerFactory loggerFactory, int totalruns = 10000)
        {
            this.logger = loggerFactory.CreateLogger(this.GetType().FullName);
            this.totalruns = totalruns;
        }

        [Route("FloodPeersWithLowFeeTransactions")]
        [HttpPost]
        public async Task FloodPeersWithLowFeeTransactions([FromBody]RequestModels.PeerFloodingRequest request)
        {
            string walletName = request.WalletName;
            string password = request.WalletPassword;

            Random rand = new Random();
            string url = $"http://localhost:38221/api/wallet/addresses?walletname={walletName}&accountname=account 0";
            string[] addresses = url.GetJsonAsync<AddressesModel>().GetAwaiter().GetResult().Addresses.Select(add => add.Address).ToArray();

            int totalRuns = this.totalruns;
            for (int i = 0; i < totalRuns; i++)
            {
                var destinationAddress = addresses[rand.Next(addresses.Count())];
                this.logger.Log(LogLevel.Debug, $"{i}/{totalRuns} => destinationAddress");

                BuildTransactionRequest buildTransactionRequest = new BuildTransactionRequest
                {
                    WalletName = "walletNamefdskgjfdhgkjfdshgsd",
                    AccountName = "account 0",
                    AllowUnconfirmed = true,
                    Amount = "1",
                    FeeAmount = "",
                    Password = password,
                    DestinationAddress = destinationAddress
                };
                try
                {
                    dynamic newTransaction = await "http://localhost:38221/api/wallet/build-transaction"
                        .PostJsonAsync(buildTransactionRequest)
                        .ReceiveJson();
                
                Console.WriteLine(newTransaction);

                var response = await "http://localhost:38221/api/wallet/send-transaction"
                        .PostJsonAsync(new SendTransactionRequest(newTransaction.hex));

                    //Thread.Sleep(5000);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("code 400"))
                    {
                        Thread.Sleep(600);
                    }
                }
            }
        }
    }
}
