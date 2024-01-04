using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using BLL;
using DAL;
using System.Collections;


namespace Comuns
{
    public class Utils
    {
        Connexions conns = new Connexions();
        dsSPR.ListaCondicionesRegaloDataTable _dt = new dsSPR.ListaCondicionesRegaloDataTable();

        //---- GSG (02/07/2012)
        //public int ExisteCliente(int iIdCliente)
        //{
        //    adGESTCRMCE adCE = new adGESTCRMCE();
        //    return adCE.ExisteCliente(conns.sqlConnCE, iIdCliente);
        //}

        public int ExisteCliente(string sIdCliente)
        {
            adGESTCRMCE adCE = new adGESTCRMCE();
            return adCE.ExisteCliente(conns.sqlConnCE, sIdCliente);
        }

        
        public int StockMaterial(string sCodNacional)
        {
            adGESTCRMCE adCE = new adGESTCRMCE();
            return adCE.StockMaterial(conns.sqlConnCE, sCodNacional);

        }

        //---- FI GSG

        public int ExistePedido(string sIdPedido)
        {
            adGESTCRMCE adCE = new adGESTCRMCE();
            return adCE.ExistePedido(conns.sqlConnCE, sIdPedido);

        }


        public int ExisteCodNacional(string sCodNacional) //---- GSG (18/05/2012)
        {
            adGESTCRMCE adCE = new adGESTCRMCE();
            return adCE.ExisteCodNacional(conns.sqlConnCE, sCodNacional);
        }

        public int EnQueEstadoExisteCodNacional(string sCodNacional) //---- GSG (10/06/2013)
        {
            adGESTCRMCE adCE = new adGESTCRMCE();
            return adCE.EnQueEstadoExisteCodNacional(conns.sqlConnCE, sCodNacional);
        }

        public string NombreMaterial(string sCodNacional) //---- GSG (05/07/2012)
        {
            adGESTCRMCE adCE = new adGESTCRMCE();
            return adCE.NombreMaterial(conns.sqlConnCE, sCodNacional);
        }

        public float PrecioMaterial(string sCodNacional) //---- GSG (13/02/2013)
        {
            adGESTCRMCE adCE = new adGESTCRMCE();
            return adCE.PrecioMaterial(conns.sqlConnCE, sCodNacional);
        }

        //---- GSG (15/05/2014)
        public float PVPMaterial(string sCodSAP, string sCodNacional, DateTime dFecha) 
        {
            adPRECIOS adPRECIOS = new adPRECIOS();
            return adPRECIOS.PVPMaterial(conns.sqlConnPRECIOS, sCodSAP, sCodNacional, dFecha);
        }


        
        //---- GSG (24/02/2016)
        //public int ExistePedidoEnRecepcion(string sCodPedido, string sCodCliente,)
        public int ExistePedidoEnRecepcion(string sCodPedido, string sCodCliente, DateTime fechaEnvio, int totalLineas, int totalUnidades)
        {
            adSPR adSPR = new adSPR();
            //return adSPR.ExistePedidoEnRecepcion(conns.sqlConnWS, sCodPedido, sCodCliente);
            return adSPR.ExistePedidoEnRecepcion(conns.sqlConnWS, sCodPedido, sCodCliente, fechaEnvio, totalLineas, totalUnidades);
        }


        public int InsertarCabeceraPedido(string sCodCli, string sCodPed, string sTipoPedido,
                                          string sCondServicio, string sCargoCoop, string sAplazCargo, string sAplazPago,
                                          double nDtoCargoAdicional, string sEmpFacturadora, string sAlmacenServ,
                                          DateTime dFechaEnvio, string sDiaEnvPedido, int iTotalLineas, int iTotalBonif,
                                          DateTime dFeultact, int iTotalQty, int iDistribuidor, string sFicheroDatos)
        {
            adSPR adSPR = new adSPR();
            return adSPR.InsertarCabeceraPedido(conns.sqlConnWS, sCodCli, sCodPed, sTipoPedido, sCondServicio, 
                                                sCargoCoop, sAplazCargo, sAplazPago, nDtoCargoAdicional, sEmpFacturadora, 
                                                sAlmacenServ, dFechaEnvio, sDiaEnvPedido, iTotalLineas, iTotalBonif,
                                                dFeultact, iTotalQty, iDistribuidor, sFicheroDatos);

        }


        public int InsertarLineaPedido(int pedID, string sCodArticulo, int iCantidad,
                                       double fBonificacion, double fDescuento, DateTime dFeultact)
        {
            adSPR adSPR = new adSPR();
            return adSPR.InsertarLineaPedido(conns.sqlConnWS, pedID, sCodArticulo, iCantidad,  
                                             fBonificacion, fDescuento, dFeultact);

        }

        //---- GSG (24/11/2015)
        public int InsertarCabeceraPedidoRechazado(string sCodCli, string sCodPed, string sTipoPedido,
                                          string sCondServicio, string sCargoCoop, string sAplazCargo, string sAplazPago,
                                          double nDtoCargoAdicional, string sEmpFacturadora, string sAlmacenServ,
                                          DateTime dFechaEnvio, string sDiaEnvPedido, int iTotalLineas, int iTotalBonif,
                                          DateTime dFeultact, int iTotalQty, int iDistribuidor, string sFicheroDatos)
        {
            adSPR adSPR = new adSPR();
            return adSPR.InsertarCabeceraPedidoRechazado(conns.sqlConnWS, sCodCli, sCodPed, sTipoPedido, sCondServicio,
                                                sCargoCoop, sAplazCargo, sAplazPago, nDtoCargoAdicional, sEmpFacturadora,
                                                sAlmacenServ, dFechaEnvio, sDiaEnvPedido, iTotalLineas, iTotalBonif,
                                                dFeultact, iTotalQty, iDistribuidor, sFicheroDatos);

        }


        public int InsertarLineaPedidoRechazado(int pedID, string sCodArticulo, int iCantidad,
                                       double fBonificacion, double fDescuento, DateTime dFeultact)
        {
            adSPR adSPR = new adSPR();
            return adSPR.InsertarLineaPedidoRechazado(conns.sqlConnWS, pedID, sCodArticulo, iCantidad,
                                             fBonificacion, fDescuento, dFeultact);

        }

        //---- GSG (14/09/2012)
        //public void InsertarLineaLog(string sCodCli, string sCodPed, string sDescError, DateTime dFeultact, string sTipoLin)
        //{
        //    adSPR adSPR = new adSPR();
        //    adSPR.InsertarLineaLog(conns.sqlConnWS, sCodCli, sCodPed, sDescError, dFeultact, sTipoLin);
        //}

        //---- GSG (18/02/2016)
        //public void InsertarLineaLog(string sCodCli, string sCodPed, string sDescError, DateTime dFeultact, string sTipoLin, string sNomFit, bool bAceptado)
        //{
        //    adSPR adSPR = new adSPR();
        //    adSPR.InsertarLineaLog(conns.sqlConnWS, sCodCli, sCodPed, sDescError, dFeultact, sTipoLin, sNomFit, bAceptado);
        //}

        //---- GSG (18/10/2019)
        //public void InsertarLineaLog(string sCodCli, string sCodPed, string sDescError, DateTime dFeultact, string sTipoLin, string sNomFit, bool bAceptado, int idPedido)
        //{
        //    adSPR adSPR = new adSPR();
        //    adSPR.InsertarLineaLog(conns.sqlConnWS, sCodCli, sCodPed, sDescError, dFeultact, sTipoLin, sNomFit, bAceptado, idPedido);
        //}
        public void InsertarLineaLog(string sCodCli, string sCodPed, string sDescError, DateTime dFeultact, string sTipoLin, string sNomFit, bool bAceptado, int idPedido, string sCantidad)
        {
            adSPR adSPR = new adSPR();
            if (sCantidad == null || sCantidad == "")
                sCantidad = "0";
            adSPR.InsertarLineaLog(conns.sqlConnWS, sCodCli, sCodPed, sDescError, dFeultact, sTipoLin, sNomFit, bAceptado, idPedido, int.Parse(sCantidad));
        }

        //---- GSG (20/12/2012)
        public int EsCodNacionalCH(string sCodNacional) 
        {
            adGESTCRMCE adCE = new adGESTCRMCE();
            return adCE.EsCodNacionalCH(conns.sqlConnCE, sCodNacional);
        }

        
        //---- GSG (13/02/2013)
        public string getConfguracion(string sConfig)
        {
            string ret = "";
            adSPR adSPR = new adSPR();

            ret = adSPR.GetConfiguracion(conns.sqlConnWS, sConfig);

            return ret;
        }

        //---- GSG (17/07/2013)
        public bool updConfiguracion(string sConfig, string sValor)
        {
            bool bRet = true;
            adSPR adSPR = new adSPR();

            bRet = adSPR.UpdateConfiguracion(conns.sqlConnWS, sConfig, sValor);

            return bRet;
        }



        //---- GSG (15/04/2013)
        //Retorno:  -1 si no és material aparte
        //           0 es material aparte
        //           1 es material aparte ch
        public int EsCodNacionalAparte(string sCodNacional)
        {
            adSPR adSPR = new adSPR();
            return adSPR.EsCodNacionalAparte(conns.sqlConnWS, sCodNacional);
        }
        

        //---- GSG (26/05/2015)
        //Obtiene la lista de materiales aparte
        public List<dsSPR.ListaMaterialesAparteRow> GetListaMaterialesAparte()
        {
            adSPR adSPR = new adSPR();
            dsSPR.ListaMaterialesAparteDataTable dt = new dsSPR.ListaMaterialesAparteDataTable();

            dt = adSPR.GetListaCodNacionalAparte(conns.sqlConnWS);
            int numCods = dt.Count;

            if (numCods > 0)
            {
                //SCS 14/12/2022
                List<dsSPR.ListaMaterialesAparteRow> lRet = new List<dsSPR.ListaMaterialesAparteRow>();

                foreach (dsSPR.ListaMaterialesAparteRow row in dt)
                {
                    lRet.Add(row);
                }

                return lRet;
                //List<string[]> lRet = new List<string[]>();

                //foreach (dsSPR.ListaMaterialesAparteRow row in dt)
                //{
                //    //string[] lin = (string[])dt.Rows[0].ItemArray;
                //    //string[] lin = new string[6];
                //    string[] lin = new string[7]; //---- GSG (17/06/2019)
                //    //string[] lin = Array.ConvertAll<object, string>(dt.Rows[0].ItemArray, ConvertObjectToString);
                //    lin = Array.ConvertAll<object, string>(row.ItemArray, ConvertObjectToString);
                //    lRet.Add(lin);
                //}

            }

            return null;
        }

        string ConvertObjectToString(object obj)
        {
            return (obj == null) ? string.Empty : obj.ToString(); 
        }

        //Obtiene la lista de ficheros a generar para materiales aparte
        public List<string> GetFicherosAparte()
        {
            adSPR adSPR = new adSPR();
            dsSPR.FicherosDataTable dt = new dsSPR.FicherosDataTable();

            dt = adSPR.GetFicherosAparte(conns.sqlConnWS);
            int numFits = dt.Count;

            if (numFits > 0)
            {
                //SCS 14/12/2022
                List<string> lRet = new List<string>();

                foreach (dsSPR.FicherosRow row in dt)
                {
                    lRet.Add(row[0].ToString());
                }


                //string[] lRet = new string[numFits];
                //int index = 0;

                //foreach (dsSPR.FicherosRow row in dt)
                //{
                //    lRet[index] = row[0].ToString();
                //    index++;
                //}

                return lRet;
            }

            return null;
        }

        
        //---- GSG (16/04/2013)
        public bool EstaEnLista(string caden, string[] lista)
        {
            bool ret = false;

            for (int i = 0; i < lista.Length; i++)
            {
                if (lista[i] == null)
                    i = lista.Length;
                else
                {
                    if (lista[i] == caden)
                    {
                        ret = true;
                        i = lista.Length;
                    }
                }
            }

            return ret;
        }



        public List<ArrayList> GetListaCondicionesRegalo()
        {
            adSPR adSPR = new adSPR();
            _dt.Clear(); //---- GSG (26/05/2015)
            
            _dt = adSPR.GetListaCondicionesRegalo(conns.sqlConnWS);
            

            int numCods = _dt.Count;

            List<ArrayList> tablaContadorRegalos = new List<ArrayList>();

            if (numCods > 0)
            {
                string codRegaloAnt = "";
                string codRegalo = "";
                int iMin = 0;
                int iCont = 0;
                int iUnidadesRegalo = 0;
                bool bMultiplo = false;
                bool bEsRegalo = false;
                string sTipo = ""; 
                bool bParte = false;
                string sGrupo = "";
                string sFichero = ""; //---- GSG (26/05/2015)

                foreach (DataRow row in _dt.Rows)
                {
                    // A diferencia del CRM aquí treballo amb cods nacionals
                    //codRegalo = row["sIdMaterialRegalo"].ToString().Trim(); 
                    codRegalo = row["sCodNacionalNoRegalo"].ToString().Trim();

                    if (codRegalo != codRegaloAnt) 
                    {
                        ArrayList linia = new ArrayList();

                        iMin = int.Parse(row["iUnidadesMin"].ToString());
                        iUnidadesRegalo = int.Parse(row["iUnidadesRegalar"].ToString());
                        bMultiplo = bool.Parse(row["bAplicarMultiplo"].ToString());
                        bEsRegalo = bool.Parse(row["bEsRegalo"].ToString());
                        sTipo = row["sIdTipoPedido"].ToString();
                        bParte = bool.Parse(row["bPartePedido"].ToString());
                        sGrupo = row["sGrupo"].ToString();
                        sFichero = row["sFichero"].ToString();

                        linia.Add(codRegalo);
                        linia.Add(iMin);
                        linia.Add(iCont); //Aquí després hi guardarem la suma dels que venen a la comanda
                        linia.Add(iUnidadesRegalo);
                        linia.Add(bMultiplo);
                        linia.Add(bEsRegalo);
                        linia.Add(sTipo);
                        linia.Add(bParte);
                        linia.Add(sGrupo);
                        linia.Add(sFichero);


                        tablaContadorRegalos.Add(linia);
                    }

                    codRegaloAnt = codRegalo;

                }
            }

            return tablaContadorRegalos;
        }


        //Devuelve la lista de regalos en los que interviene un material (normalmente será uno)
        //un mateix material pot estar a més d'un grup
        public List<string> getGruposRegalo(string psCodNacional)
        {
            string codMaterial = "";
            string codRegalo = "";
            List<string> codsRegalo = new List<string>();

            //---- GSG (26/05/2015)
            adSPR adSPR = new adSPR();
            _dt.Clear();
            _dt = adSPR.GetListaCondicionesRegalo(conns.sqlConnWS);
            //---- FI GSG

            foreach (DataRow row in _dt.Rows)
            {
                codMaterial = row["sCodNacional"].ToString().Trim();
                if (codMaterial == psCodNacional)
                {
                    codRegalo = row["sCodNacionalNoRegalo"].ToString().Trim();

                    if (codsRegalo.IndexOf(codRegalo) < 0)
                        codsRegalo.Add(codRegalo);
                }               
            }

            return codsRegalo;

           

        }

        //---- GSG (10/06/2013)
        public bool EsClienteSinCodsPedido(string sCodCliente)
        {
            adSPR adSPR = new adSPR();
            return adSPR.EsClienteSinCodsPedido(conns.sqlConnWS, sCodCliente);
        }

        //---- GSG (28/06/2013)
        public int EsDistribuidorOK(string sCodDistribuidor)
        {
            adGESTCRMCE adCE = new adGESTCRMCE();
            return adCE.EsDistribuidorOK(conns.sqlConnCE, sCodDistribuidor);
        }

        //---- GSG (05/03/2014)
        public string  EsMaterialSustituido(string sCodNacional)
        {
            adGESTCRMCE adCE = new adGESTCRMCE();
            return adCE.EsMaterialSustituido(conns.sqlConnCE, sCodNacional);
        }

        //----- GSG (12/05/2016)
        public string  GetSustituto(string sCodNacional, string sTipoPedido, string sCodCliente)
        {
            adGESTCRMCE adCE = new adGESTCRMCE();
            return adCE.GetSustituto(conns.sqlConnCE, sCodNacional, sTipoPedido, sCodCliente);
        }

        public int GetUnidadesBaseSustituto(string sCodNacional)
        {
            adGESTCRMCE adCE = new adGESTCRMCE();
            return adCE.GetUnidadesBaseSustituto(conns.sqlConnCE, sCodNacional);
        }
        


        //---- GSG (26/05/2015)
        public string GetFicheroMaterial(string sIdMaterial, bool bCH, string sTipoPedido)
        {
            adSPR adSPR = new adSPR();
            return adSPR.GetFicheroMaterial(conns.sqlConnWS, sIdMaterial, bCH, sTipoPedido);
        }

        public string GetFicheroMaterialRegalo(string sIdMaterialRegalo, string sGrupo, string sTipoPedido)
        {
            adSPR adSPR = new adSPR();
            return adSPR.GetFicheroMaterialRegalo(conns.sqlConnWS, sIdMaterialRegalo, sGrupo, sTipoPedido);
        }

        //---- GSG (19/11/2015)
        public bool EsMaterialNoComercializado(string sCodNacional, string sTipoPedido)
        {
            adSPR adSPR = new adSPR();
            return adSPR.EsMaterialRechazo(conns.sqlConnWS, sCodNacional, sTipoPedido);
        }

        //---- GSG (05/02/2016)
        public bool EsEstupefaciente(string sCodNacional)
        {
            adSPR adSPR = new adSPR();
            return adSPR.EsEstupefaciente(conns.sqlConnWS, sCodNacional);
        }

        public List<string> getEstupefacientes()
        {
            adSPR adSPR = new adSPR();
            dsSPR.ListaEstupefacientesDataTable dt = new dsSPR.ListaEstupefacientesDataTable();

            dt = adSPR.getEstupefacientes(conns.sqlConnWS);
            int numCods = dt.Count;

            if (numCods > 0)
            {
                List<string> lRet = new List<string>();

                foreach (dsSPR.ListaEstupefacientesRow row in dt)
                {
                    lRet.Add(row.sCodNacional);
                }

                return lRet;
            }

            return null;
        }

        public List<string> getNoComercializados(string sTipoPedido)
        {
            adSPR adSPR = new adSPR();
            dsSPR.ListaCodsMaterialesRechazoDataTable dt = new dsSPR.ListaCodsMaterialesRechazoDataTable();

            dt = adSPR.getCodsRechazo(conns.sqlConnWS, sTipoPedido);
            int numCods = dt.Count;

            if (numCods > 0)
            {
                List<string> lRet = new List<string>();

                foreach (dsSPR.ListaCodsMaterialesRechazoRow row in dt)
                {
                    lRet.Add(row.sCodNacional);
                }

                return lRet;
            }

            return null;
        }

        //---- GSG (12/05/2016)
        //public List<string> getCodsConSustituto()
        //{
        //    adGESTCRMCE adCE = new adGESTCRMCE();
        //    dsGESTCRMCE.ListaMaterialesConSustitutoDataTable dt = new dsGESTCRMCE.ListaMaterialesConSustitutoDataTable();

        //    dt = adCE.getCodsConSustituto(conns.sqlConnWS);
        //    int numCods = dt.Count;

        //    if (numCods > 0)
        //    {
        //        List<string> lRet = new List<string>();

        //        foreach (dsGESTCRMCE.ListaMaterialesConSustitutoRow row in dt)
        //        {
        //            lRet.Add(row.sCodNacional);
        //        }

        //        return lRet;
        //    }

        //    return null;
        //}
        public List<string> getCodsConSustituto(string sTipoPedido, string sCodCliente)
        {
            adGESTCRMCE adCE = new adGESTCRMCE();
            dsGESTCRMCE.ListaMaterialesConSustitutoDataTable dt = new dsGESTCRMCE.ListaMaterialesConSustitutoDataTable();

            dt = adCE.getCodsConSustituto(conns.sqlConnWS, sTipoPedido, sCodCliente);
            int numCods = dt.Count;

            if (numCods > 0)
            {
                List<string> lRet = new List<string>();

                foreach (dsGESTCRMCE.ListaMaterialesConSustitutoRow row in dt)
                {
                    lRet.Add(row.sCodNacional);
                }

                return lRet;
            }

            return null;
        }

        //---- GSG (06/02/2017)
        public List<Item> getListaSustitucion(string sTipoPedido, string sCodCliente)
        {
            adGESTCRMCE adCE = new adGESTCRMCE();
            dsGESTCRMCE.ListaSustitucionMaterialesDataTable dt = new dsGESTCRMCE.ListaSustitucionMaterialesDataTable();

            dt = adCE.getMaterialesSustitucion(conns.sqlConnWS, sTipoPedido, sCodCliente);
            int numCods = dt.Count;

            if (numCods > 0)
            {
                List<Item> lRet = new List<Item>();

                foreach (dsGESTCRMCE.ListaSustitucionMaterialesRow row in dt)
                {
                    Item item = new Item();

                    item.s1 = row.sCodNacional;
                    item.s2 = row.sCodNacionalSustituto;

                    lRet.Add(item);
                }

                return lRet;
            }

            return null;
        }



        //---- GSG (03/03/2015)
        public int EsComercializadoInactivo(string sCodNacional)
        {
            adSPR adSPR = new adSPR();
            return adSPR.EsComercializadoInactivo(conns.sqlConnPRECIOS, sCodNacional);
        }


        //---- GSG (12/05/2016)
        public List<string> getCodsBloqueados(string sTipoPedido, string sCodCliente)
        {
            adGESTCRMCE adCE = new adGESTCRMCE();
            dsGESTCRMCE.ListaMaterialesBloqueadosDataTable dt = new dsGESTCRMCE.ListaMaterialesBloqueadosDataTable();

            dt = adCE.getBloqueados(conns.sqlConnCE, sTipoPedido, sCodCliente);
            int numCods = dt.Count;

            if (numCods > 0)
            {
                List<string> lRet = new List<string>();

                foreach (dsGESTCRMCE.ListaMaterialesBloqueadosRow row in dt)
                {
                    lRet.Add(row.sCodNacional);
                }

                return lRet;
            }

            return null;
        }

        //---- GSG (07/07/2016)

        public string getCondicionServicio(string sCodCliente)
        {            
            adGESTCRMCE adCE = new adGESTCRMCE();
            dsGESTCRMCE.ListaMayoristasClientes_SAPDataTable dt = new dsGESTCRMCE.ListaMayoristasClientes_SAPDataTable();
            string sMayorista = "";
            int iIdCliente = -1;

            iIdCliente = adCE.GetIdCliente(conns.sqlConnCE, sCodCliente);


            dt = adCE.getMayoristas(conns.sqlConnCE, iIdCliente);
            int numCods = dt.Count;

            if (numCods > 0)
            {
                foreach (dsGESTCRMCE.ListaMayoristasClientes_SAPRow row in dt)
                {
                    sMayorista = row.sIdCliente;
                    break;
                }
            }

            return sMayorista;
        }

        
        public static bool IsNumeric(string val)
        {
            try
            {
                float.Parse(val);
                return true;
            }
            catch
            {
                return false;
            }
        }


        //---- GSG (14/07/2016)
        public string getCNRegalo(string sCodNacional)
        {
            adSPR adSPR = new adSPR();
            return adSPR.getCNRegalo(conns.sqlConnWS, sCodNacional);
        }



        //---- GSG (22/05/2018)
        public List<Item> getConversionMayoristas(string mayorista)
        {
            adGESTCRMCE adCE = new adGESTCRMCE();
            dsGESTCRMCE.SRMayoristasDataTable dt = new dsGESTCRMCE.SRMayoristasDataTable();

            dt = adCE.GetSRMayoristas(conns.sqlConnCE, mayorista);
            int numCods = dt.Count;

            if (numCods > 0)
            {
                List<Item> lRet = new List<Item>();

                foreach (dsGESTCRMCE.SRMayoristasRow row in dt)
                {
                    Item item = new Item();

                    item.s1 = row.codMay;
                    item.s2 = row.codMayEquivalente;

                    lRet.Add(item);
                }

                return lRet;
            }

            return null;
        }

    }


    public class Item
    {
        public string s1 { get; set; }
        public string s2 { get; set; }
    }
}
