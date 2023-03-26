using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class BillingAddressRepository : Repository
    {
        public BillingAddressRepository(string path) : base(path)
        {
        }
        public async Task<int> Insert(string cf1cliente, CF0 BillingAddress, bool isForUpdate)
        {
            if (isForUpdate)
            {
                //deletar CFB atual
                await Delete(cf1cliente);
            }
            //inserir novo CFB
            string command = @"INSERT INTO CF0 (CF0CLIENTE, CF0ENDCOB, CF0CEPCOB) 
                                VALUES (@cf0cliente, @cf0endcob, @cf0cepcob)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    cf0cliente = cf1cliente,
                    cf0endcob = BillingAddress.Cf0endcob,
                    cf0cepcob = BillingAddress.Cf0cepcob,
                });

                return affectedRows;
            }
        }

        public async Task Delete(string cf1cliente)
        {
            string command = @"DELETE FROM CF0 WHERE CF0CLIENTE = @cf1cliente";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                await connection.ExecuteAsync(command, new { cf1cliente });
            }
        }

        public async Task<CF0> Find(string cf1cliente)
        {
            string command = @"SELECT CF0.*, CF3NOME, CF3ESTADO, CF2LOGRA 
                               FROM CF0
                               INNER JOIN CF2 ON CF2CEP = CF0CEPCOB
                               INNER JOIN CF3 ON CF3LOCAL = CF2LOCAL
                               WHERE CF0CLIENTE = @cf1cliente";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CF0 cf0 = await connection.QueryFirstOrDefaultAsync<CF0>(command, new { cf1cliente });
                return cf0;
            }
        }
    }
}
