using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class SizeGridRepository : Repository
    {
        public SizeGridRepository(string path) : base(path)
        {
        }

        public async Task<IList<GR0>> FindList()
        {
            string command = @"SELECT * FROM GR0";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<GR0> sizegridlist = await connection.QueryAsync<GR0>(command);

                return sizegridlist.ToList();
            }
        }
        public async Task<GR0> Find(string gr0grade)
        {
            string command = @"SELECT * FROM GR0
                                WHERE GR0GRADE = @gr0grade";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                GR0 sizegrid = await connection.QueryFirstOrDefaultAsync<GR0>(command, new { gr0grade });

                return sizegrid;
            }
        }
        public async Task<IList<GR0>> Search(string search, int limit, int offset)
        {
            search = string.Format("%{0}%", search);
            search = search.Replace(" ", "%");
            string command = @"SELECT *
                               FROM GR0
                               WHERE GR0GRADE LIKE @search
                                  OR GR0DESC LIKE @search
                               LIMIT @limit OFFSET @offset";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<GR0> sizegrids = await connection.QueryAsync<GR0>(command, new { search, limit, offset });

                return sizegrids.ToList();
            }
        }

        public async Task<int> Update(GR0 gr0)
        { 
            string command = @"UPDATE GR0
                               SET GR0DESC = @gr0desc WHERE GR0GRADE = @gr0grade";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    gr0desc = gr0.Gr0desc,
                    gr0grade = gr0.Gr0grade
                });

                return affectedRows;
            }
        }

        public async Task<int> Insert(GR0 gr0)
        {
            string command = @"INSERT INTO GR0 (GR0GRADE, GR0DESC)
                               VALUES (@gr0grade, @gr0desc)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    gr0grade = gr0.Gr0grade,
                    gr0desc = gr0.Gr0desc
                });

                return affectedRows;
            }
        }

        public async Task<int> Delete(string gr0grade)
        {
            string command = @"DELETE FROM GR0 WHERE GR0GRADE = @gr0grade";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { gr0grade });
            }
        }        
    }
}
