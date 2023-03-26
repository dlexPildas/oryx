using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class PurchaseOrderRepository : Repository
    {
        public PurchaseOrderRepository(string path) : base(path)
        {
        }

        public async Task<PC1> Find(string pc1pedcom)
        {
            string command = @"SELECT * FROM PC1 WHERE PC1PEDCOM = @pc1pedcom";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                PC1 pc1 = await connection.QueryFirstOrDefaultAsync<PC1>(command, new { pc1pedcom });

                return pc1;
            }
        }
    }
}
