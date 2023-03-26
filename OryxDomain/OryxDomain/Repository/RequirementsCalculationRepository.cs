using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Utilities;
using System;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class RequirementsCalculationRepository : Repository
    {
        public RequirementsCalculationRepository(string path) : base(path)
        {
        }

        public async Task<DateTime> FindCalcDate()
        {
            string command = @"SELECT PL1DHCALC FROM PL1 WHERE PL1MESTRE = 1";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                DateTime pl1dhcalc = await connection.QueryFirstOrDefaultAsync<DateTime>(command);

                return pl1dhcalc;
            }
        }
    }
}
