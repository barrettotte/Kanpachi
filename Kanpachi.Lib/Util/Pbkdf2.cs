using System;
using System.Security.Cryptography;
using System.Text;

namespace Kanpachi.Lib{

    public class Pbkdf2{

        int DefaultIterations {get;}
        int HashSize {get;}

        public Pbkdf2(){
            DefaultIterations = 1000000;
            HashSize = 16;
        }

        public Pbkdf2(int defaultIterations, int hashSize){
            DefaultIterations = defaultIterations;
            HashSize = hashSize;
        }

        public string Hash(string key){
            return Hash(key, SecUtils.GenerateSalt());   
        }

        public string Hash(string key, byte[] salt){
            return Hash(Encoding.UTF8.GetBytes(key), salt, DefaultIterations);
        }

        public string Hash(byte[] key, byte[] salt){
            return Hash(key, salt, DefaultIterations);
        }

        private string Hash(byte[] key, byte[] salt, int iterations){
            byte[] hash = Pbkdf2Bytes(key, salt, iterations);
            return Convert.ToBase64String(hash);
        }

        // validate hash matches expected hash
        public bool Validate(string key, string storedHash){
            var compare = Pbkdf2Bytes(Encoding.UTF8.GetBytes(key), SecUtils.GenerateSalt(), DefaultIterations);
            return SequenceEqual(compare, Convert.FromBase64String(storedHash));
        }

        // generate hash with PBKDF2
        private byte[] Pbkdf2Bytes(byte[] key, byte[] salt, int iterations){
            using(var alg = new Rfc2898DeriveBytes(key, salt, iterations)){
                return alg.GetBytes(HashSize);
            }
        }

        // bytewise compare equal
        private bool SequenceEqual(byte[] a, byte[] b){
            var diff = (uint)a.Length ^ (uint)b.Length;
            for(uint i = 0; i < a.Length && i < b.Length; i++){
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }
    }
}
