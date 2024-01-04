using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using DAL;

namespace BLL
{
    public class adSPR
    {
        private SqlCommand _oCommand;

        public adSPR()
        {
            _oCommand = new SqlCommand();
            _oCommand.CommandType = CommandType.StoredProcedure;
        }


        public int InsertarCabeceraPedido(SqlConnection sqlConn, string sCodCli, string sCodPed, string sTipoPedido, 
                                          string sCondServicio, string sCargoCoop, string sAplazCargo, string sAplazPago, 
		                                  double nDtoCargoAdicional, string sEmpFacturadora, string sAlmacenServ, 
                                          DateTime dFechaEnvio, string sDiaEnvPedido, int iTotalLineas, int iTotalBonif,
                                          DateTime dFeultact, int iTotalQty, int iDistribuidor, string sFicheroDatos)
        {
            int ret = -1;

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "SetPedidosCab";
                _oCommand.Parameters.Add("@sCodCli", SqlDbType.VarChar, 16).Value = sCodCli;
                _oCommand.Parameters.Add("@sCodPed", SqlDbType.VarChar, 10).Value = sCodPed;
                _oCommand.Parameters.Add("@sTipoPedido", SqlDbType.VarChar, 6).Value = sTipoPedido;
                _oCommand.Parameters.Add("@sCondServicio", SqlDbType.VarChar, 6).Value = sCondServicio;
                _oCommand.Parameters.Add("@sCargoCoop", SqlDbType.VarChar, 1).Value = sCargoCoop;
                _oCommand.Parameters.Add("@sAplazCargo", SqlDbType.VarChar, 1).Value = sAplazCargo;
                _oCommand.Parameters.Add("@sAplazPago", SqlDbType.VarChar, 3).Value = sAplazPago;
                _oCommand.Parameters.Add("@nDtoCargoAdicional", SqlDbType.Float).Value = nDtoCargoAdicional;
                _oCommand.Parameters.Add("@sEmpFacturadora", SqlDbType.VarChar, 3).Value = sEmpFacturadora;
                _oCommand.Parameters.Add("@sAlmacenServ", SqlDbType.VarChar, 4).Value = sAlmacenServ;
                _oCommand.Parameters.Add("@dFechaEnvio", SqlDbType.DateTime).Value = dFechaEnvio;
                _oCommand.Parameters.Add("@sDiaEnvPedido", SqlDbType.VarChar, 2).Value = sDiaEnvPedido;
                _oCommand.Parameters.Add("@iTotalLineas", SqlDbType.Int).Value = iTotalLineas;
                _oCommand.Parameters.Add("@iTotalBonif", SqlDbType.Int).Value = iTotalBonif;
                _oCommand.Parameters.Add("@dFeultact", SqlDbType.DateTime).Value = dFeultact;
                _oCommand.Parameters.Add("@iTotalQty", SqlDbType.Int).Value = iTotalQty;
                _oCommand.Parameters.Add("@iDistribuidor", SqlDbType.Int).Value = iDistribuidor;
                _oCommand.Parameters.Add("@sFicheroDatos", SqlDbType.VarChar, 100).Value = sFicheroDatos;

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


        public int InsertarLineaPedido(SqlConnection sqlConn, int pedID, string sCodArticulo, int iCantidad,
                                          double fBonificacion, double fDescuento, DateTime dFeultact)
        {
            int ret = -1;

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "SetLineaPedido";
                _oCommand.Parameters.Add("@pedID", SqlDbType.Int).Value = pedID;
                _oCommand.Parameters.Add("@sCodArticulo", SqlDbType.VarChar, 13).Value = sCodArticulo;
                _oCommand.Parameters.Add("@iCantidad", SqlDbType.Int).Value = iCantidad;
                _oCommand.Parameters.Add("@fBonificacion", SqlDbType.Float).Value = fBonificacion;
                _oCommand.Parameters.Add("@fDescuento", SqlDbType.Float).Value = fDescuento;
                _oCommand.Parameters.Add("@dFeultact", SqlDbType.DateTime).Value = dFeultact;
                _oCommand.Parameters.Add("@Res", SqlDbType.Int).Direction = ParameterDirection.Output;

                _oCommand.ExecuteNonQuery();

                ret = int.Parse(_oCommand.Parameters["@Res"].Value.ToString());
            }
            catch (Exception e)
            { }
            finally
            {
                sqlConn.Close();
            }

            return ret;
        }

        //---- GSG (14/09/2012)
        //public void InsertarLineaLog(SqlConnection sqlConn, string sCodCli, string sCodPed, string sDescError, DateTime dFeultact, string sTipoLin)
        //---- GSG (18/02/2016)
        //public void InsertarLineaLog(SqlConnection sqlConn, string sCodCli, string sCodPed, string sDescError, DateTime dFeultact, string sTipoLin, string sFicheroDatos, bool bAceptado)
        //---- GSG (18/10/2019)
        //public void InsertarLineaLog(SqlConnection sqlConn, string sCodCli, string sCodPed, string sDescError, DateTime dFeultact, string sTipoLin, string sFicheroDatos, bool bAceptado, int idPedido)
        public void InsertarLineaLog(SqlConnection sqlConn, string sCodCli, string sCodPed, string sDescError, DateTime dFeultact, string sTipoLin, string sFicheroDatos, bool bAceptado, int idPedido, int iCantidad)
        {
            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "SetLog";
                _oCommand.Parameters.Add("@sCodCli", SqlDbType.VarChar, 16).Value = sCodCli;
                _oCommand.Parameters.Add("@sCodPed", SqlDbType.VarChar, 10).Value = sCodPed;
                _oCommand.Parameters.Add("@sDescError", SqlDbType.VarChar, 250).Value = sDescError;
                _oCommand.Parameters.Add("@dFeultact", SqlDbType.DateTime).Value = dFeultact;
                _oCommand.Parameters.Add("@sTipoLin", SqlDbType.VarChar, 1).Value = sTipoLin;
                _oCommand.Parameters.Add("@sFicheroDatos", SqlDbType.VarChar, 100).Value = sFicheroDatos; //---- GSG (14/09/2012)
                _oCommand.Parameters.Add("@bAceptado", SqlDbType.Bit).Value = bAceptado; //---- GSG (14/09/2012)
                _oCommand.Parameters.Add("@lip_IDPedido", SqlDbType.Int).Value = idPedido; //---- GSG (18/02/2016)
                _oCommand.Parameters.Add("@iCantidad", SqlDbType.Int).Value = iCantidad; //---- GSG (18/10/2019)

                _oCommand.ExecuteNonQuery();

            }
            catch (Exception e)
            { }
            finally
            {
                sqlConn.Close();
            }
        }

        //---- GSG (24/02/2016)
        //public int ExistePedidoEnRecepcion(SqlConnection sqlConn, string psCodPedido, string psCodCliente)
        public int ExistePedidoEnRecepcion(SqlConnection sqlConn, string psCodPedido, string psCodCliente, DateTime pfechaEnvio, int ptotalLineas, int ptotalUnidades)
        {
            int ret = 0;

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "GetPedidoExiste";
                _oCommand.Parameters.Add("@sCodPedido", SqlDbType.VarChar, 10).Value = psCodPedido;
                _oCommand.Parameters.Add("@sCodCliente", SqlDbType.VarChar, 16).Value = psCodCliente;

                _oCommand.Parameters.Add("@dFechaEnvio", SqlDbType.DateTime, 8).Value = pfechaEnvio;
                _oCommand.Parameters.Add("@iTotalLineas", SqlDbType.Int, 4).Value = ptotalLineas;
                _oCommand.Parameters.Add("@iTotalCantidad", SqlDbType.Int, 4).Value = ptotalUnidades;

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


        //---- GSG (13/02/2013)
        public string GetConfiguracion(SqlConnection sqlConn, string psConfig)
        {
            string ret = "";

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "Obtener_Configuracion";
                _oCommand.Parameters.Add("@sConfig", SqlDbType.VarChar, 25).Value = psConfig;
                _oCommand.Parameters.Add("@Val", SqlDbType.VarChar, 30).Direction = ParameterDirection.Output;

                _oCommand.ExecuteNonQuery();

                ret = _oCommand.Parameters["@Val"].Value.ToString();
            }
            catch (Exception e) { }
            finally
            {
                sqlConn.Close();
            }

            return ret;
        }

        //---- GSG (17/07/2013)
        public bool UpdateConfiguracion(SqlConnection sqlConn, string sConfig, string sValor)
        {
            bool bRet = true;

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "Update_Configuracion";
                _oCommand.Parameters.Add("@sConfig", SqlDbType.VarChar, 25).Value = sConfig;
                _oCommand.Parameters.Add("@Val", SqlDbType.VarChar, 30).Value = sValor;

                _oCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                string err = e.Message;
                bRet = false;
            }
            finally
            {
                sqlConn.Close();
            }

            return bRet;
        }

        //---- GSG (15/04/2013)
        public int EsCodNacionalAparte(SqlConnection sqlConn, string psCodNacional)
        {
            int ret = 0;

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "GetEsCodNacionalAparte";
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

        

        //---- GSG (26/05/2015)
        public dsSPR.ListaMaterialesAparteDataTable GetListaCodNacionalAparte(SqlConnection sqlConn)
        {
            dsSPR.ListaMaterialesAparteDataTable dt = new dsSPR.ListaMaterialesAparteDataTable();

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "ListaMaterialesAparte";

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

        public dsSPR.FicherosDataTable GetFicherosAparte(SqlConnection sqlConn)
        {
            dsSPR.FicherosDataTable dt = new dsSPR.FicherosDataTable();
            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "GetFicheros";

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




        public dsSPR.ListaCondicionesRegaloDataTable GetListaCondicionesRegalo(SqlConnection sqlConn)
        {
            dsSPR.ListaCondicionesRegaloDataTable dt = new dsSPR.ListaCondicionesRegaloDataTable();

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "ListaCondicionesRegalo";

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


        //---- GSG (15/04/2013)
        public bool EsClienteSinCodsPedido(SqlConnection sqlConn, string psCodCliente)
        {
            bool ret = false;

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "GetEsClienteSinCodsPedido";
                _oCommand.Parameters.Add("@sCodCliente", SqlDbType.VarChar, 20).Value = psCodCliente;
                _oCommand.Parameters.Add("@Res", SqlDbType.Bit).Direction = ParameterDirection.Output;

                _oCommand.ExecuteNonQuery();

                ret = bool.Parse(_oCommand.Parameters["@Res"].Value.ToString());
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


        public string GetFicheroMaterial(SqlConnection sqlConn, string psIdMaterial, bool pbCH, string psTipoPedido)
        {
            string ret = "";

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "GetFicheroMaterial";
                _oCommand.Parameters.Add("@sIdMaterial", SqlDbType.VarChar, 18).Value = psIdMaterial;
                _oCommand.Parameters.Add("@bEsCH", SqlDbType.Bit).Value = pbCH;
                _oCommand.Parameters.Add("@sTipoPedido", SqlDbType.VarChar, 6).Value = psTipoPedido;
                _oCommand.Parameters.Add("@Res", SqlDbType.VarChar, 4).Direction = ParameterDirection.Output;

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

        public string GetFicheroMaterialRegalo(SqlConnection sqlConn, string psIdMaterialRegalo, string psGrupo, string psTipoPedido)
        {
            string ret = "";

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "GetFicheroMaterialRegalo";
                _oCommand.Parameters.Add("@sIdMaterialRegalo", SqlDbType.VarChar, 18).Value = psIdMaterialRegalo;
                _oCommand.Parameters.Add("@sGrupo", SqlDbType.VarChar, 4).Value = psGrupo;
                _oCommand.Parameters.Add("@sTipoPedido", SqlDbType.VarChar, 6).Value = psTipoPedido;
                _oCommand.Parameters.Add("@Res", SqlDbType.VarChar, 4).Direction = ParameterDirection.Output;

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

        //---- GSG (19/11/2015)
        public bool EsMaterialRechazo(SqlConnection sqlConn, string psCodNacional, string psTipoPedido)
        {
            bool ret = false;

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "GetEsMaterialRechazo";
                _oCommand.Parameters.Add("@sCodNacional", SqlDbType.VarChar, 18).Value = psCodNacional;
                _oCommand.Parameters.Add("@sTipoPedido", SqlDbType.VarChar, 6).Value = psTipoPedido;
                _oCommand.Parameters.Add("@Res", SqlDbType.VarChar, 4).Direction = ParameterDirection.Output;

                _oCommand.ExecuteNonQuery();

                int retorno = int.Parse(_oCommand.Parameters["@Res"].Value.ToString());
                if (retorno == 1)
                    ret = true;
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

        public int InsertarCabeceraPedidoRechazado(SqlConnection sqlConn, string sCodCli, string sCodPed, string sTipoPedido,
                                          string sCondServicio, string sCargoCoop, string sAplazCargo, string sAplazPago,
                                          double nDtoCargoAdicional, string sEmpFacturadora, string sAlmacenServ,
                                          DateTime dFechaEnvio, string sDiaEnvPedido, int iTotalLineas, int iTotalBonif,
                                          DateTime dFeultact, int iTotalQty, int iDistribuidor, string sFicheroDatos)
        {
            int ret = -1;

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "SetPedidosCabRechazados";
                _oCommand.Parameters.Add("@sCodCli", SqlDbType.VarChar, 16).Value = sCodCli;
                _oCommand.Parameters.Add("@sCodPed", SqlDbType.VarChar, 10).Value = sCodPed;
                _oCommand.Parameters.Add("@sTipoPedido", SqlDbType.VarChar, 6).Value = sTipoPedido;
                _oCommand.Parameters.Add("@sCondServicio", SqlDbType.VarChar, 6).Value = sCondServicio;
                _oCommand.Parameters.Add("@sCargoCoop", SqlDbType.VarChar, 1).Value = sCargoCoop;
                _oCommand.Parameters.Add("@sAplazCargo", SqlDbType.VarChar, 1).Value = sAplazCargo;
                _oCommand.Parameters.Add("@sAplazPago", SqlDbType.VarChar, 3).Value = sAplazPago;
                _oCommand.Parameters.Add("@nDtoCargoAdicional", SqlDbType.Float).Value = nDtoCargoAdicional;
                _oCommand.Parameters.Add("@sEmpFacturadora", SqlDbType.VarChar, 3).Value = sEmpFacturadora;
                _oCommand.Parameters.Add("@sAlmacenServ", SqlDbType.VarChar, 4).Value = sAlmacenServ;
                _oCommand.Parameters.Add("@dFechaEnvio", SqlDbType.DateTime).Value = dFechaEnvio;
                _oCommand.Parameters.Add("@sDiaEnvPedido", SqlDbType.VarChar, 2).Value = sDiaEnvPedido;
                _oCommand.Parameters.Add("@iTotalLineas", SqlDbType.Int).Value = iTotalLineas;
                _oCommand.Parameters.Add("@iTotalBonif", SqlDbType.Int).Value = iTotalBonif;
                _oCommand.Parameters.Add("@dFeultact", SqlDbType.DateTime).Value = dFeultact;
                _oCommand.Parameters.Add("@iTotalQty", SqlDbType.Int).Value = iTotalQty;
                _oCommand.Parameters.Add("@iDistribuidor", SqlDbType.Int).Value = iDistribuidor;
                _oCommand.Parameters.Add("@sFicheroDatos", SqlDbType.VarChar, 100).Value = sFicheroDatos;

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


        public int InsertarLineaPedidoRechazado(SqlConnection sqlConn, int pedID, string sCodArticulo, int iCantidad,
                                          double fBonificacion, double fDescuento, DateTime dFeultact)
        {
            int ret = -1;

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "SetLineaPedidoRechazado";
                _oCommand.Parameters.Add("@pedID", SqlDbType.Int).Value = pedID;
                _oCommand.Parameters.Add("@sCodArticulo", SqlDbType.VarChar, 13).Value = sCodArticulo;
                _oCommand.Parameters.Add("@iCantidad", SqlDbType.Int).Value = iCantidad;
                _oCommand.Parameters.Add("@fBonificacion", SqlDbType.Float).Value = fBonificacion;
                _oCommand.Parameters.Add("@fDescuento", SqlDbType.Float).Value = fDescuento;
                _oCommand.Parameters.Add("@dFeultact", SqlDbType.DateTime).Value = dFeultact;
                _oCommand.Parameters.Add("@Res", SqlDbType.Int).Direction = ParameterDirection.Output;

                _oCommand.ExecuteNonQuery();

                ret = int.Parse(_oCommand.Parameters["@Res"].Value.ToString());
            }
            catch (Exception e)
            { }
            finally
            {
                sqlConn.Close();
            }

            return ret;
        }

        //---- GSG (06/02/2016)
        public bool EsEstupefaciente(SqlConnection sqlConn, string psCodNacional)
        {
            bool ret = false;

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "GetEsEstupefaciente";
                _oCommand.Parameters.Add("@sCodNacional", SqlDbType.VarChar, 18).Value = psCodNacional;
                _oCommand.Parameters.Add("@Res", SqlDbType.VarChar, 4).Direction = ParameterDirection.Output;

                _oCommand.ExecuteNonQuery();

                int retorno = int.Parse(_oCommand.Parameters["@Res"].Value.ToString());
                if (retorno == 1)
                    ret = true;
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

        public dsSPR.ListaEstupefacientesDataTable getEstupefacientes(SqlConnection sqlConn)
        {
            dsSPR.ListaEstupefacientesDataTable dt = new dsSPR.ListaEstupefacientesDataTable();

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "ListaEstupefacientes";

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


        public dsSPR.ListaCodsMaterialesRechazoDataTable getCodsRechazo(SqlConnection sqlConn, string psTipoPedido)
        {
            dsSPR.ListaCodsMaterialesRechazoDataTable dt = new dsSPR.ListaCodsMaterialesRechazoDataTable();

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "ListaCodsMaterialesRechazo";
                _oCommand.Parameters.Add("@sTipoPedido", SqlDbType.VarChar, 6).Value = psTipoPedido;

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


        //---- GSG (03/03/2016)
        public int EsComercializadoInactivo(SqlConnection sqlConn, string psCodNacional)
        {
            int ret = 0;

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "EsComercializadoInactivo";
                _oCommand.Parameters.Add("@sCodNacional", SqlDbType.VarChar, 6).Value = psCodNacional;
                _oCommand.Parameters.Add("@Res", SqlDbType.Int, 4).Direction = ParameterDirection.Output;

                _oCommand.ExecuteNonQuery();

                ret = int.Parse(_oCommand.Parameters["@Res"].Value.ToString());
            }
            catch (Exception e) { string error = e.Message; }
            finally
            {
                sqlConn.Close();
            }

            return ret;
        }

        //---- GSG (14/07/2016)
        public string getCNRegalo(SqlConnection sqlConn, string psCodNacional)
        {
            string ret = null;

            try
            {
                _oCommand.Parameters.Clear();
                _oCommand.Connection = sqlConn;
                _oCommand.CommandText = "GetCNRegalo";
                _oCommand.Parameters.Add("@sCodNacional", SqlDbType.VarChar, 6).Value = psCodNacional;
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

    }
}



