using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class RecordDeparturesRepository : Repository
    {
        public RecordDeparturesRepository(string path) : base(path)
        {
        }


        public async Task<List<OFA>> FindOfa( string of3ordem, string of3lote)
        {
            string command = @"SELECT OFA.* , 
                             (SELECT MIN(ME2DOC) 
                             AS ME2DOC 
                             FROM ME2 
                             WHERE ME2INSUMO = OFAINSUMO 
                             AND ME2COR = OFACOR 
                             AND ME2TAMANHO = OFATAMINS) 
                             AS ME2DOC
                             FROM OFA 
                             WHERE OFAORDEM = @of3ordem
                             AND OFALOTE = @of3lote
                             ORDER BY OFAPOSICAO";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<OFA> ofalst = await connection.QueryAsync<OFA>(command, new
                {
                    of3ordem,
                    of3lote
                });

                return ofalst.ToList();
            }
        }
    }
}
