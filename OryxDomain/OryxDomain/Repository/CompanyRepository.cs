using Dapper;
using OryxDomain.Models;
using OryxDomain.Models.Oryx;
using OryxDomain.Utilities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OryxDomain.Repository
{
    public class CompanyRepository : Repository
    {
        public CompanyRepository(string path) : base(path)
        {
        }

        public async Task<CompanyModel> FindCompanyInfos()
        {
            string command = @"SELECT Lx0cliente, Cf1fant, Cf1nome, B2bwhats, B2dsobre, B2ddescri
                               FROM LX0
                               INNER JOIN CF1 ON CF1CLIENTE = LX0CLIENTE
                               INNER JOIN B2B ON B2BPADRAO = 1
                               INNER JOIN B2D ON B2DCODIGO = 1";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    CompanyModel companyModel = await connection.QueryFirstOrDefaultAsync<CompanyModel>(command, new { });

                    return companyModel;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CF1> FindContacts()
        {
            string command = @"SELECT Cf1fone, Cf1email FROM CF1 INNER JOIN LX0 ON LX0CLIENTE = CF1CLIENTE";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    CF1 cf1 = await connection.QueryFirstOrDefaultAsync<CF1>(command, new { });

                    return cf1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GeoLocationModel> FindGeolocation()
        {
            string command = @"SELECT Cf1latit as Lat, Cf1longit as Lng FROM CF1 INNER JOIN LX0 ON LX0CLIENTE = CF1CLIENTE";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    GeoLocationModel geoLocation = await connection.QueryFirstOrDefaultAsync<GeoLocationModel>(command, new { });

                    return geoLocation;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> SaveB2D(B2D b2d)
        {
            string delete = @"DELETE FROM B2D";
            string command = @"INSERT INTO 
                               B2D (B2DCODIGO, B2DSOBRE, B2DDESCRI)
                               values (1, @b2dsobre, @b2ddescri)";

            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    int affectedRows = await connection.ExecuteAsync(delete);

                    if (affectedRows == 0)
                    {
                        return 0;
                    }

                    affectedRows = await connection.ExecuteAsync(command,
                    new
                    {
                        b2dsobre = b2d.B2dsobre,
                        b2ddescri = b2d.B2ddescri
                    });

                    return affectedRows;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IList<B2M>> FindSocialMedias()
        {
            string command = @"SELECT B2mtipo, B2mlink FROM B2M";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    IEnumerable<B2M> lstB2m = await connection.QueryAsync<B2M>(command, new { });

                    return lstB2m.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IList<CO0>> FindCollections()
        {
            string command = @"SELECT Co0colecao, Co0nome
                            FROM CO0
                            WHERE CO0B2B = 1";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    IEnumerable<CO0> lstCollections = await connection.QueryAsync<CO0>(command, new { });

                    return lstCollections.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IList<PRS>> FindGroups()
        {
            string command = @"SELECT Prsgrupo, Prsnome
                            FROM PRS
                            WHERE PRSB2B = 1";
            try
            {
                using (var connection = new MySqlConnection(Parameters.SqlConnection))
                {
                    connection.Open();

                    IEnumerable<PRS> lstGroups = await connection.QueryAsync<PRS>(command, new { });

                    return lstGroups.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
