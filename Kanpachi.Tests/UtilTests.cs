using System;
using Kanpachi.Lib;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Kanpachi.Tests{

    public class UtilTests{

        private readonly ITestOutputHelper output;

        public UtilTests(ITestOutputHelper output){
            this.output = output;
        }

        [Fact]
        public void Test_Pbkdf2(){
            string hash = "x7JH1xlG/PgzXTplNhXhvw=="; 
            Pbkdf2 pbkdf2 = new Pbkdf2(100000, 16);
            Assert.True(pbkdf2.Validate("password@12345", hash));
            Assert.False(pbkdf2.Validate("wasd", hash));
        }

        [Fact]
        public void Test_AesRoundTrip(){
            var passphrase = "!!!PaSs_pHr@s3_";
            var secret = "hello world";

            string encrypted = Convert.ToBase64String(SecUtils.EncryptAes(passphrase, secret));
            string decrypted = SecUtils.DecryptAes(passphrase, encrypted);
            Assert.True(secret == decrypted);

            // another round to be safe
            encrypted = Convert.ToBase64String(SecUtils.EncryptAes(passphrase, decrypted));
            decrypted = SecUtils.DecryptAes(passphrase, encrypted);
            Assert.True(secret == decrypted);
        }

        [Fact]
        public void Test_RegexIbmiObject(){
            Assert.True(RegexUtils.MatchIbmiObject("HELLORPG"));
            Assert.True(RegexUtils.MatchIbmiObject("A"));             // min name length
            Assert.True(RegexUtils.MatchIbmiObject("ABC@$$$123"));    // max name length
            Assert.False(RegexUtils.MatchIbmiObject("_AAAABCD"));     // starts with '_'
            Assert.False(RegexUtils.MatchIbmiObject("."));            // starts with '.'
            Assert.False(RegexUtils.MatchIbmiObject("A1234567890"));  // name too long
        }

        [Fact]
        public void Test_RegexQsysPaths(){
            Assert.True(RegexUtils.MatchSrcPfPath("BOLIB/QRPGLESRC"));
            Assert.False(RegexUtils.MatchSrcPfPath("BOLIB"));            // missing srcpf
            Assert.False(RegexUtils.MatchSrcPfPath("BOLIB/_QRPGLESRC")); // srcpf starts with '_'
            Assert.False(RegexUtils.MatchSrcPfPath("/QRPGLESRC"));       // missing library

            Assert.True(RegexUtils.MatchSrcMbrPath("BOLIB/QRPGLESRC/HELLORPGLE"));
            Assert.False(RegexUtils.MatchSrcMbrPath("BOLIB/QRPGLESRC"));            // missing member
            Assert.False(RegexUtils.MatchSrcMbrPath("BOLIB/QRPGLESRC//HELLO"));     // double '/'
        }
    }
}
