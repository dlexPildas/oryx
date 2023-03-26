using Dapper;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class PrintingPreferencesRepository : Repository
    {
        public PrintingPreferencesRepository(string path) : base(path)
        {
        }

        public async Task<IList<PD1>> FindList(string pd1codigo)
        {
            string command = @"SELECT PD1.*, PD0NOME, DC9NOME
                               FROM PD1
                               INNER JOIN PD0 ON PD0CODIGO = PD1CODIGO
                               INNER JOIN DC9 ON DC9RELAT = PD1RELAT
                               WHERE PD1CODIGO = @pd1codigo";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    IEnumerable<PD1> lstPd1 = await connection.QueryAsync<PD1>(command, new { pd1codigo });

                    return lstPd1.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<PD1> Find(string pd1codigo, string pd1relat)
        {
            string command = @"SELECT PD1.*, PD0NOME, DC9NOME
                                 FROM PD1
                                INNER JOIN PD0 ON PD0CODIGO = PD1CODIGO
                                INNER JOIN DC9 ON DC9RELAT = PD1RELAT
                                WHERE PD1CODIGO = @pd1codigo AND PD1RELAT = @pd1relat";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    PD1 pd1 = await connection.QueryFirstOrDefaultAsync<PD1>(command, new { pd1codigo, pd1relat });

                    return pd1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> Update(PD1 pd1)
        {
            string command = @"UPDATE PD1
                               SET PD1IMPRES = @pd1impres, PD1VIAS = @pd1vias
                               WHERE PD1CODIGO = @pd1codigo AND PD1RELAT = @pd1relat";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    var affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        pd1codigo = pd1.Pd1codigo,
                        pd1relat = pd1.Pd1relat,
                        pd1impres = pd1.Pd1impres,
                        pd1vias = pd1.Pd1vias
                    });

                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> Insert(PD1 pd1)
        {
            string command = @"INSERT INTO PD1 (PD1CODIGO, PD1RELAT, PD1IMPRES, PD1VIAS)
                               VALUES (@pd1codigo, @pd1relat, @pd1impres, @pd1vias)";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    var affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        pd1codigo = pd1.Pd1codigo,
                        pd1relat = pd1.Pd1relat,
                        pd1impres = pd1.Pd1impres,
                        pd1vias = pd1.Pd1vias
                    });

                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> Delete(string pd1codigo, string pd1relat)
        {
            string command = @"DELETE FROM PD1 WHERE PD1CODIGO = @pd1codigo AND PD1RELAT = @pd1relat";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    return await connection.ExecuteAsync(command, new { pd1codigo, pd1relat });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IList<PD1>> Search(string search, int limit, int offset)
        {
            search = string.Format("%{0}%", search);
            search = search.Replace(" ", "%");
            string command = @"SELECT PD1.*, PD0NOME, DC9NOME
                               FROM PD1
                               INNER JOIN PD0 ON PD0CODIGO = PD1CODIGO
                               INNER JOIN DC9 ON DC9RELAT = PD1RELAT
                               WHERE PD1CODIGO LIKE @search
                                  OR PD1RELAT LIKE @search
                                  OR PD0NOME LIKE @search
                                  OR DC9NOME LIKE @search
                                  OR PD1IMPRES LIKE @search
                               LIMIT @limit OFFSET @offset";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<PD1> preferences = await connection.QueryAsync<PD1>(command, new { search, limit, offset });

                return preferences.ToList();
            }
        }
    }
}
