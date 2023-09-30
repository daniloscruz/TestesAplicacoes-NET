using Microsoft.IdentityModel.Tokens;
using ServiceModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Service.Util
{
    public class TokenManager
    {
        public IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

        public RefreshToken CreateRefreshToken(string usuario, string senha, string perfilUsuario = null)
        {

            var refreshToken = new RefreshToken
            {
                UserName = usuario,
                Password = senha,
                perfilUsuario = perfilUsuario != null ? perfilUsuario : string.Empty,
                TimeExpiration = DateTime.Now.AddMinutes(Convert.ToInt32(configuration.GetSection("AppSettings").GetSection("timeValidateAccessTokenMinute").Value))
            };

            string token;
            var randomNumber = new byte[32];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                token = Convert.ToBase64String(randomNumber);
            }


            refreshToken.Token = token.Replace("+", string.Empty)
                .Replace("=", string.Empty)
                .Replace("/", string.Empty);

            var identityRefreshToken = "." + EncryptRefreshToken($"{refreshToken.UserName}|{refreshToken.Password}|{refreshToken.TimeExpiration}|{refreshToken.perfilUsuario}");

            refreshToken.Token += identityRefreshToken;
            return refreshToken;

        }

        public ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token) as JwtSecurityToken;

                var Iss = configuration.GetSection("AppSettings").GetSection("Iss").Value;

                var Aud = configuration.GetSection("AppSettings").GetSection("Aud").Value;

                var SecurityKey = configuration.GetSection("AppSettings").GetSection("SecurityKey").Value;


                if (jwtToken == null)
                {
                    return null;
                }

                byte[] key = Convert.FromBase64String(SecurityKey);

                var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecurityKey));
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateIssuer = true,
                    ValidIssuer = Iss,
                    ValidateAudience = true,
                    ValidAudience = Aud,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RequireExpirationTime = true,
                };


                SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

                var horaCriacao = jwtToken.Claims.Where(c => c.Type == "nbf").FirstOrDefault();
                var horaExpiracao = jwtToken.Claims.Where(c => c.Type == "exp").FirstOrDefault();

                return principal;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string EncryptRefreshToken(string refresh_token)
        {
            RijndaelManaged objrij = new RijndaelManaged();
            //iniciar o modo de operação
            objrij.Mode = CipherMode.CBC;
            //setar o padding de operação da string
            objrij.Padding = PaddingMode.PKCS7;
            //setar o tamanho (em bits) da operação 
            objrij.KeySize = 0x80;
            //setar o tamanho do bloco    
            objrij.BlockSize = 0x80;
            //setar a chave de criptografia
            byte[] passBytes = Encoding.UTF8.GetBytes("d6f1ee375ec511a7a4e89b0a1177dd9d");
            //setar o vetor de bytes de inicialização para criptografia
            byte[] EncryptionkeyBytes = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            int len = passBytes.Length;
            if (len > EncryptionkeyBytes.Length)
            {
                len = EncryptionkeyBytes.Length;
            }
            Array.Copy(passBytes, EncryptionkeyBytes, len);

            objrij.Key = EncryptionkeyBytes;
            objrij.IV = EncryptionkeyBytes;

            //Cria uma chave simétrica
            ICryptoTransform objtransform = objrij.CreateEncryptor();
            byte[] textDataByte = Encoding.UTF8.GetBytes(refresh_token);
            //Converte para string e retorna
            return Convert.ToBase64String(objtransform.TransformFinalBlock(textDataByte, 0, textDataByte.Length));

        }

        public string GenerateToken(string usuario, int empresaId, string perfilUsuario = null)
        {
            var SecurityKey = configuration.GetSection("AppSettings").GetSection("SecurityKey").Value;
            var Iss = configuration.GetSection("AppSettings").GetSection("Iss").Value;
            var Aud = configuration.GetSection("AppSettings").GetSection("Aud").Value;
            var timeValidateTokenSecond = configuration.GetSection("AppSettings").GetSection("timeValidateTokenSecond").Value;

            var now = DateTime.UtcNow;

            var rolePerfil = perfilUsuario != null ? perfilUsuario : string.Empty;

            var claims = new Claim[]
            {
                    new Claim("role", rolePerfil),
                    new Claim(JwtRegisteredClaimNames.Sub, usuario),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Sid, empresaId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, now.ToUniversalTime().ToString(), ClaimValueTypes.Integer64)
            };

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecurityKey));
           
            var jwt = new JwtSecurityToken(
                issuer: Iss,
                audience: Aud,
                claims: claims,
                notBefore: now,
                expires: now.Add(TimeSpan.FromSeconds(Convert.ToInt32(timeValidateTokenSecond))),
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            );
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }
    }
}
