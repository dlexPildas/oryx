using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class FamilyRepository : Repository
    {
        public FamilyRepository(string path) : base(path)
        {
        }

        public async Task<IList<PRB>> FindList()
        {
            string command = @"SELECT * FROM PRB";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<PRB> lstprbs = await connection.QueryAsync<PRB>(command);

                return lstprbs.ToList();
            }
        }

        public async Task<PRB> Find(string prbfamilia)
        {
            string command = @"SELECT * FROM PRB
                                WHERE PRBFAMILIA = @prbfamilia";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                PRB prb = await connection.QueryFirstOrDefaultAsync<PRB>(command, new { prbfamilia });

                return prb;
            }
        }

        public async Task<IList<PRB>> Search(string search, int limit, int offset)
        {
            search = string.Format("%{0}%", search);
            search = search.Replace(" ", "%");
            string command = @"SELECT *
                               FROM PRB
                               WHERE PRBFAMILIA LIKE @search
                                  OR PRBNOME LIKE @search
                               LIMIT @limit OFFSET @offset";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<PRB> lstprbs = await connection.QueryAsync<PRB>(command, new { search, limit, offset });

                return lstprbs.ToList();
            }
        }

        public async Task<int> Update(PRB prb)
        {
            string command = @"UPDATE PRB
                               SET PRBNOME = @prbnome
                               WHERE PRBFAMILIA = @prbfamilia";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    prbfamilia = prb.Prbfamilia,
                    prbnome = prb.Prbnome,
                });

                return affectedRows;
            }
        }

        public async Task<int> Insert(PRB prb)
        {
            string command = @"INSERT INTO PRB (PRBFAMILIA, PRBNOME)
                               VALUES (@prbfamilia, @prbnome)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    prbfamilia = prb.Prbfamilia,
                    prbnome = prb.Prbnome
                });

                return affectedRows;
            }
        }

        public async Task<int> Delete(string prbfamilia)
        {
            string command = @"DELETE FROM PRB WHERE PRBFAMILIA = @prbfamilia";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { prbfamilia });
            }
        }

    }
}
