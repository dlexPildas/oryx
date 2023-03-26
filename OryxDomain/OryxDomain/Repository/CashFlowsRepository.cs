using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class CashFlowsRepository : Repository
    {
        public CashFlowsRepository(string path) : base(path)
        {
        }

        public async Task<IList<CX1>> FindByCashier(string cx1caixa)
        {
            string command = @"SELECT * FROM CX1 WHERE CX1CAIXA = @cx1caixa";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<CX1> lstCx1 = await connection.QueryAsync<CX1>(command, new { cx1caixa });

                return lstCx1.ToList();
            }
        }

        public async Task<CX1> Find(string cx1registr)
        {
            string command = @"SELECT * FROM CX1 WHERE CX1REGISTR = @cx1registr";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CX1 cx1 = await connection.QueryFirstOrDefaultAsync<CX1>(command, new { cx1registr });

                return cx1;
            }
        }

        public async Task<int> Insert(CX1 cx1)
        {
            string command = @"INSERT INTO CX1 (CX1REGISTR,CX1CAIXA,CX1TIPO,CX1VALOR,CX1DATA,CX1ENTSAI)
                               VALUES (@cx1registr,@cx1caixa,@cx1tipo,@cx1valor,@cx1data,@cx1entsai)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    cx1registr = cx1.Cx1registr,
                    cx1caixa = cx1.Cx1caixa,
                    cx1tipo = cx1.Cx1tipo,
                    cx1valor = cx1.Cx1valor,
                    cx1data = cx1.Cx1data,
                    cx1entsai = cx1.Cx1entsai
                });

                return affectedRows;
            }
        }

        public async Task<int> Delete(string cx1registr)
        {
            string command = @"DELETE FROM CX1 WHERE CX1REGISTR = @cx1registr";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { cx1registr });
            }
        }

        public async Task<int> DeleteByCashier(string cx1caixa)
        {
            string command = @"DELETE FROM CX1 WHERE CX1CAIXA = @cx1caixa";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { cx1caixa });
            }
        }

        public async Task<int> Update(CX1 cx1)
        {
            string command = @"UPDATE CX1
                               SET CX1CAIXA = @cx1caixa, CX1TIPO = @cx1tipo, CX1VALOR = @cx1valor, CX1DATA = @cx1data, CX1ENTSAI = @cx1entsai
                               WHERE CX1REGISTR = @cx1registr";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    cx1registr = cx1.Cx1registr,
                    cx1caixa = cx1.Cx1caixa,
                    cx1tipo = cx1.Cx1tipo,
                    cx1valor = cx1.Cx1valor,
                    cx1data = cx1.Cx1data,
                    cx1entsai = cx1.Cx1entsai,
                });

                return affectedRows;
            }
        }
    }
}
