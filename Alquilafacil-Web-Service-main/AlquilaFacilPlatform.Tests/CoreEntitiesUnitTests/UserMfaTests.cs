using Xunit;
using AlquilaFacilPlatform.IAM.Domain.Model.Aggregates;
using OtpNet;

namespace AlquilaFacilPlatform.Tests.CoreEntitiesUnitTests
{
    public class UserMfaTests
    {
        [Fact(Skip = "Ignorada para ejecución local")]
        public void VerifyMfaCode_ReturnsTrue_ForValidCode()
        {
            var secretKey = KeyGeneration.GenerateRandomKey(20);
            var base32Key = Base32Encoding.ToString(secretKey);
            var user = new User("testuser", "hash", "test@email.com").UpdateMfaKey(base32Key);
            var totp = new Totp(secretKey);
            var code = totp.ComputeTotp();
            Assert.True(user.VerifyMfaCode(code));
        }

        [Fact(Skip = "Ignorada para ejecución local")]
        public void VerifyMfaCode_ReturnsFalse_ForInvalidCode()
        {
            var secretKey = KeyGeneration.GenerateRandomKey(20);
            var base32Key = Base32Encoding.ToString(secretKey);
            var user = new User("testuser", "hash", "test@email.com").UpdateMfaKey(base32Key);
            Assert.False(user.VerifyMfaCode("000000"));
        }
    }
}
