using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class BankAccountsRepository : Repository
    {
        public BankAccountsRepository(string path) : base(path)
        {
        }

        public async Task<IList<string>> FindAgentForBankSlip()
        {
            string statement = "SELECT DISTINCT FN3AGENTE FROM FN3 WHERE TRIM(FN3FIMBLQ) <> '' AND  (FN3AGENTE='0041' OR FN3AGENTE='0085' OR FN3AGENTE='0237' OR FN3AGENTE='0341'  OR FN3AGENTE='0422'  OR FN3AGENTE='0748'   OR FN3AGENTE='0756'   OR FN3AGENTE='0033' OR FN3AGENTE='0001' OR FN3AGENTE='0999' OR FN3AGENTE='0136' ) ORDER BY FN3PRINCIP DESC";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                IEnumerable<string> bankAccounts = await connection.QueryAsync<string>(statement);
                return bankAccounts.ToList();
            }
        }

        public async Task<IList<string>> FindAccountForBankSlip(string fn3agente)
        {
            string statement = "SELECT FN3CONTA FROM FN3 WHERE FN3AGENTE = @fn3agente AND TRIM(FN3FIMBLQ) <> '' ORDER BY FN3PRINCIP DESC";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                IEnumerable<string> bankAccounts = await connection.QueryAsync<string>(statement, new { fn3agente });
                return bankAccounts.ToList();
            }
        }

        public async Task<FN3> Find(string fn3agente, string fn3conta)
        {
            string command = @"SELECT * FROM FN3 WHERE FN3ATGENTE = @fn3agente AND FN3CONTA = @fn3conta";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                FN3 fn3 = await connection.QueryFirstOrDefaultAsync<FN3>(command, new { fn3agente, fn3conta });

                return fn3;
            }
        }

        public async Task<int> Insert(FN3 fn3)
        {
            string command = @"INSERT INTO FN3 (FN3AGENTE, FN3CONTA, FN3AGENCIA, FN3PROGREM, FN3PROGRET, FN3AGENNOM, FN3CODEMP, FN3CONTAL, FN3EMITECH, FN3PROGECH, FN3CART, FN3CEDENTE, FN3INIBLQ, FN3FIMBLQ, FN3JUROS, FN3DIASPRO, FN3MULTA, FN3MENS1, FN3MENS2, FN3MENS3, FN3MENS4, FN3PRINCIP)
                               VALUES (@fn3agente,@fn3conta,@fn3agencia,@fn3progrem,@fn3progret,@fn3agennom,@fn3codemp,@fn3contal,@fn3emitech,@fn3progech,@fn3cart,@fn3cedente,@fn3iniblq,@fn3fimblq,@fn3juros,@fn3diaspro,@fn3multa,@fn3mens1,@fn3mens2,@fn3mens3,@fn3mens4,@fn3princip)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    fn3.Fn3agente,
                    fn3.Fn3conta,
                    fn3.Fn3agencia,
                    fn3.Fn3progrem,
                    fn3.Fn3progret,
                    fn3.Fn3agennom,
                    fn3.Fn3codemp,
                    fn3.Fn3contal,
                    fn3.Fn3emitech,
                    fn3.Fn3progech,
                    fn3.Fn3cart,
                    fn3.Fn3cedente,
                    fn3.Fn3iniblq,
                    fn3.Fn3fimblq,
                    fn3.Fn3juros,
                    fn3.Fn3diaspro,
                    fn3.Fn3multa,
                    fn3.Fn3mens1,
                    fn3.Fn3mens2,
                    fn3.Fn3mens3,
                    fn3.Fn3mens4,
                    fn3.Fn3princip,

                });

                return affectedRows;
            }
        }

        public async Task<int> Delete(string fn3agente, string fn3conta)
        {
            string command = @"DELETE FROM FN3 WHERE FN3CONTA = @fn3conta AND FN3CONTA = @fn3conta";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { fn3agente, fn3conta });
            }
        }

        public async Task<int> Update(FN3 fn3)
        {
            string command = @"UPDATE FN3
                                  SET FN3AGENCIA = @fn3agencia,
                                      FN3PROGREM = @fn3progrem,
                                      FN3PROGRET = @fn3progret,
                                      FN3AGENNOM = @fn3agennom,
                                      FN3CODEMP = @fn3codemp,
                                      FN3CONTAL = @fn3contal,
                                      FN3EMITECH = @fn3emitech,
                                      FN3PROGECH = @fn3progech,
                                      FN3CART = @fn3cart,
                                      FN3CEDENTE = @fn3cedente,
                                      FN3INIBLQ = @fn3iniblq,
                                      FN3FIMBLQ = @fn3fimblq,
                                      FN3JUROS = @fn3juros,
                                      FN3DIASPRO = @fn3diaspro,
                                      FN3MULTA = @fn3multa,
                                      FN3MENS1 = @fn3mens1,
                                      FN3MENS2 = @fn3mens2,
                                      FN3MENS3 = @fn3mens3,
                                      FN3MENS4 = @fn3mens4,
                                      FN3PRINCIP = @fn3princip
                                WHERE FN3AGENTE = @fn3agente AND FN3CONTA = @fn3conta";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    fn3.Fn3agente,
                    fn3.Fn3conta,
                    fn3.Fn3agencia,
                    fn3.Fn3progrem,
                    fn3.Fn3progret,
                    fn3.Fn3agennom,
                    fn3.Fn3codemp,
                    fn3.Fn3contal,
                    fn3.Fn3emitech,
                    fn3.Fn3progech,
                    fn3.Fn3cart,
                    fn3.Fn3cedente,
                    fn3.Fn3iniblq,
                    fn3.Fn3fimblq,
                    fn3.Fn3juros,
                    fn3.Fn3diaspro,
                    fn3.Fn3multa,
                    fn3.Fn3mens1,
                    fn3.Fn3mens2,
                    fn3.Fn3mens3,
                    fn3.Fn3mens4,
                    fn3.Fn3princip,
                });

                return affectedRows;
            }
        }
    }
}
