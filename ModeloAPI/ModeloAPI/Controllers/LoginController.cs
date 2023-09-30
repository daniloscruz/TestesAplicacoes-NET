using Microsoft.AspNetCore.Mvc;
using Service;
using ServiceModels;

namespace ModeloAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        [ProducesResponseType(typeof(JsonWebToken), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("Account")]
        public ActionResult Account([FromBody] UserApiViewModel userApi)
        {
            SecurityJWT token = new SecurityJWT();

            try
            {
                if (userApi == null || userApi.User == null || userApi.Password == null)
                {
                    return BadRequest("Informe os dados para autenticação.");
                }

                var jwtUser = token.GenerateJWTForUser(userApi);

                if (jwtUser != null)
                {
                    return Ok(jwtUser);
                }

                return NotFound("Credencias Inválidas");
            }
            catch (Exception e)
            {
                return BadRequest($"Ocorreu um erro ao tentar autenticar o usuário. {e.Message}");
            }
        }
    }
}
