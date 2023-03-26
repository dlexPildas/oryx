using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class FiscalDocumentTypeRepository : Repository
    {
        public FiscalDocumentTypeRepository(string path) : base(path)
        {
        }

        public async Task<IList<CV1>> FindList()
        {
            string command = @"SELECT * FROM CV1";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<CV1> lstCv1 = await connection.QueryAsync<CV1>(command);

                return lstCv1.ToList();
            }
        }

        public async Task<CV1> Find(string cv1docfis)
        {
            string command = @"SELECT * FROM CV1 WHERE CV1DOCFIS = @cv1docfis";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CV1 cv1 = await connection.QueryFirstOrDefaultAsync<CV1>(command, new { cv1docfis });

                return cv1;
            }
        }

        public async Task<int> Insert(CV1 cv1)
        {
            string command = @"INSERT INTO CV1 (CV1DOCFIS,CV1NOME,CV1MODELO,CV1SERIE,CV1SUBSER,CV1RELAT,CV1ULTIMO) 
                                        VALUES (@Cv1docfis,@Cv1nome,@Cv1modelo,@Cv1serie,@Cv1subser,@Cv1relat,@Cv1ultimo)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    cv1.Cv1docfis,
                    cv1.Cv1modelo,
                    cv1.Cv1nome,
                    cv1.Cv1relat,
                    cv1.Cv1serie,
                    cv1.Cv1subser,
                    cv1.Cv1ultimo
                });

                return affectedRows;
            }
        }

        public async Task<int> Delete(string cv1docfis)
        {
            string command = @"DELETE FROM CV1 WHERE CV1DOCFIS = @cv1docfis";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { cv1docfis });
            }
        }

        public async Task<int> Update(CV1 cv1)
        {
            string command = @"UPDATE CV1
                               SET CV1NOME = @Cv1nome,CV1MODELO = @Cv1modelo,CV1SERIE = @Cv1serie,CV1SUBSER = @Cv1subser,CV1RELAT = @Cv1relat,CV1ULTIMO = @Cv1ultimo
                               WHERE CV1DOCFIS = @Cv1docfis";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    cv1.Cv1docfis,
                    cv1.Cv1modelo,
                    cv1.Cv1nome,
                    cv1.Cv1relat,
                    cv1.Cv1serie,
                    cv1.Cv1subser,
                    cv1.Cv1ultimo
                });

                return affectedRows;
            }
        }

        public async Task<IList<CV1>> Search(string search, int limit, int offset)
        {
            search = string.Format("%{0}%", search);
            search = search.Replace(" ", "%");
            string command = @"SELECT *
                               FROM CV1
                               WHERE CV1DOCFIS LIKE @search
                                  OR CV1NOME LIKE @search
                               LIMIT @limit OFFSET @offset";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<CV1> lstCv1 = await connection.QueryAsync<CV1>(command, new { search, limit, offset });

                return lstCv1.ToList();
            }
        }

        public async Task<string> FindLastNumber(string tipo)
        {
            string command = @"SELECT CV1ULTIMO FROM CV1 WHERE CV1DOCFIS = @tipo";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                string cv1ultimo = await connection.QueryFirstOrDefaultAsync<string>(command, new { tipo });

                return cv1ultimo;
            }
        }
    }
}
