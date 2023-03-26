using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class ReleasedPiecesRepository : Repository
    {
        public ReleasedPiecesRepository(string path) : base(path)
        {
        }

        public async Task<OF0> Find(string of0peca)
        {
            string command = @"SELECT *
                                FROM OF0
                               WHERE OF0PECA = @of0peca";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                OF0 of0 = await connection.QueryFirstOrDefaultAsync<OF0>(command, new { of0peca });

                return of0;
            }
        }
    }
}
