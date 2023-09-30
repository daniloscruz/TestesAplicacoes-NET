using ServiceModels;
using System.Data.SqlClient;

namespace ServiceRepository
{
    public class ConsultaProdutos
    {
        public List<Produtos> ObterProdutos()
        {
            //Adiciona o arquivo config para pegar a string
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            String strConexao = configuration.GetConnectionString("dataBase");

            Conexao con = new Conexao(strConexao);

            String strSelect = "";

            ExcCmd exec = new ExcCmd();

            List<Produtos> ListarProdutos = new List<Produtos>();

            try
            {

                strSelect = $"SELECT ProductID, Name, ProductNumber, Color, StandardCost FROM SalesLT.Product";

                SqlDataReader read;

                con.Conecta(strConexao);

                read = exec.ExecSelect(strSelect, con.getConexao());

                if (read.HasRows)
                {
                    while (read.Read())
                    {
                        try
                        {
                            Produtos produto = new Produtos();

                            produto.ProductID = int.Parse(read["ProductID"].ToString());
                            produto.Name = read["Name"].ToString();
                            produto.ProductNumber = read["ProductNumber"].ToString();
                            produto.Color = read["Color"].ToString();
                            produto.StandardCost = read["StandardCost"].ToString();

                            ListarProdutos.Add(produto);

                        }
                        catch (Exception Err)
                        {
                            throw new Exception($"Erro ao capturar campo {Err.Message}");

                        }
                    }
                }
            }
            catch (Exception Err)
            {
                throw new Exception($"Erro ao buscar registro no banco {Err.Message}");

            }
            finally
            {
                con.Desconecta();
            }

            return ListarProdutos;

        }
    }
}
