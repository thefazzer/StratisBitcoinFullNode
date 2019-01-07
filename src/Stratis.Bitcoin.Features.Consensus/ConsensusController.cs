﻿using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NBitcoin;
using Stratis.Bitcoin.Base;
using Stratis.Bitcoin.Base.Deployments;
using Stratis.Bitcoin.Consensus;
using Stratis.Bitcoin.Consensus.Rules;
using Stratis.Bitcoin.Controllers;
using Stratis.Bitcoin.Features.Consensus.Rules.CommonRules;
using Stratis.Bitcoin.Utilities;
using Stratis.Bitcoin.Utilities.JsonErrors;

namespace Stratis.Bitcoin.Features.Consensus
{
    /// <summary>
    /// A <see cref="FeatureController"/> that provides API and RPC methods from the consensus loop.
    /// </summary>
    public class ConsensusController : FeatureController
    {
        /// <summary>Instance logger.</summary>
        private readonly ILogger logger;

        public ConsensusController(
            ILoggerFactory loggerFactory,
            IChainState chainState,
            IConsensusManager consensusManager,
            ConcurrentChain chain)
            : base(chainState: chainState, consensusManager: consensusManager, chain: chain)
        {
            Guard.NotNull(loggerFactory, nameof(loggerFactory));
            Guard.NotNull(chain, nameof(chain));
            Guard.NotNull(chainState, nameof(chainState));

            this.logger = loggerFactory.CreateLogger(this.GetType().FullName);
        }

        /// <summary>
        /// Implements the getbestblockhash RPC call.
        /// </summary>
        /// <returns>A <see cref="uint256"/> hash of the block at the consensus tip.</returns>
        [ActionName("getbestblockhash")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ActionDescription("Get the hash of the block at the consensus tip.")]
        public uint256 GetBestBlockHashRPC()
        {
            return this.ChainState.ConsensusTip?.HashBlock;
        }

        /// <summary>
        /// Get the threshold states of softforks currently being deployed.
        /// Allowable states are: Defined, Started, LockedIn, Failed, Active.
        /// </summary>
        /// <returns>Json formatted type with Deployment Index <see cref="int"/>, 
        /// State Value <see cref="ThresholdState"/>, human readable Threshold State <see cref="string"/>
        /// Returns <see cref="IActionResult"/> formatted error if fails.
        /// </returns>.
        [Route("api/[controller]/getdeploymentflags")]
        [HttpGet]
        public IActionResult GetDeploymentFlags()
        {
            try
            {
               var rule = this.ConsensusManager.ConsensusRules.GetRule<SetActivationDeploymentsFullValidationRule>();
               
               // Ensure threshold conditions cached.
               ThresholdState[] thresholdStates = rule.Parent.NodeDeployments.BIP9.GetStates(this.ChainState.ConsensusTip.Previous);

               object metrics = rule.Parent.NodeDeployments.BIP9.EnrichStatesWithBlockMetrics(thresholdStates);

               return this.Json(metrics);
            }
            catch (Exception e)
            {
                this.logger.LogError("Exception occurred: {0}", e.ToString());
                return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, e.Message, e.ToString());
            }
        }
        
        /// <summary>
        /// Get the hash of the block at the consensus tip.
        /// API wrapper of RPC call.
        /// </summary>
        /// <returns>Json formatted <see cref="uint256"/> hash of the block at the consensus tip. Returns <see cref="IActionResult"/> formatted error if fails.</returns>
        [Route("api/[controller]/getbestblockhash")]
        [HttpGet]
        public IActionResult GetBestBlockHashAPI()
        {
            try
            {
                return this.Json(this.GetBestBlockHashRPC());
            }
            catch (Exception e)
            {
                this.logger.LogError("Exception occurred: {0}", e.ToString());
                return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, e.Message, e.ToString());
            }
        }

        /// <summary>
        /// Implements the getblockhash RPC call.
        /// </summary>
        /// <param name="height">The requested block height.</param>
        /// <returns>A <see cref="uint256"/> hash of the block at the given height. <c>Null</c> if block not found.</returns>
        [ActionName("getblockhash")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ActionDescription("Gets the hash of the block at the given height.")]
        public uint256 GetBlockHashRPC(int height)
        {
            this.logger.LogDebug("GetBlockHash {0}", height);

            uint256 bestBlockHash = this.ConsensusManager.Tip?.HashBlock;
            ChainedHeader bestBlock = bestBlockHash == null ? null : this.Chain.GetBlock(bestBlockHash);
            if (bestBlock == null)
                return null;
            ChainedHeader block = this.Chain.GetBlock(height);
            return block == null || block.Height > bestBlock.Height ? null : block.HashBlock;
        }

        /// <summary>
        /// Gets the hash of the block at the given height.
        /// API wrapper of RPC call.
        /// </summary>
        /// <param name="request">A <see cref="GetBlockHashRequestModel"/> request containing the height.</param>
        /// <returns>Json formatted <see cref="uint256"/> hash of the block at the given height. <c>Null</c> if block not found. Returns <see cref="IActionResult"/> formatted error if fails.</returns>
        [Route("api/[controller]/getblockhash")]
        [HttpGet]
        public IActionResult GetBlockHashAPI([FromQuery] int height)
        {
            try
            {
                return this.Json(this.GetBlockHashRPC(height));
            }
            catch (Exception e)
            {
                this.logger.LogError("Exception occurred: {0}", e.ToString());
                return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, e.Message, e.ToString());
            }
        }
    }
}
