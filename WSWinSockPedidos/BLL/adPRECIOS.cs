using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using DAL;


namespace BLL
{
    public class adPRECIOS
    {
        private SqlCommand _oCommand;

        public adPRECIOS()
        {
            _oCommand = new SqlCommand();
            _oCommand.CommandType = CommandType.StoredProcedure;
        }


        public float PVPMaterial(SqlConnection sqlConn, string psCodSAP, string psCodNacional, DateTime pdFecha)
        {
            float ret = 0;

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "GetPVPMat";
                _oCommand.Parameters.Add("@sCodSAP", SqlDbType.VarChar, 18).Value = psCodSAP;
                _oCommand.Parameters.Add("@sCodNacional", SqlDbType.VarChar, 6).Value = psCodNacional;
                _oCommand.Parameters.Add("@dFec", SqlDbType.DateTime).Value = pdFecha;
                _oCommand.Parameters.Add("@Res", SqlDbType.Float).Direction = ParameterDirection.Output;

                _oCommand.ExecuteNonQuery();

                ret = float.Parse(_oCommand.Parameters["@Res"].Value.ToString());
            }
            catch (Exception e) { string error = e.Message; }
            finally
            {
                sqlConn.Close();
            }

            return ret;
        }
    }
}
