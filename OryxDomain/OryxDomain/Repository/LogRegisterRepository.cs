using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class LogRegisterRepository : Repository
    {
        public LogRegisterRepository(string path) : base(path)
        {
        }

        public async Task<int> Insert(LX8 lx8)
        {
            string command = @"INSERT INTO LX8(LX8USUARIO,LX8COMPUT,LX8ACESSO,LX8ARQUIVO,LX8ACAO,LX8CHAVE,LX8DATAH,LX8INFO)
		                                VALUES (@lx8usuario,@lx8comput,@lx8acesso,@lx8arquivo,@lx8acao,@lx8chave,@lx8datah,@lx8info)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    lx8.Lx8usuario,
                    lx8.Lx8comput,
                    lx8.Lx8acesso,
                    lx8.Lx8arquivo,
                    lx8.Lx8acao,
                    lx8.Lx8chave,
                    lx8.Lx8datah,
                    lx8.Lx8info
                });

                return affectedRows;
            }
        }
    }
}
