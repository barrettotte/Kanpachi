using System;
using System.Security.Cryptography;
using System.Text;

namespace Kanpachi.Lib{

    public static class Pbkdf2{

        private const int defaultIterations = 100000;
        private const int hashSize = 16;

        public static string Hash(string key){
            return Hash(key, SecUtils.GenerateSalt());   
        }

        public static string Hash(string key, byte[] salt){
            return Hash(Encoding.UTF8.GetBytes(key), salt, defaultIterations);
        }

        public static string Hash(byte[] key, byte[] salt){
            return Hash(key, salt, defaultIterations);
        }

        private static string Hash(byte[] key, byte[] salt, int iterations){
            byte[] hash = Pbkdf2Bytes(key, salt, iterations);
            return Convert.ToBase64String(hash);
        }

        // validate hash matches expected hash
        public static bool Validate(string key, string storedHash){
            var compare = Pbkdf2Bytes(Encoding.UTF8.GetBytes(key), SecUtils.GenerateSalt(), defaultIterations);
            return SequenceEqual(compare, Convert.FromBase64String(storedHash));
        }

        // generate hash with PBKDF2
        private static byte[] Pbkdf2Bytes(byte[] key, byte[] salt, int iterations){
            using(var alg = new Rfc2898DeriveBytes(key, salt, iterations)){
                return alg.GetBytes(hashSize);
            }
        }

        // bytewise compare equal
        private static bool SequenceEqual(byte[] a, byte[] b){
            var diff = (uint)a.Length ^ (uint)b.Length;
            for(uint i = 0; i < a.Length && i < b.Length; i++){
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }
    }
}
