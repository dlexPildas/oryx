using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class FiscalClassificationRepository : Repository
    {
        public FiscalClassificationRepository(string path) : base(path)
        {
        }

        public async Task<IList<FI0>> FindList()
        {
            string command = @"SELECT * FROM FI0";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<FI0> lstFi0 = await connection.QueryAsync<FI0>(command);

                return lstFi0.ToList();
            }
        }

        public async Task<FI0> Find(string fi0id)
        {
            string command = @"SELECT * FROM FI0 WHERE FI0ID = @fi0id";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                FI0 fi0 = await connection.QueryFirstOrDefaultAsync<FI0>(command, new { fi0id });

                return fi0;
            }
        }

        public async Task<int> Insert(FI0 fi0)
        {
            string command = @"INSERT INTO FI0 (FI0ID,FI0CLASSIF,FI0DESC,FI0EXCINSS,FI0CEST)
                               VALUES (@fi0id,@fi0classif,@fi0desc,@fi0excinss,@fi0cest)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    fi0id = fi0.Fi0id,
                    fi0classif = fi0.Fi0classif,
                    fi0desc = fi0.Fi0desc,
                    fi0excinss = fi0.Fi0excinss,
                    fi0cest = fi0.Fi0cest
                });

                return affectedRows;
            }
        }

        public async Task<int> Delete(string fi0id)
        {
            string command = @"DELETE FROM FI0 WHERE FI0ID = @fi0id";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { fi0id });
            }
        }

        public async Task<int> Update(FI0 fi0)
        {
            string command = @"UPDATE FI0
                               SET FI0CLASSIF = @fi0classif, FI0DESC = @fi0desc, FI0EXCINSS = @fi0excinss, FI0CEST = @fi0cest
                               WHERE FI0ID = @fi0id";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    fi0id = fi0.Fi0id,
                    fi0classif = fi0.Fi0classif,
                    fi0desc = fi0.Fi0desc,
                    fi0excinss = fi0.Fi0excinss,
                    fi0cest = fi0.Fi0cest
                });

                return affectedRows;
            }
        }

        public async Task<IList<FI0>> Search(string search, int limit, int offset)
        {
            search = string.Format("%{0}%", search);
            search = search.Replace(" ", "%");
            string command = @"SELECT *
                               FROM FI0
                               WHERE FI0ID LIKE @search
                                  OR FI0CLASSIF LIKE @search
                                  OR FI0DESC LIKE @search
                               LIMIT @limit OFFSET @offset";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<FI0> fiscalClassifications = await connection.QueryAsync<FI0>(command, new { search, limit, offset });

                return fiscalClassifications.ToList();
            }
        }
    }
}