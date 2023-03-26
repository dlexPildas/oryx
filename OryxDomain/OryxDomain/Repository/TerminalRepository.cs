using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class TerminalRepository : Repository
    {
        public TerminalRepository(string path) : base(path)
        {
        }

        public async Task<IList<PD0>> FindList()
        {
            string command = @"SELECT * FROM PD0";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<PD0> lstPd0 = await connection.QueryAsync<PD0>(command);

                return lstPd0.ToList();
            }
        }

        public async Task<PD0> Find(string pd0codigo)
        {
            string command = @"SELECT * FROM PD0 WHERE PD0CODIGO = @pd0codigo";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                PD0 pd0 = await connection.QueryFirstOrDefaultAsync<PD0>(command, new { pd0codigo });

                return pd0;
            }
        }

        public async Task<int> Insert(PD0 pd0)
        {
            string command = @"INSERT INTO PD0 (PD0CODIGO, PD0NOME, PD0ANTENA, PD0CAMINHO, PD0HOST)
                               VALUES (@pd0codigo, @pd0nome, @pd0antena, @pd0caminho, @pd0host)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    pd0codigo = pd0.Pd0codigo,
                    pd0nome = pd0.Pd0nome,
                    pd0antena = pd0.Pd0antena,
                    pd0caminho = pd0.Pd0caminho,
                    pd0host = pd0.Pd0host
                    
                });

                return affectedRows;
            }
        }

        public async Task<int> Delete(string pd0codigo)
        {
            string command = @"DELETE FROM PD0 WHERE PD0CODIGO = @pd0codigo";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { pd0codigo });
            }
        }

        public async Task<int> Update(PD0 pd0)
        {
            string command = @"UPDATE PD0
                               SET PD0NOME = @pd0nome, PD0ANTENA = @pd0antena, PD0CAMINHO = @pd0caminho, PD0HOST = @pd0host
                               WHERE PD0CODIGO = @pd0codigo";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    pd0codigo = pd0.Pd0codigo,
                    pd0nome = pd0.Pd0nome,
                    pd0antena = pd0.Pd0antena,
                    pd0caminho = pd0.Pd0caminho,
                    pd0host = pd0.Pd0host
                });

                return affectedRows;
            }
        }

        public async Task<IList<PD0>> Search(string search, int limit, int offset)
        {
            search = string.Format("%{0}%", search);
            search = search.Replace(" ", "%");
            string command = @"SELECT *
                               FROM PD0
                               WHERE PD0CODIGO LIKE @search
                                  OR PD0NOME LIKE @search
                               LIMIT @limit OFFSET @offset";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<PD0> terminals = await connection.QueryAsync<PD0>(command, new { search, limit, offset });

                return terminals.ToList();
            }
        }
    }
}
