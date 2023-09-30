using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Service.Util;
using ServiceModels;

namespace ModeloAPI.Seguranca
{
    public class SecurityAPI : ActionFilterAttribute
    {
        public SecurityAPI()
        {
        }

        public string ErrorMessage { get; set; } = null;

        public bool AllowMultiple
        {
            get { return false; }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var schemaName = "Bearer";
            var headers = filterContext.HttpContext.Request.Headers;
            var authorization = headers.Where(c => c.Key == "Authorization").FirstOrDefault();

            string[] TokenAndUser = null;

            if (string.IsNullOrEmpty(authorization.Key))
            {
                var response = new ReturnViewModel()
                {
                    Erro = true,
                    Message = ErrorMessage ?? $"Token não informado!"
                };

                filterContext.Result = new BadRequestObjectResult(response);
                return;
            }

            var bearer = authorization.Value.ToString().Substring(0, 6);

            if (bearer != schemaName)
            {
                var response = new ReturnViewModel()
                {
                    Erro = true,
                    Message = ErrorMessage ?? $"Invalid Authorization Schema"
                };

                filterContext.Result = new BadRequestObjectResult(response);

                return;
            }

            TokenAndUser = authorization.Value.ToString().Split(':');

            var Token = "";

            try
            {
                Token = TokenAndUser[0].Substring(0, 7) + TokenAndUser[1];
            }
            catch (Exception ex)
            {
                var response = new ReturnViewModel()
                {
                    Erro = true,
                    Message = ErrorMessage ?? $"Token inválido!"
                };
            }


            if (string.IsNullOrEmpty(Token))
            {

                var response = new ReturnViewModel()
                {
                    Erro = true,
                    Message = ErrorMessage ?? $"Token inválido!"
                };

                filterContext.Result = new BadRequestObjectResult(response);
                return;
            }
            //remove schemaName
            var tokenBearer = Token.Replace(schemaName, "").Trim();

            TokenManager tokenManager = new TokenManager();

            var principal = tokenManager.GetPrincipal(tokenBearer);

            if (principal == null)
            {
                var response = new ReturnViewModel()
                {
                    Erro = true,
                    Message = ErrorMessage ?? $"Token inválido!"
                };

                filterContext.Result = new BadRequestObjectResult(response);
                return;

            }

            filterContext.HttpContext.User = principal;

        }
    }
}
