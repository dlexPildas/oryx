using Dapper;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class ShippingAddressRepository : Repository
    {
        public ShippingAddressRepository(string path) : base(path)
        {
        }

        public async Task<int> Insert(string cf1cliente, CFB shippingAddress, bool isForUpdate)
        {
            if (isForUpdate)
            {
                //deletar CFB atual
                await Delete(cf1cliente);
            }
            //inserir novo CFB
            string command = @"INSERT INTO CFB (CFBCLIENTE, CFBENDENT, CFBPROXIMO, CFBCEPENT) 
                                VALUES (@cfbcliente, @cfbendent, @cfbproximo, @cfbcepent)";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    var affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        cfbcliente = cf1cliente,
                        cfbendent = shippingAddress.Cfbendent,
                        cfbproximo = shippingAddress.Cfbproximo,
                        cfbcepent = shippingAddress.Cfbcepent
                    });

                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Delete(string cf1cliente)
        {
            string command = @"DELETE FROM CFB where CFBCLIENTE = @cf1cliente";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    await connection.ExecuteAsync(command, new { cf1cliente });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CFB> Find(string cf1cliente)
        {
            string command = @"SELECT CFBCLIENTE, CFBENDENT, CFBPROXIMO, CFBCEPENT, CF3NOME, CF3ESTADO, CF2LOGRA 
                               FROM CFB 
                               INNER JOIN CF2 ON CF2CEP = Cfbcepent
                               INNER JOIN CF3 ON CF3LOCAL = CF2LOCAL
                               WHERE CFBCLIENTE = @cf1cliente";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    CFB cfb = await connection.QueryFirstOrDefaultAsync<CFB>(command, new { cf1cliente });
                    return cfb;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
