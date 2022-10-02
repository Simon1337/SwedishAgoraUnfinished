namespace SwedishAgora.Data.DataServices.Results
{
    public sealed class VerifyCredentialsResult
    {
        public string Message { get; set; } = string.Empty;
        public bool AreCredentialsValid { get; set; }
    }
}
