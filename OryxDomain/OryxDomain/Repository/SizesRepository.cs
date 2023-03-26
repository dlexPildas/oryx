using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class SizesRepository : Repository
    {
        public SizesRepository(string path) : base(path)
        {
        }
        public async Task<int> Insert(GR1 gr1)
        {
            string command = @"INSERT INTO GR1 (GR1DESC, GR1GRADE, GR1ESPECIF, GR1POSICAO, GR1TAMANHO, GR1TAMEXT)
                               VALUES (@gr1desc, @gr1grade, @gr1especif, @gr1posicao, @gr1tamanho, @gr1tamext)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    gr1desc = gr1.Gr1desc,
                    gr1grade = gr1.Gr1grade,
                    gr1especif = gr1.Gr1especif,
                    gr1posicao = gr1.Gr1posicao,
                    gr1tamanho = gr1.Gr1tamanho,
                    gr1tamext = gr1.Gr1tamext,
                });

                return affectedRows;
            }
        }

        public async Task<int> Update(GR1 gr1)
        {
            string command = @"UPDATE GR1 SET GR1DESC = @gr1desc, GR1GRADE = @gr1grade, GR1ESPECIF = @gr1especif, 
                               GR1POSICAO = @gr1posicao, GR1TAMEXT = @gr1tamext WHERE GR1TAMANHO = @gr1tamanho";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    gr1desc = gr1.Gr1desc,
                    gr1grade = gr1.Gr1grade,
                    gr1especif = gr1.Gr1especif,
                    gr1posicao = gr1.Gr1posicao,
                    gr1tamanho = gr1.Gr1tamanho,
                    gr1tamext = gr1.Gr1tamext,
                });

                return affectedRows;
            }
        }

        public async Task<GR1> Find(string gr1tamanho)
        {
            string command = @"SELECT * FROM GR1
                                WHERE GR1TAMANHO = @gr1tamanho";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                GR1 size = await connection.QueryFirstOrDefaultAsync<GR1>(command, new { gr1tamanho });

                return size;
            }
        }

        public async Task<int> Delete(string gr1tamanho)
        {
            string command = @"DELETE FROM GR1 WHERE GR1TAMANHO = @gr1tamanho";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { gr1tamanho });
            }
        }

        public async Task<int> DeleteSizeFromGrid(string gr0grade)
        {
            string command = @"DELETE FROM GR1 WHERE GR0GRADE = @gr0grade";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { gr0grade });
            }
        }

        public async Task<List<GR1>> FindByGR0(string gr1grade)
        {
            string command = @"SELECT * FROM GR1
                                WHERE GR1GRADE = @gr1grade";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<GR1> size = await connection.QueryAsync<GR1>(command, new { gr1grade });

                return size.ToList();
            }
        }


    }
}