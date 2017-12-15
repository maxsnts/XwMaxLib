using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace XwMaxLib.Crypto
{
    public class XwSymCrypto
    {
        public string Encrypt(string plainText, string passPhrase, string saltValue)
        {
            int hashSize = 0;

            string hashAlgorithm = "SHA512";
            int passwordIterations = 1000;
            string initVector = "";
            int keySizeInBytes = 32;

            if (hashAlgorithm == "MD5")
                hashSize = 16;
            else if (hashAlgorithm == "SHA1")
                hashSize = 16;
            else if (hashAlgorithm == "SHA256")
                hashSize = 32;
            else if (hashAlgorithm == "SHA384")
                hashSize = 48;
            else if (hashAlgorithm == "SHA512")
                hashSize = 64;

            if (keySizeInBytes > hashSize)
                throw new Exception("Selected hash algorithm is not capable of returning the keysize");

            // Convert strings into byte arrays.
            // Let us assume that strings only contain ASCII codes.
            // If strings include Unicode characters, use Unicode, UTF7, or UTF8 
            // encoding.
            byte[] saltValueBytes = Encoding.UTF8.GetBytes(saltValue);

            // Convert our plaintext into a byte array.
            // Let us assume that plaintext contains UTF8-encoded characters.
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            // Convert our passPhrase into a byte array.
            byte[] passPhraseBytes = Encoding.UTF8.GetBytes(passPhrase);


            // First, we must create a password, from which the key will be derived.
            // This password will be generated from the specified passphrase and 
            // salt value. The password will be created using the specified hash 
            // algorithm. Password creation can be done in several iterations.
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhraseBytes, saltValueBytes, hashAlgorithm, passwordIterations);

            // Use the password to generate pseudo-random bytes for the encryption
            // key. Specify the size of the key in bytes (instead of bits).
            byte[] keyBytes = password.GetBytes(keySizeInBytes);

            byte[] initVectorBytes;
            if (initVector == "")
            {
                password.Reset();
                initVectorBytes = password.GetBytes(16);
            }
            else
                initVectorBytes = Encoding.UTF8.GetBytes(initVector);

            // Create uninitialized Rijndael encryption object.
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Padding = PaddingMode.PKCS7;

            // It is reasonable to set encryption mode to Cipher Block Chaining
            // (CBC). Use default options for other symmetric key parameters.
            symmetricKey.Mode = CipherMode.CBC;

            // Generate encryptor from the existing key bytes and initialization 
            // vector. Key size will be defined based on the number of the key 
            // bytes.
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);

            // Define memory stream which will be used to hold encrypted data.
            MemoryStream memoryStream = new MemoryStream();

            // Define cryptographic stream (always use Write mode for encryption).
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            // Start encrypting.
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);

            // Finish encrypting.
            cryptoStream.FlushFinalBlock();

            // Convert our encrypted data from a memory stream into a byte array.
            byte[] cipherTextBytes = memoryStream.ToArray();

            // Close both streams.
            memoryStream.Close();
            cryptoStream.Close();

            // Convert encrypted data into a base64-encoded string.
            string cipherText = Convert.ToBase64String(cipherTextBytes);

            // Return encrypted string.
            return cipherText;
        }

        public static string Decrypt(string cipherText, string passPhrase, string saltValue)
        {
            string hashAlgorithm = "SHA512";
            int passwordIterations = 1000;
            string initVector = "";
            int keySizeInBytes = 32;

            //if (keySize >= 256 && (hashAlgorithm != "SHA256" && hashAlgorithm != "SHA384" && hashAlgorithm != "SHA512"))
            //    throw new Exception("Selected hash algorithm is not capable of returning the keysize");

            // Convert strings defining encryption key characteristics into byte
            // arrays. Let us assume that strings only contain ASCII codes.
            // If strings include Unicode characters, use Unicode, UTF7, or UTF8
            // encoding.
            byte[] saltValueBytes = Encoding.UTF8.GetBytes(saltValue);

            // Convert our ciphertext into a byte array.
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

            // Convert our passPhrase into a byte array.
            byte[] passPhraseBytes = Encoding.UTF8.GetBytes(passPhrase);

            // First, we must create a password, from which the key will be 
            // derived. This password will be generated from the specified 
            // passphrase and salt value. The password will be created using
            // the specified hash algorithm. Password creation can be done in
            // several iterations.
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhraseBytes, saltValueBytes, hashAlgorithm, passwordIterations);
            //Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passPhrase, saltValueBytes, passwordIterations);

            // Use the password to generate pseudo-random bytes for the encryption
            // key. Specify the size of the key in bytes (instead of bits).
            byte[] keyBytes = password.GetBytes(keySizeInBytes);

            byte[] initVectorBytes;
            if (initVector == "")
            {
                password.Reset();
                initVectorBytes = password.GetBytes(16);
            }
            else
                initVectorBytes = Encoding.UTF8.GetBytes(initVector);

            // Create uninitialized Rijndael encryption object.
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Padding = PaddingMode.PKCS7;

            // It is reasonable to set encryption mode to Cipher Block Chaining
            // (CBC). Use default options for other symmetric key parameters.
            symmetricKey.Mode = CipherMode.CBC;

            // Generate decryptor from the existing key bytes and initialization 
            // vector. Key size will be defined based on the number of the key 
            // bytes.
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);

            // Define memory stream which will be used to hold encrypted data.
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

            // Define cryptographic stream (always use Read mode for encryption).
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

            // Since at this point we don't know what the size of decrypted data
            // will be, allocate the buffer long enough to hold ciphertext;
            // plaintext is never longer than ciphertext.
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            // Start decrypting.
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

            // Close both streams.
            memoryStream.Close();
            cryptoStream.Close();

            // Convert decrypted data into a string. 
            // Let us assume that the original plaintext string was UTF8-encoded.
            string plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);

            // Return decrypted string.   
            plainText = plainText.Replace("\0", "");
            return plainText;
        }
    }
}
