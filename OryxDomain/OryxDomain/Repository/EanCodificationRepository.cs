using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class EanCodificationRepository : Repository
    {
        public EanCodificationRepository(string path) : base(path)
        {
        }

        public async Task<EAN> FindEan(string pr0produto, string pr2opcao, string pr3tamanho)
        {
            string command = @"SELECT *
                                 FROM EAN
                                WHERE EANPRODUTO = @pr0produto
                                  AND EANOPCAO = @pr2opcao
                                  AND EANTAMANHO = @pr3tamanho";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                EAN ean = await connection.QueryFirstOrDefaultAsync<EAN>(command, new { pr0produto, pr2opcao, pr3tamanho });

                return ean;
            }
        }

        public async Task<IList<string>> FindAllFixedCodes(string prefixo)
        {
            prefixo += "%";
            string command = @"SELECT PR0EAN AS EANCODIGO FROM PR0 WHERE PR0EAN LIKE @prefixo ORDER BY PR0EAN";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<string> lstFixedCodes = await connection.QueryAsync<string>(command, new { prefixo });

                return lstFixedCodes.ToList();
            }
        }

        public async Task<IList<string>> FindAllAutomaticCodes(string prefixo)
        {
            prefixo += "%";
            string command = @"SELECT EANCODIGO FROM EAN WHERE EANCODIGO LIKE @prefixo ORDER BY EANCODIGO";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<string> lstAutomaticCodes = await connection.QueryAsync<string>(command, new { prefixo });

                return lstAutomaticCodes.ToList();
            }
        }

        public async Task<IList<string>> FindAllInternalEan(int eansize)
        {
            string command = @"sELECT EANCODIGO FROM EAN WHERE SUBSTRING(EANCODIGO, 1, 1) <> '2' AND LENGTH(TRIM(EANCODIGO)) <> @eansize";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<string> lstAutomaticCodes = await connection.QueryAsync<string>(command, new { eansize });

                return lstAutomaticCodes.ToList();
            }
        }

        public async Task<int> Insert(EAN ean)
        {
            string command = @"INSERT INTO EAN (EANCODIGO, EANPRODUTO, EANOPCAO, EANTAMANHO, EANCODEXT)
                               VALUES (@eancodigo, @eanproduto, @eanopcao, @eantamanho, @eancodext)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    eancodigoean = ean.Eancodigo,
                    eanproduto = ean.Eanproduto,
                    eanopcao = ean.Eanopcao,
                    eantamanho = ean.Eantamanho,
                    eancodext = ean.Eancodext
                });

                return affectedRows;
            }
        }

        public async Task<int> Delete(string eancodigo)
        {
            string command = @"DELETE FROM EAN WHERE EANCODIGO = @eancodigo";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { eancodigo });
            }
        }

        public async Task<IList<EAN>> Search(string search, int limit, int offset)
        {
            search = string.Format("%{0}%", search);
            search = search.Replace(" ", "%");
            string command = @"SELECT *
                               FROM EAN
                               WHERE EANCODIGO LIKE @search
                                  OR EANPRODUTO LIKE @search
                               LIMIT @limit OFFSET @offset";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<EAN> lstEans = await connection.QueryAsync<EAN>(command, new { search, limit, offset });

                return lstEans.ToList();
            }
        }

        public async Task<IList<EAN>> FindList()
        {
            string command = @"SELECT * FROM EAN";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<EAN> lsteans = await connection.QueryAsync<EAN>(command);

                return lsteans.ToList();
            }
        }

        public async Task<EAN> Find(string eancodigo)
        {
            string command = @"SELECT * FROM EAN
                                WHERE EANCODIGO = @eancodigo";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                EAN ean = await connection.QueryFirstOrDefaultAsync<EAN>(command, new { eancodigo });

                return ean;
            }
        }

        public async Task<IList<EAN>> FindByPr0(string eanproduto)
        {
            string command = @"SELECT * FROM EAN
                                WHERE EANPRODUTO = @eanproduto";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<EAN> ean = await connection.QueryAsync<EAN>(command, new { eanproduto });

                return ean.ToList();
            }
        }

        public async Task<int> Update(EAN ean)
        {
            string command = @"UPDATE EAN
                                SET EANPRODUTO = @eanproduto,
                                EANOPCAO = @eanopcao,
                                EANTAMANHO = @eantamanho,
                                EANCODEXT = @eancodext
                                WHERE EANCODIGO = @eancodigo";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    eanproduto = ean.Eanproduto,
                    eanopcao = ean.Eanopcao,
                    eantamanho = ean.Eantamanho,
                    eancodext = ean.Eancodext,
                    eancodigo = ean.Eancodigo,
                });

                return affectedRows;
            }
        }

        public async Task<EAN> FindEanByCodext(string eancodext)
        {
            string command = @"SELECT * FROM EAN
                               WHERE EANCODEXT = @eancodext";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                EAN ean = await connection.QueryFirstOrDefaultAsync<EAN>(command, new { eancodext });

                return ean;
            }
        }

        public async Task<EAN> FindEanByParameters(string eancodigo, string eanproduto, string eanopcao, string eantamanho)
        {
            string command = @"SELECT * FROM EAN
                               WHERE EANCODIGO <> @eancodigo 
                               AND EANPRODUTO = @eanproduto 
                               AND EANOPCAO = @eanopcao
                               AND EANTAMANHO = @eantamanho";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                EAN eanExist = await connection.QueryFirstOrDefaultAsync<EAN>(command, 
                    new { 
                        eancodigo = eancodigo,
                        eanproduto = eanproduto,
                        eanopcao = eanopcao,
                        eantamanho = eantamanho,
                    });

                return eanExist;
            }
        }
    }
}