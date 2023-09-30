using ServiceModels;
using ServiceRepository;

namespace Service
{
    public class ProdutosService
    {
        public List<Produtos> Consultar()
        {
            var consultar = new ConsultaProdutos();

            return consultar.ObterProdutos();
        }
    }
}
