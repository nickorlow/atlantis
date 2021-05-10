using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CavCash.Core;
using NBitcoin;
using NBitcoin.Protocol;
using NBitcoin.Stealth;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Block = CavCash.Core.Block;
using Transaction = CavCash.Core.Transaction;

namespace CavCash.Wallet
{
    class Program
    {
        private static List<Block> Blockchain;
        private static Core.Wallet MyWallet;
        private static Core.Peer Node;
        static async Task Main(string[] args)
        {
            Console.WriteLine("CavCash Wallet");
            Console.WriteLine("==============");
            Console.Write("Type (e) to run wallet in ephemeral mode:");
            if (Console.ReadLine() != "e")
            {
                if (!File.Exists("./Wallet.CavWallet"))
                {
                    Console.WriteLine("Creating New Wallet...");
                    MyWallet = new Core.Wallet();
                    using StreamWriter sw = new StreamWriter("./Wallet.CavWallet");
                    sw.Write(MyWallet.PrivateKey);
                }
                else
                {
                    Console.WriteLine("Importing Existing Wallet...");
                    using StreamReader sr = new StreamReader("./Wallet.CavWallet");
                    MyWallet = new Core.Wallet(sr.ReadLine());
                }
            }
            else
            {
                MyWallet = new Core.Wallet();
            }
            
            Console.Clear();
            Console.Write("Enter Node address: ");
            Node = new Peer()
            {
                IpAddress = Console.ReadLine()
            };

            Console.WriteLine("Downloading Chain...");
            await GetBlockChain();
            Console.WriteLine("Got Chain!");
            Clear();
            while (true)
            {
                Console.WriteLine($"0) Create Transaction");
                Console.WriteLine($"1) Get Updated Chain");
                Console.WriteLine($"2) Export Keys");
                Console.WriteLine($"3) Show Chain");
                Console.WriteLine($"4) Get my transactions");

                Console.Write("What would you like to do?: ");
                try
                {
                    int option = int.Parse(Console.ReadLine());
                    switch (option)
                    {
                        case 0:
                            await GetBlockChain();
                            Transaction t = CreateTransaction(MyWallet);
                            await SendMessageToNode("transaction", t);
                            Console.ReadLine();
                            break;

                        case 1:
                            await GetBlockChain();
                            break;

                        case 2:
                            Console.WriteLine($"\nPUBLIC\n{MyWallet.PublicKey}");
                            Console.ReadLine();
                            break;

                        case 3:
                            await GetBlockChain();
                            Console.WriteLine(JsonConvert.SerializeObject(Blockchain, Formatting.Indented));
                            Console.ReadLine();
                            break;
                        
                        case 4:
                            Console.WriteLine(JsonConvert.SerializeObject(MyWallet.GetTransactionsSync(Blockchain),
                                Formatting.Indented));
                            Console.ReadLine();
                            break;
                        

                        default:
                            throw new Exception();
                    }
                }
                catch (Exception e)
                {
                   Console.Clear();
                   Console.WriteLine($"Error: {e.Message}");
                   continue;
                }
                Clear();
            }
        }

        
        public static StealthPayment[] GetPayments(StealthPayment payment, PubKey[] spendKeys, Key scan)
        {
            List<StealthPayment> result = new List<StealthPayment>();
            
            
                    if(scan != null && spendKeys != null)
                    {
                        if(payment.StealthKeys.Length != spendKeys.Length)
                            return result.ToArray();

                        var expectedStealth = spendKeys.Select(s => s.UncoverReceiver(scan, payment.Metadata.EphemKey)).ToList();
                        foreach(var stealth in payment.StealthKeys)
                        {
                            var match = expectedStealth.FirstOrDefault(expected => expected.Hash == stealth.ID);
                            Console.WriteLine($"Expected: {expectedStealth[0].Hash}");
                            Console.WriteLine($"Actual: {stealth.ID}");
                            if(match != null)
                                expectedStealth.Remove(match);
                        }
                        if(expectedStealth.Count != 0)
                            return result.ToArray();
                    }
                    result.Add(payment);
                
           
            return result.ToArray();
        }
        
        
        public static void Clear()
        {
            Console.Clear();
            PrintHeader();
        }

        public static void PrintHeader()
        {
            Console.WriteLine("CavCash Wallet");
            Console.WriteLine($"Block Height: {Blockchain.Count}");
            Console.WriteLine($"Balance: {MyWallet.GetBalanceSync(Blockchain)}");
            Console.WriteLine("==============");
        }

        public static async Task GetBlockChain()
        {
            List<Block> newChain = ((JArray) await SendMessageToNode("get_chain")).ToObject<List<Block>>();
                
            if(newChain.Count == 0)
                return;
                
            if (Blockchain != null && newChain[0].BlockNumber == Blockchain.Count)
                Blockchain.AddRange(newChain);
            else 
                Blockchain = newChain;
        }

        public static async Task<object> SendMessageToNode(string endpoint, object data = null)
        {
            string load = JsonConvert.SerializeObject(data);
            var url = $"https://{Node.IpAddress}/{endpoint}";
            
            var http = (HttpWebRequest)WebRequest.Create(new Uri(url));
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

            return JsonConvert.DeserializeObject<object>(await new StreamReader(http.GetResponse().GetResponseStream()).ReadToEndAsync());

        }
        
        public static Transaction CreateTransaction(Core.Wallet ourWallet, string address = null, decimal? amountP = null)
        {
            Console.Write("To Address: ");
            string toAddress = address ?? Console.ReadLine();

            
            Console.Write("Amount: ");
            decimal amount = amountP ?? 0;
            while (amount == 0 && !decimal.TryParse(Console.ReadLine(), out amount))
            {
              Console.WriteLine("Invalid Amount");  
            }

            string secret = "idk what we really should be putting here but whatever this works well.";
            
            List<TransactionOutput> transactions =  MyWallet.GetTransactionsSync(Blockchain, true);
            
            if(transactions.Sum(x => x.Amount) < amount)
            {
                throw new Exception("You don't have enough money!!");
            }

            List<TransactionOutput> fromTransactionOutputs = new List<TransactionOutput>();
            for (int i = 0; fromTransactionOutputs.Sum(x => x.Amount) < amount; i++)
            {
               fromTransactionOutputs.Add(transactions[i]); // could be made more efficient via getting amount closest to amount
            }

            PublicWallet pw = new PublicWallet(toAddress);
            BitcoinStealthAddress recAddress = pw.SpendKey.CreateStealthAddress(pw.ScanKey, Constants.BitCoinNetwork);
            Key ephem = new Key(); 
            StealthPayment payment = recAddress.CreatePayment(ephem);
            
            PublicWallet myPublicWallet = new PublicWallet(MyWallet.Address);
            BitcoinStealthAddress myRecAddress = myPublicWallet.SpendKey.CreateStealthAddress(myPublicWallet.ScanKey, Constants.BitCoinNetwork);
            Key myEphem = new Key(); 
            StealthPayment myPayment = myRecAddress.CreatePayment(myEphem);

            List<TransactionInput> inputs = new List<TransactionInput>();
            foreach (TransactionOutput output in fromTransactionOutputs)
            {
                TransactionInput input = 
                    new TransactionInput()
                    {
                        FromOutput = output.Hash
                    };
                
                input.FromOutputProof = (output.IsCoinbase) ? MyWallet.SignString(output.Hash) : MyWallet
                    .SignTransactionInput(output, input);
                
                inputs.Add(input);
                
                Console.WriteLine($"Output: {output.Hash}");
            }
            
            Transaction t = new Transaction()
            {
               
                Time = DateTime.UtcNow.Ticks,
                Inputs = inputs,
                Outputs =
               new List<TransactionOutput>() {
                    new TransactionOutput(){
                        EpherKey = payment.Metadata.EphemKey.ToString(),
                        StealthKey = payment.StealthKeys[0].ID.ToString(),
                        Amount = amount
                    },
                }
            };

            if (fromTransactionOutputs.Sum(x => x.Amount) > amount)
            {
                t.Outputs.Add( 
                    new TransactionOutput(){
                    EpherKey = myPayment.Metadata.EphemKey.ToString(),
                    StealthKey = myPayment.StealthKeys[0].ID.ToString(),
                    Amount = fromTransactionOutputs.Sum(x => x.Amount) - amount
                });
            }
            
            foreach (var inp in t.Inputs)
            {
                Console.WriteLine($"INPUT: {inp.FromOutput}");
            }
            
            return t;
        }
    }

   
    
}