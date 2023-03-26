using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class TransporterRepository : Repository
    {
        public TransporterRepository(string path) : base(path)
        {
        }

        public async Task<IList<CF7>> FindList()
        {
            string command = @"SELECT CF7.* , CF1NOME from CF7
                               LEFT JOIN CF1 ON Cf7cliente = Cf1cliente";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<CF7> lstCf7 = await connection.QueryAsync<CF7>(command);

                return lstCf7.ToList();
            }
        }

        public async Task<CF7> Find(string cf7transp)
        {
            string command = @"SELECT CF7.* , CF1NOME from CF7
                               LEFT JOIN CF1 ON Cf7cliente = Cf1cliente
                               WHERE CF7TRANSP = @cf7transp";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CF7 cf7 = await connection.QueryFirstOrDefaultAsync<CF7>(command, new { cf7transp });

                return cf7;
            }
        }

        public async Task<int> Insert(CF7 cf7)
        {
            string command = @"INSERT INTO CF7 (CF7TRANSP,CF7NOME,CF7CLIENTE,CF7PROPRIO,CF7CODEXT)
                                        VALUES (@Cf7transp,@Cf7nome,@Cf7cliente,@Cf7proprio,@cf7codext)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    cf7.Cf7transp,
                    cf7.Cf7nome,
                    cf7.Cf7cliente,
                    cf7.Cf7proprio,
                    cf7.Cf7codext
                });

                return affectedRows;
            }
        }

        public async Task<int> Delete(string cf7transp)
        {
            string command = @"DELETE FROM CF7 WHERE CF7TRANSP = @cf7transp";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { cf7transp });
            }
        }

        public async Task<int> Update(CF7 cf7)
        {
            string command = @"UPDATE CF7
                               SET CF7NOME = @Cf7nome, CF7CLIENTE = @Cf7cliente, CF7PROPRIO = @Cf7proprio, CF7CODEXT = @cf7codext
                               WHERE CF7TRANSP = @Cf7transp";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    cf7.Cf7transp,
                    cf7.Cf7nome,
                    cf7.Cf7cliente,
                    cf7.Cf7proprio,
                    cf7.Cf7codext
                });

                return affectedRows;
            }
        }

        public async Task<IList<CF7>> Search(string search, int limit, int offset)
        {
            search = string.Format("%{0}%", search);
            search = search.Replace(" ", "%");
            string command = @"SELECT CF7.* , CF1NOME from CF7
                               LEFT JOIN CF1 ON Cf7cliente = Cf1cliente
                               WHERE CF7TRANSP LIKE @search
                                  OR CF7NOME LIKE @search
                                  OR CF1NOME LIKE @search
                               LIMIT @limit OFFSET @offset";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<CF7> carries = await connection.QueryAsync<CF7>(command, new { search, limit, offset });

                return carries.ToList();
            }
        }
    }
}
