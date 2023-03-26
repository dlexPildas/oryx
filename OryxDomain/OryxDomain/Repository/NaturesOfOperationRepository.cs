using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class NaturesOfOperationRepository : Repository
    {
        public NaturesOfOperationRepository(string path) : base(path)
        {
        }

        public async Task<FI1> Find(string cfop)
        {
            string command = @"SELECT * FROM FI1 WHERE FI1CFOP = @cfop";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                FI1 fi1 = await connection.QueryFirstOrDefaultAsync<FI1>(command, new { cfop });

                return fi1;
            }
        }
    }
}
