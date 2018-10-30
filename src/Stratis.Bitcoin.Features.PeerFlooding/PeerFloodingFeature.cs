using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NBitcoin;
using Stratis.Bitcoin.Builder;
using Stratis.Bitcoin.Builder.Feature;
using Stratis.Bitcoin.Configuration;
using Stratis.Bitcoin.Configuration.Logging;
using Stratis.Bitcoin.Connection;
using Stratis.Bitcoin.Features.Consensus;
using Stratis.Bitcoin.Features.PeerFlooding.Controllers;
using Stratis.Bitcoin.Features.RPC;
using Stratis.Bitcoin.Features.RPC.Controllers;

namespace Stratis.Bitcoin.Features.PeerFlooding
{
    public class PeerFloodingFeature : FullNodeFeature
    {
        private readonly FullNode fullNode;

        private readonly NodeSettings nodeSettings;

        private readonly ILogger logger;

        private readonly IFullNodeBuilder fullNodeBuilder;

        public PeerFloodingFeature(IFullNodeBuilder fullNodeBuilder, FullNode fullNode, NodeSettings nodeSettings, ILoggerFactory loggerFactory)
        {
            this.fullNodeBuilder = fullNodeBuilder;
            this.fullNode = fullNode;
            this.nodeSettings = nodeSettings;
            this.logger = loggerFactory.CreateLogger(this.GetType().FullName);
        }

        public override Task InitializeAsync()
        {
            return Task.CompletedTask;
        }


    }

    public static class FullNodeBuilderBlockStoreExtension
    {
        public static IFullNodeBuilder UsePeerFlooding(this IFullNodeBuilder fullNodeBuilder)
        {
            LoggingConfiguration.RegisterFeatureNamespace<PeerFloodingFeature>("PeerFlooding");

            fullNodeBuilder.ConfigureFeature(features =>
            {
                features
                    .AddFeature<PeerFloodingFeature>()
                    .FeatureServices(services => { services.AddSingleton<PeerFloodingController>(); });
            });

            return fullNodeBuilder;
        }
    }
}