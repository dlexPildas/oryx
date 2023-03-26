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
    public class CommissionClosingRepository : Repository
    {
        public CommissionClosingRepository(string path) : base(path)
        {
        }

        public async Task<IList<CO1>> FindList()
        {
            string command = @"SELECT * FROM CO1";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<CO1> lstCo1 = await connection.QueryAsync<CO1>(command);

                return lstCo1.ToList();
            }
        }

        public async Task<CO1> Find(string co1fecha)
        {
            string command = @"SELECT * FROM CO1 WHERE CO1FECHA = @co1fecha";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CO1 co1 = await connection.QueryFirstOrDefaultAsync<CO1>(command, new { co1fecha });

                return co1;
            }
        }

        public async Task<int> Insert(CO1 co1)
        {
            string command = @"INSERT INTO CO1 (CO1FECHA,CO1INICIO,CO1FIM,CO1USUARIO,CO1REPRES,CO1ABERT)
                               VALUES (@co1fecha,@co1inicio,@co1fim,@co1usuario,@co1repres,@co1abert)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    co1.Co1fecha,
                    co1.Co1inicio,
                    co1.Co1fim,
                    co1.Co1usuario,
                    co1.Co1repres,
                    co1.Co1abert
                });

                return affectedRows;
            }
        }

        public async Task<int> Delete(string co1fecha)
        {
            string command = @"DELETE FROM CO1 WHERE CO1FECHA = @co1fecha";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { co1fecha });
            }
        }

        public async Task<int> Update(CO1 co1)
        {
            string command = @"UPDATE CO1
                               SET CO1INICIO = @co1inicio, CO1FIM = @co1fim, CO1USUARIO = @co1usuario, CO1REPRES = @co1repres, CO1ABERT = @co1abert
                               WHERE CO1FECHA = @co1fecha";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    co1.Co1fecha,
                    co1.Co1inicio,
                    co1.Co1fim,
                    co1.Co1usuario,
                    co1.Co1repres
                });

                return affectedRows;
            }
        }

        public async Task<IList<CO1>> Search(string search, int limit, int offset, DateTime since, DateTime until, string orderBy)
        {
            search = string.Format("%{0}%", search);
            search = search.Replace(" ", "%");
            string command = @"SELECT *, CF6NOME
                               FROM CO1
                              INNER JOIN CF6 ON CF6REPRES = CO1REPRES
                              WHERE (CO1FECHA LIKE @search
                                 OR CO1REPRES LIKE @search
                                 OR CF6NOME LIKE @search)
                                AND CO1FIM BETWEEN @since AND @until
                              ORDER BY CO1FECHA " + orderBy + " LIMIT @limit OFFSET @offset";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<CO1> lstCo1 = await connection.QueryAsync<CO1>(command, new { search, limit, offset, since, until });

                return lstCo1.ToList();
            }
        }
    }
}
