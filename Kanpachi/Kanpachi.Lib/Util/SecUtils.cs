using System;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using DeviceId;
using DeviceId.Encoders;
using DeviceId.Formatters;

namespace Kanpachi.Lib{

    public static class SecUtils{

        // derive encryption key
        public static byte[] DeriveKey(SecureString password){
            byte[] salt = GenerateSalt();
            byte[] hash = Convert.FromBase64String(Pbkdf2.Hash(password, salt));
            return hash;
        }

        // generate salt based on current hardware
        // TODO: get salt to 128-bit
        public static byte[] GenerateSalt(){
            string hardwareSalt = new DeviceIdBuilder()
                .AddMotherboardSerialNumber()
                .AddProcessorId()
                .UseFormatter(new HashDeviceIdFormatter(() => SHA256.Create(), new Base64ByteArrayEncoder()))
                .ToString()
            ;
            return Convert.FromBase64String(hardwareSalt);
        }

        // generate simple initialization vector
        private static byte[] GenerateIv(int size){
            var cryptoRandom = new RNGCryptoServiceProvider();
            byte[] iv = new byte[size];
            
            cryptoRandom.GetBytes(iv);
            return iv;
        }
    }
}
