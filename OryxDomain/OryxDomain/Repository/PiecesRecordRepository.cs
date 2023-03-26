using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class PiecesRecordRepository : Repository
    {
        public PiecesRecordRepository(string path) : base(path)
        {
        }

        public async Task<OF3> Find(string of3peca)
        {
            string command = @"SELECT *
                               FROM OF3
                               WHERE OF3PECA = @of3peca";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                OF3 piece = await connection.QueryFirstOrDefaultAsync<OF3>(command, new { of3peca });

                return piece;
            }
        }

        public async Task<OF3> FindByRfidOrOf3peca(string peca)
        {
            string command = @"SELECT *
                                FROM OF3
                               WHERE OF3RFID = @peca OR OF3PECA = @peca";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                OF3 piece = await connection.QueryFirstOrDefaultAsync<OF3>(command, new { peca });

                return piece;
            }
        }

        public async Task<OF3> FindByOf3codext(string code)
        {
            string command = @"SELECT *
                               FROM OF3
                               WHERE OF3CODEXT = @code";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                OF3 piece = await connection.QueryFirstOrDefaultAsync<OF3>(command, new { code });

                return piece;
            }
        }
    }
}
