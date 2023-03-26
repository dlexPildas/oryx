using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class PostageAgencyRepository : Repository
    {
        public PostageAgencyRepository(string path) : base(path)
        {
        }

        public async Task<IList<LXG>> FindList()
        {
            string command = @"SELECT * FROM LXG";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<LXG> lstLXG = await connection.QueryAsync<LXG>(command);

                return lstLXG.ToList();
            }
        }

        public async Task<LXG> Find(string lxgtransp)
        {
            string command = @"SELECT * FROM LXG
                                WHERE LXGTRANSP = @lxgtransp";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                LXG lxg = await connection.QueryFirstOrDefaultAsync<LXG>(command, new { lxgtransp });

                return lxg;
            }
        }

        public async Task<IList<LXG>> Search(string search, int limit, int offset)
        {
            search = string.Format("%{0}%", search);
            search = search.Replace(" ", "%");
            string command = @"SELECT * FROM LXG
                               WHERE LXGAGENCIA LIKE @search
                                  OR LXGTRANSP LIKE @search
                               LIMIT @limit OFFSET @offset";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<LXG> lxglst = await connection.QueryAsync<LXG>(command, new { search, limit, offset });

                return lxglst.ToList();
            }
        }

        public async Task<int> Update(LXG lxg)
        {
            string command = @"UPDATE LXG
                               SET LXGAGENCIA = @Lxgagencia
                               SET WHERE LXGTRANSP = @Lxgtransp";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                   lxg.Lxgagencia,
                   lxg.Lxgtransp
                });

                return affectedRows;
            }
        }

        public async Task<int> Insert(LXG lxg)
        {
            string command = @"INSERT INTO LXG (LXGTRANSP, LXGAGENCIA)
                               VALUES (@Lxgtransp, @Lxgagencia)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    lxg.Lxgagencia,
                    lxg.Lxgtransp
                });

                return affectedRows;
            }
        }

        public async Task<int> Delete(string lxgtransp)
        {
            string command = @"DELETE FROM LXG WHERE LXGTRANSP = @lxgtransp";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { lxgtransp });
            }
        }

    }
}