using System;
using Kanpachi.Lib;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Kanpachi.Tests{

    public class ProfileTests{

        private readonly ITestOutputHelper output;

        public ProfileTests(ITestOutputHelper output){
            this.output = output;
        }

        [Fact]
        public void Test_ProfileEncryptRoundTrip(){
            var password = "mYP4SsW0rd@1234";
            KanpachiProfile profile = new KanpachiProfile("myprofile", "SOME400", "MYUSER");
            profile.PasswordDecrypted = password;

            profile.Password = SecUtils.EncryptProfile(profile);
            profile.PasswordDecrypted = SecUtils.DecryptProfile(profile);

            Assert.True(password == profile.PasswordDecrypted);
        }
    }
}
