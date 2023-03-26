using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class ColorOptionsRepository : Repository
    {
        public ColorOptionsRepository(string path) : base(path)
        {
        }

        public async Task<PR2> FindByEan(string eanproduto, string eanopcao)
        {
            string command = @"SELECT PR2.*, CR1NOME 
                                 FROM PR2
                                INNER JOIN CR1 ON CR1COR = PR2OPCAO
                                WHERE PR2OPCAO = @eanopcao
                                  AND PR2PRODUTO = @eanproduto";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                PR2 pr2 = await connection.QueryFirstOrDefaultAsync<PR2>(command, new { eanopcao, eanproduto });

                return pr2;
            }
        }

        public async Task<PR2> Find(string pr2produto, string pr2opcao)
        {
            string command = @"SELECT * FROM PR2
                                WHERE PR2PRODUTO = @pr2produto
                                AND PR2OPCAO = @pr2opcao";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                PR2 pr2 = await connection.QueryFirstOrDefaultAsync<PR2>(command, new { pr2produto, pr2opcao });

                return pr2;
            }
        }

        public async Task<IList<PR2>> FindList(string pr2produto)
        {
            string command = @"SELECT * FROM PR2 WHERE PR2PRODUTO = @pr2produto";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<PR2> lstpr2s = await connection.QueryAsync<PR2>(command, new { pr2produto });

                return lstpr2s.ToList();
            }
        }

        public async Task<IList<PR2>> Search(string search, int limit, int offset)
        {
            search = string.Format("%{0}%", search);
            search = search.Replace(" ", "%");
            string command = @"SELECT PR2.*, CR1.CR1NOME, CR1.Cr1numero
                               FROM PR2
                               INNER JOIN CR1 ON CR1COR = PR2OPCAO
                               WHERE PR2OPCAO LIKE @search
                                  OR PR2REFER LIKE @search
                                  OR PR2PRODUTO LIKE @search
                                  OR PR2COR LIKE @search
                                  OR CR1NOME LIKE @search
                               LIMIT @limit OFFSET @offset";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<PR2> lstpr2s = await connection.QueryAsync<PR2>(command, new { search, limit, offset });

                return lstpr2s.ToList();
            }
        }

        public async Task<int> Update(PR2 pr2)
        {
            string command = @"UPDATE PR2
                               SET PR2REFER = @pr2refer
                               SET PR2COR = @pr2cor
                               SET PR2IMAGEM = @pr2imagem
                               SET PR2FORACAT = @pr2foracat
                               WHERE PR2PRODUTO = @pr2produto
                               AND PR2OPCAO = @pr2opcao";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    pr2refer = pr2.Pr2refer,
                    pr2cor = pr2.Pr2cor,
                    pr2imagem = pr2.Pr2imagem,
                    pr2foracat = pr2.Pr2foracat,
                    pr2produto = pr2.Pr2produto,
                    pr2opcao = pr2.Pr2opcao,
                });

                return affectedRows;
            }
        }

        public async Task<int> Insert(PR2 pr2)
        {
            string command = @"INSERT INTO PR2 (PR2PRODUTO, PR2OPCAO, PR2REFER, PR2COR, PR2IMAGEM, PR2FORACAT)
                               VALUES (@pr2produto, @pr2opcao, @pr2refer, @pr2cor, @pr2imagem, @pr2foracat)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    pr2produto = pr2.Pr2produto,
                    pr2opcao = pr2.Pr2opcao,
                    pr2refer = pr2.Pr2refer,
                    pr2cor = pr2.Pr2cor,
                    pr2imagem = pr2.Pr2imagem,
                    pr2foracat = pr2.Pr2foracat,
                });

                return affectedRows;
            }
        }

        public async Task<int> Delete(string pr2produto, string pr2opcao)
        {
            string command = @"DELETE FROM PR2 
                               WHERE PR2PRODUTO = @pr2produto
                               AND PR2OPCAO = @pr2opcao";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { pr2opcao, pr2produto });
            }
        }
    }
}
