using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class GoalsRepository : Repository
    {
        public GoalsRepository(string path) : base(path)
        {
        }

        public async Task<IList<VE2>> FindList()
        {
            string command = @"SELECT * FROM VE2";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<VE2> lstVe2 = await connection.QueryAsync<VE2>(command);

                return lstVe2.ToList();
            }
        }

        public async Task<VE2> Find(string ve2meta)
        {
            string command = @"SELECT VE2.*, VE1NOME FROM VE2 INNER JOIN VE1 ON VE1TIPO = VE2TIPO WHERE VE2META = @ve2meta";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                VE2 ve2 = await connection.QueryFirstOrDefaultAsync<VE2>(command, new { ve2meta });

                return ve2;
            }
        }

        public async Task<int> Insert(VE2 ve2)
        {
            string command = @"INSERT INTO VE2 (VE2META,VE2MES,VE2VALOR,VE2TIPO)
                               VALUES (@ve2meta, @ve2mes, @ve2valor, @ve2tipo)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    ve2.Ve2meta,
                    ve2.Ve2mes,
                    ve2.Ve2valor,
                    ve2.Ve2tipo,
                });

                return affectedRows;
            }
        }

        public async Task<int> Delete(string ve2meta)
        {
            string command = @"DELETE FROM VE2 WHERE VE2META = @ve2meta";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { ve2meta });
            }
        }

        public async Task<int> Update(VE2 ve2)
        {
            string command = @"UPDATE VE2
                               SET VE2MES = @ve2mes,VE2VALOR = @ve2valor,VE2TIPO = @ve2tipo
                               WHERE VE2META = @ve2meta";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    ve2.Ve2meta,
                    ve2.Ve2mes,
                    ve2.Ve2valor,
                    ve2.Ve2tipo,
                });

                return affectedRows;
            }
        }

        public async Task<IList<VE2>> Search(string search, int limit, int offset, DateTime since, DateTime until)
        {
            search = string.Format("%{0}%", search);
            search = search.Replace(" ", "%");
            string command = @"SELECT VE2.*, VE1.VE1NOME
                               FROM VE2
                              INNER JOIN VE1 ON VE1TIPO = VE2TIPO
                              WHERE VE2META LIKE @search
                                 OR VE1NOME LIKE @search
                                AND VE2MES BETWEEN @since AND @until
                              LIMIT @limit OFFSET @offset";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<VE2> terminals = await connection.QueryAsync<VE2>(command, new { search, limit, offset, since, until });

                return terminals.ToList();
            }
        }

        public async Task<VE2> Find(string ve2mes, string ve2tipo)
        {
            string command = @"SELECT VE2.*, VE1NOME FROM VE2 INNER JOIN VE1 ON VE1TIPO = VE2TIPO WHERE VE2MES = @ve2mes AND VE2TIPO = @ve2tipo";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                VE2 ve2 = await connection.QueryFirstOrDefaultAsync<VE2>(command, new { ve2mes, ve2tipo });

                return ve2;
            }
        }
    }
}
