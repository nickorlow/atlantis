using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CavCash.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CavCash.BlockExplorer
{
    public class CryptoUtils
    {
        
    }

    public class CavWallet
    {  
        public Core.Wallet MyWallet { get; set; }

        public CavWallet()
        {
            MyWallet = new Wallet();
        }

    
    }

    public class CavBlockchain
    {
        public List<Block> Blockchain;

        public async Task GetBlockChain()
        {
            List<Block> newChain = ((JArray) await SendMessageToNode("get_chain")).ToObject<List<Block>>();

            if (newChain.Count == 0)
                return;

            if (Blockchain != null && newChain[0].BlockNumber == Blockchain.Count)
                Blockchain.AddRange(newChain);
            else
                Blockchain = newChain;
        }

        public static async Task<object> SendMessageToNode(string endpoint, object data = null)
        {
            string load = JsonConvert.SerializeObject(data);
            var url = $"https://localhost:5001/{endpoint}";

            var http = (HttpWebRequest) WebRequest.Create(new Uri(url));
            http.ContentType = "application/json";
            http.Method = "POST";

            if (data != null)
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                Byte[] bytes = encoding.GetBytes(load);
                Stream newStream = http.GetRequestStream();
                await newStream.WriteAsync(bytes, 0, bytes.Length);
                newStream.Close();
            }

            return JsonConvert.DeserializeObject<object>(await new StreamReader(http.GetResponse().GetResponseStream())
                .ReadToEndAsync());

        }

        public class SessionState
        {
            public CavBlockchain Blockchain { get; set; }
            public CavWallet Wallet { get; set; }
        }
    }
}