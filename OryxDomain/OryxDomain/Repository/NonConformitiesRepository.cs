using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Utilities;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class NonConformitiesRepository : Repository
    {
        public NonConformitiesRepository(string path) : base(path)
        {
        }

        public async Task<string> FindOfgNome(string ofhmotivo)
        {
            string command = @"SELECT OFGNOME
                               FROM OFG
                               WHERE OFGMOTIVO = @ofhmotivo";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                string ofh = await connection.QueryFirstOrDefaultAsync<string>(command, new { ofhmotivo });

                return ofh;
            }
        }
    }
}
