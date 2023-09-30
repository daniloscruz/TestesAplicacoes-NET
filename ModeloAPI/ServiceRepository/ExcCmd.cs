using System.Data.SqlClient;

namespace ServiceRepository
{
    public class ExcCmd
    {
        public ExcCmd()
        {

        }

        public SqlDataReader ExecSelect(string _strsql,
                              SqlConnection conex)
        {

            SqlDataAdapter da;

            da = new SqlDataAdapter(_strsql, conex);

            try
            {
                return da.SelectCommand.ExecuteReader();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao executar o SELECT!" + "\n\n"
                                        + "Instrução SQl:" + "\n"
                                        + _strsql, ex);
            }
            finally
            {
            }
        }
    }
}
