using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class PurchaseCreditRepository : Repository
    {
        public PurchaseCreditRepository(string path) : base(path)
        {

        }

        public async Task<List<FNL>> FindAll()
        {
            string command = @"SELECT * FROM FNL";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                IEnumerable<FNL> fnls = await connection.QueryAsync<FNL>(command);
                return fnls.ToList();
            }
        }

        public async Task<FNL> Find(string fnlemissor, string fnltipo, string fnldoc, string fnlcliente)
        {
            string command = @"SELECT *
                                 FROM FNL
                                WHERE FNLCLIENTE = @fnlcliente
                                  AND FNLDOC = @fnldoc
                                  AND FNLTIPO = @fnltipo
                                  AND FNLEMISSOR = @fnlemissor";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                FNL fnl = await connection.QueryFirstOrDefaultAsync<FNL>(command, new { fnlemissor, fnltipo, fnldoc, fnlcliente });
                return fnl;
            }
        }

        public async Task<IList<FNL>> FindAll(string fnlcliente)
        {
            string command = @"SELECT FNL.*, CF1.Cf1nome
                                 FROM FNL
                                LEFT JOIN CF1 ON CF1CLIENTE = FNLCLIENTE
                                WHERE FNLCLIENTE = @fnlcliente ORDER BY FNLDATACRE";
                            
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<FNL> lstFnl = await connection.QueryAsync<FNL>(command, new { fnlcliente });
                return lstFnl.ToList();
            }
        }

        public async Task<int> Delete(string fnlemissor, string fnltipo, string fnldoc, string fnlcliente)
        {
            string command = @"DELETE FROM FNL WHERE FNLCLIENTE = @fnlcliente AND FNLDOC = @fnldoc AND FNLEMISSOR = @fnlemissor AND FNLTIPO = @fnltipo";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                int effectedRows = await connection.ExecuteAsync(command, new { fnlemissor, fnltipo, fnldoc, fnlcliente });
                return effectedRows;
            }
        }

        public async Task<int> Insert(FNL fnl)
        {
            string command = @"INSERT INTO FNL(FNLCLIENTE, FNLCREDITO, FNLDATACRE, FNLDOC, FNLTIPO, FNLEMISSOR)
		                                VALUES (@Fnlcliente, @Fnlcredito, @Fnldatacre, @Fnldoc, @Fnltipo, @Fnlemissor)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    fnl.Fnlcliente,
                    fnl.Fnlcredito,
                    fnl.Fnldatacre,
                    fnl.Fnldoc, 
                    fnl.Fnltipo,
                    fnl.Fnlemissor
                });

                return affectedRows;
            }
        }

        public async Task<int> Update(FNL fnl)
        {
            string command = @"UPDATE FNL SET FNLCREDITO = @FnlCredito, FNLDATACRE = @FnlDatacre WHERE FNLCLIENTE = @Fnlcliente AND FNLDOC = @Fnldoc AND FNLTIPO = @fnltipo AND FNLEMISSOR = @Fnlemissor";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    fnl.Fnlcliente,
                    fnl.Fnlcredito,
                    fnl.Fnldatacre,
                    fnl.Fnldoc,
                    fnl.Fnltipo,
                    fnl.Fnlemissor
                });

                return affectedRows;
            }
        }

        public async Task<IList<FNL>> Search(string search, int limit, int offset, string orderBy)
        {
            search = string.Format("%{0}%", search);
            search = search.Replace(" ", "%");
            string command = @"SELECT FNL.*, CF1.Cf1nome
                                 FROM FNL
                                INNER JOIN CF1 ON CF1CLIENTE = FNLCLIENTE
                                WHERE FNLCLIENTE LIKE @search
                                   OR FNLDOC LIKE @search
                                   OR CF1NOME LIKE @search
                                ORDER BY CF1NOME " + orderBy + " LIMIT @limit OFFSET @offset";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                IEnumerable<FNL> terminals = await connection.QueryAsync<FNL>(command, new { search, limit, offset });
                return terminals.ToList();
            }
        }

        public async Task<FNL> Find(string fnlemissor, string fnlcliente, string vdkdoc)
        {
            string command = @"SELECT *
                                 FROM FNL
                                 LEFT JOIN CV5 ON CV5DOC = FNLDOC AND CV5TIPO = FNLTIPO AND CV5EMISSOR = FNLEMISSOR
                                WHERE FNLCLIENTE = @fnlcliente
                                  AND FNLEMISSOR = @fnlemissor
                                  AND CV5DOCDEV = @vdkdoc";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                FNL fnl = await connection.QueryFirstOrDefaultAsync<FNL>(command, new { fnlemissor, fnlcliente, vdkdoc });
                return fnl;
            }
        }

        public async Task<FNL> FindLastByCustomer(string vd1cliente)
        {
            string command = @"SELECT *
                                 FROM FNL
                                WHERE FNLCLIENTE = @vd1cliente
                                ORDER BY FNLDATACRE DESC
                                LIMIT 1";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                FNL fnl = await connection.QueryFirstOrDefaultAsync<FNL>(command, new { vd1cliente });
                return fnl;
            }
        }


        public async Task<IList<CV5>> FindFiscalDocuments(string fnlcliente, int limit, int offset)
        {
            string command = @"SELECT DISTINCT CV5DOC, CV5EMISSOR, CV5TIPO, CV5EMISSAO, CV5TOTALNF
                                 FROM CV5
                                WHERE (CV5CLIENTE = @fnlcliente
                                   OR CV5EMISSOR = @fnlcliente)
                                  AND TRIM(CV5SITUA) = ''
                                  AND CV5EMITIDO = 1
                                  AND NOT EXISTS (SELECT 1 FROM FNL WHERE FNLDOC = CV5DOC AND FNLTIPO = CV5TIPO AND FNLEMISSOR = CV5EMISSOR)
                                ORDER BY CV5EMISSAO DESC
                                LIMIT @limit OFFSET @offset";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                IEnumerable<CV5> terminals = await connection.QueryAsync<CV5>(command, new { fnlcliente, limit, offset });
                return terminals.ToList();
            }
        }
    }
}
