using System;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Kanpachi.Lib{

    public static class Pbkdf2{

        private const int defaultIterations = 100000;
        private const int hashSize = 16;

        public static string Hash(string password){
            return Hash(password, SecUtils.GenerateSalt());   
        }

        public static string Hash(string password, byte[] salt){
            return Hash(Encoding.UTF8.GetBytes(password), salt, defaultIterations);
        }

        private static string Hash(byte[] password, byte[] salt, int iterations){
            byte[] hash = Pbkdf2Bytes(password, salt, iterations);
            return Convert.ToBase64String(hash);
        }

        // validate password hash matches expected hash
        public static bool Validate(string password, string storedHash){
            var compare = Pbkdf2Bytes(Encoding.UTF8.GetBytes(password), SecUtils.GenerateSalt(), defaultIterations);
            return SequenceEqual(compare, Convert.FromBase64String(storedHash));
        }

        // generate hash with PBKDF2
        private static byte[] Pbkdf2Bytes(byte[] password, byte[] salt, int iterations){
            using(var alg = new Rfc2898DeriveBytes(password, salt, iterations)){
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
