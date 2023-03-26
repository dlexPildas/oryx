using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class GroupRepository : Repository
    {
        public GroupRepository(string path) : base(path)
        {
        }

        public async Task<IList<PRS>> FindList()
        {
            string command = @"SELECT * FROM PRS";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<PRS> lstPrs = await connection.QueryAsync<PRS>(command);

                return lstPrs.ToList();
            }
        }

        public async Task<PRS> Find(string prsgrupo)
        {
            string command = @"SELECT * FROM PRS
                                WHERE PRSGRUPO = @prsgrupo";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                PRS prs = await connection.QueryFirstOrDefaultAsync<PRS>(command, new { prsgrupo });

                return prs;
            }
        }

        public async Task<IList<PRS>> Search(string search, int limit, int offset)
        {
            search = string.Format("%{0}%", search);
            search = search.Replace(" ", "%");
            string command = @"SELECT *
                               FROM PRS
                               WHERE PRSGRUPO LIKE @search
                                  OR PRSNOME LIKE @search
                               LIMIT @limit OFFSET @offset";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<PRS> groups = await connection.QueryAsync<PRS>(command, new { search, limit, offset });

                return groups.ToList();
            }
        }

        public async Task<int> Update(PRS prs)
        {
            string command = @"UPDATE PRS
                               SET PRSNOME = @prsnome, PRSB2B = @prsb2b
                               WHERE PRSGRUPO = @prsgrupo";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    prsgrupo = prs.Prsgrupo,
                    prsnome = prs.Prsnome,
                    prsb2b = prs.Prsb2b
                });

                return affectedRows;
            }
        }

        public async Task<int> Insert(PRS prs)
        {
            string command = @"INSERT INTO PRS (PRSGRUPO, PRSNOME, PRSB2B)
                               VALUES (@prsgrupo, @prsnome, @prsb2b)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    prsgrupo = prs.Prsgrupo,
                    prsnome = prs.Prsnome,
                    prsb2b = prs.Prsb2b
                });

                return affectedRows;
            }
        }

        public async Task<int> Delete(string prsgrupo)
        {
            string command = @"DELETE FROM PRS WHERE PRSGRUPO = @prsgrupo";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { prsgrupo });
            }
        }
    }
}
