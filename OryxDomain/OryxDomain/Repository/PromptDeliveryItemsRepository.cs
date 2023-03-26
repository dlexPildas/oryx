using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class PromptDeliveryItemsRepository : Repository
    {
        public PromptDeliveryItemsRepository(string path) : base(path)
        {
        }

        public async Task<PLE> Find(string pleproduto, string pleopcao, string pletamanho)
        {
            string command = @"SELECT * FROM PLE WHERE PLEPRODUTO = @pleproduto AND PLEOPCAO = @pleopcao AND PLETAMANHO = @pletamanho";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                PLE ple = await connection.QueryFirstOrDefaultAsync<PLE>(command, new { pleproduto, pleopcao, pletamanho });

                return ple;
            }
        }
    }
}
