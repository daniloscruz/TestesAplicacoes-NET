using System.Data.SqlClient;

namespace ServiceRepository
{
    public class Conexao
    {
        SqlConnection conex;

        public Conexao(String strcon)
        {
            this.conex = new SqlConnection();
        }

        public void Conecta(String strConexao)
        {
            conex.ConnectionString = strConexao;
            conex.Open();
        }

        public void Desconecta()
        {
            conex.Close();
        }

        public SqlConnection getConexao()
        {
            return conex;
        }
    }
}
