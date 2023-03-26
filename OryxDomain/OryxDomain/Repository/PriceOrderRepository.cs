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
    public class PriceOrderRepository : Repository
    {
        public PriceOrderRepository(string path) : base(path)
        {
        }

        public async Task<VD5> Find(string vd5pedido, string vd5produto, string vd5tamanho, string vd5opcao)
        {
            string command = @"SELECT * FROM VD5
                                WHERE VD5PEDIDO = @vd5pedido
                                AND VD5PRODUTO = @vd5produto
                                AND VD5TAMANHO = @vd5tamanho
                                AND VD5OPCAO = @vd5opcao";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                VD5 vd5 = await connection.QueryFirstOrDefaultAsync<VD5>(command, new
                {
                    vd5pedido,
                    vd5produto,
                    vd5tamanho,
                    vd5opcao
                });

                return vd5;
            }
        }

        public async Task<List<VD5>> FindBySize(string vd5produto, string vd5tamanho)
        {
            string command = @"SELECT * FROM VD5
                                WHERE VD5PRODUTO = @vd5produto
                                AND VD5TAMANHO = @vd5tamanho";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<VD5> vd5 = await connection.QueryAsync<VD5>(command, new
                {
                    vd5produto,
                    vd5tamanho
                });

                return vd5.ToList();
            }
        }

        public async Task<int> Delete(string vd5produto, string vd5tamanho)
        {
            string command = @"DELETE FROM VD5
                                WHERE VD5PRODUTO = @vd5produto
                                AND VD5TAMANHO = @vd5tamanho";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { vd5produto, vd5tamanho });
            }
        }
    }
}
