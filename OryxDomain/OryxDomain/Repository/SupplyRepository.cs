using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class SupplyRepository : Repository
    {
        public SupplyRepository(string path) : base(path)
        {
        }

        public async Task<IList<IN1>> FindList()
        {
            string command = @"SELECT * FROM IN1";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<IN1> lstIn1 = await connection.QueryAsync<IN1>(command);

                return lstIn1.ToList();
            }
        }

        public async Task<IN1> Find(string in1insumo)
        {
            string command = @"SELECT * FROM IN1 WHERE IN1INSUMO = @in1insumo";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IN1 in1 = await connection.QueryFirstOrDefaultAsync<IN1>(command, new { in1insumo });

                return in1;
            }
        }

        public async Task<int> Insert(IN1 in1)
        {
            string command = @"INSERT INTO IN1 (IN1INSUMO,IN1NOME,IN1FABR,IN1CODF,IN1UNMED,IN1CUSTO,IN1CREDICM,IN1CLASSE,IN1ALMOX,IN1ENDER,IN1CLASSIF,IN1IMAGEM,IN1PESO,IN1TITULO,IN1ESPEC,IN1INST1,IN1INST2,IN1PARAF,IN1VOLUME,IN1VARIA,IN1EAN,IN1APLIC,IN1ORIGEM,IN1TOTTRIB,IN1PESOCON,IN1UNIDCOM,IN1CONVCOM,IN1FCI,IN1LOTECOM,IN1LARG,IN1COMP)
                               VALUES (@In1insumo,@In1nome,@In1fabr,@in1codf,@In1unmed,@In1custo,@In1credicm,@In1classe,@In1almox,@In1ender,@in1classif,@In1imagem,@In1peso,@In1titulo,@In1espec,@In1inst1,@In1inst2,@In1paraf,@In1volume,@In1varia,@In1ean,@In1aplic,@In1origem,@In1tottrib,@In1pesocon,@In1unidcom,@In1convcom,@In1fci,@In1lotecom,@In1larg,@In1comp)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    in1.In1insumo,
                    in1.In1nome,
                    in1.In1fabr,
                    in1.In1codf,
                    in1.In1unmed,
                    in1.In1custo,
                    in1.In1credicm,
                    in1.In1classe,
                    in1.In1almox,
                    in1.In1ender,
                    in1.In1classif,
                    in1.In1imagem,
                    in1.In1peso,
                    in1.In1titulo,
                    in1.In1espec,
                    in1.In1inst1,
                    in1.In1inst2,
                    in1.In1paraf,
                    in1.In1volume,
                    in1.In1varia,
                    in1.In1ean,
                    in1.In1aplic,
                    in1.In1origem,
                    in1.In1tottrib,
                    in1.In1pesocon,
                    in1.In1unidcom,
                    in1.In1convcom,
                    in1.In1fci,
                    in1.In1lotecom,
                    in1.In1larg,
                    in1.In1comp
                });

                return affectedRows;
            }
        }

        public async Task<int> Delete(string in1insumo)
        {
            string command = @"DELETE FROM IN1 WHERE IN1INSUMO = @in1insumo";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { in1insumo });
            }
        }

        public async Task<int> Update(IN1 in1)
        {
            string command = @"UPDATE IN1
                               SET IN1INSUMO = @In1insumo,IN1NOME = @In1nome,IN1FABR = @In1fabr,IN1CODF = @in1codf,IN1UNMED = @In1unmed,IN1CUSTO = @In1custo,IN1CREDICM = @In1credicm,IN1CLASSE = @In1classe,IN1ALMOX = @In1almox,IN1ENDER = @In1ender,IN1CLASSIF = @in1classif,IN1IMAGEM = @In1imagem,IN1PESO = @In1peso,IN1TITULO = @In1titulo,IN1ESPEC = @In1espec,IN1INST1 = @In1inst1,IN1INST2 = @In1inst2,IN1PARAF = @In1paraf,IN1VOLUME = @In1volume,IN1VARIA = @In1varia,IN1EAN = @In1ean,IN1APLIC = @In1aplic,IN1ORIGEM = @In1origem,IN1TOTTRIB = @In1tottrib,IN1PESOCON = @In1pesocon,IN1UNIDCOM = @In1unidcom,IN1CONVCOM = @In1convcom,IN1FCI = @In1fci,IN1LOTECOM = @In1lotecom,IN1LARG = @In1larg,IN1COMP = @In1com
                               WHERE IN1INSUMO = @in1insumo";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    in1.In1insumo,
                    in1.In1nome,
                    in1.In1fabr,
                    in1.In1codf,
                    in1.In1unmed,
                    in1.In1custo,
                    in1.In1credicm,
                    in1.In1classe,
                    in1.In1almox,
                    in1.In1ender,
                    in1.In1classif,
                    in1.In1imagem,
                    in1.In1peso,
                    in1.In1titulo,
                    in1.In1espec,
                    in1.In1inst1,
                    in1.In1inst2,
                    in1.In1paraf,
                    in1.In1volume,
                    in1.In1varia,
                    in1.In1ean,
                    in1.In1aplic,
                    in1.In1origem,
                    in1.In1tottrib,
                    in1.In1pesocon,
                    in1.In1unidcom,
                    in1.In1convcom,
                    in1.In1fci,
                    in1.In1lotecom,
                    in1.In1larg,
                    in1.In1comp
                });

                return affectedRows;
            }
        }

        public async Task<IList<IN1>> Search(string search, int limit, int offset)
        {
            search = string.Format("%{0}%", search);
            search = search.Replace(" ", "%");
            string command = @"SELECT *
                               FROM IN1
                               WHERE IN1INSUMO LIKE @search
                                  OR IN1NOME LIKE @search
                               LIMIT @limit OFFSET @offset";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<IN1> supplies = await connection.QueryAsync<IN1>(command, new { search, limit, offset });

                return supplies.ToList();
            }
        }

        #region
        public async Task<OriginModel> FindSpecificTaxes(string in1insumo)
        {
            string command = @"SELECT IN1ORIGEM AS ORIGIN, IN1TOTTRIB AS TOTTRIB FROM IN1 WHERE IN1INSUMO = @in1insumo";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                OriginModel in1 = await connection.QueryFirstOrDefaultAsync<OriginModel>(command, new { in1insumo });
                return in1;
            }
        }
        #endregion
    }
}
