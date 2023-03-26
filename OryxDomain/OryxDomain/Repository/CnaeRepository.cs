using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class CnaeRepository : Repository
    {
        public CnaeRepository(string path) : base(path)
        {
        }

        public async Task<int> Insert(CFG cfg)
        {
            string command = @"INSERT INTO CFG(CFGCLIENTE,CFGCNAE,CFGATIVIDA)
                                VALUES(@CFGCLIENTE, @CFGCNAE, @CFGATIVIDA)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    CFGCLIENTE = cfg.Cfgcliente,
                    CFGCNAE = cfg.Cfgcnae,
                    CFGATIVIDA = cfg.Cfgativida
                });

                return affectedRows;
            }
        }

        public async Task<int> Update(CFG cfg)
        {
            string command = @"UPDATE CFG SET
                            CFGCLIENTE = @CFGCLIENTE, CFGCNAE = @CFGCNAE, CFGATIVIDA = @CFGATIVIDA
                                WHERE CFGCLIENTE = @CFGCLIENTE AND CFGCNAE = @CFGCNAE";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    CFGCLIENTE = cfg.Cfgcliente,
                    CFGCNAE = cfg.Cfgcnae,
                    CFGATIVIDA = cfg.Cfgativida
                });

                return affectedRows;
            }
        }

        public async Task<int> Delete(string cpfCnpj)
        {
            string statement = "DELETE FROM CFG WHERE CFGCLIENTE = @cpfcnpj";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                var effectedRows = await connection.ExecuteAsync(statement, new { cpfCnpj });
                return effectedRows;
            }
        }

        public async Task<IList<CFG>> FindAll(string cf1cliente)
        {
            string statement = "SELECT * FROM CFG WHERE CFGCLIENTE = @cf1cliente";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                IEnumerable<CFG> lstCfg = await connection.QueryAsync<CFG>(statement, new { cf1cliente });
                return lstCfg.ToList();
            }
        }
    }
}
