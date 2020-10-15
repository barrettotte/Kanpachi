using System;
using System.Security.Cryptography;
using System.IO;
using DeviceId;
using DeviceId.Encoders;
using DeviceId.Formatters;

namespace Kanpachi.Lib{

    public static class SecUtils{

        public static (byte[], byte[]) EncryptAes(string passphrase, string plainText){
            byte[] key = DeriveKey(passphrase);
            return (key, EncryptAes(DeriveKey(passphrase), plainText));
        }

        public static byte[] EncryptAes(byte[] key, string plainText){
            byte[] encrypted;
            byte[] iv;

            using(Aes aes = Aes.Create()){
                aes.Key = key;
                aes.GenerateIV();
                iv = aes.IV;
                aes.Mode = CipherMode.CBC;

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using(var ms = new MemoryStream()){
                    using(var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write)){
                        using(var sw = new StreamWriter(cs)){
                            sw.Write(plainText);
                        }
                        encrypted = ms.ToArray();
                    }
                }
            }
            var combined = new byte[iv.Length + encrypted.Length];
            iv.CopyTo(combined, 0);
            encrypted.CopyTo(combined, iv.Length);

            return combined;
        }

        public static string DecryptAes(byte[] key, byte[] cipherCombined){
            string plainText = null;

            using(Aes aes = Aes.Create()){
                aes.Key = key;
                byte[] iv = new byte[aes.BlockSize / 8];
                byte[] cipher = new byte[cipherCombined.Length - iv.Length];

                Array.Copy(cipherCombined, iv, iv.Length);
                Array.Copy(cipherCombined, iv.Length, cipher, 0, cipher.Length);
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;

                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using(var ms = new MemoryStream(cipher)){
                    using(var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read)){
                        using(var sr = new StreamReader(cs)){
                            plainText = sr.ReadToEnd();
                        }
                    }
                }
                return plainText;
            }
        }

        // derive encryption key from passphrase and salt fed into PBKDF2
        public static byte[] DeriveKey(string passphrase){
            byte[] salt = GenerateSalt();
            return Convert.FromBase64String(Pbkdf2.Hash(passphrase, salt));
        }

        // generate salt based on current hardware
        public static byte[] GenerateSalt(){
            string hardwareSalt = new DeviceIdBuilder()
                .AddMotherboardSerialNumber()
                .AddProcessorId()
                .AddUserName()
                .UseFormatter(new HashDeviceIdFormatter(() => SHA256.Create(), new Base64ByteArrayEncoder()))
                .ToString()
            ;
            return Convert.FromBase64String(hardwareSalt);
        }

        // generate crypto random number
        private static byte[] CryptoRandom(int size){
            byte[] bytes = new byte[size];
            var cryptoRandom = new RNGCryptoServiceProvider();
            
            cryptoRandom.GetBytes(bytes);
            return bytes;
        }
    }
}