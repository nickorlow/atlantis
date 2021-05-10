using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CavCash.Core
{
    public class Block
    {
        private static readonly int BlockSize = 5;

        public List<Transaction> Transactions { get; set; }
        public int Nonce { get; set; }

        public string LastHash { get; set; }
        public string Hash => CalculateHash();
        public string HashNoNonce => CalculateHashNoNonce();
        public string ValidatorSignature { get; set; }
        public int MillisecondsToMine { get; set; }
        public int BlockNumber { get; set; }
        
        string CalculateHash()
        {
            string hashes = "";
            foreach (var tran in Transactions)
            {
                hashes += tran.Hash;
            }
            return CryptoUtils.ComputeSha256Hash( Nonce+ hashes+BlockNumber+(LastHash??"genesis"));
        }

        string CalculateHashNoNonce()
        {
            string hashes = "";
            foreach (var tran in Transactions)
            {
                hashes += tran.Hash;
            }
            return CryptoUtils.ComputeSha256Hash( hashes+BlockNumber+(LastHash??"genesis"));
        }
        

        public void AddTransaction(Transaction t)
        {
            if (Transactions.Count == BlockSize)
                throw new Exception();

            Transactions.Append(t);
        }
    }
}