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
    public class ChequePositionsRepository : Repository
    {
        public ChequePositionsRepository(string path) : base(path)
        {
        }

        public async Task<IList<PD7>> FindList()
        {

            string command = @"SELECT * FROM PD7";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    IEnumerable<PD7> lstPD7 = await connection.QueryAsync<PD7>(command);

                    return lstPD7.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<PD7> Find(string pd7agente, string pd7emissor)
        {
            string command = @"SELECT * FROM PD7 WHERE PD7AGENTE = @pd7agente and PD7EMISSOR = @pd7emissor";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    PD7 PD7 = await connection.QueryFirstOrDefaultAsync<PD7>(command, new { pd7agente, pd7emissor });

                    return PD7;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> Insert(PD7 pd7)
        {
            string command = @"INSERT INTO PD7 (PD7EMISSOR,PD7AGENTE,PD7VALOR,PD7LINHA1,PD7LINHA2,PD7NOMINAL,PD7CIDADE,PD7DIA,PD7MES,PD7ANO,PD7BOMPARA) 
                    VALUES (@PD7EMISSOR,@PD7AGENTE,@PD7VALOR,@PD7LINHA1,@PD7LINHA2,@PD7NOMINAL,@PD7CIDADE,@PD7DIA,@PD7MES,@PD7ANO,@PD7BOMPARA)";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    var affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        PD7Emissor = pd7.Pd7emissor,
                        PD7agente = pd7.Pd7agente,
                        PD7valor = pd7.Pd7valor,
                        PD7linha1 = pd7.Pd7linha1,
                        PD7linha2 = pd7.Pd7linha2,
                        PD7nominal = pd7.Pd7nominal,
                        PD7cidade = pd7.Pd7cidade,
                        PD7dia = pd7.Pd7dia,
                        PD7mes = pd7.Pd7mes,
                        PD7ano = pd7.Pd7ano,
                        PD7bompara = pd7.Pd7bompara,
                    });

                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> Delete(string pd7agente, string pd7emissor)
        {
            string command = @"DELETE FROM PD7 WHERE PD7AGENTE = @pd7agente AND PD7EMISSOR = @pd7emissor";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    return await connection.ExecuteAsync(command, new { pd7agente, pd7emissor });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> Update(PD7 pd7)
        {
            string command = @"UPDATE PD7
                                SET PD7AGENTE = @pd7agente,
                                    PD7VALOR = @pd7valor,
                                    PD7LINHA1 = @pd7linha1,
                                    PD7LINHA2 = @pd7linha2,
                                    PD7NOMINAL = @pd7nominal,
                                    PD7CIDADE = @pd7cidade,
                                    PD7DIA = @pd7dia,
                                    PD7MES = @pd7mes,
                                    PD7ANO = @pd7ano,
                                    PD7BOMPARA = @pd7bompara
                                WHERE PD7AGENTE = @pd7agente AND PD7EMISSOR = @pd7emissor";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                var affectedRows = await connection.ExecuteAsync(command,
                new
                {
                    pd7emissor = pd7.Pd7emissor,
                    pd7agente = pd7.Pd7agente,
                    pd7valor = pd7.Pd7valor,
                    pd7linha1 = pd7.Pd7linha1,
                    pd7linha2 = pd7.Pd7linha2,
                    pd7nominal = pd7.Pd7nominal,
                    pd7cidade = pd7.Pd7cidade,
                    pd7dia = pd7.Pd7dia,
                    pd7mes = pd7.Pd7mes,
                    pd7ano = pd7.Pd7ano,
                    pd7bompara = pd7.Pd7bompara
                });

                return affectedRows;
            }
        }
    }
}
