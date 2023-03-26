using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class AddressRepository : Repository
    {
        public AddressRepository(string path) : base(path)
        {
        }

        public async Task<CF2> FindCf2ByCep(string cep)
        {
            string command = @"SELECT Cf2local, Cf2cep, Cf2logra
                            FROM CF2
                            WHERE Cf2cep = @cep";
            
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CF2 cf2 = await connection.QueryFirstOrDefaultAsync<CF2>(command, new { cep });

                return cf2;
            }
        }

        public async Task<CF2> FindCf2ByIbge(string ibge)
        {
            string command = @"SELECT Cf2local, Cf2cep, Cf2logra
                            FROM CF2
                            WHERE Cf2local = @ibge";
            
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CF2 cf2 = await connection.QueryFirstOrDefaultAsync<CF2>(command, new { ibge });

                return cf2;
            }
        }

        public async Task<CF3> FindCf3ByIbge(string ibge)
        {
            string command = @"SELECT Cf3local, Cf3nome, Cf3estado, Cf3regiao
                            FROM CF3
                            WHERE Cf3local = @ibge";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CF3 cf3 = await connection.QueryFirstOrDefaultAsync<CF3>(command, new { ibge });

                return cf3;
            }
        }

        public async Task<int> InsertCf2(CF2 cf2)
        {
            string command = @"INSERT INTO 
                               cf2 (cf2cep, cf2local, cf2logra)
                               values (@cf2cep, @cf2local, @cf2logra)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    cf2cep = cf2.Cf2cep,
                    cf2local = cf2.Cf2local,
                    cf2logra = cf2.Cf2logra
                });

                return affectedRows;
            }
        }

        public async Task<int> InsertCf3(CF3 cf3)
        {
            string command = @"INSERT INTO 
                               cf3 (cf3local, cf3nome, cf3estado)
                               values (@cf3local, @cf3nome, @cf3estado)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    cf3local = cf3.Cf3local,
                    cf3nome = cf3.Cf3nome,
                    cf3estado = cf3.Cf3estado
                });

                return affectedRows;
            }
        }
        public async Task<IList<string>> FindCustomerZipCodes()
        {
            string command = @"SELECT DISTINCT CF1CEP FROM CF1 WHERE CF1CEP <> '' AND CF1CEP NOT IN (SELECT CF2CEP FROM CF2)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<string> lstZipCodes = await connection.QueryAsync<string>(command);

                return lstZipCodes.ToList();
            }
        }

    }
}
