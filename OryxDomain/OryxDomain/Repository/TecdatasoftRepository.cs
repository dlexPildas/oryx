using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class TecdatasoftRepository : Repository
    {
        public TecdatasoftRepository(string path) : base(path)
        {
        }

        public async Task<TecdatasoftModel> Find(string cv7emissor, string cv7tipo, string cv7doc)
        {
            string command = @"SELECT * FROM TECDATASOFT WHERE CV7EMISSOR = @cv7emissor AND CV7TIPO = @cv7tipo AND CV7DOC = @cv7doc";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                TecdatasoftModel tecdatasoft = await connection.QueryFirstOrDefaultAsync<TecdatasoftModel>(command, new { cv7emissor, cv7tipo, cv7doc });

                return tecdatasoft;
            }
        }

        public async Task<int> Insert(TecdatasoftModel tec)
        {
            string command = @"INSERT INTO TECDATASOFT (CV7EMISSOR,CV7TIPO,CV7DOC,CV7SUBTOT,AUTENTICACAO) 
                               VALUES (@Cv7emissor,@Cv7tipo,@Cv7doc,@Cv7subtot,@Autenticacao)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    tec.Cv7emissor,
                    tec.Cv7tipo,
                    tec.Cv7doc,
                    tec.Cv7subtot,
                    tec.Autenticacao
                });

                return affectedRows;
            }
        }

        public async Task<int> Delete(string cv7emissor, string cv7tipo, string cv7doc)
        {
            string command = @"DELETE FROM TECDATASOFT WHERE CV7EMISSOR = @Cv7emissor AND CV7TIPO = @Cv7tipo AND CV7DOC = @Cv7doc";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { cv7emissor, cv7tipo, cv7doc });
            }
        }

        public async Task<int> Update(TecdatasoftModel tec)
        {
            string command = @"UPDATE PD0
                               SET CV7SUBTOT = @Cv7subtot, AUTENTICACAO = @Autenticacao
                               WHERE CV7EMISSOR = @Cv7emissor AND CV7TIPO = @Cv7tipo AND CV7DOC = @Cv7doc";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    tec.Cv7emissor,
                    tec.Cv7tipo,
                    tec.Cv7doc,
                    tec.Cv7subtot,
                    tec.Autenticacao
                });

                return affectedRows;
            }
        }
    }
}
