using System;
using System.Security.Cryptography;

namespace XwMaxLib.Crypto
{
    public enum XwAsymCryptoPrivider
    {
        PROV_RSA_FULL = 1,
        PROV_RSA_SCHANNEL = 12,
        PROV_RSA_AES = 24
    }

    public class XwAsymCrypto : IDisposable
    {
        //default settings
        private Int32 KeySize = 2048;
        private XwAsymCryptoPrivider ProviderType = XwAsymCryptoPrivider.PROV_RSA_AES;
        private HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA1;

        private RSACryptoServiceProvider rsaProvider = null;
        private byte[] privateKey = null;
        private byte[] publicKey = null;
        

        //********************************************************************************
        public XwAsymCrypto()
        {
            throw new Exception("Work in progress... maybe!");
            //PrepareObject();
        }

        //********************************************************************************
        public XwAsymCrypto(Int32 keySize, XwAsymCryptoPrivider providerType, HashAlgorithmName hashAlgorithm)
        {
            throw new Exception("Work in progress... maybe!");
            /*
            KeySize = keySize;
            ProviderType = providerType;
            HashAlgorithm = hashAlgorithm;
            PrepareObject();
            */
        }

        //********************************************************************************
        private void PrepareObject()
        {
            CspParameters cspParams = new CspParameters();
            cspParams.ProviderType = (int)ProviderType;
            rsaProvider = new RSACryptoServiceProvider(KeySize, cspParams);
        }

        //********************************************************************************
        ~XwAsymCrypto()
        {
            Dispose(false);
        }

        //********************************************************************************
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // called via myClass.Dispose(). 
                    // OK to use any private object references
                }
                disposed = true;
            }
        }

        //********************************************************************************
        public void Dispose()
        {
            if (rsaProvider != null)
            {
                rsaProvider.Clear();
                rsaProvider.Dispose();
            }
            GC.SuppressFinalize(this);
        }

        public void CreateKeyPairB64(out string privateKeyBase64, out string publicKeyB64)
        {
            privateKeyBase64 = Convert.ToBase64String(rsaProvider.ExportCspBlob(true));
            publicKeyB64 = Convert.ToBase64String(rsaProvider.ExportCspBlob(false));
        }

        //********************************************************************************
        public byte[] Encrypt(byte[] data)
        {
            rsaProvider.ImportCspBlob(publicKey);
            return rsaProvider.Encrypt(data, false);
        }

        //********************************************************************************
        public byte[] Decrypt(byte[] data)
        {
            rsaProvider.ImportCspBlob(privateKey);
            return rsaProvider.Decrypt(data, false);
        }

        //********************************************************************************
        public byte[] GenerateSignature(byte[] data)
        {
            rsaProvider.ImportCspBlob(privateKey);
            SHA1Managed hash = new SHA1Managed();
            byte[] hashedData = hash.ComputeHash(data);
            return rsaProvider.SignHash(hashedData, HashAlgorithm, RSASignaturePadding.Pkcs1);
        }

        //********************************************************************************
        public bool VerifySignature(byte[] data, byte[] signature)
        {
            rsaProvider.ImportCspBlob(publicKey);
            SHA1Managed hash = new SHA1Managed();
            byte[] hashedData = hash.ComputeHash(data);
            return rsaProvider.VerifyHash(hashedData, signature, HashAlgorithm, RSASignaturePadding.Pkcs1);
        }
    }
}
