using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class NonConformingPiecesRepository : Repository
    {
        public NonConformingPiecesRepository(string path) : base(path)
        {
        }

        public async Task<OFH> Find(string ofhpeca)
        {
            string command = @"SELECT *
                               FROM OFH
                               WHERE OFHPECA = @ofhpeca";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                OFH ofh = await connection.QueryFirstOrDefaultAsync<OFH>(command, new { ofhpeca });

                return ofh;
            }
        }
    }
}
