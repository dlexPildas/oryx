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
    public class ReportsRepository : Repository
    {
        public ReportsRepository(string path) : base(path)
        {
        }

        public async Task<DC9> Find(string dc9relat)
        {
            string command = @"SELECT * FROM DC9 WHERE DC9RELAT = @dc9relat";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                DC9 dc9 = await connection.QueryFirstOrDefaultAsync<DC9>(command, new { dc9relat });

                return dc9;
            }
        }

        public async Task<IList<DC9>> FindAllDc9(string dc5usuario)
        {
            string command = @"SELECT DC9.* FROM DC9 WHERE DC9ARQUIVO = '' OR DC9ARQUIVO IN (SELECT DC5ARQUIVO FROM DC5 WHERE DC5USUARIO = @dc5usuario)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<DC9> lstDc9 = await connection.QueryAsync<DC9>(command, new { dc5usuario });

                return lstDc9.ToList();
            }
        }
    }
}
