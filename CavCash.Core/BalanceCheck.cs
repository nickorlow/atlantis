using System;
using System.Security.Cryptography;
using System.Text;

namespace CavCash.Core
{
    public class BalanceCheck
    {
        public string Address { get; set; }
        public decimal Balance { get; set; }
        
        public string LastCheckHash { get; set; }
        public string Hash => CalculateHash();

        string CalculateHash()
        {
            return ComputeSha256Hash(Address + Balance+LastCheckHash);
        }
        static string ComputeSha256Hash(string rawData)  
        {  
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())  
            {  
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));  
  
                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();  
                for (int i = 0; i < bytes.Length; i++)  
                {  
                    builder.Append(bytes[i].ToString("x2"));  
                }  
                return builder.ToString();  
            }  
        }  
    }
}