using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class PieceLiberationRepository : Repository
    {
        public PieceLiberationRepository(string path) : base(path)
        {
        }

        public async Task<List<OF0>> FindLiberation(string of3peca)
        {
            string command = @"SELECT * FROM OF0 
                               WHERE OF0PECA = @of3peca";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<OF0> liberationlst = await connection.QueryAsync<OF0>(command, new { of3peca });

                return liberationlst.ToList();
            }
        }
    }
}
