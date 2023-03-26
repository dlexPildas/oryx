using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class ProductSizesRepository : Repository
    {
        public ProductSizesRepository(string path) : base(path)
        {
        }

        public async Task<IList<PR3>> FindList(string pr3produto)
        {
            string command = @"SELECT * FROM PR3 WHERE PR3PRODUTO = @pr3produto";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<PR3> lstpr3 = await connection.QueryAsync<PR3>(command, new { pr3produto });

                return lstpr3.ToList();
            }
        }

        public async Task<PR3> Find(string pr3produto, string pr3tamanho)
        {
            string command = @"SELECT * FROM PR3 WHERE PR3PRODUTO = @pr3produto AND PR3TAMANHO = @pr3tamanho";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                PR3 pr3 = await connection.QueryFirstOrDefaultAsync<PR3>(command, new { pr3produto, pr3tamanho });

                return pr3;
            }
        }

        public async Task<int> Insert(PR3 pr3)
        {
            string command = @"INSERT INTO PR3 (PR3PRODUTO, PR3TAMANHO, PR3LOTEMUL, PR3PRECO, PR3PESOLIQ)
                               VALUES (@Pr3produto, @Pr3tamanho, @Pr3lotemul, @Pr3preco, @Pr3pesoliq)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    pr3.Pr3produto,
                    pr3.Pr3tamanho,
                    pr3.Pr3lotemul,
                    pr3.Pr3preco,
                    pr3.Pr3pesoliq
                });

                return affectedRows;
            }
        }

        public async Task<int> Delete(string pr3produto, string pr3tamanho)
        {
            string command = @"DELETE FROM PR3 WHERE PR3PRODUTO = @pr3produto AND PR3TAMANHO = @pr3tamanho";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { pr3produto, pr3tamanho });
            }
        }

        public async Task<int> Update(PR3 pr3)
        {
            string command = @"UPDATE PR3
                               SET PR3LOTEMUL = @Pr3lotemul, PR3PRECO = @Pr3preco, PR3PESOLIQ = @Pr3pesoliq
                               WHERE PR3PRODUTO = @pr3produto AND PR3TAMANHO = @pr3tamanho";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    pr3.Pr3produto,
                    pr3.Pr3tamanho,
                    pr3.Pr3lotemul,
                    pr3.Pr3preco,
                    pr3.Pr3pesoliq
                });

                return affectedRows;
            }
        }

        public async Task<IList<PR3>> Search(string search, int limit, int offset)
        {
            search = string.Format("%{0}%", search);
            search = search.Replace(" ", "%");
            string command = @"SELECT PR2.*, GR1.GR1DESC
                               FROM PR3
                               INNER JOIN GR1 ON GR1TAMANHO = PR3TAMANHO
                               WHERE PR3PRODUTO LIKE @search
                                OR PR3TAMANHO LIKE @search
                                OR GR1DESC LIKE @search
                               LIMIT @limit OFFSET @offset";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<PR3> pr3lst = await connection.QueryAsync<PR3>(command, new { search, limit, offset });

                return pr3lst.ToList();
            }
        }
    }
}
