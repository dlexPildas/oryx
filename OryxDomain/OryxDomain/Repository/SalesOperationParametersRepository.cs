using Dapper;
using OryxDomain.Utilities;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Enums;
using OryxDomain.Models.Oryx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class SalesOperationParametersRepository : Repository
    {
        public SalesOperationParametersRepository(string path) : base(path)
        {
        }

        public async Task<IList<PD2>> FindAll()
        {
            string command = @"SELECT * FROM PD2";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<PD2> lstPd2 = await connection.QueryAsync<PD2>(command);

                return lstPd2.ToList();
            }
        }

        public async Task<PD2> Find(int pd2codigo)
        {
            string command = @"SELECT * FROM PD2 WHERE PD2CODIGO = @pd2codigo";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                PD2 pd2 = await connection.QueryFirstOrDefaultAsync<PD2>(command, new { pd2codigo });

                return pd2;
            }
        }

        public async Task<string> FindOperCom(SalesOperationType pd2tipoope, string pd2tipo, OperationType pd2estadua, bool pd2contrib, bool pd2emispro)
        {
            string command = @"SELECT PD2OPERCOM 
                                 FROM PD2
                                WHERE PD2TIPOOPE = @pd2tipoope
                                  AND PD2TIPO = @pd2tipo
                                  AND PD2ESTADUA = @pd2estadua 
                                  AND PD2CONTRIB = @pd2contrib
                                  AND PD2EMISPRO = @pd2emispro";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                string pd2opercom = await connection.QueryFirstOrDefaultAsync<string>(command, new { pd2tipoope, pd2tipo, pd2estadua, pd2contrib, pd2emispro });

                return pd2opercom;
            }
        }

        public async Task<string> FindOperCom(SalesOperationType pd2tipoope, string pd2tipo, OperationType pd2estadua, bool pd2contrib, bool pd2emispro, string cv4estado)
        {
            string command = @"SELECT PD2OPERCOM 
                                 FROM PD2
                                INNER JOIN CV4 ON CV4OPERCOM = PD2OPERCOM
                                WHERE PD2TIPOOPE = @pd2tipoope
                                  AND PD2TIPO = @pd2tipo
                                  AND PD2ESTADUA = @pd2estadua 
                                  AND PD2CONTRIB = @pd2contrib
                                  AND PD2EMISPRO = @pd2emispro
                                  AND CV4ESTADO = @cv4estado";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                string pd2opercom = await connection.QueryFirstOrDefaultAsync<string>(command, new { pd2tipoope, pd2tipo, pd2estadua, pd2contrib, pd2emispro, cv4estado });

                return pd2opercom;
            }
        }

        public async Task<string> FindOperCom(SalesOperationType pd2tipoope, string pd2tipo, OperationType pd2estadua, bool pd2contrib, bool pd2emispro, int pd2codigo)
        {
            string command = @"SELECT PD2OPERCOM FROM PD2 WHERE PD2TIPOOPE = @pd2tipoope AND PD2TIPO = @pd2tipo AND PD2ESTADUA = @pd2estadua AND PD2CONTRIB = @pd2contrib AND PD2CODIGO <> @pd2codigo AND PD2EMISPRO = @pd2emispro";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                string pd2opercom = await connection.QueryFirstOrDefaultAsync<string>(command, new { pd2tipoope, pd2tipo, pd2estadua, pd2contrib, pd2codigo, pd2emispro });

                return pd2opercom;
            }
        }

        public async Task<int> Update(PD2 pd2)
        {
            string command = @"UPDATE PD2
                               SET PD2TIPOOPE = @pd2tipoope, PD2TIPO = @pd2tipo, PD2ESTADUA = @pd2estadua, PD2CONTRIB = @pd2contrib, PD2OPERCOM = @pd2opercom, PD2EMISPRO = @pd2emispro
                               WHERE PD2CODIGO = @pd2codigo";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    pd2codigo = pd2.Pd2codigo,
                    pd2tipoope = pd2.Pd2tipoope,
                    pd2tipo = pd2.Pd2tipo,
                    pd2estadua = pd2.Pd2estadua,
                    pd2contrib = pd2.Pd2contrib,
                    pd2opercom = pd2.Pd2opercom,
                    pd2emispro = pd2.Pd2emispro
                });

                return affectedRows;
            }
        }

        public async Task<IList<PD2>> Search(
            int? pd2codigo,
            SalesOperationType? pd2tipoope,
            string pd2tipo,
            OperationType? pd2estadua,
            bool? pd2contrib,
            bool? pd2emispro,
            string pd2opercom,
            int limit,
            int offset)
        {
            string command = @"SELECT PD2.*, CV1.CV1NOME, CV3.CV3NOME
                                 FROM PD2, CV1, CV3
                                WHERE CV1.CV1DOCFIS = PD2.PD2TIPO
                                  AND CV3.CV3OPERCOM = PD2.PD2OPERCOM";
            
            if (pd2codigo.HasValue)
            {
                command += string.Format(" AND PD2CODIGO = {0}", pd2codigo.Value);
            }
            if (pd2tipoope.HasValue)
            {
                command += string.Format(" AND PD2TIPOOPE = {0}", (int)pd2tipoope.Value);
            }
            if (pd2tipo != null)
            {
                command += string.Format(" AND PD2TIPO = '{0}'", pd2tipo);
            }
            if (pd2estadua.HasValue)
            {
                command += string.Format(" AND PD2ESTADUA = {0}", (int)pd2estadua.Value);
            }
            if (pd2contrib.HasValue)
            {
                command += string.Format(" AND PD2CONTRIB = {0}", pd2contrib.Value);
            }
            if (pd2emispro.HasValue)
            {
                command += string.Format(" AND PD2EMISPRO = {0}", pd2emispro.Value);
            }
            if (pd2opercom != null)
            {
                command += string.Format(" AND PD2OPERCOM = {0}", pd2opercom);
            }

            command += string.Format(" LIMIT {0} OFFSET {1}", limit, offset);
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<PD2> lstPd2 = await connection.QueryAsync<PD2>(command);

                return lstPd2.ToList();
            }
        }

        public async Task<int> Insert(PD2 pd2)
        {
            string command = @"INSERT INTO PD2 (PD2TIPOOPE,PD2TIPO,PD2ESTADUA,PD2CONTRIB,PD2OPERCOM,PD2EMISPRO)
                               VALUES (@pd2tipoope, @pd2tipo, @pd2estadua,@pd2contrib,@pd2opercom,@pd2emispro)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    pd2codigo = pd2.Pd2codigo,
                    pd2tipoope = pd2.Pd2tipoope,
                    pd2tipo = pd2.Pd2tipo,
                    pd2estadua = pd2.Pd2estadua,
                    pd2contrib = pd2.Pd2contrib,
                    pd2opercom = pd2.Pd2opercom,
                    pd2emispro = pd2.Pd2emispro
                });

                return affectedRows;
            }
        }

        public async Task<int> Delete(int pd2codigo)
        {
            string command = @"DELETE FROM PD2 WHERE PD2CODIGO = @pd2codigo";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { pd2codigo });
            }
        }

        public async Task<IList<PD2>> FindAllBySalesOperationType(SalesOperationType returnType)
        {
            string command = @"SELECT * FROM PD2 WHERE PD2TIPOOPE = @returnType";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<PD2> lstPd2 = await connection.QueryAsync<PD2>(command, new { returnType });

                return lstPd2.ToList();
            }
        }
    }
}
