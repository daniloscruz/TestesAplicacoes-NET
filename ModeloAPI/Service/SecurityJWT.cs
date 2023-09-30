using Service.Util;
using ServiceModels;
using System.Net;

namespace Service
{
    public class SecurityJWT
    {
        public JsonWebToken GenerateJWTForUser(UserApiViewModel userApi, bool generatedNewRefreshToken = true, string refreshToken = "")
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            TokenManager token = new TokenManager();

            //Caso o controle de usuário for via banco precisaria criar uma consulta aqui
            var usuario = configuration.GetSection("AppSettings").GetSection("Usuario").Value;
            var senha = configuration.GetSection("AppSettings").GetSection("Senha").Value;

            if (userApi.User == usuario && userApi.Password == senha)
            {
                var clienteAPI = 1;

                try
                {
                    var accessToken = token.GenerateToken("0", clienteAPI);
                    var newRefreshToken = token.CreateRefreshToken(userApi.User, userApi.Password);
                    var refreshTokenResult = generatedNewRefreshToken == true ? newRefreshToken.Token : refreshToken;

                    var tokenAccessToken = new JsonWebToken()
                    {
                        AccessToken = $"{newRefreshToken.Token}:{accessToken}",
                        RefreshToken = refreshTokenResult,
                        ExpiresIn = newRefreshToken.TimeExpiration,
                        HttpCode = (int)HttpStatusCode.OK,
                        Msg = "OK"
                    };
                    return tokenAccessToken;
                }
                catch (Exception ex)
                {
                    throw new ArgumentNullException($"Ocorreu um erro ao tentar autenticar o usuário: {ex.Message}");
                }
            }
            return new JsonWebToken { };
        }
    }
}
