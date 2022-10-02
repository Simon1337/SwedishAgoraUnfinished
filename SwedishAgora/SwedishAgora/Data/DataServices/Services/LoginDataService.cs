using SwedishAgora.Data.DataServices.Interfaces;
using SwedishAgora.Data.DataServices.Results;

namespace SwedishAgora.Data.DataServices.Classes
{
    public sealed class LoginDataService : ILoginDataService
    {
        public Task SetUserLoggedInAsync(string userName)
        {
            throw new NotImplementedException();
        }

        public Task<VerifyCredentialsResult> VerifyCredentialsAsync(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}
