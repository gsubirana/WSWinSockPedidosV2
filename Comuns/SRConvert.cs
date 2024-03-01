using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Comuns
{
    public class SRConvert
    {
        private System.Data.SqlClient.SqlDataAdapter sqldaSRTiposPedido;
        private System.Data.SqlClient.SqlDataAdapter sqldaSRTiposPosicion;
        private System.Data.SqlClient.SqlDataAdapter sqldaSROrgVentas;
        private System.Data.SqlClient.SqlDataAdapter sqldaSRCanalDist;
        private System.Data.SqlClient.SqlDataAdapter sqldaMatSAPAlemania;
        private System.Data.SqlClient.SqlDataAdapter sqldaSRCondPago;
        private System.Data.SqlClient.SqlDataAdapter sqldaSRClientes;

        private System.Data.SqlClient.SqlCommand sqlCmdSRConversion;
        private System.Data.SqlClient.SqlCommand sqlCmdMatSAPAlemania;
        private System.Data.SqlClient.SqlCommand sqlCmdCondPago;
        private System.Data.SqlClient.SqlCommand sqlCmdClientes;

        protected System.Data.SqlClient.SqlConnection sqlConnGESTCRM;

        private DAL.dsConvertSR _dsConvertSR;

        public SRConvert()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
            this.sqlConnGESTCRM = new System.Data.SqlClient.SqlConnection();
            this.sqlConnGESTCRM.ConnectionString = ((string)(configurationAppSettings.GetValue("CEConnectionString", typeof(string))));
            this.sqlConnGESTCRM.FireInfoMessageEventOnUserErrors = false;


            this.sqldaSRTiposPedido = new System.Data.SqlClient.SqlDataAdapter();
            this.sqldaSRTiposPosicion = new System.Data.SqlClient.SqlDataAdapter();
            this.sqldaSROrgVentas = new System.Data.SqlClient.SqlDataAdapter();
            this.sqldaSRCanalDist = new System.Data.SqlClient.SqlDataAdapter();
            this.sqldaMatSAPAlemania = new System.Data.SqlClient.SqlDataAdapter();
            this.sqldaSRCondPago = new System.Data.SqlClient.SqlDataAdapter();
            this.sqldaSRClientes = new System.Data.SqlClient.SqlDataAdapter();

            this.sqlCmdSRConversion = new System.Data.SqlClient.SqlCommand();
            this.sqlCmdMatSAPAlemania = new System.Data.SqlClient.SqlCommand();
            this.sqlCmdCondPago = new System.Data.SqlClient.SqlCommand();
            this.sqlCmdClientes = new System.Data.SqlClient.SqlCommand();


            this.sqldaSRTiposPedido.SelectCommand = this.sqlCmdSRConversion;
            this.sqldaSRTiposPedido.TableMappings.AddRange(new System.Data.Common.DataTableMapping[] {
                    new System.Data.Common.DataTableMapping("Table", "SRTiposPedidos", new System.Data.Common.DataColumnMapping[] {
                         new System.Data.Common.DataColumnMapping("sID", "sID"),
                         new System.Data.Common.DataColumnMapping("idInterfaz", "idInterfaz"),
                         new System.Data.Common.DataColumnMapping("sCod", "sCod"),
                         new System.Data.Common.DataColumnMapping("sDescripcion", "sDescripcion"),
                         new System.Data.Common.DataColumnMapping("iEstado", "iEstado")})});

            this.sqldaSRTiposPosicion.SelectCommand = this.sqlCmdSRConversion;
            this.sqldaSRTiposPosicion.TableMappings.AddRange(new System.Data.Common.DataTableMapping[] {
                    new System.Data.Common.DataTableMapping("Table", "SRTiposPosicion", new System.Data.Common.DataColumnMapping[] {
                         new System.Data.Common.DataColumnMapping("sID", "sID"),
                         new System.Data.Common.DataColumnMapping("idInterfaz", "idInterfaz"),
                         new System.Data.Common.DataColumnMapping("sCod", "sCod"),
                         new System.Data.Common.DataColumnMapping("sDescripcion", "sDescripcion"),
                         new System.Data.Common.DataColumnMapping("iEstado", "iEstado")})});

            this.sqldaSROrgVentas.SelectCommand = this.sqlCmdSRConversion;
            this.sqldaSROrgVentas.TableMappings.AddRange(new System.Data.Common.DataTableMapping[] {
                    new System.Data.Common.DataTableMapping("Table", "SROrgVentas", new System.Data.Common.DataColumnMapping[] {
                         new System.Data.Common.DataColumnMapping("sID", "sID"),
                         new System.Data.Common.DataColumnMapping("idInterfaz", "idInterfaz"),
                         new System.Data.Common.DataColumnMapping("sCod", "sCod"),
                         new System.Data.Common.DataColumnMapping("sDescripcion", "sDescripcion"),
                         new System.Data.Common.DataColumnMapping("iEstado", "iEstado")})});

            this.sqldaSRCanalDist.SelectCommand = this.sqlCmdSRConversion;
            this.sqldaSRCanalDist.TableMappings.AddRange(new System.Data.Common.DataTableMapping[] {
                    new System.Data.Common.DataTableMapping("Table", "SRCanalDist", new System.Data.Common.DataColumnMapping[] {
                         new System.Data.Common.DataColumnMapping("sID", "sID"),
                         new System.Data.Common.DataColumnMapping("idInterfaz", "idInterfaz"),
                         new System.Data.Common.DataColumnMapping("sCod", "sCod"),
                         new System.Data.Common.DataColumnMapping("sDescripcion", "sDescripcion"),
                         new System.Data.Common.DataColumnMapping("iEstado", "iEstado")})});


            this.sqldaMatSAPAlemania.SelectCommand = this.sqlCmdMatSAPAlemania;
            this.sqldaMatSAPAlemania.TableMappings.AddRange(new System.Data.Common.DataTableMapping[] {
                    new System.Data.Common.DataTableMapping("Table", "dMatSAPAlemania", new System.Data.Common.DataColumnMapping[] {
                         new System.Data.Common.DataColumnMapping("sIdMaterial", "sIdMaterial"),
                         new System.Data.Common.DataColumnMapping("sCodNacional", "sCodNacional"),
                         new System.Data.Common.DataColumnMapping("codAlemania", "codAlemania")})});


            this.sqldaSRCondPago.SelectCommand = this.sqlCmdCondPago;
            this.sqldaSRCondPago.TableMappings.AddRange(new System.Data.Common.DataTableMapping[] {
                    new System.Data.Common.DataTableMapping("Table", "SRCondPago", new System.Data.Common.DataColumnMapping[] {
                         new System.Data.Common.DataColumnMapping("codCondPago", "codCondPago"),
                         new System.Data.Common.DataColumnMapping("codCondPagoEquivalente", "codCondPagoEquivalente")})});


            this.sqldaSRClientes.SelectCommand = this.sqlCmdClientes;
            this.sqldaSRClientes.TableMappings.AddRange(new System.Data.Common.DataTableMapping[] {
                    new System.Data.Common.DataTableMapping("Table", "SRClientes", new System.Data.Common.DataColumnMapping[] {
                         new System.Data.Common.DataColumnMapping("sIdCliente", "sIdCliente"),
                         new System.Data.Common.DataColumnMapping("codCliEquivalente", "codCliEquivalente")})});


            this.sqlCmdSRConversion.Connection = sqlConnGESTCRM;
            this.sqlCmdSRConversion.CommandText = "GetConversionSR";

            this.sqlCmdMatSAPAlemania.Connection = sqlConnGESTCRM;
            this.sqlCmdMatSAPAlemania.CommandText = "GetMatSAPAlemania";

            this.sqlCmdCondPago.Connection = sqlConnGESTCRM;
            this.sqlCmdCondPago.CommandText = "GetSRCondPago";

            this.sqlCmdClientes.Connection = sqlConnGESTCRM;
            this.sqlCmdClientes.CommandText = "GetSRClientes";


            this.sqlCmdSRConversion.CommandType = System.Data.CommandType.StoredProcedure;
            this.sqlCmdSRConversion.Connection = this.sqlConnGESTCRM;
            this.sqlCmdSRConversion.Parameters.AddRange(new System.Data.SqlClient.SqlParameter[] {
            new System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, false, ((byte)(0)), ((byte)(0)), "", System.Data.DataRowVersion.Current, null),
            new System.Data.SqlClient.SqlParameter("@sTabla", System.Data.SqlDbType.VarChar, 50),
            new System.Data.SqlClient.SqlParameter("@idInterfaz", System.Data.SqlDbType.Int, 4),
            new System.Data.SqlClient.SqlParameter("@sID", System.Data.SqlDbType.VarChar, 50),
            new System.Data.SqlClient.SqlParameter("@sCod", System.Data.SqlDbType.VarChar, 50),            
            new System.Data.SqlClient.SqlParameter("@iEstado", System.Data.SqlDbType.SmallInt, 2)});


            this.sqlCmdMatSAPAlemania.CommandType = System.Data.CommandType.StoredProcedure;
            this.sqlCmdMatSAPAlemania.Connection = this.sqlConnGESTCRM;
            this.sqlCmdMatSAPAlemania.Parameters.AddRange(new System.Data.SqlClient.SqlParameter[] {
            new System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, false, ((byte)(0)), ((byte)(0)), "", System.Data.DataRowVersion.Current, null),
            new System.Data.SqlClient.SqlParameter("@sIdMaterial", System.Data.SqlDbType.VarChar, 50),
            new System.Data.SqlClient.SqlParameter("@sCodNacional", System.Data.SqlDbType.VarChar, 6)});


            this.sqlCmdCondPago.CommandType = System.Data.CommandType.StoredProcedure;
            this.sqlCmdCondPago.Connection = this.sqlConnGESTCRM;
            this.sqlCmdCondPago.Parameters.AddRange(new System.Data.SqlClient.SqlParameter[] {
            new System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, false, ((byte)(0)), ((byte)(0)), "", System.Data.DataRowVersion.Current, null),
            new System.Data.SqlClient.SqlParameter("@iTipo", System.Data.SqlDbType.Int, 4),
            new System.Data.SqlClient.SqlParameter("@codCondPago", System.Data.SqlDbType.VarChar, 4),
            new System.Data.SqlClient.SqlParameter("@codDest", System.Data.SqlDbType.VarChar, 10),
            new System.Data.SqlClient.SqlParameter("@tipoPed", System.Data.SqlDbType.VarChar, 4),
            new System.Data.SqlClient.SqlParameter("@orgVentas", System.Data.SqlDbType.VarChar, 4),
            new System.Data.SqlClient.SqlParameter("@codSolicit", System.Data.SqlDbType.VarChar, 10)});


            this.sqlCmdClientes.CommandType = System.Data.CommandType.StoredProcedure;
            this.sqlCmdClientes.Connection = this.sqlConnGESTCRM;
            this.sqlCmdClientes.Parameters.AddRange(new System.Data.SqlClient.SqlParameter[] {
            new System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, false, ((byte)(0)), ((byte)(0)), "", System.Data.DataRowVersion.Current, null),
            new System.Data.SqlClient.SqlParameter("@sIdCliente", System.Data.SqlDbType.VarChar, 20)});


            _dsConvertSR = new DAL.dsConvertSR();
        }


        public DAL.dsConvertSR.SRTiposPedidoDataTable CargarSRTiposPedido()
        {
            try
            {
                _dsConvertSR.SRTiposPedido.Clear();

                if (sqlConnGESTCRM.State.ToString() == "Closed")
                    sqlConnGESTCRM.Open();

                sqldaSRTiposPedido.SelectCommand.Parameters["@sTabla"].Value = "SRTiposPedido";

                sqldaSRTiposPedido.Fill(_dsConvertSR.SRTiposPedido);
            }
            catch (Exception error) { }

            return _dsConvertSR.SRTiposPedido;
        }


        public DAL.dsConvertSR.SRTiposPosicionDataTable CargarSRTiposPosicion()
        {
            try
            {
                _dsConvertSR.SRTiposPosicion.Clear();

                if (sqlConnGESTCRM.State.ToString() == "Closed")
                    sqlConnGESTCRM.Open();

                sqldaSRTiposPosicion.SelectCommand.Parameters["@sTabla"].Value = "SRTiposPosicion";

                sqldaSRTiposPosicion.Fill(_dsConvertSR.SRTiposPosicion);
            }
            catch (Exception error) { }

            return _dsConvertSR.SRTiposPosicion;
        }


        public DAL.dsConvertSR.SROrgVentasDataTable CargarSROrgVentas()
        {
            try
            {
                _dsConvertSR.SROrgVentas.Clear();

                if (sqlConnGESTCRM.State.ToString() == "Closed")
                    sqlConnGESTCRM.Open();

                sqldaSROrgVentas.SelectCommand.Parameters["@sTabla"].Value = "SROrgVentas";

                sqldaSROrgVentas.Fill(_dsConvertSR.SROrgVentas);
            }
            catch (Exception error) { }

            return _dsConvertSR.SROrgVentas;
        }


        public DAL.dsConvertSR.SRCanalDistDataTable CargarSRCanalDist()
        {
            try
            {
                _dsConvertSR.SRCanalDist.Clear();

                if (sqlConnGESTCRM.State.ToString() == "Closed")
                    sqlConnGESTCRM.Open();

                sqldaSRCanalDist.SelectCommand.Parameters["@sTabla"].Value = "SRCanalDist";

                sqldaSRCanalDist.Fill(_dsConvertSR.SRCanalDist);
            }
            catch (Exception error) { }

            return _dsConvertSR.SRCanalDist;
        }


        public DAL.dsConvertSR.MatSAPAlemaniaDataTable CargarMateriales(string mat)
        {
            try
            {
                _dsConvertSR.MatSAPAlemania.Clear();

                if (sqlConnGESTCRM.State.ToString() == "Closed")
                    sqlConnGESTCRM.Open();

                sqldaMatSAPAlemania.SelectCommand.Parameters["@sCodNacional"].Value = mat;

                sqldaMatSAPAlemania.Fill(_dsConvertSR.MatSAPAlemania);
            }
            catch (Exception error) { }

            return _dsConvertSR.MatSAPAlemania;
        }


        public DAL.dsConvertSR.SRCondPagoDataTable CargarSRCondicionPago(int piTipo, string psDestinatario, string psTipoPedido, string psOrgVentas, string psSolicitante)
        {
            try
            {
                _dsConvertSR.SRCondPago.Clear();

                if (sqlConnGESTCRM.State.ToString() == "Closed")
                    sqlConnGESTCRM.Open();

                sqldaSRCondPago.SelectCommand.Parameters["@iTipo"].Value = piTipo;
                sqldaSRCondPago.SelectCommand.Parameters["@codCondPago"].Value = null;
                sqldaSRCondPago.SelectCommand.Parameters["@codDest"].Value = psDestinatario;
                sqldaSRCondPago.SelectCommand.Parameters["@tipoPed"].Value = psTipoPedido;
                sqldaSRCondPago.SelectCommand.Parameters["@orgVentas"].Value = psOrgVentas;
                sqldaSRCondPago.SelectCommand.Parameters["@codSolicit"].Value = psSolicitante;

                sqldaSRCondPago.Fill(_dsConvertSR.SRCondPago);
            }
            catch (Exception error) { }

            return _dsConvertSR.SRCondPago;
        }


        public DAL.dsConvertSR.SRClientesDataTable CargarSRClientes(string sIdCliente)
        {
            try
            {
                _dsConvertSR.SRClientes.Clear();

                if (sqlConnGESTCRM.State.ToString() == "Closed")
                    sqlConnGESTCRM.Open();

                sqldaSRClientes.SelectCommand.Parameters["@sIdCliente"].Value = sIdCliente;

                sqldaSRClientes.Fill(_dsConvertSR.SRClientes);
            }
            catch (Exception error) { }

            return _dsConvertSR.SRClientes;
        }

    }
}