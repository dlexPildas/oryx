using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class SalesConfirmationItemsRepository : Repository
    {
        public SalesConfirmationItemsRepository(string path) : base(path)
        {
        }

        #region VDF
        public async Task<int> Insert(VDF vdf, IDbTransaction transaction = null)
        {
            string command = @"INSERT INTO VDF (VDFDOC,VDFPECA,VDFLEITURA,VDFVOLUME)
                               VALUES (@vdfdoc, @vdfpeca, @vdfleitura,@vdfvolume)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    vdf.Vdfdoc,
                    vdf.Vdfpeca,
                    vdf.Vdfleitura,
                    vdf.Vdfvolume
                },
                transaction);

                return affectedRows;
            }
        }

        public async Task<IList<VDF>> FindAllVDF(string vdfdoc)
        {
            string command = @"SELECT VDF.*
                                    , OF3PRODUTO
                                    , OF3OPCAO
                                    , OF3TAMANHO
                                    , OF3RFID
                                    , CR1NOME 
                                 FROM VDF 
                                INNER JOIN OF3 ON OF3PECA = VDFPECA 
                                 LEFT JOIN PR2 ON PR2PRODUTO = OF3PRODUTO AND PR2OPCAO = OF3OPCAO 
                                 LEFT JOIN CR1 ON CR1COR = PR2COR 
                                WHERE VDFDOC = @vdfdoc";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<VDF> lstVdf = await connection.QueryAsync<VDF>(command, new { vdfdoc });

                return lstVdf.ToList();
            }
        }

        public async Task<int> Update(VDF vdf)
        {
            string command = @"UPDATE VDF
                                  SET VDFPECA = @vdfpeca, VDFLEITURA = @vdfleitura, VDFVOLUME = @vdfvolume
                                WHERE VDFDOC = @vdfdoc";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    vdf.Vdfdoc,
                    vdf.Vdfpeca,
                    vdf.Vdfleitura,
                    vdf.Vdfvolume
                });

                return affectedRows;
            }
        }

        public async Task<int> DeleteVdf(string vdfdoc)
        {
            string command = @"DELETE FROM VDF WHERE VDFDOC = @vdfdoc";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { vdfdoc });
            }
        }

        public async Task<int> DeleteVdfByPiece(string vdlpeca)
        {
            string command = @"DELETE FROM VDF WHERE VDFPECA = @vdlpeca";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { vdlpeca });
            }
        }

        public async Task<IList<VDF>> FindAllVDFForCustomerHistory(string vdecliente, DateTime since, DateTime until)
        {
            string command = @"SELECT VDF.*
                                 FROM VDF 
                                INNER JOIN VDE ON VDEDOC = VDFDOC
                                WHERE VDECLIENTE = @vdecliente
                                  AND VDFLEITURA BETWEEN @since AND @until";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<VDF> lstVdf = await connection.QueryAsync<VDF>(command, new { vdecliente, since, until });

                return lstVdf.ToList();
            }
        }
        #endregion

        #region VDZ
        public async Task<int> Insert(VDZ vdz, IDbTransaction transaction = null)
        {
            string command = @"INSERT INTO VDZ (VDZDOC,VDZPECA,VDZLEITURA,VDZQTDE,VDZPRODUTO,VDZOPCAO,VDZTAMANHO,VDZVOLUME)
                               VALUES (@vdzdoc,@vdzpeca,@vdzleitura,@vdzqtde,@vdzproduto,@vdzopcao,@vdztamanho,@vdzvolume)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    vdz.Vdzdoc,
                    vdz.Vdzpeca,
                    vdz.Vdzleitura,
                    vdz.Vdzqtde,
                    vdz.Vdzproduto,
                    vdz.Vdzopcao,
                    vdz.Vdztamanho,
                    vdz.Vdzvolume
                },
                transaction);

                return affectedRows;
            }
        }

        public async Task<string> GetNextNumber()
        {
            string command = @"SELECT VDEDOC FROM VDE ORDER BY VDEDOC DESC LIMIT 1";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                string vdedoc = await connection.QueryFirstOrDefaultAsync<string>(command);

                return vdedoc;
            }
        }

        public async Task<IList<VDZ>> FindAllVDZ(string vdzdoc)
        {
            string command = @"SELECT VDZ.*
                                    , CR1.CR1NOME 
                                 FROM VDZ 
                                 LEFT JOIN PR2 ON PR2PRODUTO = VDZPRODUTO AND PR2OPCAO = VDZOPCAO 
                                 LEFT JOIN CR1 ON CR1COR = PR2COR 
                                WHERE VDZDOC = @vdzdoc";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<VDZ> lstVdf = await connection.QueryAsync<VDZ>(command, new { vdzdoc });

                return lstVdf.ToList();
            }
        }

        public async Task<int> Update(VDZ vdz)
        {
            string command = @"UPDATE VDZ
                                  SET VDZPECA = @vdzpeca,
                                      VDZLEITURA = @vdzleitura,
                                      VDZQTDE = @vdzqtde,
                                      VDZPRODUTO = @vdzproduto,
                                      VDZOPCAO = @vdzopcao,
                                      VDZTAMANHO = @vdztamanho,
                                      VDZVOLUME = @Vdzvolume
                                WHERE VDZDOC = @vdzdoc";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    vdz.Vdzdoc,
                    vdz.Vdzpeca,
                    vdz.Vdzleitura,
                    vdz.Vdzqtde,
                    vdz.Vdzproduto,
                    vdz.Vdzopcao,
                    vdz.Vdztamanho,
                    vdz.Vdzvolume
                });

                return affectedRows;
            }
        }

        public async Task<int> DeleteVdz(string vdzdoc)
        {
            string command = @"DELETE FROM VDZ WHERE VDZDOC = @vdzdoc";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { vdzdoc });
            }
        }

        public async Task<int> DeleteVdzByPiece(string Vdedoc, string vdzpeca, string vdzproduto, string vdzopcao, string vdztamanho, decimal vdzqtde)
        {
            string command = @"UPDATE VDZ
                                  SET VDZQTDE = VDZQTDE - @vdzqtde
                                WHERE VDZDOC = @Vdedoc
                                  AND VDZPECA = @vdzpeca
                                  AND VDZPRODUTO = @vdzproduto
                                  AND VDZOPCAO = @vdzopcao
                                  AND VDZTAMANHO = @vdztamanho
                                  AND VDZQTDE > 0";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command, new { Vdedoc, vdzpeca, vdzproduto, vdzopcao, vdztamanho, vdzqtde });

                return affectedRows;
            }
        }

        public async Task<int> ClearZeroLines(string vdedoc)
        {
            string command = @"DELETE FROM VDZ WHERE VDZDOC = @Vdedoc AND VDZQTDE = 0";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command, new { vdedoc });

                return affectedRows;
            }
        }
        #endregion
    }
}
