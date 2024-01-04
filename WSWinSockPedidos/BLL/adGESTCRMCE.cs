using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using DAL;

namespace BLL
{
    public class adGESTCRMCE
    {
        private SqlCommand _oCommand;

        public adGESTCRMCE()
        {
            _oCommand = new SqlCommand();
            _oCommand.CommandType = CommandType.StoredProcedure;
        }


        //---- GSG (02/07/2012)
        //public int ExisteCliente(SqlConnection sqlConn, int piIdCliente)
        //{
        //    int ret = 0;

        //    try
        //    {
        //        _oCommand.Parameters.Clear();
        //        _oCommand.Connection = sqlConn;
        //        _oCommand.CommandText = "GetClienteExiste";
        //        _oCommand.Parameters.Add("@iIdCliente", SqlDbType.Int).Value = piIdCliente;
        //        _oCommand.Parameters.Add("@Res", SqlDbType.Int).Direction = ParameterDirection.Output;

        //        _oCommand.ExecuteNonQuery();

        //        ret = int.Parse(_oCommand.Parameters["@Res"].Value.ToString());
        //    }
        //    catch { }
        //    finally
        //    {
        //        sqlConn.Close();
        //    }

        //    return ret;
        //}

        public int ExisteCliente(SqlConnection sqlConn, string psIdCliente)
        {
            int ret = 0;

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "GetSClienteExiste";
                _oCommand.Parameters.Add("@sIdCliente", SqlDbType.VarChar).Value = psIdCliente;
                _oCommand.Parameters.Add("@Res", SqlDbType.Int).Direction = ParameterDirection.Output;

                _oCommand.ExecuteNonQuery();

                ret = int.Parse(_oCommand.Parameters["@Res"].Value.ToString());
            }
            catch (Exception e) 
            {
                string err = e.Message;
            }
            finally
            {
                sqlConn.Close();
            }

            return ret;
        }
        //---- FI GSG


        public int ExistePedido(SqlConnection sqlConn, string psIdPedido)
        {
            int ret = 0;

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "GetPedidoExiste";
                _oCommand.Parameters.Add("@sIdPedido", SqlDbType.VarChar).Value = psIdPedido;
                _oCommand.Parameters.Add("@Res", SqlDbType.Int).Direction = ParameterDirection.Output;

                _oCommand.ExecuteNonQuery();

                ret = int.Parse(_oCommand.Parameters["@Res"].Value.ToString());
            }
            catch { }
            finally
            {
                sqlConn.Close();
            }

            return ret;
        }

        //---- GSG (17/05/2012)
        public int ExisteCodNacional(SqlConnection sqlConn, string psCodNacional)
        {
            int ret = 0;

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "GetCodNacionalExiste";
                _oCommand.Parameters.Add("@sCodNacional", SqlDbType.Char, 20).Value = psCodNacional;
                _oCommand.Parameters.Add("@Res", SqlDbType.Int).Direction = ParameterDirection.Output;

                _oCommand.ExecuteNonQuery();

                ret = int.Parse(_oCommand.Parameters["@Res"].Value.ToString());
            }
            catch (Exception e)
            {
                string error = e.Message;
            }
            finally
            {
                sqlConn.Close();
            }

            return ret;
        }


        //---- GSG (10/05/2013)
        public int EnQueEstadoExisteCodNacional(SqlConnection sqlConn, string psCodNacional)
        {
            int ret = 0;

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "GetCodNacionalEstadoExiste";
                _oCommand.Parameters.Add("@sCodNacional", SqlDbType.Char, 20).Value = psCodNacional;
                _oCommand.Parameters.Add("@Res", SqlDbType.Int).Direction = ParameterDirection.Output;

                _oCommand.ExecuteNonQuery();

                ret = int.Parse(_oCommand.Parameters["@Res"].Value.ToString());
            }
            catch (Exception e)
            {
                string error = e.Message;
            }
            finally
            {
                sqlConn.Close();
            }

            return ret;
        }


        //---- GSG (02/07/2012)
        public int StockMaterial(SqlConnection sqlConn, string psCodNacional)
        {
            int ret = 0;

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "GetStockMaterial";
                _oCommand.Parameters.Add("@sCodNacional", SqlDbType.VarChar).Value = psCodNacional;
                _oCommand.Parameters.Add("@Res", SqlDbType.Int).Direction = ParameterDirection.Output;

                _oCommand.ExecuteNonQuery();

                ret = int.Parse(_oCommand.Parameters["@Res"].Value.ToString());
            }
            catch { }
            finally
            {
                sqlConn.Close();
            }

            return ret;
        }


        //---- GSG (02/07/2012)
        public string NombreMaterial(SqlConnection sqlConn, string psCodNacional)
        {
            string ret = "";

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "GetOnlyNombreMaterial";
                _oCommand.Parameters.Add("@sCodNacional", SqlDbType.VarChar, 20).Value = psCodNacional;
                _oCommand.Parameters.Add("@Res", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;

                _oCommand.ExecuteNonQuery();

                ret = _oCommand.Parameters["@Res"].Value.ToString();
            }
            catch (Exception e) {
                string err = e.Message;
            }
            finally
            {
                sqlConn.Close();
            }

            return ret;
        }

        //---- GSG (20/12/2012)
        public int EsCodNacionalCH(SqlConnection sqlConn, string psCodNacional)
        {
            int ret = 0;

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "GetEsCodNacionalCH";
                _oCommand.Parameters.Add("@sCodNacional", SqlDbType.Char, 20).Value = psCodNacional;
                _oCommand.Parameters.Add("@Res", SqlDbType.Int).Direction = ParameterDirection.Output;

                _oCommand.ExecuteNonQuery();

                ret = int.Parse(_oCommand.Parameters["@Res"].Value.ToString());
            }
            catch (Exception e)
            {
                string error = e.Message;
            }
            finally
            {
                sqlConn.Close();
            }

            return ret;
        }


        //---- GSG (13/02/2013)
        public float PrecioMaterial(SqlConnection sqlConn, string psCodNacional)
        {
            float ret = 0;

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "GetPrecioMaterial";
                _oCommand.Parameters.Add("@sCodNacional", SqlDbType.VarChar).Value = psCodNacional;
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



        //---- GSG (28/06/2013)
        public int EsDistribuidorOK(SqlConnection sqlConn, string psCodDistribuidor)
        {
            int ret = 0;

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "GetExisteCodDistribuidor";
                _oCommand.Parameters.Add("@sIdCliente", SqlDbType.Char, 20).Value = psCodDistribuidor;
                _oCommand.Parameters.Add("@Res", SqlDbType.Int).Direction = ParameterDirection.Output;

                _oCommand.ExecuteNonQuery();

                ret = int.Parse(_oCommand.Parameters["@Res"].Value.ToString());
            }
            catch (Exception e)
            {
                string error = e.Message;
            }
            finally
            {
                sqlConn.Close();
            }

            return ret;
        }


        //---- GSG (05/03/2014)
        public string EsMaterialSustituido(SqlConnection sqlConn, string psCodNacional)
        {
            string ret = null;

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "GetExisteMaterialSustituto";
                _oCommand.Parameters.Add("@sCodNacional", SqlDbType.VarChar, 6).Value = psCodNacional;
                //_oCommand.Parameters.Add("@Res", SqlDbType.VarChar, 6).Direction = ParameterDirection.Output; 
                _oCommand.Parameters.Add("@Res", SqlDbType.VarChar, 13).Direction = ParameterDirection.Output; //---- GSG (12/11/2014)

                _oCommand.ExecuteNonQuery();

                ret = _oCommand.Parameters["@Res"].Value.ToString();
            }
            catch (Exception e)
            {
                string error = e.Message;
            }
            finally
            {
                sqlConn.Close();
            }

            return ret;
        }


        //---- GSG (10/02/2016)
        public dsGESTCRMCE.ListaMaterialesConSustitutoDataTable getCodsConSustituto(SqlConnection sqlConn)
        {
            dsGESTCRMCE.ListaMaterialesConSustitutoDataTable dt = new dsGESTCRMCE.ListaMaterialesConSustitutoDataTable();

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "ListaMaterialesConSustituto";

                SqlDataAdapter oSqlAdapter = new SqlDataAdapter(_oCommand);
                oSqlAdapter.Fill(dt);
            }
            catch (Exception e)
            {
                string error = e.Message;
            }
            finally
            {
                sqlConn.Close();
            }

            return dt;
        }
    }
}
