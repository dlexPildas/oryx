using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class GoalTypesRepository : Repository
    {
        public GoalTypesRepository(string path) : base(path)
        {
        }

        public async Task<IList<VE1>> FindList()
        {
            string command = @"SELECT * FROM VE1";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<VE1> lstVe1 = await connection.QueryAsync<VE1>(command);

                return lstVe1.ToList();
            }
        }

        public async Task<VE1> Find(string ve1tipo)
        {
            string command = @"SELECT * FROM VE1 WHERE VE1TIPO = @ve1tipo";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                VE1 ve1 = await connection.QueryFirstOrDefaultAsync<VE1>(command, new { ve1tipo });

                return ve1;
            }
        }

        public async Task<int> Insert(VE1 ve1)
        {
            string command = @"INSERT INTO VE1 (VE1TIPO,VE1NOME)
                               VALUES (@ve1tipo,@ve1nome)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    ve1.Ve1tipo,
                    ve1.Ve1nome,
                });

                return affectedRows;
            }
        }

        public async Task<int> Delete(string ve1tipo)
        {
            string command = @"DELETE FROM VE1 WHERE VE1TIPO = @ve1tipo";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { ve1tipo });
            }
        }

        public async Task<int> Update(VE1 ve1)
        {
            string command = @"UPDATE VE1
                               SET VE1NOME = @ve1nome
                               WHERE VE1TIPO = @ve1tipo";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    ve1.Ve1tipo,
                    ve1.Ve1nome,
                });

                return affectedRows;
            }
        }

        public async Task<IList<VE1>> Search(string search, int limit, int offset)
        {
            search = string.Format("%{0}%", search);
            search = search.Replace(" ", "%");
            string command = @"SELECT *
                               FROM VE1
                              WHERE VE1TIPO LIKE @search
                                 OR VE1NOME LIKE @search
                              LIMIT @limit OFFSET @offset";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<VE1> terminals = await connection.QueryAsync<VE1>(command, new { search, limit, offset });

                return terminals.ToList();
            }
        }
    }
}
