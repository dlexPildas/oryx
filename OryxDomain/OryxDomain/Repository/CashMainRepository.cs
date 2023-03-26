using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class CashMainRepository : Repository
    {
        public CashMainRepository(string path) : base(path)
        {
        }

        public async Task<int> Update(PD3 pd3)
        {
            string command = @"UPDATE PD3 SET PD3VIEW1 = @pd3view1, PD3VIEW2 = @pd3view2, PD3VIEW3 = @pd3view3, PD3VIEW4 = @pd3view4";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    pd3.Pd3view1,
                    pd3.Pd3view2,
                    pd3.Pd3view3,
                    pd3.Pd3view4
                });

                return affectedRows;
            }
        }

        public async Task<int> Delete()
        {
            string command = @"DELETE FROM PD3";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command);
            }
        }

        public async Task<int> Insert(PD3 pd3)
        {
            string command = @"INSERT INTO PD3 (PD3VIEW1,PD3VIEW2,PD3VIEW3,PD3VIEW4)
                               VALUES (@pd3view1,@pd3view2,@pd3view3,@pd3view4)";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    var affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        pd3.Pd3view1,
                        pd3.Pd3view2,
                        pd3.Pd3view3,
                        pd3.Pd3view4,

                    });

                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<PD3> Find()
        {
            string command = @"SELECT * FROM PD3";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                PD3 pd3 = await connection.QueryFirstOrDefaultAsync<PD3>(command);

                return pd3;
            }
        }
    }
}
