namespace SafeVisionPlatform.IAM.Infrastructure.Tokens.JWT.Configuration;

public class TokenSettings
{
    /**
 * <summary>
 *     This class is used to store the token settings.
 *     It is used to configure the token settings in the app settings .json file.
 * </summary>
 */
    public required string Secret { get; set; }
}