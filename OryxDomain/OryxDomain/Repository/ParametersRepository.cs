using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class ParametersRepository : Repository
    {
        public ParametersRepository(string path) : base(path)
        {
        }

        public async Task<LX0> GetLx0()
        {
            string command = @"SELECT LX0.*, CF1FANT, CF1LOGIN, CF1SENHA FROM LX0 INNER JOIN CF1 ON CF1CLIENTE = LX0CLIENTE";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                LX0 lx0 = await connection.QueryFirstOrDefaultAsync<LX0>(command);

                return lx0;
            }
        }

        public async Task<LX4> GetLx4()
        {
            string command = @"SELECT * FROM LX4";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                LX4 lx4 = await connection.QueryFirstOrDefaultAsync<LX4>(command);

                return lx4;
            }
        }

        public async Task<B2B> GetB2b()
        {
            string command = @"SELECT * from B2B";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                B2B b2b = await connection.QueryFirstOrDefaultAsync<B2B>(command);

                return b2b;
            }
        }

        public async Task<int> CreateTableTecdatasoft()
        {
            string command = @"CREATE TABLE IF NOT EXISTS TECDATASOFT(
                                CV7EMISSOR char(14) NOT NULL,
                                CV7TIPO char(3) NOT NULL,
                                CV7DOC char(6) NOT NULL,
                                CV7SUBTOT DECIMAL(12,2) DEFAULT 0,
                                AUTENTICACAO char(250) default ' ',
                                CONSTRAINT PK_TECDATASOFT PRIMARY KEY (CV7EMISSOR,CV7TIPO,CV7DOC,CV7SUBTOT))";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command);
            }
        }

        #region Selects
        public async Task<OryxNFeModel> FindOryxNFE()
        {
            string command = @"SELECT LX0.*,CF4CODIBGE,CF3ESTADO FROM LX0,CF1,CF2,CF3,CF4 WHERE LX0CLIENTE=CF1CLIENTE AND CF1CEP=CF2CEP AND CF2LOCAL=CF3LOCAL AND CF3ESTADO=CF4ESTADO";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                OryxNFeModel oryxNfe = await connection.QueryFirstOrDefaultAsync<OryxNFeModel>(command);

                return oryxNfe;
            }
        }

        public async Task<string> FindCodIBGE()
        {
            string command = @"SELECT CF4CODIBGE FROM CF4,LX0,CF1,CF2,CF3 WHERE LX0CLIENTE=CF1CLIENTE AND CF1CEP=CF2CEP AND CF2LOCAL=CF3LOCAL AND CF3ESTADO=CF4ESTADO";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                string codibge= await connection.QueryFirstOrDefaultAsync<string>(command);

                return codibge;
            }
        }
        #endregion
    }
}
