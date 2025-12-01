namespace SafeVisionPlatform.IAM.Interfaces.REST.Resources
{
    public class VerifyMfaCodeRequest
    {
        public int UserId { get; set; }
        public string Code { get; set; } = string.Empty;
    }
}

