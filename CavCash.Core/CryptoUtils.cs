using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using NBitcoin;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;

namespace CavCash.Core
{
    public class CryptoUtils
    {

	    public static string ComputeSha256Hash(string rawData)  
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