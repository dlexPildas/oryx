using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Enums;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class LogisticIntegrationRepository : Repository
    {
        public LogisticIntegrationRepository(string path) : base(path)
        {
        }

        public async Task<IList<LXF>> FindList()
        {
            string command = @"SELECT * FROM LXF";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<LXF> lstLxf = await connection.QueryAsync<LXF>(command);

                return lstLxf.ToList();
            }
        }

        public async Task<LXF> Find(string lxfcodigo)
        {
            string command = @"SELECT * FROM LXF
                                WHERE LXFCODIGO = @lxfcodigo";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                LXF lxf = await connection.QueryFirstOrDefaultAsync<LXF>(command, new { lxfcodigo });

                return lxf;
            }
        }

        public async Task<LXF> FindByType(LogisticIntegrationType lxftipo)
        {
            string command = @"SELECT * FROM LXF
                                WHERE LXFTIPO = @lxftipo";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                LXF lxf = await connection.QueryFirstOrDefaultAsync<LXF>(command, new { lxftipo });

                return lxf;
            }
        }

        public async Task<IList<LXF>> Search(string search, int limit, int offset)
        {
            search = string.Format("%{0}%", search);
            search = search.Replace(" ", "%");
            string command = @"SELECT * FROM LXF
                               WHERE LXFNOME LIKE @search
                                  OR LXFCODIGO LIKE @search
                               LIMIT @limit OFFSET @offset";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<LXF> lxflst = await connection.QueryAsync<LXF>(command, new { search, limit, offset });

                return lxflst.ToList();
            }
        }

        public async Task<int> Update(LXF lxf)
        {
            string command = @"UPDATE LXF
                               SET LXFAUT1 = @lxfaut1
                                   LXFAUT2 = @lxfaut2,
                                   LXFAUT3 = @lxfaut3,
                                   LXFAUT4 = @lxfaut4,
                                   LXFBASEURL = @lxfbaseurl,
                                   LXFNOME = @lxfnome,
                                   LXFPAGAUT = @lxfpagaut,
                                   LXFPERMISS = @lxfpermiss,
                                   LXFTIPO = @lxftipo,
                                   LXFTOKEN = @lxftoken,
                                   LXFURLCALL = @lxfurlcall
                               WHERE LXFCODIGO = @lxfcodigo";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    lxfaut1 = lxf.Lxfaut1,
                    lxfaut2 = lxf.Lxfaut2,
                    lxfaut3 = lxf.Lxfaut3,
                    lxfaut4 = lxf.Lxfaut4,
                    lxfbaseurl = lxf.Lxfbaseurl,
                    lxfcodigo = lxf.Lxfcodigo,
                    lxfpagaut = lxf.Lxfpagaut,
                    lxfpermiss = lxf.Lxfpermiss,
                    lxftipo = lxf.Lxftipo,
                    lxftoken = lxf.Lxftoken,
                    lxfurlcall = lxf.Lxfurlcall,
                    lxfnome = lxf.Lxfnome
                });

                return affectedRows;
            }
        }

        public async Task<int> Insert(LXF lxf)
        {
            string command = @"INSERT INTO LXF (LXFAUT1, LXFAUT2, LXFAUT3, LXFAUT4, LXFBASEURL, LXFNOME, LXFPAGAUT, LXFPERMISS, LXFTIPO, LXFTOKEN, LXFURLCALL, LXFCODIGO)
                               VALUES (@lxfaut1, @lxfaut2, @lxfaut3, @lxfaut4, @lxfbaseurl, @lxfnome, @lxfpagaut, @lxfpermiss, @lxftipo, @lxftoken, @lxfurlcall, @lxfcodigo)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    lxfaut1 = lxf.Lxfaut1,
                    lxfaut2 = lxf.Lxfaut2,
                    lxfaut3 = lxf.Lxfaut3,
                    lxfaut4 = lxf.Lxfaut4,
                    lxfbaseurl = lxf.Lxfbaseurl,
                    lxfnome = lxf.Lxfnome,
                    lxfpagaut = lxf.Lxfpagaut,
                    lxfpermiss = lxf.Lxfpermiss,
                    lxftipo = lxf.Lxftipo,
                    lxftoken = lxf.Lxftoken,
                    lxfurlcall = lxf.Lxfurlcall,
                    lxfcodigo = lxf.Lxfcodigo
                });

                return affectedRows;
            }
        }

        public async Task<int> Delete(string lxfcodigo)
        {
            string command = @"DELETE FROM LXF WHERE LXFCODIGO = @lxfcodigo";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { lxfcodigo });
            }
        }

    }
}