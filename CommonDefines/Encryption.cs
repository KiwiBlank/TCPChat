using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace CommonDefines
{
    public class Encryption
    {
        public static RSAParameters RSAPublicKey;
        public static RSAParameters RSAPrivateKey;
        public static RSAParameters clientCopyOfServerPublicKey;

        public static byte[] RSAModulus;
        public static byte[] RSAExponent;

        public static byte[] AESKey;
        public static byte[] AESIV;

        public const int keySize = 2048;

        public const int keyLength = 256;

        public static string DecryptMessageData(byte[] dataMessage)
        {
            // Extract the 256 bytes that make up the AES Key and IV at the beginning of the message.
            byte[] keyData = Encryption.ExtractKeyFromMessage(dataMessage);

            // Get the AES Key and IV bytes and decrypt them using the server's private RSA Key
            // Returns 48 Bytes
            // 32 Bytes Key
            // 16 Bytes IV
            byte[] decryptKeyRSA = Encryption.RSADecrypt(keyData, Encryption.RSAPrivateKey);

            // Separate the Key and IV from decryptKeyRSA
            byte[] AESKey = Encryption.ExtractKeyFromBytes(decryptKeyRSA);
            byte[] AESIV = Encryption.ExtractIVFromBytes(decryptKeyRSA);

            // Remove the keys from the other message data.
            // The keys are appended at the beginning of the stream
            // As such they are not part of the json formatting and have to be removed.

            byte[] RemoveKeysFromDataBytes = new byte[dataMessage.Length - keyLength];
            Array.Copy(dataMessage, keyLength, RemoveKeysFromDataBytes, 0, RemoveKeysFromDataBytes.Length);

            // Decrypt the main message using the decryped key and IV
            byte[] AESDecrypt = Encryption.AESDecrypt(RemoveKeysFromDataBytes, AESKey, AESIV);

            string message = System.Text.Encoding.ASCII.GetString(AESDecrypt);

            return message;
        }
        public static void GenerateRSAKeys()
        {
            RSA rsa = RSA.Create(keySize);

            // Includes private key.
            RSAParameters rsaPrivate = rsa.ExportParameters(true);

            RSAParameters rsaPublic = rsa.ExportParameters(false);

            Encryption.RSAPublicKey = rsaPublic;
            Encryption.RSAPrivateKey = rsaPrivate;

            RSAModulus = rsaPublic.Modulus;
            RSAExponent = rsaPublic.Exponent;

        }

        public static void GenerateAESKeys()
        {
            Aes aes = Aes.Create();

            AESKey = aes.Key;
            AESIV = aes.IV;
        }

        public static RSAParameters RSAParamaterCombiner(byte[] modulus, byte[] exponent)
        {
            RSAParameters result = new RSAParameters
            {
                Modulus = modulus,
                Exponent = exponent
            };

            return result;
        }


        public static byte[] AESDecrypt(byte[] data, byte[] key, byte[] IV)
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = 128;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.Zeros;
                aes.Key = key;
                aes.IV = IV;


                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    using (var ms = new MemoryStream())
                    using (var cryptoStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(data, 0, data.Length);
                        cryptoStream.FlushFinalBlock();

                        return ms.ToArray();
                    }
                }
            }
        }
        public static byte[] AESEncrypt(byte[] data, byte[] key, byte[] IV)
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = 128;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.Zeros;
                aes.Key = key;
                aes.IV = IV;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    using (var ms = new MemoryStream())
                    using (var cryptoStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(data, 0, data.Length);
                        cryptoStream.FlushFinalBlock();

                        return ms.ToArray();
                    }
                }
            }
        }
        public static byte[] RSAEncrypt(byte[] data, RSAParameters publicKey)
        {
            RSACryptoServiceProvider csp = new RSACryptoServiceProvider(keySize);

            csp.ImportParameters(publicKey);

            var encryptedData = csp.Encrypt(data, false);

            return encryptedData;
        }
        public static byte[] RSADecrypt(byte[] data, RSAParameters privateKey)
        {
            RSACryptoServiceProvider csp = new RSACryptoServiceProvider(keySize);

            csp.ImportParameters(privateKey);

            var decryptedData = csp.Decrypt(data, false);

            return decryptedData;
        }

        public static byte[] ExtractKeyFromMessage(byte[] data)
        {
            List<byte> byteList = new();

            for (int i = 0; i < data.Length; i++)
            {
                if (i < keyLength) // 256
                {
                    byteList.Add(data[i]);
                }
            }
            byte[] bytes = byteList.ToArray();
            return bytes;
        }
        public static byte[] ExtractIVFromBytes(byte[] data)
        {
            List<byte> byteList = new();

            for (int i = 0; i < data.Length; i++)
            {
                if (i >= 0 && i < 16) // AES IV length
                {
                    byteList.Add(data[i]);
                }
            }
            byte[] bytes = byteList.ToArray();
            return bytes;
        }
        public static byte[] ExtractKeyFromBytes(byte[] data)
        {
            List<byte> byteList = new();

            for (int i = 0; i < data.Length; i++)
            {
                if (i > 15 && i < 48) // AES Key length
                {
                    byteList.Add(data[i]);
                }
            }
            byte[] bytes = byteList.ToArray();
            return bytes;
        }
        public static byte[] AppendKeyToMessage(byte[] data, byte[] IV, byte[] key, RSAParameters publicKey)
        {
            List<byte> listKey = new();
            List<byte> listMain = new();

            // Add the keys to a separate list
            listKey.AddRange(key);
            listKey.AddRange(IV);
            byte[] byteKeyArray = listKey.ToArray();

            // 256 Bytes
            byte[] encryptKey = Encryption.RSAEncrypt(byteKeyArray, publicKey);

            listMain.AddRange(encryptKey);
            listMain.AddRange(data);
            byte[] finalBytes = listMain.ToArray();

            return finalBytes;
        }
    }
}
