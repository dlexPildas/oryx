using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class MoldRepository : Repository
    {
        public MoldRepository(string path) : base(path)
        {
        }
    
        public async Task<GR3> Find(string gr3molde)
        {
            string command = @"SELECT * FROM GR3
                                WHERE GR3MOLDE = @gr3molde";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                GR3 gr3 = await connection.QueryFirstOrDefaultAsync<GR3>(command, new { gr3molde });

                return gr3;
            }
        }
    }
}
