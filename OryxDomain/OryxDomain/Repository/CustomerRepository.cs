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
    public class CustomerRepository : Repository
    {
        public CustomerRepository(string path) : base(path)
        {
        }

        public async Task<int> InsertB2b(CF1 cf1)
        {
            string command = @"insert into 
                               cf1 (cf1cliente, cf1email, cf1nome, cf1foto1, cf1abert, cf1fant, cf1insest, cf1fone, cf1confone, cf1tipo, cf1cep, cf1ender1, cf1bairro, cf1numero, cf1compl, cf1login, cf1senha)
                               values (@cf1cliente, @cf1email, @cf1nome, @cf1foto1, @cf1abert, @cf1fant, @cf1insest, @cf1fone, @cf1confone, @cf1tipo, @cf1cep, @cf1ender1, @cf1bairro, @cf1numero, @cf1compl, @cf1login, @cf1senha)";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    var affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        cf1cliente = cf1.Cf1cliente,
                        cf1email = cf1.Cf1email,
                        cf1nome = cf1.Cf1nome,
                        cf1foto1 = cf1.Cf1foto1,
                        cf1abert = DateTime.UtcNow,
                        cf1fant = cf1.Cf1fant,
                        cf1insest = cf1.Cf1insest,
                        cf1fone = cf1.Cf1fone,
                        cf1confone = cf1.Cf1confone,
                        cf1tipo = cf1.Cf1tipo,
                        cf1cep = cf1.Cf1cep,
                        cf1ender1 = cf1.Cf1ender1,
                        cf1bairro = cf1.Cf1bairro,
                        cf1numero = cf1.Cf1numero,
                        cf1compl = cf1.Cf1compl,
                        cf1login = cf1.Cf1cliente,
                        cf1senha = cf1.Cf1senha
                    });

                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> Insert(CF1 cf1)
        {
            string command = @"insert into 
                               cf1 (CF1CLIENTE,CF1NOME,CF1FANT,CF1TIPO,CF1CODANT,CF1ENDER1,CF1ENDER2,CF1OBSERVA,CF1CEP,CF1INSEST,CF1CONTATO,CF1CONDPTO,CF1CONFONE,CF1CONANDI,CF1CONANME,CF1FONE,CF1FAX,CF1EMAIL,CF1REPRES,CF1REDESP,CF1AVISO,CF1USUARIO,CF1ABERT,cf1transp,CF1LOGIN,CF1SENHA,CF1CONCPF,CF1CONANAN,cf1conmail,CF1SITE,CF1BAIRRO,CF1NUMERO,CF1COMPL,CF1ETIQ,CF1SUFRAMA,CF1OPERCOM,CF1INSCMUN,CF1CNAE,CF1CONPGTO,cf1descon,CF1LATIT,CF1LONGIT,CF1PRGPROT,CF1EMAILNF,CF1FOTO1,CF1FOTO2,CF1FOTO3,CF1FOTO4,CF1LOCACAO)
                               values (@cf1cliente,@cf1nome,@cf1fant,@cf1tipo,@cf1codant,@cf1ender1,@cf1ender2,@cf1observa,@cf1cep,@cf1insest,@cf1contato,@cf1condpto,@cf1confone,@cf1conandi,@cf1conanme,@cf1fone,@cf1fax,@cf1email,@cf1repres,@cf1redesp,@cf1aviso,@cf1usuario,@cf1abert,@cf1transp,@cf1login,@cf1senha,@cf1concpf,@cf1conanan,@cf1conmail,@cf1site,@cf1bairro,@cf1numero,@cf1compl,@cf1etiq,@cf1suframa,@cf1opercom,@cf1inscmun,@cf1cnae,@cf1conpgto,@cf1descon,@cf1latit,@cf1longit,@cf1prgprot,@cf1emailnf,@cf1foto1,@cf1foto2,@cf1foto3,@cf1foto4,@cf1locacao)";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    var affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        cf1cliente = cf1.Cf1cliente,
                        cf1nome = cf1.Cf1nome,
                        cf1fant = cf1.Cf1fant,
                        cf1tipo = cf1.Cf1tipo,
                        cf1codant = cf1.Cf1codant,
                        cf1ender1 = cf1.Cf1ender1,
                        cf1ender2 = cf1.Cf1ender2,
                        cf1observa = cf1.Cf1observa,
                        cf1cep = cf1.Cf1cep,
                        cf1insest = cf1.Cf1insest,
                        cf1contato = cf1.Cf1contato,
                        cf1condpto = cf1.Cf1condpto,
                        cf1confone = cf1.Cf1confone,
                        cf1conandi = cf1.Cf1conandi,
                        cf1conanme = cf1.Cf1conanme,
                        cf1fone = cf1.Cf1fone,
                        cf1fax = cf1.Cf1fax,
                        cf1email = cf1.Cf1email,
                        cf1repres = cf1.Cf1repres,
                        cf1redesp = cf1.Cf1redesp,
                        cf1aviso = cf1.Cf1aviso,
                        cf1usuario = cf1.Cf1usuario,
                        cf1abert = cf1.Cf1abert,
                        cf1transp = cf1.Cf1transp,
                        cf1login = cf1.Cf1login,
                        cf1senha = cf1.Cf1senha,
                        cf1concpf = cf1.Cf1concpf,
                        cf1conanan = cf1.Cf1conanan,
                        cf1conmail = cf1.Cf1conmail,
                        cf1site = cf1.Cf1site,
                        cf1bairro = cf1.Cf1bairro,
                        cf1numero = cf1.Cf1numero,
                        cf1compl = cf1.Cf1compl,
                        cf1etiq = cf1.Cf1etiq,
                        cf1suframa = cf1.Cf1suframa,
                        cf1opercom = cf1.Cf1opercom,
                        cf1inscmun = cf1.Cf1inscmun,
                        cf1cnae = cf1.Cf1cnae,
                        cf1conpgto = cf1.Cf1conpgto,
                        cf1descon = cf1.Cf1descon,
                        cf1latit = cf1.Cf1latit,
                        cf1longit = cf1.Cf1longit,
                        cf1prgprot = cf1.Cf1prgprot,
                        cf1emailnf = cf1.Cf1emailnf,
                        cf1foto1 = cf1.Cf1foto1,
                        cf1foto2 = cf1.Cf1foto2,
                        cf1foto3 = cf1.Cf1foto3,
                        cf1foto4 = cf1.Cf1foto4,
                        cf1locacao = cf1.Cf1locacao,
                    });

                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<CF1>> Find(string search, int limit, int offset)
        {
            #region selectQuery
            string query = @"SELECT CF1CLIENTE,
                                    CF1NOME,
                                    CF1FANT,
                                    CF1TIPO,
                                    CF1CODANT,
                                    CF1ENDER1,
                                    CF1ENDER2,
                                    CF1OBSERVA,
                                    CF1CEP,
                                    CF1INSEST,
                                    CF1CONTATO,
                                    CF1CONDPTO,
                                    CF1CONFONE,
                                    CF1CONANDI,
                                    CF1CONANME,
                                    CF1FONE,
                                    CF1FAX,
                                    CF1EMAIL,
                                    CF1REPRES,
                                    CF1REDESP,
                                    CF1AVISO,
                                    CF1USUARIO,
                                    CF1ABERT,
                                    cf1transp,
                                    CF1LOGIN,
                                    CF1SENHA,
                                    CF1CONCPF,
                                    CF1CONANAN,
                                    cf1conmail,
                                    CF1SITE,
                                    CF1BAIRRO,
                                    CF1NUMERO,
                                    CF1COMPL,
                                    CF1ETIQ,
                                    CF1SUFRAMA,
                                    CF1OPERCOM,
                                    CF1INSCMUN,
                                    CF1CNAE,
                                    CF1CONPGTO,
                                    cf1descon,
                                    CF1LATIT,
                                    CF1LONGIT,
                                    CF1PRGPROT,
                                    CF1EMAILNF,
                                    CF1FOTO1,
                                    CF1FOTO2,
                                    CF1FOTO3,
                                    CF1FOTO4,
                                    CF1LOCACAO
                                FROM cf1
                                	WHERE CF1CLIENTE LIKE @search
                                       OR CF1NOME LIKE @search
                                       OR CF1FANT LIKE @search
                                       OR CF1TIPO LIKE @search
                                       OR CF1INSEST LIKE @search
                                       OR CF1EMAIL LIKE @search
                                LIMIT @limit OFFSET @offset";
            #endregion selectQuery
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();
                    search = string.Format("%{0}%", search);
                    search = search.Replace(" ", "%");
                    IEnumerable<CF1> cf1 = await connection.QueryAsync<CF1>(query, new { search, limit, offset});
                    return cf1.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<decimal> FindAmountSalesByLastYear(string cf1cliente)
        {
            string command = @"SELECT COALESCE(SUM(CV8VALOR),0) AS Amount
                                 FROM CV8
                                INNER JOIN CV5 ON CV8DOC = CV5DOC AND CV8TIPO = CV5TIPO AND CV8EMISSOR = CV5EMISSOR
                                 LEFT JOIN VD1 ON CV5PEDIDO = VD1PEDIDO
                                 LEFT JOIN VD6 ON CV5EMBARQ=VD6EMBARQ AND VD1PEDIDO=VD6PEDIDO
                                WHERE CV5CLIENTE = @cf1cliente
                                  AND CV5SITUA = ' ' AND CV5EMITIDO = 1
                                  AND YEAR(CV5EMISSAO) = YEAR(NOW())
                                  AND ((CV5.CV5MODELO <> '55' AND CV5.CV5MODELO <> '65') OR ((CV5.CV5MODELO = '55' OR CV5.CV5MODELO = '65') AND NOT EXISTS (SELECT CV5.CV5DOC FROM CV5 WHERE  CV5.CV5MODELO = '99' AND CV5PEDIDO=VD1PEDIDO AND CV5EMBARQ=VD6EMBARQ AND CV5.CV5SITUA = '' AND CV5.CV5EMITIDO = 1 AND CV5.CV5ENTSAI = 1)))
                                  AND CV5CONSIG = 0
                                  AND CV5ENTSAI<>2
                                  AND CV5ENTSAI<>4";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                decimal amount = await connection.QueryFirstOrDefaultAsync<decimal>(command, new { cf1cliente });

                return amount;
            }
        }

        public async Task<CF1> FindUserAuth(string cf1cliente)
        {
            string command = @"SELECT   Cf1cliente
                                      , Cf1senha
                                      , CASE 
                                            WHEN lx0cliente IS NULL OR LENGTH(lx0cliente) = 0 THEN 'customerb2b' 
                                            ELSE 'Administrator'
                                        END AS Rule  
                               FROM CF1
                               LEFT JOIN lx0 ON lx0cliente = cf1cliente
                               WHERE cf1cliente =  @cf1cliente
                               AND cf1login = @cf1cliente";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CF1 cf1 = await connection.QueryFirstOrDefaultAsync<CF1>(command, new { cf1cliente });

                return cf1;
            }
        }

        public async Task<CF1> FindByCpfCnpj(string cpfCnpj)
        {
            string command = @"SELECT CF1.*,
                                      CF3.Cf3nome,
                                      CF3.Cf3estado,
                                      CF2.Cf2local
                               FROM CF1
                               LEFT JOIN CF2 ON CF2CEP = CF1CEP
                               LEFT JOIN CF3 ON CF3LOCAL = CF2LOCAL
                               WHERE CF1CLIENTE =  @cpfcnpj";
            using (var connection = new MySqlConnection(Parameters.SqlConnection))
            {
                connection.Open();

                CF1 cf1 = await connection.QueryFirstOrDefaultAsync<CF1>(command, new { cpfCnpj });

                return cf1;
            }
        }

        public async Task<int> UpdateType(string cf1cliente, string cf1tipo)
        {
            string command = @"UPDATE CF1
                               SET CF1TIPO = @cf1tipo
                               WHERE CF1CLIENTE = @cf1cliente";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    var affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        cf1cliente,
                        cf1tipo
                    });

                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> UpdateB2b(CF1 cf1)
        {
            string command = @"UPDATE CF1
                               SET CF1CLIENTE = @cf1cliente, cf1email = @cf1email, cf1nome = @cf1nome, cf1foto1 = @cf1foto1, cf1fant = @cf1fant, cf1insest = @cf1insest, cf1fone = @cf1fone, cf1confone = @cf1confone, cf1tipo = @cf1tipo, cf1cep = @cf1cep, cf1ender1 = @cf1ender1, cf1bairro = @cf1bairro, cf1numero = @cf1numero, cf1compl = @cf1compl
                               WHERE CF1CLIENTE = @cf1cliente";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    var affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        cf1cliente = cf1.Cf1cliente,
                        cf1email = cf1.Cf1email,
                        cf1nome = cf1.Cf1nome,
                        cf1foto1 = cf1.Cf1foto1,
                        cf1fant = cf1.Cf1fant,
                        cf1insest = cf1.Cf1insest,
                        cf1fone = cf1.Cf1fone,
                        cf1confone = cf1.Cf1confone,
                        cf1tipo = cf1.Cf1tipo,
                        cf1cep = cf1.Cf1cep,
                        cf1ender1 = cf1.Cf1ender1,
                        cf1bairro = cf1.Cf1bairro,
                        cf1numero = cf1.Cf1numero,
                        cf1compl = cf1.Cf1compl
                    });

                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> Update(CF1 cf1)
        {
            string command = @"UPDATE CF1
                               SET CF1CLIENTE = @cf1cliente,CF1NOME = @cf1nome,CF1FANT = @cf1fant,CF1TIPO = @cf1tipo,CF1CODANT = @cf1codant,CF1ENDER1 = @cf1ender1,CF1ENDER2 = @cf1ender2,CF1OBSERVA = @cf1observa,CF1CEP = @cf1cep,CF1INSEST = @cf1insest,CF1CONTATO = @cf1contato,CF1CONDPTO = @cf1condpto,CF1CONFONE = @cf1confone,CF1CONANDI = @cf1conandi,CF1CONANME = @cf1conanme,CF1FONE = @cf1fone,CF1FAX = @cf1fax,CF1EMAIL = @cf1email,CF1REPRES = @cf1repres,CF1REDESP = @cf1redesp,CF1AVISO = @cf1aviso,CF1USUARIO = @cf1usuario,CF1ABERT = @cf1abert,CF1TRANSP = @cf1transp,CF1LOGIN = @cf1login,CF1SENHA = @cf1senha,CF1CONCPF = @cf1concpf,CF1CONANAN = @cf1conanan,CF1CONMAIL = @cf1conmail,CF1SITE = @cf1site,CF1BAIRRO = @cf1bairro,CF1NUMERO = @cf1numero,CF1COMPL = @cf1compl,CF1ETIQ = @cf1etiq,CF1SUFRAMA = @cf1suframa,CF1OPERCOM = @cf1opercom,CF1INSCMUN = @cf1inscmun,CF1CNAE = @cf1cnae,CF1CONPGTO = @cf1conpgto,CF1DESCON = @cf1descon,CF1LATIT = @cf1latit,CF1LONGIT = @cf1longit,CF1PRGPROT = @cf1prgprot,CF1EMAILNF = @cf1emailnf,CF1FOTO1 = @cf1foto1,CF1FOTO2 = @cf1foto2,CF1FOTO3 = @cf1foto3,CF1FOTO4 = @cf1foto4,CF1LOCACAO = @cf1locacao
                               WHERE CF1CLIENTE = @cf1cliente";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    var affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        cf1cliente = cf1.Cf1cliente,
                        cf1nome = cf1.Cf1nome,
                        cf1fant = cf1.Cf1fant,
                        cf1tipo = cf1.Cf1tipo,
                        cf1codant = cf1.Cf1codant,
                        cf1ender1 = cf1.Cf1ender1,
                        cf1ender2 = cf1.Cf1ender2,
                        cf1observa = cf1.Cf1observa,
                        cf1cep = cf1.Cf1cep,
                        cf1insest = cf1.Cf1insest,
                        cf1contato = cf1.Cf1contato,
                        cf1condpto = cf1.Cf1condpto,
                        cf1confone = cf1.Cf1confone,
                        cf1conandi = cf1.Cf1conandi,
                        cf1conanme = cf1.Cf1conanme,
                        cf1fone = cf1.Cf1fone,
                        cf1fax = cf1.Cf1fax,
                        cf1email = cf1.Cf1email,
                        cf1repres = cf1.Cf1repres,
                        cf1redesp = cf1.Cf1redesp,
                        cf1aviso = cf1.Cf1aviso,
                        cf1usuario = cf1.Cf1usuario,
                        cf1abert = cf1.Cf1abert,
                        cf1transp = cf1.Cf1transp,
                        cf1login = cf1.Cf1login,
                        cf1senha = cf1.Cf1senha,
                        cf1concpf = cf1.Cf1concpf,
                        cf1conanan = cf1.Cf1conanan,
                        cf1conmail = cf1.Cf1conmail,
                        cf1site = cf1.Cf1site,
                        cf1bairro = cf1.Cf1bairro,
                        cf1numero = cf1.Cf1numero,
                        cf1compl = cf1.Cf1compl,
                        cf1etiq = cf1.Cf1etiq,
                        cf1suframa = cf1.Cf1suframa,
                        cf1opercom = cf1.Cf1opercom,
                        cf1inscmun = cf1.Cf1inscmun,
                        cf1cnae = cf1.Cf1cnae,
                        cf1conpgto = cf1.Cf1conpgto,
                        cf1descon = cf1.Cf1descon,
                        cf1latit = cf1.Cf1latit,
                        cf1longit = cf1.Cf1longit,
                        cf1prgprot = cf1.Cf1prgprot,
                        cf1emailnf = cf1.Cf1emailnf,
                        cf1foto1 = cf1.Cf1foto1,
                        cf1foto2 = cf1.Cf1foto2,
                        cf1foto3 = cf1.Cf1foto3,
                        cf1foto4 = cf1.Cf1foto4,
                        cf1locacao = cf1.Cf1locacao,
                    });

                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IList<CVS>> FindBlocks(string cf1cliente)
        {
            string command = @"SELECT *
                               FROM CVS
                               WHERE CVSCLIENTE = @cf1cliente
                               AND CVSDESBLOQ = '1899-12-30 00:00:00'";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    IEnumerable<CVS> listCvs = await connection.QueryAsync<CVS>(command, new { cf1cliente });

                    return listCvs.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IList<CVS>> FindBlocksByCvsfatur(string cf1cliente)
        {
            string command = @"SELECT *
                                 FROM CVS
                                WHERE CVSCLIENTE = @cf1cliente
                                  AND CVSDESBLOQ = '1899-12-30 00:00:00'
                                  AND CVSFATUR=1";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    IEnumerable<CVS> listCvs = await connection.QueryAsync<CVS>(command, new { cf1cliente });

                    return listCvs.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> UpdateCredentials(string cf1login, string cf1senha)
        {
            string command = @"UPDATE CF1
                               SET CF1LOGIN = @cf1login, CF1SENHA = @cf1senha
                               WHERE CF1CLIENTE = @cf1login";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    var affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        cf1login,
                        cf1senha
                    });
                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IList<CF1>> FindPending(string b2bclicon)
        {
            string command = @"SELECT CF1.*, Cf3nome, Cf3estado
                               FROM CF1
                               LEFT JOIN CF2 ON CF2CEP = CF1CEP
                               LEFT JOIN CF3 ON CF3LOCAL = CF2LOCAL
                               WHERE CF1TIPO = @b2bclicon";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    IEnumerable<CF1> listCf1 = await connection.QueryAsync<CF1>(command, new { b2bclicon });

                    return listCf1.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    
        public async Task<int> Delete(string cpfCnpj)
        {
            string statement = "DELETE FROM CF1 WHERE CF1CLIENTE = @cpfcnpj";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();
                    var effectedRows = await connection.ExecuteAsync(statement, new { cpfCnpj });
                    return effectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IList<CV8>> FindOpenTitles(string cliente, DateTime now)
        {
            string statement = @"SELECT  CV8VENCIM
                                        , CV8EMISSOR
                                        , CV8TIPO
                                        , CV8DOC
                                        , CV8PARCELA
                                        , CV8VALOR 
                                   FROM CV8, CV5 
                                  WHERE CV5CLIENTE = @cliente
                                    AND CV5ENTSAI <> 2 
                                    AND CV5ENTSAI <> 4
                                    AND cv8vencim < @now
                                    AND CV8EMISSOR = CV5EMISSOR
                                    AND CV8TIPO = CV5TIPO
                                    AND CV8DOC = CV5DOC 
                                    AND CV5EMITIDO = 1
                                    AND CV5SITUA = ' '
                                    ORDER BY CV8VENCIM";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();
                    IEnumerable<CV8> lstOpenTitles = await connection.QueryAsync<CV8>(statement, new { cliente, now });
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
