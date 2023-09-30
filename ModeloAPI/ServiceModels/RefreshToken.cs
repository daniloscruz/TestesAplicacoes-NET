namespace ServiceModels
{
    public class RefreshToken
    {
        public string Token { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime TimeExpiration { get; set; }
        public int CodigoUsuario { get; set; }
        public string perfilUsuario { get; set; }
    }
}
