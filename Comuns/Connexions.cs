using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;


namespace Comuns
{
    public class Connexions
    {
        private SqlConnection _sqlConnWS;
        private SqlConnection _sqlConnCE;
        private SqlConnection _sqlConnPRECIOS;


        #region Connexió WS
        public SqlConnection sqlConnWS
        {
            get
            {
                if (_sqlConnWS == null)
                {
                    _sqlConnWS = new SqlConnection(ConnectionStringWS);
                }
                if (_sqlConnWS.State == System.Data.ConnectionState.Closed)
                    _sqlConnWS.Open();

                return _sqlConnWS;
            }
            set { _sqlConnWS = value; }
        }

        public static string ConnectionStringWS
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["WSConnectionString"];
            }
        }

        #endregion

        #region Connexió CE
        public SqlConnection sqlConnCE
        {
            get
            {
                if (_sqlConnCE == null)
                {
                    _sqlConnCE = new SqlConnection(ConnectionStringCE);
                }
                if (_sqlConnCE.State == System.Data.ConnectionState.Closed)
                    _sqlConnCE.Open();

                return _sqlConnCE;
            }
            set { _sqlConnCE = value; }
        }


        public static string ConnectionStringCE
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["CEConnectionString"];
            }
        }

        #endregion


        #region Connexió PRECIOS_STADA
        public SqlConnection sqlConnPRECIOS
        {
            get
            {
                if (_sqlConnPRECIOS == null)
                {
                    _sqlConnPRECIOS = new SqlConnection(ConnectionStringPRECIOS);
                }
                if (_sqlConnPRECIOS.State == System.Data.ConnectionState.Closed)
                    _sqlConnPRECIOS.Open();

                return _sqlConnPRECIOS;
            }
            set { _sqlConnPRECIOS = value; }
        }


        public static string ConnectionStringPRECIOS
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["PRECIOSConnectionString"];
            }
        }

        #endregion

    }
}
