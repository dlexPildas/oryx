using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class ChequeParametersRepository : Repository
    {
        public ChequeParametersRepository(string path) : base(path)
        {
        }
        public async Task<int> Update(PD6 pd6)
        {
            string command = @"UPDATE PD6 SET PD6AGENTEP = @Pd6agentep, PD6CRUZAR = @Pd6cruzar, PD6EMISSOR = @Pd6emissor, PD6IMPRES = @Pd6impres, PD6NOMINAL = @Pd6nominal, PD6ROTACAO = @Pd6rotacao";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    var affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        pd6.Pd6agentep,
                        pd6.Pd6cruzar,
                        pd6.Pd6emissor,
                        pd6.Pd6impres,
                        pd6.Pd6nominal,
                        pd6.Pd6rotacao
                    });

                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> Delete()
        {
            string command = @"DELETE FROM PD6";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    return await connection.ExecuteAsync(command);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> Insert(PD6 pd6)
        {
            string command = @"INSERT INTO PD6 (PD6EMISSOR,PD6AGENTEP,PD6CRUZAR,PD6IMPRES,PD6NOMINAL,PD6ROTACAO) VALUES(@Pd6emissor,@Pd6agentep,@Pd6cruzar,@Pd6impres,@Pd6nominal,@Pd6rotacao)";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    var affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        pd6.Pd6agentep,
                        pd6.Pd6cruzar,
                        pd6.Pd6emissor,
                        pd6.Pd6impres,
                        pd6.Pd6nominal,
                        pd6.Pd6rotacao
                    });

                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<PD6> Find()
        {
            string command = @"SELECT  PD6.* FROM PD6 ";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                PD6 PD6 = await connection.QueryFirstOrDefaultAsync<PD6>(command);

                return PD6;
            }
        }
    }
}


