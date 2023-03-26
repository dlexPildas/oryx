using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Utilities;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class ImplantedPiecesRepository : Repository
    {
        public ImplantedPiecesRepository(string path) : base(path)
        {
        }

        public async Task<string> FindOf7Doc(string of3peca)
        {
            string command = @"SELECT OF7DOC
                               FROM OF7
                               WHERE OF7PECA = @of3peca";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                string of7doc = await connection.QueryFirstOrDefaultAsync<string>(command, new { of3peca });

                return of7doc;
            }
        }
    }
}
