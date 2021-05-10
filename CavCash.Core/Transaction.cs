using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using NBitcoin;
using NBitcoin.Stealth;

namespace CavCash.Core
{
    public class TransactionInput
    {
        public string FromOutputProof { get; set; } //used to verify we're the owner of the last transaction
        public string FromOutput { get; set; } //used for everything else
        public string FromBlock { get; set; } //used for coinbase
        public bool IsCoinbase => FromBlock != null;
        public string Hash => CalculateHash();
        
        public string CalculateHash()
        {
            return CryptoUtils.ComputeSha256Hash(!IsCoinbase ? FromOutput : FromBlock);
        }
    }

    public class TransactionOutput
    {
        public string EpherKey { get; set; } // use address for coinbase, hashaddress for regular
        public string StealthKey { get; set; }
        public decimal Amount { get; set; }
        public string To { get; set; }
        public long Time { get; set; } // so coin base transactions don't have same hash
        
        public bool IsCoinbase => !string.IsNullOrWhiteSpace(To);
        
        public string Hash => CalculateHash();
        
        public PubKey GetEpherKey()
        {
            return new PubKey(EpherKey);
        }
        
        public StealthSpendKey GetStealthKey()
        {
            return new StealthSpendKey(new KeyId(StealthKey), null);
        }
        
        public string CalculateHash()
        {
            return CryptoUtils.ComputeSha256Hash((To ?? (EpherKey + GetStealthKey().ID.ToString()) + Amount.ToString("0.00000000")) + Time);
        }
    }
    
    public class Transaction
    {

        public long Time { get; set; }
        public string Hash => CalculateHash();

        public List<TransactionOutput> Outputs { get; set; }
        public List<TransactionInput> Inputs { get; set; }
       

        string CalculateHash()
        {
            string hashes = "";
            foreach (var output in Outputs)
            {
                hashes += output.CalculateHash();
            }
            foreach (var input in Inputs)
            {
                hashes += input.CalculateHash();
            }
            return CryptoUtils.ComputeSha256Hash(hashes + Time);
        }
    }
}