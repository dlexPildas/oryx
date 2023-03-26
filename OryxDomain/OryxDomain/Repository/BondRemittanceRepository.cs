using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Utilities;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class BondRemittanceRepository : Repository
    {
        public BondRemittanceRepository(string path) : base(path)
        {
        }

        public async Task<string> FindAgentOfBond(string cv0emissor, string cv0docfis, string cv0doc, string cv0parcela)
        {
            string statement = "SELECT CV9AGENTE FROM CV9,CV0 WHERE  CV9REMESSA=CV0REMESSA AND CV0EMISSOR = @cv0emissor AND CV0DOCFIS = @cv0docfis AND CV0DOC = @cv0doc AND CV0PARCELA = @cv0parcela ORDER BY CV9ABERT";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                string agent = await connection.QueryFirstOrDefaultAsync<string>(statement, new { cv0emissor, cv0docfis, cv0doc, cv0parcela });
                return agent;
            }
        }
    }
}
