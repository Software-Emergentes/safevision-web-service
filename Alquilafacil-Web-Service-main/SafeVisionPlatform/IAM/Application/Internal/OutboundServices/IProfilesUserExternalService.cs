namespace SafeVisionPlatform.IAM.Application.Internal.OutboundServices;

public interface IProfilesUserExternalService
{
    Task<int> CreateProfile(
        string name,
        string fatherName,
        string motherName,
        string dateOfBirth,
        string documentNumber,
        string phone,
        int userId
    );

    Task<int> UpdateProfile(

        string name,
        string fatherName,
        string motherName,
        string dateOfBirth,
        string documentNumber,
        string phone,
        string BankAccountNumber,
        string InterbankAccountNumber,
        int userId
    );
}