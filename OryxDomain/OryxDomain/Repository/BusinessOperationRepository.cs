using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class BusinessOperationRepository : Repository
    {
        public BusinessOperationRepository(string path) : base(path)
        {
        }

        public async Task<CV3> Find(string cv3opercom)
        {
            string command = @"SELECT CV3.*
                                 FROM CV3
                                WHERE CV3OPERCOM = @cv3opercom";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CV3 cv3 = await connection.QueryFirstOrDefaultAsync<CV3>(command, new { cv3opercom });

                return cv3;
            }
        }
    }
}
