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
    public class CashierRepository : Repository
    {
        public CashierRepository(string path) : base(path)
        {
        }

        public async Task<IList<CX0>> FindList()
        {
            string command = @"SELECT * FROM CX0";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<CX0> lstCx0 = await connection.QueryAsync<CX0>(command);

                return lstCx0.ToList();
            }
        }

        public async Task<CX0> Find(string cx0caixa)
        {
            string command = @"SELECT * FROM CX0 WHERE CX0CAIXA = @cx0caixa";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CX0 cx0 = await connection.QueryFirstOrDefaultAsync<CX0>(command, new { cx0caixa });

                return cx0;
            }
        }

        public async Task<CX0> FindLastByTerminal(string terminal)
        {
            string command = @"SELECT * FROM CX0 WHERE CX0CODIGO = @terminal ORDER BY CX0ABERT DESC";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CX0 cx0 = await connection.QueryFirstOrDefaultAsync<CX0>(command, new { terminal });

                return cx0;
            }
        }

        public async Task<int> Insert(CX0 cx0)
        {
            string command = @"INSERT INTO CX0 (CX0CAIXA,CX0ABERT,CX0FECHA,CX0USUABE,CX0USUFEC,CX0VALINI,CX0CODIGO )
                               VALUES (@cx0caixa,@cx0abert,@cx0fecha,@cx0usuabe,@cx0usufec,@cx0valini,@cx0codigo)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    cx0caixa = cx0.Cx0caixa,
                    cx0abert = cx0.Cx0abert,
                    cx0fecha = cx0.Cx0fecha,
                    cx0usuabe = cx0.Cx0usuabe,
                    cx0usufec = cx0.Cx0usufec,
                    cx0valini = cx0.Cx0valini,
                    cx0codigo = cx0.Cx0codigo,
                });

                return affectedRows;
            }
        }

        public async Task<int> Delete(string cx0caixa)
        {
            string command = @"DELETE FROM CX0 WHERE CX0CAIXA = @cx0caixa";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { cx0caixa });
            }
        }

        public async Task<int> Update(CX0 cx0)
        {
            string command = @"UPDATE CX0
                               SET CX0ABERT = @cx0abert, CX0FECHA = @cx0fecha, CX0USUABE = @cx0usuabe, CX0USUFEC = @cx0usufec, CX0VALINI = @cx0valini, CX0CODIGO = @cx0codigo
                               WHERE CX0CAIXA = @cx0caixa";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    cx0caixa = cx0.Cx0caixa,
                    cx0abert = cx0.Cx0abert,
                    cx0fecha = cx0.Cx0fecha,
                    cx0usuabe = cx0.Cx0usuabe,
                    cx0usufec = cx0.Cx0usufec,
                    cx0valini = cx0.Cx0valini,
                    cx0codigo = cx0.Cx0codigo,
                });

                return affectedRows;
            }
        }

        public async Task<IList<CX0>> Search(string search, int limit, int offset, DateTime since, DateTime until, string orderBy)
        {
            search = string.Format("%{0}%", search);
            search = search.Replace(" ", "%");
            string command = @"SELECT *
                               FROM CX0
                               WHERE (CX0CAIXA LIKE @search
                                  OR CX0CODIGO LIKE @search)
                                 AND CX0ABERT BETWEEN @since AND @until
                               ORDER BY CX0ABERT " + orderBy + " LIMIT @limit OFFSET @offset";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<CX0> cashiers = await connection.QueryAsync<CX0>(command, new { search, limit, offset, since, until });

                return cashiers.ToList();
            }
        }
    }
}
