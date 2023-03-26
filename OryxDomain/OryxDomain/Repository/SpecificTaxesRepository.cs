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
    public class SpecificTaxesRepository : Repository
    {
        public SpecificTaxesRepository(string path) : base(path)
        {
        }

        public async Task<IList<CVN>> FindList(string cvnproduto)
        {
            string command = @"SELECT * FROM CVN 
                               WHERE CVNPRODUTO = @cvnproduto";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<CVN> lstCvn = await connection.QueryAsync<CVN>(command, new { cvnproduto });

                return lstCvn.ToList();
            }
        }

        public async Task<CVN> Find(string cvnopercom, string cvnproduto = "", string cvninsumo = "")
        {
            string command = @"SELECT * FROM CVN 
                               WHERE CVNOPERCOM = @cvnopercom 
                               AND CVNPRODUTO = @cvnproduto 
                               AND CVNINSUMO = @cvninsumo";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CVN cvn = await connection.QueryFirstOrDefaultAsync<CVN>(command, new { cvnopercom, cvnproduto, cvninsumo });

                return cvn;
            }
        }

        public async Task<CVN> FindByContad(CVN cvn)
        {
            string command = @"SELECT * FROM CVN 
                               WHERE CVNOPERCOM = @cvnopercom 
                               AND CVNCONTAD <> @cvncontad 
                               AND (CVNPRODUTO <> @cvnproduto OR CVNINSUMO <> @cvninsumo)";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CVN findedcvn = await connection.QueryFirstOrDefaultAsync<CVN>(command, new
                {
                    cvnopercom = cvn.Cvnopercom,
                    cvnproduto = cvn.Cvnproduto,
                    cvninsumo = cvn.Cvninsumo,
                    cvncontad = cvn.Cvncontad
                });

                return findedcvn;
            }
        }

        public async Task<CVN> FindByContac(CVN cvn)
        {
            string command = @"SELECT* FROM 
                               CVN WHERE CVNOPERCOM = @cvnopercom 
                               AND CVNCONTAC<> @cvncontac 
                               AND (CVNPRODUTO <> @cvnproduto 
                               OR CVNINSUMO <> @cvninsumo)";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CVN findedcvn = await connection.QueryFirstOrDefaultAsync<CVN>(command, new
                {
                    cvnopercom = cvn.Cvnopercom,
                    cvnproduto = cvn.Cvnproduto,
                    cvncontac = cvn.Cvncontac,
                    cvninsumo = cvn.Cvninsumo
                });

                return findedcvn;
            }
        }

        public async Task<int> Insert(CVN cvn)
        {
            string command = @"INSERT INTO CVN 
                               (CVNPRODUTO,
                               CVNOPERCOM,
                               CVNCFOP,
                               CVNCST,
                               CVNBASEICM,
                               CVNALIQICM,
                               CVNBASEISE,
                               CVNBASEOUT,
                               CVNBASESUB,
                               CVNALIQSUB,
                               CVNBASEIPI,
                               CVNALIQIPI,
                               CVNCSTIPI,
                               CVNORIGEM,
                               CVNINSUMO,
                               CVNCONTAC,
                               CVNCONTAD,
                               CVNDESCFAT,
                               CVNCSTPIS,
                               CVNCSTCOF,
                               CVNBASEPIS,
                               CVNBASECOF,
                               CVNALIQPIS,
                               CVNALIQCOF,
                               CVNCBENEF,
                               CVNINFADIC,
                               CVNPDIF)
                               VALUES 
                               (@Cvnproduto,
                               @Cvnopercom,
                               @Cvncfop,
                               @Cvncst,
                               @Cvnbaseicm,
                               @Cvnaliqicm,
                               @Cvnbaseise,
                               @Cvnbaseout,
                               @Cvnbasesub,
                               @Cvnaliqsub,
                               @Cvnbaseipi,
                               @Cvnaliqipi,
                               @Cvncstipi,
                               @Cvnorigem,
                               @Cvninsumo,
                               @Cvncontac,
                               @Cvncontad,
                               @Cvndescfat,
                               @Cvncstpis,
                               @Cvncstcof,
                               @Cvnbasepis,
                               @Cvnbasecof,
                               @Cvnaliqpis,
                               @Cvnaliqcof,
                               @Cvncbenef,
                               @cvninfadic,
                               @cvnpdif)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    cvn.Cvnproduto,
                    cvn.Cvnopercom,
                    cvn.Cvncfop,
                    cvn.Cvncst,
                    cvn.Cvnbaseicm,
                    cvn.Cvnaliqicm,
                    cvn.Cvnbaseise,
                    cvn.Cvnbaseout,
                    cvn.Cvnbasesub,
                    cvn.Cvnaliqsub,
                    cvn.Cvnbaseipi,
                    cvn.Cvnaliqipi,
                    cvn.Cvncstipi,
                    cvn.Cvnorigem,
                    cvn.Cvninsumo,
                    cvn.Cvncontac,
                    cvn.Cvncontad,
                    cvn.Cvndescfat,
                    cvn.Cvncstpis,
                    cvn.Cvncstcof,
                    cvn.Cvnbasepis,
                    cvn.Cvnbasecof,
                    cvn.Cvnaliqpis,
                    cvn.Cvnaliqcof,
                    cvn.Cvncbenef,
                    cvn.Cvninfadic,
                    cvn.Cvnpdif
                });

                return affectedRows;
            }
        }

        public async Task<IList<CVN>> Search(string search, int limit, int offset)
        {
            search = string.Format("%{0}%", search);
            search = search.Replace(" ", "%");
            string command = @"SELECT *
                               FROM CVN
                               WHERE CVNPRODUTO LIKE @search
                               OR CVNOPERCOM LIKE @search
                               OR CVNINSUMO LIKE @search
                               LIMIT @limit OFFSET @offset";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<CVN> lstcvns = await connection.QueryAsync<CVN>(command, new { search, limit, offset });

                return lstcvns.ToList();
            }
        }

        public async Task<int> Delete(string cvnopercom, string cvnproduto = "", string cvninsumo = "")
        {
            string command = @"DELETE FROM CVN 
                               WHERE CVNOPERCOM = @cvnopercom 
                               AND CVNPRODUTO = @cvnproduto 
                               AND CVNINSUMO = @cvninsumo";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { cvnopercom, cvnproduto, cvninsumo });
            }
        }

        public async Task<int> Update(CVN cvn)
        {
            string command = @"UPDATE CVN
                               SET CVNCFOP = @Cvncfop,CVNCST = @Cvncst,CVNBASEICM = @Cvnbaseicm,CVNALIQICM = @Cvnaliqicm,CVNBASEISE = @Cvnbaseise,CVNBASEOUT = @Cvnbaseout,CVNBASESUB = @Cvnbasesub,CVNALIQSUB = @Cvnaliqsub,CVNBASEIPI = @Cvnbaseipi,CVNALIQIPI = @Cvnaliqipi,CVNCSTIPI = @Cvncstipi,CVNORIGEM = @Cvnorigem,CVNCONTAC = @Cvncontac,CVNCONTAD = @Cvncontad,CVNDESCFAT = @Cvndescfat,CVNCSTPIS = @Cvncstpis,CVNCSTCOF = @Cvncstcof,CVNBASEPIS = @Cvnbasepis,CVNBASECOF = @Cvnbasecof,CVNALIQPIS = @Cvnaliqpis,CVNALIQCOF = @Cvnaliqcof,CVNCBENEF = @Cvncbenef, CVNINFADIC = @cvninfadic, CVNPDIF = @cvnpdif
                               WHERE CVNOPERCOM = @cvnopercom AND CVNPRODUTO = @cvnproduto AND CVNINSUMO = @cvninsumo";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    cvn.Cvnproduto,
                    cvn.Cvnopercom,
                    cvn.Cvncfop,
                    cvn.Cvncst,
                    cvn.Cvnbaseicm,
                    cvn.Cvnaliqicm,
                    cvn.Cvnbaseise,
                    cvn.Cvnbaseout,
                    cvn.Cvnbasesub,
                    cvn.Cvnaliqsub,
                    cvn.Cvnbaseipi,
                    cvn.Cvnaliqipi,
                    cvn.Cvncstipi,
                    cvn.Cvnorigem,
                    cvn.Cvninsumo,
                    cvn.Cvncontac,
                    cvn.Cvncontad,
                    cvn.Cvndescfat,
                    cvn.Cvncstpis,
                    cvn.Cvncstcof,
                    cvn.Cvnbasepis,
                    cvn.Cvnbasecof,
                    cvn.Cvnaliqpis,
                    cvn.Cvnaliqcof,
                    cvn.Cvncbenef,
                    cvn.Cvninfadic,
                    cvn.Cvnpdif
                });

                return affectedRows;
            }
        }

        public async Task<IList<GenericItemModel>> GetGenericProductsList(string cvnopercom)
        {

            string command = @"SELECT PR0PRODUTO, 
                               PR0DESC 
                               FROM PR0 
                               WHERE PR0PRODUTO 
                               NOT IN (SELECT CVNPRODUTO FROM CVN WHERE CVNOPERCOM = @cvnopercom 
                               ORDER BY CVNPRODUTO)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                IEnumerable<PR0> pr0lst = await connection.QueryAsync<PR0>(command, new { cvnopercom });

                IList<GenericItemModel> genericitemlst = pr0lst.Select(pr0 => new GenericItemModel
                {
                    Code = pr0.Pr0produto,
                    Description = pr0.Pr0desc
                }).ToList();

                return genericitemlst;
            }
        }

        public async Task<IList<GenericItemModel>> GetGenericSupplyList(string cvnopercom)
        {

            string command = @"SELECT IN1INSUMO,
                               IN1NOME FROM IN1 
                               WHERE IN1INSUMO 
                               NOT IN (SELECT IN1INSUMO FROM CVN 
                               WHERE CVNOPERCOM = @cvnopercom 
                               ORDER BY IN1INSUMO)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();
                IEnumerable<IN1> in1lst = await connection.QueryAsync<IN1>(command, new { cvnopercom });

                IList<GenericItemModel> genericitemlst = in1lst.Select(in1 => new GenericItemModel
                {
                    Code = in1.In1insumo,
                    Description = in1.In1nome
                }).ToList();

                return genericitemlst;
            }
        }

      
    }
}
