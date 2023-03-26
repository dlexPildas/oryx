using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class IntegrationRepresentativeRepository : Repository
    {
        public IntegrationRepresentativeRepository(string path) : base(path)
        {
        }

        public async Task<IList<PD8>> FindAll()
        {
            string command = @"SELECT * FROM PD8";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<PD8> lstPd8 = await connection.QueryAsync<PD8>(command);

                return lstPd8.ToList();
            }
        }

        public async Task<PD8> Find(string pd8codigo)
        {
            string command = @"SELECT * FROM PD8 WHERE PD8CODIGO = @pd8codigo";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                PD8 pd8 = await connection.QueryFirstOrDefaultAsync<PD8>(command, new { pd8codigo });

                return pd8;
            }
        }

        public async Task<int> Update(PD8 pd8)
        {
            string command = @"UPDATE PD8
                               SET PD8NOME = @pd8nome,
                                   PD8BASEURL = @pd8baseurl,
                                   PD8TOKEN = @pd8token,
                                   PD8TIPO = @pd8tipo,
                                   PD8FLAG1 = @pd8flag1,
                                   PD8FLAG2 = @pd8flag2,
                                   PD8FLAG3 = @pd8flag3
                             WHERE PD8CODIGO = @pd8codigo";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    pd8codigo = pd8.Pd8codigo,
                    pd8nome = pd8.Pd8nome,
                    pd8baseurl = pd8.Pd8baseurl,
                    pd8token = pd8.Pd8token,
                    pd8tipo = pd8.Pd8tipo,
                    pd8flag1 = pd8.Pd8flag1,
                    pd8flag2 = pd8.Pd8flag2,
                    pd8flag3 = pd8.Pd8flag3,
                });

                return affectedRows;
            }
        }

        public async Task<IList<PD8>> Search(string search, int limit, int offset)
        {
            search = string.Format("%{0}%", search);
            search = search.Replace(" ", "%");
            string command = @"SELECT *
                               FROM PD8
                               WHERE PD8CODIGO LIKE @search
                                  OR PD8NOME LIKE @search
                               LIMIT @limit OFFSET @offset";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<PD8> lstPd8 = await connection.QueryAsync<PD8>(command, new { search, limit, offset });

                return lstPd8.ToList();
            }
        }

        public async Task<int> Insert(PD8 pd8)
        {
            string command = @"INSERT INTO PD8 (PD8CODIGO,PD8NOME,PD8BASEURL,PD8TOKEN,PD8TIPO,PD8FLAG1,PD8FLAG2,PD8FLAG3)
                               VALUES (@pd8codigo,@pd8nome,@pd8baseurl,@pd8token,@pd8tipo,@pd8flag1,@pd8flag2,@pd8flag3)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    pd8codigo = pd8.Pd8codigo,
                    pd8nome = pd8.Pd8nome,
                    pd8baseurl = pd8.Pd8baseurl,
                    pd8token = pd8.Pd8token,
                    pd8tipo = pd8.Pd8tipo,
                    pd8flag1 = pd8.Pd8flag1,
                    pd8flag2 = pd8.Pd8flag2,
                    pd8flag3 = pd8.Pd8flag3,
                });

                return affectedRows;
            }
        }

        public async Task<int> Delete(string pd8codigo)
        {
            string command = @"DELETE FROM PD8 WHERE PD8CODIGO = @pd8codigo";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { pd8codigo });
            }
        }
    }
}
