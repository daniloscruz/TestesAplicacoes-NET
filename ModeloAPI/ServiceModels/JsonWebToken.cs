namespace ServiceModels
{
    public class JsonWebToken : ResponseStatus
    {
        public JsonWebToken()
        {
            AccessToken = string.Empty;
            RefreshToken = string.Empty;
            ExpiresIn = null;

        }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? ExpiresIn { get; set; }
    }
}
