using Dapper;
using OryxDomain.Utilities;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace OryxDomain.Repository
{
    public class DictionaryRepository : Repository
    {
        public DictionaryRepository(string path) : base(path)
        {
        }
        public async Task<int?> FindFieldLengthOryx(string field)
        {
            string command = @"SELECT DC1TAMANHO FROM DC1 WHERE DC1CAMPO = @field";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                int? length = await connection.QueryFirstOrDefaultAsync<int?>(command, new { field });

                return length;
            }
        }

        public async Task<IList<DC1>> FindDC1ByDc1arquivo(string dc1arquivo)
        {
            string command = @"SELECT * FROM DC1 WHERE DC1ARQUIVO = @dc1arquivo";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<DC1> lstDc1 = await connection.QueryAsync<DC1>(command, new { dc1arquivo });

                return lstDc1.ToList();
            }
        }

        public async Task<DC0> FindDC0ByDc0arquivo(string dc0arquivo)
        {
            string command = @"SELECT * FROM DC0 WHERE DC0ARQUIVO = @dc0arquivo";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                DC0 dc0 = await connection.QueryFirstOrDefaultAsync<DC0>(command, new { dc0arquivo });

                return dc0;
            }
        }

        public async Task<DC1> FindFieldByForeignKey(string dc1arquivo, string dc1arqpai)
        {

            string command = @"SELECT * FROM DC1 WHERE DC1ARQUIVO = @dc1arquivo AND DC1ARQPAI = @dc1arqpai";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                DC1 dc1 = await connection.QueryFirstOrDefaultAsync<DC1>(command, new { dc1arqpai, dc1arquivo });

                return dc1;
            }
        }

        public async Task<Int64> GetLastNumber(string field)
        {
            string command = @"SELECT DC1ULTIMO FROM DC1 WHERE DC1CAMPO = @field";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                string ultimo = await connection.QueryFirstOrDefaultAsync<string>(command, new { field });

                if (string.IsNullOrWhiteSpace(ultimo))
                {
                    return 0;
                }

                return Convert.ToInt64(ultimo);
            }
        }

        public async Task<int> SaveNextNumber(string field, string value, IDbTransaction transaction = null)
        {
            string command = @"UPDATE DC1 SET DC1ULTIMO = @value WHERE DC1CAMPO = @field";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    value,
                    field
                },
                transaction);

                return affectedRows;
            }
        }

        public async Task<DC1> FindDC1ByDc1campo(string dc1campo)
        {
            string command = @"SELECT * FROM DC1 WHERE UPPER(DC1CAMPO) = UPPER(@dc1campo)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                DC1 dc1 = await connection.QueryFirstOrDefaultAsync<DC1>(command, new { dc1campo });

                return dc1;
            }
        }

        public async Task<IList<DC0>> FindDC0ByLstDc0arquivo(IList<string> lstDc0arquivo)
        {
            string command = @"SELECT * FROM DC0 WHERE DC0ARQUIVO IN @lstDc0arquivo";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<DC0> lstDc0 = await connection.QueryAsync<DC0>(command, new { lstDc0arquivo });

                return lstDc0.ToList();
            }
        }

        public async Task<IList<DC1>> FindDC1ByLstDc1arquivo(IList<string> lstDc1arquivo)
        {
            string command = @"SELECT * FROM DC1 WHERE DC1ARQUIVO IN @lstDc1arquivo";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<DC1> lstDc1 = await connection.QueryAsync<DC1>(command, new { lstDc1arquivo });

                return lstDc1.ToList();
            }
        }

        public async Task<string> FindLasUsed(string dc1campo, string dc1arquivo)
        {
            string command = string.Format("SELECT MAX({0}) FROM {1}", dc1campo, dc1arquivo);

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                string lasUsed = await connection.QueryFirstOrDefaultAsync<string>(command);

                return lasUsed;
            }
        }
    }
}
