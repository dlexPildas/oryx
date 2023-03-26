using Dapper;
using MySql.Data.MySqlClient;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using System;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class FiscalParametersRepository : Repository
    {
        public FiscalParametersRepository(string path) : base(path)
        {
        }

        public async Task<LX3> Find()
        {
            string command = @"SELECT * FROM LX3";

            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                LX3 lx3 = await connection.QueryFirstOrDefaultAsync<LX3>(command);

                return lx3;
            }
        }

        public async Task<int> Insert(LX3 lx3)
        {
            string command = @"INSERT INTO LX3 (LX3CLIENTE,LX3MASCON,LX3REINIC,LX3ALIQ1,LX3TOTAL1,LX3ALIQ2,LX3TOTAL2,LX3ALIQ3,LX3TOTAL3,LX3ALIQ4,LX3TOTAL4,LX3ALIQ5,LX3TOTAL5,LX3ALIQ6,LX3TOTAL6,LX3ALIQ7,LX3TOTAL7,LX3ALIQ8,LX3TOTAL8,LX3ALIQ9,LX3TOTAL9,LX3ALIQ10,LX3TOTAL10,LX3OPERCOM,LX3CONFIRM,LX3ABREECF,LX3PESO,LX3ENTREGA,LX3FECHA,LX3TITULO1,LX3TITULO2,LX3TITULO3,LX3CONFIMP,LX3CNPJ,LX3CRC,LX3RESP,LX3UFCRC,LX3DTCRC,LX3SEQCRC,LX3ICMSIPI,LX3HORVER,LX3SKU,LX3ESPEC,LX3CFVLMAX)
                               VALUES (@lx3cliente,@lx3mascon,@lx3reinic,@lx3aliq1,@lx3total1,@lx3aliq2,@lx3total2,@lx3aliq3,@lx3total3,@lx3aliq4,@lx3total4,@lx3aliq5,@lx3total5,@lx3aliq6,@lx3total6,@lx3aliq7,@lx3total7,@lx3aliq8,@lx3total8,@lx3aliq9,@lx3total9,@lx3aliq10,@lx3total10,@lx3opercom,@lx3confirm,@lx3abreecf,@lx3peso,@lx3entrega,@lx3fecha,@lx3titulo1,@lx3titulo2,@lx3titulo3,@lx3confimp,@lx3cnpj,@lx3crc,@lx3resp,@lx3ufcrc,@lx3dtcrc,@lx3seqcrc,@lx3icmsipi,@lx3horver,@lx3sku,@lx3espec,@lx3cfvlmax)";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    var affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        lx3cliente = lx3.Lx3cliente,
                        lx3mascon = lx3.Lx3mascon,
                        lx3reinic = lx3.Lx3reinic,
                        lx3aliq1 = lx3.Lx3aliq1,
                        lx3total1 = lx3.Lx3total1,
                        lx3aliq2 = lx3.Lx3aliq2,
                        lx3total2 = lx3.Lx3total2,
                        lx3aliq3 = lx3.Lx3aliq3,
                        lx3total3 = lx3.Lx3total3,
                        lx3aliq4 = lx3.Lx3aliq4,
                        lx3total4 = lx3.Lx3total4,
                        lx3aliq5 = lx3.Lx3aliq5,
                        lx3total5 = lx3.Lx3total5,
                        lx3aliq6 = lx3.Lx3aliq6,
                        lx3total6 = lx3.Lx3total6,
                        lx3aliq7 = lx3.Lx3aliq7,
                        lx3total7 = lx3.Lx3total7,
                        lx3aliq8 = lx3.Lx3aliq8,
                        lx3total8 = lx3.Lx3total8,
                        lx3aliq9 = lx3.Lx3aliq9,
                        lx3total9 = lx3.Lx3total9,
                        lx3aliq10 = lx3.Lx3aliq10,
                        lx3total10 = lx3.Lx3total10,
                        lx3opercom = lx3.Lx3opercom,
                        lx3confirm = lx3.Lx3confirm,
                        lx3abreecf = lx3.Lx3abreecf,
                        lx3peso = lx3.Lx3peso,
                        lx3entrega = lx3.Lx3entrega,
                        lx3fecha = lx3.Lx3fecha,
                        lx3titulo1 = lx3.Lx3titulo1,
                        lx3titulo2 = lx3.Lx3titulo2,
                        lx3titulo3 = lx3.Lx3titulo3,
                        lx3confimp = lx3.Lx3confimp,
                        lx3cnpj = lx3.Lx3cnpj,
                        lx3crc = lx3.Lx3crc,
                        lx3resp = lx3.Lx3resp,
                        lx3ufcrc = lx3.Lx3ufcrc,
                        lx3dtcrc = lx3.Lx3dtcrc,
                        lx3seqcrc = lx3.Lx3seqcrc,
                        lx3icmsipi = lx3.Lx3icmsipi,
                        lx3horver = lx3.Lx3horver,
                        lx3sku = lx3.Lx3sku,
                        lx3espec = lx3.Lx3espec,
                        lx3cfvlmax = lx3.Lx3cfvlmax
                    });

                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> Update(LX3 lx3)
        {
            string command = @"UPDATE LX3
                                  SET LX3CLIENTE = @lx3cliente,LX3MASCON = @lx3mascon,LX3REINIC = @lx3reinic,LX3ALIQ1 = @lx3aliq1,LX3TOTAL1 = @lx3total1,LX3ALIQ2 = @lx3aliq2,LX3TOTAL2 = @lx3total2,LX3ALIQ3 = @lx3aliq3,LX3TOTAL3 = @lx3total3,LX3ALIQ4 = @lx3aliq4,LX3TOTAL4 = @lx3total4,LX3ALIQ5 = @lx3aliq5,LX3TOTAL5 = @lx3total5,LX3ALIQ6 = @lx3aliq6,LX3TOTAL6 = @lx3total6,LX3ALIQ7 = @lx3aliq7,LX3TOTAL7 = @lx3total7,LX3ALIQ8 = @lx3aliq8,LX3TOTAL8 = @lx3total8,LX3ALIQ9 = @lx3aliq9,LX3TOTAL9 = @lx3total9,LX3ALIQ10 = @lx3aliq10,LX3TOTAL10 = @lx3total10,LX3OPERCOM = @lx3opercom,LX3CONFIRM = @lx3confirm,LX3ABREECF = @lx3abreecf,LX3PESO = @lx3peso,LX3ENTREGA = @lx3entrega,LX3FECHA = @lx3fecha,LX3TITULO1 = @lx3titulo1,LX3TITULO2 = @lx3titulo2,LX3TITULO3 = @lx3titulo3,LX3CONFIMP = @lx3confimp,LX3CNPJ = @lx3cnpj,LX3CRC = @lx3crc,LX3RESP = @lx3resp,LX3UFCRC = @lx3ufcrc,LX3DTCRC = @lx3dtcrc,LX3SEQCRC = @lx3seqcrc,LX3ICMSIPI = @lx3icmsipi,LX3HORVER = @lx3horver,LX3SKU = @lx3sku,LX3ESPEC = @lx3espec, LX3CFVLMAX = @lx3cfvlmax";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    var affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        lx3cliente = lx3.Lx3cliente,
                        lx3mascon = lx3.Lx3mascon,
                        lx3reinic = lx3.Lx3reinic,
                        lx3aliq1 = lx3.Lx3aliq1,
                        lx3total1 = lx3.Lx3total1,
                        lx3aliq2 = lx3.Lx3aliq2,
                        lx3total2 = lx3.Lx3total2,
                        lx3aliq3 = lx3.Lx3aliq3,
                        lx3total3 = lx3.Lx3total3,
                        lx3aliq4 = lx3.Lx3aliq4,
                        lx3total4 = lx3.Lx3total4,
                        lx3aliq5 = lx3.Lx3aliq5,
                        lx3total5 = lx3.Lx3total5,
                        lx3aliq6 = lx3.Lx3aliq6,
                        lx3total6 = lx3.Lx3total6,
                        lx3aliq7 = lx3.Lx3aliq7,
                        lx3total7 = lx3.Lx3total7,
                        lx3aliq8 = lx3.Lx3aliq8,
                        lx3total8 = lx3.Lx3total8,
                        lx3aliq9 = lx3.Lx3aliq9,
                        lx3total9 = lx3.Lx3total9,
                        lx3aliq10 = lx3.Lx3aliq10,
                        lx3total10 = lx3.Lx3total10,
                        lx3opercom = lx3.Lx3opercom,
                        lx3confirm = lx3.Lx3confirm,
                        lx3abreecf = lx3.Lx3abreecf,
                        lx3peso = lx3.Lx3peso,
                        lx3entrega = lx3.Lx3entrega,
                        lx3fecha = lx3.Lx3fecha,
                        lx3titulo1 = lx3.Lx3titulo1,
                        lx3titulo2 = lx3.Lx3titulo2,
                        lx3titulo3 = lx3.Lx3titulo3,
                        lx3confimp = lx3.Lx3confimp,
                        lx3cnpj = lx3.Lx3cnpj,
                        lx3crc = lx3.Lx3crc,
                        lx3resp = lx3.Lx3resp,
                        lx3ufcrc = lx3.Lx3ufcrc,
                        lx3dtcrc = lx3.Lx3dtcrc,
                        lx3seqcrc = lx3.Lx3seqcrc,
                        lx3icmsipi = lx3.Lx3icmsipi,
                        lx3horver = lx3.Lx3horver,
                        lx3sku = lx3.Lx3sku,
                        lx3espec = lx3.Lx3espec,
                        lx3cfvlmax = lx3.Lx3cfvlmax
                    });

                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> Delete()
        {
            string command = @"DELETE FROM LX3";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    return await connection.ExecuteAsync(command);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
