using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModeloAPI.Seguranca;
using Service;
using ServiceModels;

namespace ModeloAPI.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        //Segurança para acessar com o token
        [SecurityAPI]
        [HttpGet]
        public List<Produtos> GetProdutos()
        {
            var resultado = new ProdutosService();

            return resultado.Consultar();
        }
    }
}
