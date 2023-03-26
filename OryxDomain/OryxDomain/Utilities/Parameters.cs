using IniParser;
using Microsoft.Win32;
using System;
using System.IO;

namespace OryxDomain.Utilities
{
    public static class Parameters
    {
        public static string SqlConnection { get; set; }
        public static string DataSourceName { get; set; }

        public static void InitParameters(string iniPath)
        {
            var parser = new IniDataParser();
            parser.Configuration.SkipInvalidLines = true;
            parser.Configuration.ParseComments = false;
            IniData data = parser.Parse(File.ReadAllText(iniPath));

            DataSourceName = data["CONEXAO"]["Conexao1"];

            if (!string.IsNullOrWhiteSpace(DataSourceName))
            {
                string database = DataSourceName.Substring(DataSourceName.IndexOf("/") + 1);
                if (database.ToUpper().Equals("MYSQL"))
                {
                    InitMySqlConnection();
                }
            }
        }

        private static void InitMySqlConnection()
        {
            int init = DataSourceName.IndexOf(" ") + 1;
            int length = DataSourceName.IndexOf("/");
            DataSourceName = DataSourceName.Substring(init, length - init);

            RegistryKey reg = (Registry.CurrentUser).OpenSubKey("Software");
            reg = reg.OpenSubKey("ODBC");
            reg = reg.OpenSubKey("ODBC.INI");
            reg = reg.OpenSubKey(DataSourceName);
            if (reg != null)
            {
                string server = reg.GetValue("SERVER") != null ? reg.GetValue("SERVER").ToString() : string.Empty;
                string port = reg.GetValue("PORT") != null ? reg.GetValue("PORT").ToString() : "3306";
                string database = reg.GetValue("DATABASE") != null ? reg.GetValue("DATABASE").ToString() : string.Empty;
                string uid = reg.GetValue("UID") != null ? reg.GetValue("UID").ToString() : string.Empty;
                string pwd = reg.GetValue("PWD") != null ? reg.GetValue("PWD").ToString() : string.Empty;

                SqlConnection = string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4};SslMode=none;Pooling=False;", server, port, database, uid, pwd);
            }
            else
            {
                throw new Exception(Resources.Resources.Message_ErrorInGetConnectionString);
            }
        }
    }
}
