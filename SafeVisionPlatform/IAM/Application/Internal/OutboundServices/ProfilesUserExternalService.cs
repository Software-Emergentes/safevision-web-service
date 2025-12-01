namespace SafeVisionPlatform.IAM.Application.Internal.OutboundServices;

public class ProfilesUserExternalService : IProfilesUserExternalService
{
    public async Task<int> CreateProfile(
        string name,
        string fatherName,
        string motherName,
        string dateOfBirth,
        string documentNumber,
        string phone,
        int userId
    )
    {
        // TODO: Implement profile creation when Profiles bounded context is available
        await Task.Delay(0);
        return userId;
    }

    public async Task<int> UpdateProfile(
        string name,
        string fatherName,
        string motherName,
        string dateOfBirth,
        string documentNumber,
        string phone,
        string bankAccountNumber,
        string interbankAccountNumber,
        int userId
    )
    {
        // TODO: Implement profile update when Profiles bounded context is available
        await Task.Delay(0);
        return userId;
    }
}