using SwedishAgora.Data.DataServices.Results;

namespace SwedishAgora.Data.DataServices.Interfaces
{
    public interface ILoginDataService
    {
        Task SetUserLoggedInAsync(string userName);
        Task<VerifyCredentialsResult> VerifyCredentialsAsync(string username, string password);
    }
}
