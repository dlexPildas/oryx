using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models;
using OryxDomain.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class SisRepository : Repository
    {
        public SisRepository(string path) : base(path)
        {
        }

        public async Task<IList<BondSettled>> FindAllBondsSettled(string cv8emissor, string cv8tipo, string cv8doc, string cv8parcela, string recebivel)
        {
            string statement = @"SELECT FN6VALOR, FN5ABERT 
                                   FROM FN6, FN5
                                  WHERE FN6EMISSOR = @cv8emissor
                                    AND FN6TIPO = @cv8tipo
                                    AND FN6DOCFIS = @cv8doc
                                    AND FN6PARCELA = @cv8parcela
                                    AND FN6DOC = FN5DOC " + recebivel;
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();
                    IEnumerable<BondSettled> lstOpenTitles = await connection.QueryAsync<BondSettled>(statement, new { cv8emissor, cv8tipo, cv8doc, cv8parcela });
                    return lstOpenTitles.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
