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
                _oCommand.Parameters.Add("@sIdCliente", SqlDbType.VarChar, 20).Value = psIdCliente;
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
                _oCommand.Parameters.Add("@sIdPedido", SqlDbType.VarChar, 12).Value = psIdPedido;
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
                _oCommand.Parameters.Add("@sCodNacional", SqlDbType.VarChar, 20).Value = psCodNacional;
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
                _oCommand.Parameters.Add("@sCodNacional", SqlDbType.VarChar, 20).Value = psCodNacional;
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
                _oCommand.Parameters.Add("@sCodNacional", SqlDbType.VarChar, 6).Value = psCodNacional;
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
                _oCommand.Parameters.Add("@sCodNacional", SqlDbType.VarChar, 20).Value = psCodNacional;
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
                _oCommand.Parameters.Add("@sCodNacional", SqlDbType.VarChar, 6).Value = psCodNacional;
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
                _oCommand.Parameters.Add("@sIdCliente", SqlDbType.VarChar, 20).Value = psCodDistribuidor;
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
        //public dsGESTCRMCE.ListaMaterialesConSustitutoDataTable getCodsConSustituto(SqlConnection sqlConn)
        public dsGESTCRMCE.ListaMaterialesConSustitutoDataTable getCodsConSustituto(SqlConnection sqlConn, string psTipoPedido, string psCodCliente) //---- GSG (12/05/2016)
        {
            dsGESTCRMCE.ListaMaterialesConSustitutoDataTable dt = new dsGESTCRMCE.ListaMaterialesConSustitutoDataTable();

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "ListaMaterialesConSustituto";
                //---- GSG (12/05/2016)
                _oCommand.Parameters.Add("@sIdTipoPedido", SqlDbType.VarChar, 4).Value = psTipoPedido;
                _oCommand.Parameters.Add("@iIdDelegado", SqlDbType.Int, 4).Value = -1;
                _oCommand.Parameters.Add("@sIdCliente", SqlDbType.VarChar, 20).Value = psCodCliente;

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


        //---- GSG (12/05/2016)
        public string GetSustituto(SqlConnection sqlConn, string psCodNacional, string psTipoPedido, string psCodCliente)
        {
            string ret = null;

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "GetMaterialSustituto";
                _oCommand.Parameters.Add("@sCodNacional", SqlDbType.VarChar, 6).Value = psCodNacional;
                _oCommand.Parameters.Add("@iIdDelegado", SqlDbType.Int, 4).Value = -1;
                _oCommand.Parameters.Add("@sIdCliente", SqlDbType.VarChar, 20).Value = psCodCliente;
                _oCommand.Parameters.Add("@Res", SqlDbType.VarChar, 13).Direction = ParameterDirection.Output; 

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


        public int GetUnidadesBaseSustituto(SqlConnection sqlConn, string psCodNacional)
        {
            int ret = 0;

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "GetUnidadesBaseSustituto";
                _oCommand.Parameters.Add("@sCodNacional", SqlDbType.VarChar, 6).Value = psCodNacional;
                _oCommand.Parameters.Add("@Res", SqlDbType.Int, 4).Direction = ParameterDirection.Output;

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


        public dsGESTCRMCE.ListaMaterialesBloqueadosDataTable getBloqueados(SqlConnection sqlConn, string psTipoPedido, string psCodCliente)
        {
            dsGESTCRMCE.ListaMaterialesBloqueadosDataTable dt = new dsGESTCRMCE.ListaMaterialesBloqueadosDataTable();

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "ListaMaterialesBloqueados";
                _oCommand.Parameters.Add("@sIdTipoPedido", SqlDbType.VarChar, 4).Value = psTipoPedido;
                _oCommand.Parameters.Add("@iIdDelegado", SqlDbType.Int, 4).Value = -1;
                _oCommand.Parameters.Add("@sIdCliente", SqlDbType.VarChar, 20).Value = psCodCliente;

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


        //---- GSG (07/07/2016)
        public dsGESTCRMCE.ListaMayoristasClientes_SAPDataTable getMayoristas(SqlConnection sqlConn, int piCodCliente)
        {
            dsGESTCRMCE.ListaMayoristasClientes_SAPDataTable dt = new dsGESTCRMCE.ListaMayoristasClientes_SAPDataTable();

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "ListaMayoristasClientes_SAP";
                _oCommand.Parameters.Add("@iIdCliente", SqlDbType.Int, 4).Value = piCodCliente;

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


        public int GetIdCliente(SqlConnection sqlConn, string psCodCliente)
        {
            int ret = -1;

            dsGESTCRMCE.IdClienteDataTable dt = new dsGESTCRMCE.IdClienteDataTable();
            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "Obtener_iIdCliente";
                _oCommand.Parameters.Add("@sIdCliente", SqlDbType.VarChar, 20).Value = psCodCliente;
                //_oCommand.Parameters.Add("@sIdCentro", SqlDbType.VarChar, 20).Value = null;

                SqlDataAdapter oSqlAdapter = new SqlDataAdapter(_oCommand);
                oSqlAdapter.Fill(dt);

                if (dt.Count > 0)
                {
                    foreach (dsGESTCRMCE.IdClienteRow row in dt)
                    {
                        ret = row.iIdCliente;
                        break;
                    }
                }

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


        //---- GSG (06/02/2017)
        public dsGESTCRMCE.ListaSustitucionMaterialesDataTable getMaterialesSustitucion(SqlConnection sqlConn, string psTipoPedido, string psCodCliente)
        {
            dsGESTCRMCE.ListaSustitucionMaterialesDataTable dt = new dsGESTCRMCE.ListaSustitucionMaterialesDataTable();

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "ListaSustitucionMateriales";
                _oCommand.Parameters.Add("@sIdTipoPedido", SqlDbType.VarChar, 4).Value = psTipoPedido;
                _oCommand.Parameters.Add("@iIdDelegado", SqlDbType.Int, 4).Value = -1;
                _oCommand.Parameters.Add("@sIdCliente", SqlDbType.VarChar, 20).Value = psCodCliente;

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



        //---- GSG (22/05/2018)
        public dsGESTCRMCE.SRMayoristasDataTable GetSRMayoristas(SqlConnection sqlConn, string psMayorista) 
        {
            dsGESTCRMCE.SRMayoristasDataTable dt = new dsGESTCRMCE.SRMayoristasDataTable();

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "GetSRMayoristas";
                _oCommand.Parameters.Add("@codMay", SqlDbType.VarChar, 20).Value = psMayorista;

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
