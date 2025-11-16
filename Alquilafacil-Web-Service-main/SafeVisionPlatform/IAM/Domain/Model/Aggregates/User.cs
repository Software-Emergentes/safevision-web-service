using System.Text.Json.Serialization;
using OtpNet;
using SafeVisionPlatform.IAM.Domain.Model.Entities;
using SafeVisionPlatform.IAM.Domain.Model.ValueObjects;

namespace SafeVisionPlatform.IAM.Domain.Model.Aggregates
{
    public class User
    {
        public User(string username, string passwordHash, string email)
        {
            Username = username;
            PasswordHash = passwordHash;
            Email = email;
            RoleId = (int)EUserRoles.User;  // Valor predeterminado para un usuario normal.
        }

        public int Id { get; set; }
        public string Username { get; private set; }
        public string Email { get; set; }
        public int RoleId { get; set; }

        [JsonIgnore] 
        public string PasswordHash { get; private set; }

        [JsonIgnore]
        public string? MfaKey { get; private set; }

        public virtual ICollection<MfaCode>? MfaCodes { get; set; }

        public User UpdateUsername(string username)
        {
            Username = username;
            return this;
        }

        public User UpdatePasswordHash(string passwordHash)
        {
            PasswordHash = passwordHash;
            return this;
        }
        
        public void UpgradeToAdmin()
        {
            RoleId = (int)EUserRoles.Admin;
        }

        public User UpdateMfaKey(string mfaKey)
        {
            MfaKey = mfaKey;
            return this;
        }

        // Este método requiere Otp.NET para la verificación TOTP
        
        public bool VerifyMfaCode(string code)
        {
            if (string.IsNullOrEmpty(MfaKey)) return false;
            var totp = new Totp(Base32Encoding.ToBytes(MfaKey));
            return totp.VerifyTotp(code, out _);
        }
    }
}