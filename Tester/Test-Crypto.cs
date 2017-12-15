using System;

namespace Tester
{
    partial class Program
    {
        //***********************************************************************************
        static public void MenuCrypto()
        {
            while (true)
            {
                Console.ResetColor();
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("------------------------- Crypto ---------------------------");
                Console.WriteLine("0 - Main Menu");
                Console.WriteLine("1 - Asymetric Encrypt / Decrypt");
                //Console.WriteLine("3 - Symetric Encrypt");
                Console.ResetColor();
                Console.Write("Select option: ");
                string option = Console.ReadLine().Trim();
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("--------------------------- Run Test ----------------------------");
                Console.ResetColor();
                switch (option)
                {
                    case "0":
                        return;
                    case "1":
                        ASYMETRIC_ENCRIPT_DECRYPT();
                        break;
                   // case "3":
                   //     SYMETRIC_ENCRIPT();
                   //     break;
                    default:
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Unknown option");
                        }
                        break;
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("----------------------------- DONE ------------------------------");
                Console.ReadKey();
            }
        }

        //***********************************************************************************
        static public void ASYMETRIC_ENCRIPT_DECRYPT()
        {
            // Generate the test keys
            // Keys are returned in Base64 and Base32

            /*
            string senderPriKeyB64 = string.Empty;
            string senderPubKeyB64 = string.Empty;
            string senderPriKeyB32 = string.Empty;
            string senderPubKeyB32 = string.Empty;
            using (XwAsymCrypto sender = new XwAsymCrypto())
            {
                sender.CreateKeyPairB64(out senderPriKeyB64, out senderPubKeyB64);
                sender.CreateKeyPairB32(out senderPriKeyB32, out senderPubKeyB32);
            }

            string receiverPriKeyB64 = string.Empty;
            string receiverPubKeyB64 = string.Empty;
            string receiverPriKeyB32 = string.Empty;
            string receiverPubKeyB32 = string.Empty;
            using (XwAsymCrypto receiver = new XwAsymCrypto())
            {
                receiver.CreateKeyPairB64(out receiverPriKeyB64, out receiverPubKeyB64);
                receiver.CreateKeyPairB32(out receiverPriKeyB32, out receiverPubKeyB32);
            }

            byte[] encdata = null;
            using (XwAsymCrypto crypto = new XwAsymCrypto())
            {
                crypto.SetPublicKeyB64(receiverPubKeyB64);
                encdata = crypto.Encrypt(Encoding.UTF8.GetBytes("Testing Data !!!!"));
            }

            using (XwAsymCrypto crypto = new XwAsymCrypto())
            {
                crypto.SetPrivateKeyB64(receiverPriKeyB64);
                string s = Encoding.UTF8.GetString(crypto.Decrypt(encdata));
            }
            */

            /*
            byte[] decdata = null;
            using (XwAsymCrypto crypto = new XwAsymCrypto())
            {
                crypto.SetPrivateKeyB32(receiverPriKeyB64);
                decdata = crypto.Decrypt(encdata);
                crypto.SetPrivateKeyB32(senderPubKeyB64);
                decdata = crypto.Decrypt(decdata);
            }
            ***/
            /*

            // First Test Base64
            // - (Sender) Encrypt
            // - (Sender) Generate Signature with encoded data (Sender unprotected: can be known without decrypting data)
            string dataB64 = string.Empty;
            string signB64 = string.Empty;
            using (XwAsymCrypto crypto = new XwAsymCrypto())
            {
                //Set our own private key for signature generation
                crypto.SetPrivateKeyB64(senderPriKeyB64);
                //Set destination public key for encryption
                crypto.SetPublicKeyB64(receiverPubKeyB64);

                Console.WriteLine("--------- Encrypting Text to Base64 -----------");
                dataB64 = crypto.EncryptB64("Testing data!");
                Console.WriteLine(dataB64);
                Console.WriteLine("--------- Signature in Base64 -----------");
                signB64 = crypto.GenerateSignatureB64(dataB64);
                Console.WriteLine(signB64);
            }

            // - (... Send...)

            // - (Receiver) Verify Sender Signature with encoded data
            // - If Signatiure OK, decrypt data
            using (XwAsymCrypto crypto = new XwAsymCrypto())
            {
                //Set our own private key for decryption
                crypto.SetPrivateKeyB64(receiverPriKeyB64);
                //Set sender public key for signature verification
                crypto.SetPublicKeyB64(senderPubKeyB64);

                Console.WriteLine("--------- Verify signature from Base64 --------");
                if (crypto.VerifySignatureB64(dataB64, signB64))
                {
                    Console.WriteLine("Signatire Virified");
                    Console.WriteLine("--------- Decrypting Text from Base64 ---------");
                    string decdata64 = crypto.DecryptB64(dataB64);
                    Console.WriteLine(decdata64);
                }
            }

            // First Test Base32
            // - (Sender) Encrypt
            // - (Sender) Generate Signature with clear data (Sender protected: can be known only after decrypting data)
            string dataB32 = string.Empty;
            string signB32 = string.Empty;
            using (XwAsymCrypto crypto = new XwAsymCrypto())
            {
                //Set our own private key for signature generation
                crypto.SetPrivateKeyB32(senderPriKeyB32);
                //Set destination public key for encryption
                crypto.SetPublicKeyB32(receiverPubKeyB32);

                Console.WriteLine("--------- Encrypting Text to Base32 -----------");
                dataB32 = crypto.EncryptB32("Testing data!");
                Console.WriteLine(dataB32);
                Console.WriteLine("--------- Signature in Base32 -----------");
                signB32 = crypto.GenerateSignatureB32(MaxLib.Converter.Base32.Encode(Encoding.UTF8.GetBytes("Testing data!")));
                Console.WriteLine(signB32);
            }

            // - (... Send...)

            // - (Receiver) Decrypt data
            // - (Receiver) Verify Sender Signature with decrypted data
            using (XwAsymCrypto crypto = new XwAsymCrypto())
            {
                //Set our own private key for decryption
                crypto.SetPrivateKeyB32(receiverPriKeyB32);
                //Set sender public key for signature verification
                crypto.SetPublicKeyB32(senderPubKeyB32);

                Console.WriteLine("--------- Decrypting Text from Base32 ---------");
                string decdata32 = crypto.DecryptB32(dataB32);
                Console.WriteLine(decdata32);
                Console.WriteLine("--------- Verify signature from Base32 --------");
                if (crypto.VerifySignatureB32(MaxLib.Converter.Base32.Encode(Encoding.UTF8.GetBytes(decdata32)), signB32))
                {
                    Console.WriteLine("Signatire Virified");
                }
            }

    */
        }

        //***********************************************************************************
        static public void SYMETRIC_ENCRIPT()
        {


        }
        
    }
}
