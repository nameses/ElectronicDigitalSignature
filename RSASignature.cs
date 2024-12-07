using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ElectronicDigitalSignature
{
    public class RSASignature
    {
        private RSACryptoServiceProvider _rsa;

        public RSASignature(string containerName)
        {
            var cspParameters = new CspParameters { KeyContainerName = containerName };
            _rsa = new RSACryptoServiceProvider(cspParameters);
        }

        public string ExportPublicKey()
        {
            return _rsa.ToXmlString(false);
        }

        public void ImportPublicKey(string publicKey)
        {
            _rsa.FromXmlString(publicKey);
        }

        public string GenerateSignature(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] hashBytes = SHA1.Create().ComputeHash(messageBytes);
            byte[] signatureBytes = _rsa.SignHash(hashBytes, CryptoConfig.MapNameToOID("SHA1"));
            return Convert.ToBase64String(signatureBytes);
        }

        public bool VerifySignature(string message, string signature)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] hashBytes = SHA1.Create().ComputeHash(messageBytes);
            byte[] signatureBytes = Convert.FromBase64String(signature);
            return _rsa.VerifyHash(hashBytes, CryptoConfig.MapNameToOID("SHA1"), signatureBytes);
        }
    }
}
