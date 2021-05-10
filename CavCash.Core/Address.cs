using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CavCash.Core;
using NBitcoin;
using NBitcoin.Protocol;
using NBitcoin.Stealth;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Block = NBitcoin.Block;
using Transaction = CavCash.Core.Transaction;

namespace CavCash.Core
{
    public enum KeyType
    {
        Spend,
        Scan
    }
    
    public class PublicWallet
    {
        public PubKey SpendKey { get; set; }
        public PubKey ScanKey { get; set; }
        public string Address => $"{ScanKey}x{SpendKey}";

        public PublicWallet(string strAddress)
        {
            int splitlen = strAddress.IndexOf('x');
            
            string scanKey = strAddress.Substring(0, splitlen);
            splitlen++;
            string spendKey = strAddress.Substring(splitlen, strAddress.Length-splitlen);

            ScanKey = new PubKey(scanKey);
            SpendKey = new PubKey(spendKey);
        }
    }
    
    public class Wallet
    {
        public string PrivateKey => $"{ScanKey.GetWif(Constants.BitCoinNetwork)}:{SpendKey.GetWif(Constants.BitCoinNetwork)}";
        public string Address => $"{ScanPublicKey}x{SpendPublicKey}";
        public string PublicKey => Address; // For cosmetics
        private string ScanPublicKey => ScanKey.PubKey.ToString();
        private string SpendPublicKey  => SpendKey.PubKey.ToString();
        private Key SpendKey { get; set; }
        private Key ScanKey { get; set; }

        public Wallet()
        {
            ScanKey = new Key();
            SpendKey = new Key();
        }


        public Wallet(string privateKey)
        {
            int splitlen = privateKey.IndexOf(':');
            
            string scanKey = privateKey.Substring(0, splitlen);
            splitlen++;
            string spendKey = privateKey.Substring(splitlen, privateKey.Length-splitlen);

            ScanKey = Key.Parse(scanKey, Constants.BitCoinNetwork);
            SpendKey = Key.Parse(scanKey, Constants.BitCoinNetwork);
        }
        
        public string SignTransactionInput(TransactionOutput output, TransactionInput newInput)
        {
            var expectedStealth = SpendKey.Uncover(ScanKey, output.GetEpherKey());
            bool match = expectedStealth.PubKey.Hash == output.GetStealthKey().ID;
            
            if (!match)
                throw new Exception("You don't have keys to this transaction");
            
            return expectedStealth.SignMessage(newInput.Hash);
        }
        
        public string SignString(string signMe, KeyType type = KeyType.Spend)
        {
            return (type == KeyType.Scan ? ScanKey : SpendKey).SignMessage(signMe);
        }

        public List<TransactionOutput> GetTransactionsSync(List<Block> blockchain, bool removeUsed = false)
        {
            List<Block> reverseBlockchain = blockchain;
            reverseBlockchain.Reverse();
            List<TransactionOutput> result = new List<TransactionOutput>();
            foreach (var block in reverseBlockchain)
            {
                foreach (Transaction transaction in block.Transactions)
                {
                    foreach (TransactionOutput output in transaction.Outputs)
                    {
                        if (output.To == Address) //Only for coinbase transactions TODO: enforce check on input side
                        {
                            result.Add(output);
                            continue;
                        }
                        else if (!string.IsNullOrWhiteSpace(output.To))
                        {
                            continue;
                        }

                        var expectedStealth = SpendKey.PubKey.UncoverReceiver(ScanKey, output.GetEpherKey());
                        bool match = expectedStealth.Hash == output.GetStealthKey().ID;

                        Debug.WriteLine($"Checking Output ({transaction.Hash}\\{output.Hash})");
                        Debug.WriteLine("===========================================");
                        Debug.WriteLine($"Expected: {expectedStealth.Hash}");
                        Debug.WriteLine($"Actual: {output.GetStealthKey().ID}\n\n\n");
                        if (match)
                            result.Add(output);
                    }
                }
            }

            foreach (var block in reverseBlockchain)
            {
                foreach (Transaction transaction in block.Transactions)
                {
                    foreach (TransactionInput input in transaction.Inputs)
                    {
                        //TODO: make this work more reliably
                        if (removeUsed && !string.IsNullOrWhiteSpace(input.FromOutput))
                        {
                            Debug.WriteLine($"{result.RemoveAll(x => x.Hash == input.FromOutput)} - {input.FromOutput}");
                        }
                    }
                }
            }

            return result;
        }
        
        public decimal GetBalanceSync(List<Block> blockchain)
        {
            List<TransactionOutput> transactions = GetTransactionsSync(blockchain, true);
            return transactions.Sum(x => x.Amount);
        }
    }
}