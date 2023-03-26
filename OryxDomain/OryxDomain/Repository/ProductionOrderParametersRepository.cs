using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class ProductionOrderParametersRepository : Repository
    {
        public ProductionOrderParametersRepository(string path) : base(path)
        {
        }

        public async Task<LX1> Find()
        {
            string command = @"SELECT * FROM LX1";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                LX1 lx1 = await connection.QueryFirstOrDefaultAsync<LX1>(command);
                return lx1;
            }
        }
    }
}
