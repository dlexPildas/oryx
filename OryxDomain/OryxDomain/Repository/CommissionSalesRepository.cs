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
    public class CommissionSalesRepository : Repository
    {
        public CommissionSalesRepository(string path) : base(path)
        {
        }

        public async Task<IList<CO2>> FindByCommissionClosing(string co2fecha)
        {
            string command = @"SELECT CO2.*
                                    , CV5EMISSAO
                                    , SUM(CV8VALOR) AS CV5TOTALNF
                                    , CV5COMIS
                                    , SUM(CV8VALOR) * (CV5COMIS/100) AS CommissionValue
                                    , CV5CLINOME
                                 FROM CO2
                                INNER JOIN CV5 ON CV5DOC = CO2DOC AND CV5TIPO = CO2TIPO AND CV5EMISSOR = CO2EMISSOR
                                INNER JOIN CV8 ON CV8DOC = CV5DOC AND CV8EMISSOR = CV5EMISSOR AND CV8TIPO = CV5TIPO
                                WHERE CO2FECHA = @co2fecha
                                GROUP BY CV5DOC, CV5TIPO, CV5EMISSOR
                                ORDER BY CV5EMISSAO";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<CO2> lstCo2 = await connection.QueryAsync<CO2>(command, new { co2fecha });

                return lstCo2.ToList();
            }
        }

        public async Task<int> Insert(CO2 co2)
        {
            string command = @"INSERT INTO CO2 (CO2FECHA, CO2DOC, CO2TIPO, CO2EMISSOR)
                               VALUES (@co2fecha,@co2doc,@co2tipo,@co2emissor)";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    co2.Co2fecha,
                    co2.Co2doc,
                    co2.Co2tipo,
                    co2.Co2emissor
                });

                return affectedRows;
            }
        }

        public async Task<IList<CO2>> FindCommissionSales(string co1repres, DateTime co1inicio, DateTime co1fim)
        {
            string command = @"SELECT CV5DOC AS Co2doc
                                    , CV5TIPO AS Co2tipo
                                    , CV5EMISSOR AS Co2emissor
                                    , CV5EMISSAO
                                    , SUM(CV8VALOR) AS CV5TOTALNF
                                    , CV5COMIS
                                    , SUM(CV8VALOR) * (CV5COMIS/100) AS CommissionValue
                                    , CV5CLINOME
                                 FROM CV5
                                INNER JOIN CV8 ON CV8DOC = CV5DOC AND CV8EMISSOR = CV5EMISSOR AND CV8TIPO = CV5TIPO
                                INNER JOIN VD1 ON VD1PEDIDO = CV5PEDIDO
                                INNER JOIN VD6 ON VD6PEDIDO = CV5PEDIDO AND VD6EMBARQ = CV5EMBARQ
                                WHERE CV5REPRES = @co1repres
                                  AND CV5SITUA = ''
                                  AND CV5EMITIDO = 1
                                  AND CV5ENTSAI = 1
                                  AND VD1CONSIG = 0
                                  AND CV5EMISSAO BETWEEN @co1inicio AND @co1fim
                                  AND ((CV5MODELO <> '55' AND CV5MODELO <> '65') OR ((CV5MODELO = '55' OR CV5MODELO = '65') AND NOT EXISTS (SELECT CV5DOC FROM CV5 WHERE CV5PEDIDO = VD1PEDIDO AND CV5EMBARQ = VD6EMBARQ AND CV5MODELO = '99' AND CV5SITUA = '' AND CV5EMITIDO = 1 AND CV5ENTSAI = 1)))
                                  AND NOT EXISTS (SELECT 1 FROM CO2 WHERE CV5DOC =  CO2DOC AND CV5TIPO = CO2TIPO AND CV5EMISSOR = CO2EMISSOR)
                                GROUP BY CV5DOC, CV5TIPO, CV5EMISSOR
                                ORDER BY CV5EMISSAO";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                IEnumerable<CO2> lstCo2 = await connection.QueryAsync<CO2>(command, new { co1repres, co1inicio, co1fim });

                return lstCo2.ToList();
            }
        }

        public async Task<int> Delete(string co2fecha)
        {
            string command = @"DELETE FROM CO2 WHERE CO2FECHA = @co2fecha";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { co2fecha });
            }
        }

        public async Task<int> DeleteCommissionSale(string co2fecha, string co2doc, string co2tipo, string co2emissor)
        {
            string command = @"DELETE FROM CO2 WHERE CO2FECHA = @co2fecha AND CO2DOC = @co2doc AND CO2TIPO = @co2tipo AND CO2EMISSOR = @co2emissor";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                return await connection.ExecuteAsync(command, new { co2fecha, co2doc, co2tipo, co2emissor });
            }
        }
    }
}
