using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CavCash.Core;

namespace CavCash.Node.ASP
{
    public static class CurrentNode
    {
        public static List<Transaction> PendingTransactions;
        public static List<Block> BlockChain;
        public static List<Peer> Peers;
        public static Peer MyNodePeer;
        public static Wallet MyWallet;
        public static bool IsInitialized;
        
        public static async Task StartNode(string ip = null)
        {
            PendingTransactions = new List<Transaction>();
            BlockChain = new List<Block>();
            MyNodePeer = new Peer();

            Console.Write("Would you like to start a new blockchain? (Y/n): ");

            if (true || Console.ReadLine() == "Y")
            {
                BlockChain.Add(new Block()
                {
                    BlockNumber = 0,
                    Transactions = new List<Transaction>()
                });
            }
            else
            {
                // TODO: Logic
            }

            if (!File.Exists("./Wallet.CavWallet"))
            {
                Console.WriteLine("Creating New Wallet...");
                MyWallet = new Core.Wallet();
                using StreamWriter sw = new StreamWriter("./Wallet.CavWallet");
                sw.Write(MyWallet.PrivateKey);
                Console.WriteLine("Created New Wallet!");
            }
            else
            {
                Console.WriteLine("Importing Existing Wallet...");
                using StreamReader sr = new StreamReader("./Wallet.CavWallet");
                MyWallet = new Wallet(sr.ReadToEnd());
                Console.WriteLine("Imported Existing Wallet!");
            }

            Console.Write("IP Address/Hostname to broadcast to other nodes: ");
            MyNodePeer.IpAddress = ip ?? Console.ReadLine();

            IsInitialized = true;
            CavConsole.WriteLine("Node Started", ConsoleColor.DarkGreen, ConsoleColor.Black);
        }

        public static async Task MineCurrentBlock()
        {
            await ConstructNextBlock();
            BlockChain[^1] = ValidateAndMineBlock(BlockChain[^1]);
            Console.WriteLine($"Successfully mined block {BlockChain[^1].BlockNumber}");
            List<TransactionInput> coinbaseInputs = new List<TransactionInput>()
            {
                new ()
                {
                    FromBlock = BlockChain[^1].Hash,
                }
            };

            List<TransactionOutput> coinbaseOutputs = new List<TransactionOutput>(){new TransactionOutput()
                {
                    Amount = (decimal) 100,
                    To = MyWallet.Address,
                    Time = DateTime.UtcNow.Ticks
                }
            };

            BlockChain.Add(new Block()
            {
                BlockNumber = BlockChain[^1].BlockNumber + 1,
                LastHash = BlockChain[^1].Hash,
                Transactions = new List<Transaction>()
                {
                    new Transaction()
                    {
                        Inputs =coinbaseInputs,
                        Outputs =coinbaseOutputs,
                        Time = DateTime.UtcNow.Ticks,
                    }
                }
            });
        }
        
        public static Block ValidateAndMineBlock(Block b)
        {
            b = ValidateBlock(b);
            b = MineBlock(b);
            return b;
        }

        public static async Task ConstructNextBlock()
        {
            int numToGet = PendingTransactions.Count < 5 ? PendingTransactions.Count : 5;
            if(numToGet == 0)
                return;
            
            BlockChain[^1].Transactions.AddRange(PendingTransactions.GetRange(0,numToGet));
            PendingTransactions = PendingTransactions.GetRange(numToGet, PendingTransactions.Count-numToGet);
        }
        
        public static Block ValidateBlock(Block block)
        {
            if (block.Transactions == null)
                block.Transactions = new List<Transaction>();
            block = VerifyTransactions(block);
            block.ValidatorSignature = MyWallet.SignString(block.HashNoNonce);
            return block;
        }
        
        public static Block MineBlock(Block block)
        {
            DateTime start = DateTime.Now;
            
            // figure out what difficulty we're mining at
            string hash = "";
            for (int i = 0; i < Constants.Difficulties.Count; i++)
            {
                if (Constants.Difficulties[i].BlockNumber >= block.BlockNumber)
                {
                    hash = Constants.Difficulties[i].HashPrefix;
                    break;
                }
            }

            if (hash == "")
                hash = Constants.Difficulties[^1].HashPrefix;
                
            for (block.Nonce = 0; block.Hash.Substring(0, hash.Length) != hash; block.Nonce++)
            {
            }
            block.MillisecondsToMine = (int)(DateTime.Now - start).TotalMilliseconds;
            Console.WriteLine($"MINED BLOCK");
            Console.Write("HASH: ");
            CavConsole.Write(hash, ConsoleColor.White, ConsoleColor.Blue);
            Console.WriteLine($"{block.Hash.Substring(hash.Length)}");
            Console.WriteLine($"NONCE: {block.Nonce}");
            Console.WriteLine($"TIME: {block.MillisecondsToMine}ms ({TimeSpan.FromMilliseconds(block.MillisecondsToMine).ToString()})");
            return block;
        }
        
                
        public static bool HasOutputBeenSpent(string hash, List<Block> blockchain)
        {
            List<Block> reverseBlockchain = blockchain.GetRange(0, blockchain.Count);
            reverseBlockchain.Reverse();
            for (int i = 1; i < reverseBlockchain.Count; i++)
            {
                foreach (Transaction t in reverseBlockchain[i].Transactions)
                {
                    foreach (var input in t.Inputs)
                    {
                        if (input.FromOutput == hash)
                            return true;
                    }
                }
            }

            return false;
        }
        
        public static TransactionOutput GetOutputSync(string hash, List<Block> blockchain)
        {
            List<Block> reverseBlockchain = blockchain.GetRange(0, blockchain.Count);
            reverseBlockchain.Reverse();
            for (int i = 0; i < reverseBlockchain.Count; i++)
            {
                foreach (Transaction t in reverseBlockchain[i].Transactions)
                {
                    foreach (var output in t.Outputs)
                    {
                        if (output.Hash == hash)
                            return output;
                    }
                }
            }

            return null;
        }


        // TODO: Double spend check!! (right now we're relying on good faith of the miners)
        public static Block VerifyTransactions(Block block, List<Block> blockchain = null)
        {
            blockchain = blockchain ?? BlockChain;
            for (int i = 0; i < block.Transactions.Count; i++)
            {
                try
                {
                    decimal inputAmount = 0;
                    decimal outputAmount = 0;
                    foreach (TransactionInput input in block.Transactions[i].Inputs)
                    {
                        if (!input.IsCoinbase)
                        {
                            TransactionOutput lastOutput = GetOutputSync(input.FromOutput, blockchain);

                            if (lastOutput == null)
                            {
                                block.Transactions.RemoveAt(i);
                                i--;
                                throw new Exception("Invalid Transaction - Non-Existent Input");
                            }

                            if (lastOutput.IsCoinbase)
                            {
                                TransactionOutput transactionWithCoinbase = GetOutputSync(input.FromOutput, blockchain);
                                PublicWallet pw = new PublicWallet(lastOutput.To);
                                if (!pw.SpendKey.VerifyMessage(transactionWithCoinbase.Hash,
                                    input.FromOutputProof)
                                ) //technically we could check whatever we want for the content
                                {
                                    block.Transactions.RemoveAt(i);
                                    i--;
                                    throw new Exception("Invalid Transaction - Not Coinbase Signer");
                                }
                            }
                            // we want to check THIS transaction's hash so someone can't use the proof for their own transaction
                            else if (lastOutput.GetEpherKey().VerifyMessage(input.Hash,
                                input.FromOutputProof))
                            {
                                block.Transactions.RemoveAt(i);
                                i--;
                                throw new Exception("Invalid Transaction - Invalid Stealth Sig");
                            }
                            
                            if (HasOutputBeenSpent(input.FromOutput, blockchain))
                            {
                                block.Transactions.RemoveAt(i);
                                i--;
                                throw new Exception("Invalid Transaction - Double Spend");
                            }

                            inputAmount += lastOutput.Amount;
                        }
                        else
                        {
                            foreach (var output in block.Transactions[i].Outputs)
                            {
                                PublicWallet pw = new PublicWallet(output.To);
                                if (!pw.SpendKey.VerifyMessage(blockchain[^2].HashNoNonce,
                                    blockchain[^2].ValidatorSignature))
                                {
                                    block.Transactions.RemoveAt(i);
                                    i--;
                                    throw new Exception("Invalid Transaction - Invalid Coinbase Signer (2)");
                                }
                            }
                        }
                    }
                    
                    
                    foreach (var output in block.Transactions[i].Outputs)
                    {
                        if (output.Amount <= 0)
                        {
                            block.Transactions.RemoveAt(i);
                            i--;
                            throw new Exception("Invalid Transaction - Output is Negative");
                        }

                        if (output.IsCoinbase)
                            inputAmount += output.Amount;

                        outputAmount += output.Amount;
                    }

                    if (outputAmount != inputAmount)
                    {
                        block.Transactions.RemoveAt(i);
                        i--;
                        throw new Exception($"Invalid Transaction - Inputs not equal to outputs  (in/out {inputAmount} != {outputAmount})");
                    }
                        
                    // if we make it here the transaction is good to go
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Unable to process transaction {block.Transactions[i].Hash}: "+e.Message);
                }
            }
            
            return block;
        }
    }
}