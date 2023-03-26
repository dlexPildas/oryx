using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class OrderParametersRepository : Repository
    {
        public OrderParametersRepository(string path) : base(path)
        {
        }

        public async Task<LX2> Find()
        {
            string command = @"SELECT LX2.*, CF1NOME AS StockName FROM LX2 LEFT JOIN CF1 ON CF1CLIENTE = LX2CLIEST";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                LX2 lx2 = await connection.QueryFirstOrDefaultAsync<LX2>(command);
                return lx2;
            }
        }

        public async Task<int> Insert(LX2 lx2)
        {
            string command = @"INSERT INTO LX2 (LX2PADRAO, LX2ENTREGA, LX2IMPLANT, LX2ENTREG2, LX2IMPLAN2, LX2NAOCONF, LX2CATIVA, LX2OPCOES, LX2DESTINA, LX2PRECO, LX2PRONTA, LX2OUTROS, LX2SCRIPT, LX2ETIQ, LX2DIASDEV, LX2VENCONS, LX2ESTDEV, LX2DEBEST, LX2CLIEST)
                               VALUES (@lx2padrao,@lx2entrega,@lx2implant,@lx2entreg2,@lx2implan2,@lx2naoconf,@lx2cativa,@lx2opcoes,@lx2destina,@lx2preco,@lx2pronta,@lx2outros,@lx2script,@lx2etiq,@lx2diasdev,@lx2vencons,@lx2estdev,@lx2debest,@lx2cliest)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    lx2padrao = lx2.Lx2padrao,
                    lx2entrega = lx2.Lx2entrega,
                    lx2implant = lx2.Lx2implant,
                    lx2entreg2 = lx2.Lx2entreg2,
                    lx2implan2 = lx2.Lx2implan2,
                    lx2naoconf = lx2.Lx2naoconf,
                    lx2cativa = lx2.Lx2cativa,
                    lx2opcoes = lx2.Lx2opcoes,
                    lx2destina = lx2.Lx2destina,
                    lx2preco = lx2.Lx2preco,
                    lx2pronta = lx2.Lx2pronta,
                    lx2outros = lx2.Lx2outros,
                    lx2script = lx2.Lx2script,
                    lx2etiq = lx2.Lx2etiq,
                    lx2diasdev = lx2.Lx2diasdev,
                    lx2vencons = lx2.Lx2vencons,
                    lx2estdev = lx2.Lx2estdev,
                    lx2debest = lx2.Lx2debest,
                    lx2cliest = lx2.Lx2cliest
                });

                return affectedRows;
            }
        }

        public async Task<int> Update(LX2 lx2)
        {
            string command = @"UPDATE LX2
                                  SET LX2ENTREGA = @lx2entrega, LX2IMPLANT = @lx2implant, LX2ENTREG2 = @lx2entreg2, LX2IMPLAN2 = @lx2implan2, LX2NAOCONF = @lx2naoconf, LX2CATIVA = @lx2cativa, LX2OPCOES = @lx2opcoes, LX2DESTINA = @lx2destina, LX2PRECO = @lx2preco, LX2PRONTA = @lx2pronta, LX2OUTROS = @lx2outros, LX2SCRIPT = @lx2script, LX2ETIQ = @lx2etiq, LX2DIASDEV = @lx2diasdev, LX2VENCONS = @lx2vencons, LX2ESTDEV = @lx2estdev, LX2DEBEST = @lx2debest, LX2CLIEST = @lx2cliest
                                where LX2PADRAO = @lx2padrao";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    lx2padrao = lx2.Lx2padrao,
                    lx2entrega = lx2.Lx2entrega,
                    lx2implant = lx2.Lx2implant,
                    lx2entreg2 = lx2.Lx2entreg2,
                    lx2implan2 = lx2.Lx2implan2,
                    lx2naoconf = lx2.Lx2naoconf,
                    lx2cativa = lx2.Lx2cativa,
                    lx2opcoes = lx2.Lx2opcoes,
                    lx2destina = lx2.Lx2destina,
                    lx2preco = lx2.Lx2preco,
                    lx2pronta = lx2.Lx2pronta,
                    lx2outros = lx2.Lx2outros,
                    lx2script = lx2.Lx2script,
                    lx2etiq = lx2.Lx2etiq,
                    lx2diasdev = lx2.Lx2diasdev,
                    lx2vencons = lx2.Lx2vencons,
                    lx2estdev = lx2.Lx2estdev,
                    lx2debest = lx2.Lx2debest,
                    lx2cliest = lx2.Lx2cliest
                });

                return affectedRows;
            }
        }

        public async Task<int> Delete()
        {
            string command = @"DELETE FROM LX2";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command);
            }
        }
    }
}
