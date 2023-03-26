using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class AppliedMaterialsRepository : Repository
    {
        public AppliedMaterialsRepository(string path) : base(path)
        {
        }
    
        public async Task<List<PR8>> FindList(string pr8produto, string pr8tamanho)
        {
            string command = @"SELECT * FROM PR8
                                WHERE PR8PRODUTO = @pr8produto
                                AND PR8TAMANHO = @pr8tamanho";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<PR8> pr8lst = await connection.QueryAsync<PR8>(command, new { pr8produto, pr8tamanho });

                return pr8lst.ToList();
            }
        }

        public async Task<int> Delete(string pr8produto, string pr8tamanho)
        {
            string command = @"DELETE FROM PR8
                                WHERE PR8PRODUTO = @pr8produto
                                AND PR8TAMANHO = @pr8tamanho";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { pr8produto, pr8tamanho });
            }
        }
    }
}
