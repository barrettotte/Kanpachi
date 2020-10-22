using System;
using System.Security.Cryptography;
using System.IO;
using DeviceId;
using DeviceId.Encoders;
using DeviceId.Formatters;

namespace Kanpachi.Lib{

    public static class SecUtils{

        public static string EncryptProfile(KanpachiProfile profile){
            var passphrase = BuildPassphrase(profile);
            return Convert.ToBase64String(EncryptAes(passphrase, profile.PasswordDecrypted));
        }

        public static byte[] EncryptAes(string passphrase, string plainText){
            return EncryptAes(DeriveKey(passphrase), plainText);
        }

        public static byte[] EncryptAes(byte[] key, string plainText){
            byte[] encrypted;
            byte[] iv;

            using(Aes aes = Aes.Create()){
                aes.Key = key;
                aes.GenerateIV();
                iv = aes.IV;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

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

        public static string DecryptProfile(KanpachiProfile profile){
            return DecryptAes(BuildPassphrase(profile), profile.Password);
        }

        public static string DecryptAes(string passphrase, byte[] cipherCombined){
            return DecryptAes(DeriveKey(passphrase), cipherCombined);
        }

        public static string DecryptAes(string passphrase, string cipherCombined){
            return DecryptAes(DeriveKey(passphrase), Convert.FromBase64String(cipherCombined));
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
                aes.Padding = PaddingMode.PKCS7;

                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using(var ms = new MemoryStream(cipher)){
                    using(var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read)){
                        using(var sr = new StreamReader(cs)){
                            plainText = sr.ReadToEnd();
                        }
                    }
                }
            }
            return plainText;
        }

        // derive encryption key from passphrase and salt fed into PBKDF2
        public static byte[] DeriveKey(string passphrase){
            var pbkdf2 = new Pbkdf2();
            byte[] salt = GenerateSalt();
            return Convert.FromBase64String(pbkdf2.Hash(passphrase, salt));
        }

        // generate salt based on current hardware
        public static byte[] GenerateSalt(){
            string hardwareSalt = new DeviceIdBuilder()
                .AddMotherboardSerialNumber()
                .AddUserName()
                .AddSystemDriveSerialNumber()
                .UseFormatter(new HashDeviceIdFormatter(() => SHA256.Create(), new Base64ByteArrayEncoder()))
                .ToString()
            ;
            return Convert.FromBase64String(hardwareSalt);
        }

        // build simple passphrase for encryption
        private static string BuildPassphrase(KanpachiProfile profile){
            return $"{profile.Name}+{profile.Host}+{profile.User}";
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