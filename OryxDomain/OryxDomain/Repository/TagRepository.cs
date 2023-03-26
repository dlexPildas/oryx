using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class TagRepository : Repository
    {
        public TagRepository(string path) : base(path)
        {
        }

        public async Task<IList<ET0>> FindList()
        {
            string command = @"SELECT * FROM ET0";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<ET0> lstEt0 = await connection.QueryAsync<ET0>(command);

                return lstEt0.ToList();
            }
        }

        public async Task<ET0> Find(string et0etiq)
        {
            string command = @"SELECT * FROM ET0
                                WHERE ET0ETIQ = @et0etiq";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                ET0 et0 = await connection.QueryFirstOrDefaultAsync<ET0>(command, new { et0etiq });

                return et0;
            }
        }

        public async Task<IList<ET0>> Search(string search, int limit, int offset)
        {
            search = string.Format("%{0}%", search);
            search = search.Replace(" ", "%");
            string command = @"SELECT *
                               FROM ET0
                               WHERE ET0ETIQ LIKE @search
                                  OR ET0NOME LIKE @search
                               LIMIT @limit OFFSET @offset";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<ET0> tags = await connection.QueryAsync<ET0>(command, new { search, limit, offset });

                return tags.ToList();
            }
        }

        public async Task<int> Update(ET0 et0)
        {
            string command = @"UPDATE ET0
                               SET ET0NOME = @et0nome
                               WHERE ET0ETIQ = @et0etiq";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    et0nome = et0.Et0nome,
                    et0Etiq = et0.Et0etiq
                });

                return affectedRows;
            }
        }

        public async Task<int> Insert(ET0 et0)
        {
            string command = @"INSERT INTO ET0 (ET0ETIQ, ET0NOME)
                               VALUES (@et0etiq, @et0nome)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    et0etiq = et0.Et0etiq,
                    et0nome = et0.Et0nome
                });

                return affectedRows;
            }
        }

        public async Task<int> Delete(string et0etiq)
        {
            string command = @"DELETE FROM ET0 WHERE ET0ETIQ = @et0etiq";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { et0etiq });
            }
        }
    }
}
