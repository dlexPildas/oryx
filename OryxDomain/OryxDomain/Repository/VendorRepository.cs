using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class VendorRepository : Repository
    {
        public VendorRepository(string path) : base(path)
        {
        }

        public async Task<IList<VE0>> FindList()
        {
            string command = @"SELECT * FROM VE0";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<VE0> lstVe0 = await connection.QueryAsync<VE0>(command);

                return lstVe0.ToList();
            }
        }

        public async Task<VE0> Find(string ve0vend)
        {
            string command = @"SELECT VE0.*, CF1NOME FROM VE0 LEFT JOIN CF1 ON CF1CLIENTE = VE0CLIENTE WHERE VE0VEND = @ve0vend";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                VE0 ve0 = await connection.QueryFirstOrDefaultAsync<VE0>(command, new { ve0vend });

                return ve0;
            }
        }

        public async Task<int> Insert(VE0 ve0)
        {
            string command = @"INSERT INTO VE0 (VE0VEND,VE0CLIENTE,VE0ATIVO,VE0COMIS)
                               VALUES (@ve0vend,@ve0cliente,@ve0ativo,@ve0comis)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    ve0.Ve0vend,
                    ve0.Ve0cliente,
                    ve0.Ve0ativo,
                    ve0.Ve0comis
                });

                return affectedRows;
            }
        }

        public async Task<int> Delete(string ve0vend)
        {
            string command = @"DELETE FROM VE0 WHERE VE0VEND = @ve0vend";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { ve0vend });
            }
        }

        public async Task<int> Update(VE0 ve0)
        {
            string command = @"UPDATE VE0
                               SET VE0CLIENTE = @ve0cliente, VE0ATIVO = @ve0ativo, VE0COMIS = @ve0comis
                               WHERE VE0VEND = @ve0vend";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    ve0.Ve0vend,
                    ve0.Ve0cliente,
                    ve0.Ve0ativo,
                    ve0.Ve0comis
                });

                return affectedRows;
            }
        }

        public async Task<IList<VE0>> Search(string search, int limit, int offset)
        {
            search = string.Format(" %{0}%", search);
            search = search.Replace(" ", "%");
            string command = @"SELECT *
                               FROM VE0
                              INNER JOIN CF1 ON CF1CLIENTE = VE0CLIENTE
                              WHERE VE0VEND LIKE @search
                                 OR VE0CLIENTE LIKE @search
                                 OR CF1NOME LIKE @search
                              LIMIT @limit OFFSET @offset";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<VE0> terminals = await connection.QueryAsync<VE0>(command, new { search, limit, offset });

                return terminals.ToList();
            }
        }
    }
}
