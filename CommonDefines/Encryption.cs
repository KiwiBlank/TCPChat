using System.IO;
using System.Security.Cryptography;

namespace CommonDefines
{
    public class Encryption
    {
        public static RSAParameters pubKey;
        public static RSAParameters privKey;
        public static RSAParameters clientCopyOfServerPublicKey;

        public static byte[] rsaModulus;
        public static byte[] rsaExponent;

        public static byte[] AESKey;
        public static byte[] AESIV;

        public const int keySize = 2048;
        public static void GenerateKeyPair()
        {
            RSA rsa = RSA.Create(keySize);

            // Includes private key.
            RSAParameters rsaPrivate = rsa.ExportParameters(true);
            RSAParameters rsaPublic = rsa.ExportParameters(false);

            Encryption.pubKey = rsaPublic;
            Encryption.privKey = rsaPrivate;

            rsaModulus = rsaPublic.Modulus;
            rsaExponent = rsaPublic.Exponent;


        }

        public static RSAParameters RSAParamaterCombiner(byte[] modulus, byte[] exponent)
        {
            RSAParameters result = new RSAParameters();
            result.Modulus = modulus;
            result.Exponent = exponent;

            return result;
        }

        public static void GenerateAESKeys()
        {
            Aes aes = Aes.Create();
            AESKey = aes.Key;
            AESIV = aes.IV;
        }
        public static byte[] AESDecrypt(byte[] data, byte[] key, byte[] IV)
        {

            using (var aes = Aes.Create())
            {
                aes.KeySize = 128;
                aes.BlockSize = 128;
                aes.Padding = System.Security.Cryptography.PaddingMode.Zeros;
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
                aes.Padding = System.Security.Cryptography.PaddingMode.Zeros;
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
        public static byte[] EncryptData(byte[] data, RSAParameters publicKey)
        {


            RSACryptoServiceProvider csp = new RSACryptoServiceProvider(keySize);

            csp.ImportParameters(publicKey);

            var encryptedData = csp.Encrypt(data, false);

            return encryptedData;
        }
        public static byte[] DecryptData(byte[] data, RSAParameters privateKey)
        {

            RSACryptoServiceProvider csp = new RSACryptoServiceProvider(keySize);

            csp.ImportParameters(privateKey);


            var decryptedData = csp.Decrypt(data, false);



            return decryptedData;
        }
    }
}
