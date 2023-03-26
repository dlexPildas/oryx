using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class FinancialParametersRepository : Repository
    {
        public FinancialParametersRepository(string path) : base(path)
        {
        }

        public async Task<LXA> Find()
        {
            string command = @"SELECT * from LXA";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                LXA lxa = await connection.QueryFirstOrDefaultAsync<LXA>(command);

                return lxa;
            }
        }

        public async Task<int> Insert(LXA lxa)
        {
            string command = @"INSERT INTO LXA (LXACAIXA,LXADIAUTIL,LXACAPITAL,LXAARREDON,LXAMORA,LXANAOCONS,LXAMULTA,LXACONTA,LXACUSTOFI)
                               VALUES (@lxacaixa,@lxadiautil,@lxacapital,@lxaarredon,@lxamora,@lxanaocons,@lxamulta,@lxaconta,@lxacustofi)";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    var affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        lxacaixa = lxa.Lxacaixa,
                        lxadiautil = lxa.Lxadiautil,
                        lxacapital = lxa.Lxacapital,
                        lxaarredon = lxa.Lxaarredon,
                        lxamora = lxa.Lxamora,
                        lxanaocons = lxa.Lxanaocons,
                        lxamulta = lxa.Lxamulta,
                        lxaconta = lxa.Lxaconta,
                        lxacustofi = lxa.Lxacustofi
                    });

                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> Update(LXA lxa)
        {
            string command = @"UPDATE LXA
                                  SET LXACAIXA = @lxacaixa,LXADIAUTIL = @lxadiautil,LXACAPITAL = @lxacapital,LXAARREDON = @lxaarredon,LXAMORA = @lxamora,LXANAOCONS = @lxanaocons,LXAMULTA = @lxamulta,LXACONTA = @lxaconta,LXACUSTOF = @lxacustofi";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    var affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        lxacaixa = lxa.Lxacaixa,
                        lxadiautil = lxa.Lxadiautil,
                        lxacapital = lxa.Lxacapital,
                        lxaarredon = lxa.Lxaarredon,
                        lxamora = lxa.Lxamora,
                        lxanaocons = lxa.Lxanaocons,
                        lxamulta = lxa.Lxamulta,
                        lxaconta = lxa.Lxaconta,
                        lxacustofi = lxa.Lxacustofi
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
            string command = @"DELETE FROM LXA";

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
    }
}
