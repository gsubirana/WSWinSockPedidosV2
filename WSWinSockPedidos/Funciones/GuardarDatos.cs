using Comuns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSWinSockPedidos.Funciones
{
    class GuardarDatos
    {
        Utils U = new Utils();

        #region Funciones Guardar Datos

        //---- GSG (24/11/2015)
        //private int GuardarCapPedidoBD(string sCapcalera, int iTotalLineas, int iTotalQty, int iTotalBonif, bool bCodsNacCH, bool bAparte, bool bAparteCH)
        public int GuardarCapPedidoBD(
            string sCapcalera, int iTotalLineas, int iTotalQty, int iTotalBonif, bool bCodsNacCH, bool bAparte, bool bAparteCH, bool bPedidoOK,
            string _sNomfitxerDesti, string DEFAULT_FILE_STORE_LOC, string _sCodPed)
        {
            int iPedID = -1;

            //Agafa valors de la línia de capçalera
            string sCodCli = sCapcalera.Substring(4, 16);
            //string sCodPed = sCapcalera.Substring(20, 10);
            string sTipoPedido = sCapcalera.Substring(30, 6);
            string sCondServicio = sCapcalera.Substring(36, 6);
            string sCargoCoop = sCapcalera.Substring(42, 1);
            string sAplazCargo = sCapcalera.Substring(43, 3);
            string psAplazPago = sCapcalera.Substring(46, 3);
            string sDtoCargoAd = sCapcalera.Substring(49, 4); //Les dues últimes posicions són els decimals
            sDtoCargoAd = sDtoCargoAd.Substring(0, 2) + "," + sDtoCargoAd.Substring(2, 2); //Posa la coma dels decimals
            double nDtoCargoAdicional = double.Parse(sDtoCargoAd);
            string sEmpFacturadora = sCapcalera.Substring(53, 3);
            string sAlmacenServ = sCapcalera.Substring(56, 4);
            string sFecha = sCapcalera.Substring(66, 2) + "/" + sCapcalera.Substring(64, 2) + "/" + sCapcalera.Substring(60, 4);
            DateTime dFechaEnvio;
            if (DateTime.TryParse(sFecha, out dFechaEnvio))
                dFechaEnvio = Convert.ToDateTime(sFecha);
            else
                dFechaEnvio = System.DateTime.Now;
            string sDiaEnvPedido = sCapcalera.Substring(68, 2);
            //Data actual
            DateTime dFeultact = System.DateTime.Now;
            //Aquests valors no els tenim (a client sí)
            int iDistribuidor = -1;
            string sFicheroDatos = "";

            sFicheroDatos = _sNomfitxerDesti.Replace(DEFAULT_FILE_STORE_LOC, "");


            if (bPedidoOK)
                iPedID = U.InsertarCabeceraPedido(sCodCli, _sCodPed, sTipoPedido, sCondServicio,
                                                 sCargoCoop, sAplazCargo, psAplazPago, nDtoCargoAdicional,
                                                 sEmpFacturadora, sAlmacenServ, dFechaEnvio, sDiaEnvPedido,
                                                 iTotalLineas, iTotalBonif, dFeultact, iTotalQty, iDistribuidor,
                                                 sFicheroDatos);
            else //---- GSG (24/11/2015)
                iPedID = U.InsertarCabeceraPedidoRechazado(sCodCli, _sCodPed, sTipoPedido, sCondServicio,
                                             sCargoCoop, sAplazCargo, psAplazPago, nDtoCargoAdicional,
                                             sEmpFacturadora, sAlmacenServ, dFechaEnvio, sDiaEnvPedido,
                                             iTotalLineas, iTotalBonif, dFeultact, iTotalQty, iDistribuidor,
                                             sFicheroDatos);


            return iPedID;
        }

        //---- GSG (24/11/2015)
        //private int GuardarLinPedidoBD(string sLin, int pedID) 
        public int GuardarLinPedidoBD(string sLin, int pedID, bool bPedidoOK)
        {
            int iLinID = -1;

            string sCodArticulo = sLin.Substring(4, 13);
            int iCantidad = int.Parse(sLin.Substring(17, 4).Trim());
            double fBonificacion = 0.0;
            double fDescuento = 0.0;
            //Data actual
            DateTime dFeultact = System.DateTime.Now;

            //---- GSG (05/02/2016)
            string tipoLin = sLin.Substring(0, 4);
            //if (sLin.Length > 23) //Línia amb bonificació
            if (tipoLin == "1030") //Línia amb bonificació (perquè ara la 1040 tb té longitud > 23)
            {
                string auxDouble = sLin.Substring(21, 4);
                auxDouble = auxDouble.Substring(0, 2) + "," + auxDouble.Substring(2, 2);
                fBonificacion = double.Parse(auxDouble.Trim());

                auxDouble = sLin.Substring(25, 4);
                auxDouble = auxDouble.Substring(0, 2) + "," + auxDouble.Substring(2, 2);
                fDescuento = double.Parse(auxDouble.Trim());
            }

            if (bPedidoOK)
                iLinID = U.InsertarLineaPedido(pedID, sCodArticulo, iCantidad, fBonificacion, fDescuento, dFeultact);
            else  //---- GSG (24/11/2015)
                iLinID = U.InsertarLineaPedidoRechazado(pedID, sCodArticulo, iCantidad, fBonificacion, fDescuento, dFeultact);

            return iLinID;
        }

        //---- GSG (18/10/2019)
        //private void GuardarLinLogBD(string sCodCli, string sCodPed, string sDescError, string sTipoLin)
        public void GuardarLinLogBD(
            string sCodCli, string sCodPed, string sDescError, string sTipoLin, string sCantidad,
            string _sNomfitxerDesti, string DEFAULT_FILE_STORE_LOC, int _iPedIdR)
        {
            DateTime dFeultact = System.DateTime.Now;

            string sFicheroDatos = "";
            sFicheroDatos = _sNomfitxerDesti.Replace(DEFAULT_FILE_STORE_LOC, "");

            //---- GSG (18/02/2016)
            //U.InsertarLineaLog(sCodCli, sCodPed, sDescError, dFeultact, sTipoLin, sFicheroDatos, false);
            //---- GSG (18/10/2019)
            //U.InsertarLineaLog(sCodCli, sCodPed, sDescError, dFeultact, sTipoLin, sFicheroDatos, false, _iPedIdR);
            U.InsertarLineaLog(sCodCli, sCodPed, sDescError, dFeultact, sTipoLin, sFicheroDatos, false, _iPedIdR, sCantidad);
        }

        #endregion
    }
}