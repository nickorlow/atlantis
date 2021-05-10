using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CavCash.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CavCash.Node.ASP.Controllers
{
    [ApiController]
    public class NodeController : ControllerBase
    {
        
        /*
         *
         *     case "transaction":
                        return await ReceiveTransaction(((JObject) message.Content).ToObject<Transaction>());

                    case "get_chain":
                        return await ReceiveChainRequest(((JObject) message.Content).ToObject<ChainRequest>());
                    
                    case "mined_block":
                        return await ReceiveMinedBlockResponse(((JObject) message.Content).ToObject<ChainResponse>());
                    
                    case "register_peer":
                        return await ReceivePeerRegistration(((JObject) message.Content).ToObject<Peer>());
         */
        [HttpPost]
        [Route("transaction")]
        public async Task<object> MakeTransaction(Transaction t)
        {
            CurrentNode.PendingTransactions.Add(t);
            return Ok();
        }
        
        [HttpPost]
        [Route("get_chain")]
        public async Task<object> GetBlockchain()
        {
            return Ok(CurrentNode.BlockChain);
        }
    }
}