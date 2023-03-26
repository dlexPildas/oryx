using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class CollectionRepository : Repository
    {
        public CollectionRepository(string path) : base(path)
        {
        }

        public async Task<IList<CO0>> FindList()
        {
            string command = @"SELECT * FROM CO0";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<CO0> lstCo0 = await connection.QueryAsync<CO0>(command);

                return lstCo0.ToList();
            }
        }

        public async Task<CO0> Find(string co0colecao)
        {
            string command = @"SELECT * FROM CO0
                                WHERE CO0COLECAO = @co0colecao";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CO0 co0 = await connection.QueryFirstOrDefaultAsync<CO0>(command, new { co0colecao });

                return co0;
            }
        }

        public async Task<IList<CO0>> Search(string search, int limit, int offset)
        {
            search = string.Format("%{0}%", search);
            search = search.Replace(" ", "%");
            string command = @"SELECT *
                               FROM CO0
                               WHERE CO0COLECAO LIKE @search
                                  OR CO0NOME LIKE @search
                               LIMIT @limit OFFSET @offset";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<CO0> collections = await connection.QueryAsync<CO0>(command, new { search, limit, offset });

                return collections.ToList();
            }
        }

        public async Task<int> Update(CO0 co0)
        {
            string command = @"UPDATE CO0
                               SET CO0NOME = @co0nome, CO0B2B = @co0b2b
                               WHERE CO0COLECAO = @co0colecao";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    co0colecao = co0.Co0colecao,
                    co0nome = co0.Co0nome,
                    co0b2b = co0.Co0b2b
                });

                return affectedRows;
            }
        }

        public async Task<int> Insert(CO0 co0)
        {
            string command = @"INSERT INTO CO0 (CO0COLECAO, CO0NOME, CO0B2B)
                               VALUES (@co0colecao, @co0nome, @co0b2b)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    co0colecao = co0.Co0colecao,
                    co0nome = co0.Co0nome,
                    co0b2b = co0.Co0b2b
                });

                return affectedRows;
            }
        }

        public async Task<int> Delete(string co0colecao)
        {
            string command = @"DELETE FROM CO0 WHERE CO0COLECAO = @co0colecao";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { co0colecao });
            }
        }

    }
}
