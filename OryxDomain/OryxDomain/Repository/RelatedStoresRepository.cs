using Dapper;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class RelatedStoresRepository : Repository
    {
        public RelatedStoresRepository(string path) : base(path)
        {
        }

        public async Task<IList<CF6>> Find(string state)
        {
            string command = @"SELECT Cf6repres, cf6nome, cf1fone, cf3nome
                               FROM CF6
                               INNER JOIN CF1 ON CF1CLIENTE = CF6CLIENTE
                               INNER JOIN CF2 ON CF2CEP = CF1CEP
                               INNER JOIN CF3 ON CF3LOCAL = CF2LOCAL AND CF3ESTADO = @state";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<CF6> lstCf6 = await connection.QueryAsync<CF6>(command, new { state });

                return lstCf6.ToList();
            }
        }

        public async Task<IList<CF4>> FindStates()
        {
            string command = @"SELECT Cf4estado, Cf4nome FROM CF4";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<CF4> lstCf4 = await connection.QueryAsync<CF4>(command);

                return lstCf4.ToList();
            }
        }
    }
}
