using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class PaymentConditionRepository : Repository
    {
        public PaymentConditionRepository(string path) : base(path)
        {
        }

        public async Task<VD4> Find(string vd4conpgto)
        {
            string command = @"SELECT *
                                 FROM VD4
                                WHERE VD4CONPGTO = @vd4conpgto";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    VD4 vd4 = await connection.QueryFirstOrDefaultAsync<VD4>(command, new { vd4conpgto });

                    return vd4;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IList<VD4>> FindAll()
        {
            string command = @"SELECT * FROM VD4 WHERE VD4FORACAT = 0";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    IEnumerable<VD4> lstVd4 = await connection.QueryAsync<VD4>(command);

                    return lstVd4.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IList<VD9>> FindVd9List(string vd9conpgto)
        {
            string command = @"SELECT * FROM VD9 WHERE VD9CONPGTO = @vd9conpgto";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    IEnumerable<VD9> lstVd9 = await connection.QueryAsync<VD9>(command, new { vd9conpgto });

                    return lstVd9.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IList<LastPaymentConditionModel>> FindLastByCustomer(string cf1cliente)
        {
            string command1 = @"SELECT CONCAT(CV5DOC,CV5TIPO,CV5EMISSOR) FROM CV5 WHERE CV5CLIENTE = @cf1cliente AND CV5ENTSAI = 1 AND CV5EMITIDO = 1 AND CV5SITUA = '' AND EXISTS (SELECT 1 FROM CV8 WHERE CV8DOC = CV5DOC and CV5TIPO = CV8TIPO and CV8EMISSOR = CV5EMISSOR) ORDER BY CV5EMISSAO DESC LIMIT 1";
            string command2 = @"SELECT CV8.CV8TIPOTIT, CV2NOME, COUNT(*) AS Times
                                 FROM CV8
                                INNER JOIN CV2 ON CV2TITULO = CV8TIPOTIT
                                WHERE CONCAT(CV8DOC,CV8TIPO,CV8EMISSOR) = @maxdoc
                                GROUP BY CV8TIPOTIT";
            
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                string maxdoc = await connection.QueryFirstOrDefaultAsync<string>(command1, new { cf1cliente });
                if (string.IsNullOrWhiteSpace(maxdoc))
                    return null;
                    
                IEnumerable<LastPaymentConditionModel> lstLastConditions = await connection.QueryAsync<LastPaymentConditionModel>(command2, new { maxdoc });

                return lstLastConditions.ToList();
            }
        }
    }
}
