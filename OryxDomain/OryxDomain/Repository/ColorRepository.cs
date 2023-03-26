using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class ColorRepository : Repository
    {
        public ColorRepository(string path) : base(path)
        {
        }

        public async Task<IList<CR1>> FindList()
        {
            string command = @"SELECT * FROM CR1";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<CR1> lstCR1 = await connection.QueryAsync<CR1>(command);

                return lstCR1.ToList();
            }
        }
        public async Task<CR1> Find(string cr1cor)
        {
            string command = @"SELECT * FROM CR1
                                WHERE CR1COR = @cr1cor";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CR1 cr1 = await connection.QueryFirstOrDefaultAsync<CR1>(command, new { cr1cor });

                return cr1;
            }
        }
        public async Task<IList<CR1>> Search(string search, int limit, int offset)
        {
            search = string.Format("%{0}%", search);
            search = search.Replace(" ", "%");
            string command = @"SELECT *
                               FROM CR1
                               WHERE CR1COR LIKE @search
                                  OR CR1NOME LIKE @search
                               LIMIT @limit OFFSET @offset";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<CR1> colors = await connection.QueryAsync<CR1>(command, new { search, limit, offset });

                return colors.ToList();
            }
        }

        public async Task<int> Update(CR1 cr1)
        {
            string command = @"UPDATE CR1
                               SET CR1NOME = @cr1nome, CR1NUMERO = @cr1numero, 
                               CR1PANTONE = @cr1pantone, CR1RAL = @cr1ral
                               WHERE CR1COR = @cr1cor";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    cr1nome = cr1.Cr1nome,
                    cr1numero = cr1.Cr1numero,
                    cr1pantone = cr1.Cr1pantone,
                    cr1ral = cr1.Cr1ral,
                    cr1cor = cr1.Cr1cor
                });

                return affectedRows;
            }
        }

        public async Task<int> Insert(CR1 cr1)
        {
            string command = @"INSERT INTO CR1 (CR1COR, CR1NOME, CR1NUMERO, CR1PANTONE, CR1RAL)
                               VALUES (@cr1cor, @cr1nome, @cr1numero, @cr1pantone, @cr1ral)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    cr1nome = cr1.Cr1nome,
                    cr1numero = cr1.Cr1numero,
                    cr1pantone = cr1.Cr1pantone,
                    cr1ral = cr1.Cr1ral,
                    cr1cor = cr1.Cr1cor
                });

                return affectedRows;
            }
        }

        public async Task<int> Delete(string @cr1cor)
        {
            string command = @"DELETE FROM CR1 WHERE CR1COR = @cr1cor";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { cr1cor });
            }
        }

    }
}
