using Comuns;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace WSWinSockPedidos.Funciones
{
    class XML
    {

        //---- GSG (17/04/2018) SAP interfaz SAP rollout

        // Recuperar solo los ficheros creados para generar sus correspondientes XML
        // Habrá que mirar en la carpeta destino y también en Backup por si se da el caso de que ya se ha procesado
        //private bool GenerarXMLs(string fileOrigenGenerics, string fileOrigenCH, List<string> lsfileOrigenGenericsVX, List<string> lsfileOrigenCHVX)
        public bool GenerarXMLs(string fileOrigenGenerics, string fileOrigenCH, List<string> lsfileOrigenGenericsVX, List<string> lsfileOrigenCHVX, bool Ok, 
            string _sCodBotiquin, string DEFAULT_FILE_STORE_XML_NOK_LOC, string DEFAULT_FILE_STORE_XML_LOC, string DEFAULT_FILE_STORE_XML_LOC_DIR, bool _esDirecte, string DEFAULT_FILE_STORE_BACKUP_LOC, string DEFAULT_FILE_STORE_LOC)
        {
            string fileOrigen = "";
            string fileEnBackup = "";
            string fileDestiXML = "";


            //---- Tablas de conversión 

            SRConvert sr = new SRConvert();

            DataTable dtTP = new DataTable("TiposPedido");
            //dtTP = sr.CargarSRTiposPedido();

            //DataTable dtTPos = new DataTable("TiposPosicion");
            //dtTPos = sr.CargarSRTiposPosicion();

            DataTable dtOV = new DataTable("OrgVentas");
            dtOV = sr.CargarSROrgVentas();

            DataTable dtCD = new DataTable("CanalDistribucion");
            //dtCD = sr.CargarSRCanalDist();

            DataTable dtMat = new DataTable("MatSAPAlemania");
            dtMat = sr.CargarMateriales(null);

            DataTable dtCli = new DataTable("SRClientes");
            dtCli = sr.CargarSRClientes(null);


            //-----------------------


            // Genèrics
            fileOrigen = fileOrigenGenerics;
            fileEnBackup = DEFAULT_FILE_STORE_BACKUP_LOC + "\\" + fileOrigenGenerics.Replace(DEFAULT_FILE_STORE_LOC, "");

            string sDiaHora = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') +
                              DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0');
            int index1 = fileOrigenGenerics.IndexOf('_') + 1;
            int index2 = fileOrigenGenerics.IndexOf('.');
            string coletilla = fileOrigenGenerics.Substring(index1, index2 - index1);

            if (Ok)
            {
                //---- GSG (31/10/2019)
                //fileDestiXML = DEFAULT_FILE_STORE_XML_LOC + "Order_Pedido_" + sDiaHora + "_" + coletilla + ".xml";
                if (_esDirecte)
                    fileDestiXML = DEFAULT_FILE_STORE_XML_LOC_DIR + "Order_Pedido_" + sDiaHora + "_" + coletilla + ".xml";
                else
                    fileDestiXML = DEFAULT_FILE_STORE_XML_LOC + "Order_Pedido_" + sDiaHora + "_" + coletilla + ".xml";
            }
            else
                fileDestiXML = DEFAULT_FILE_STORE_XML_NOK_LOC + "Order_Pedido_" + sDiaHora + "_" + coletilla + ".xml";

            if (File.Exists(fileOrigen))
                parseOrder(fileOrigen, fileDestiXML, dtTP, dtOV, dtCD, dtMat, dtCli, _sCodBotiquin);
            else if (File.Exists(fileEnBackup))
                parseOrder(fileEnBackup, fileDestiXML, dtTP, dtOV, dtCD, dtMat, dtCli, _sCodBotiquin);

            // CH
            fileOrigen = fileOrigenCH;
            fileEnBackup = DEFAULT_FILE_STORE_BACKUP_LOC + "\\" + fileOrigenCH.Replace(DEFAULT_FILE_STORE_LOC, "");

            sDiaHora = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') +
                              DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0');
            index1 = fileOrigenCH.IndexOf('_') + 1;
            index2 = fileOrigenCH.IndexOf('.');
            coletilla = fileOrigenCH.Substring(index1, index2 - index1);

            if (Ok)
            {
                //---- GSG (31/10/2019)
                //fileDestiXML = DEFAULT_FILE_STORE_XML_LOC + "Order_Pedido_" + sDiaHora + "_" + coletilla + ".xml";
                if (_esDirecte)
                    fileDestiXML = DEFAULT_FILE_STORE_XML_LOC_DIR + "Order_Pedido_" + sDiaHora + "_" + coletilla + ".xml";
                else
                    fileDestiXML = DEFAULT_FILE_STORE_XML_LOC + "Order_Pedido_" + sDiaHora + "_" + coletilla + ".xml";
            }
            else
                fileDestiXML = DEFAULT_FILE_STORE_XML_NOK_LOC + "Order_Pedido_" + sDiaHora + "_" + coletilla + ".xml";



            if (File.Exists(fileOrigen))
                parseOrder(fileOrigen, fileDestiXML, dtTP, dtOV, dtCD, dtMat, dtCli, _sCodBotiquin);
            else if (File.Exists(fileEnBackup))
                parseOrder(fileEnBackup, fileDestiXML, dtTP, dtOV, dtCD, dtMat, dtCli, _sCodBotiquin);


            // Genèrics Vx
            foreach (string fit in lsfileOrigenGenericsVX)
            {
                fileOrigen = fit;
                fileEnBackup = DEFAULT_FILE_STORE_BACKUP_LOC + "\\" + fit.Replace(DEFAULT_FILE_STORE_LOC, "");

                sDiaHora = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') +
                              DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0');
                index1 = fit.IndexOf('_') + 1;
                index2 = fit.IndexOf('.');
                coletilla = fit.Substring(index1, index2 - index1);

                if (Ok)
                {
                    //---- GSG (31/10/2019)
                    //fileDestiXML = DEFAULT_FILE_STORE_XML_LOC + "Order_Pedido_" + sDiaHora + "_" + coletilla + ".xml";
                    if (_esDirecte)
                        fileDestiXML = DEFAULT_FILE_STORE_XML_LOC_DIR + "Order_Pedido_" + sDiaHora + "_" + coletilla + ".xml";
                    else
                        fileDestiXML = DEFAULT_FILE_STORE_XML_LOC + "Order_Pedido_" + sDiaHora + "_" + coletilla + ".xml";
                }
                else
                    fileDestiXML = DEFAULT_FILE_STORE_XML_NOK_LOC + "Order_Pedido_" + sDiaHora + "_" + coletilla + ".xml";


                if (File.Exists(fileOrigen))
                    parseOrder(fileOrigen, fileDestiXML, dtTP, dtOV, dtCD, dtMat, dtCli, _sCodBotiquin);
                else if (File.Exists(fileEnBackup))
                    parseOrder(fileEnBackup, fileDestiXML, dtTP, dtOV, dtCD, dtMat, dtCli, _sCodBotiquin);
            }


            // CH Vx
            foreach (string fit in lsfileOrigenCHVX)
            {
                fileOrigen = fit;
                fileEnBackup = DEFAULT_FILE_STORE_BACKUP_LOC + "\\" + fit.Replace(DEFAULT_FILE_STORE_LOC, "");

                //fileDestiXML = DEFAULT_FILE_STORE_XML_LOC + fit.Replace(DEFAULT_FILE_STORE_LOC, "").Replace("txt", "xml");                
                sDiaHora = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') +
                              DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0');
                index1 = fit.IndexOf('_') + 1;
                index2 = fit.IndexOf('.');
                coletilla = fit.Substring(index1, index2 - index1);

                if (Ok)
                {
                    //---- GSG (31/10/2019)
                    //fileDestiXML = DEFAULT_FILE_STORE_XML_LOC + "Order_Pedido_" + sDiaHora + "_" + coletilla + ".xml";
                    if (_esDirecte)
                        fileDestiXML = DEFAULT_FILE_STORE_XML_LOC_DIR + "Order_Pedido_" + sDiaHora + "_" + coletilla + ".xml";
                    else
                        fileDestiXML = DEFAULT_FILE_STORE_XML_LOC + "Order_Pedido_" + sDiaHora + "_" + coletilla + ".xml";
                }
                else
                    fileDestiXML = DEFAULT_FILE_STORE_XML_NOK_LOC + "Order_Pedido_" + sDiaHora + "_" + coletilla + ".xml";


                if (File.Exists(fileOrigen))
                    parseOrder(fileOrigen, fileDestiXML, dtTP, dtOV, dtCD, dtMat, dtCli, _sCodBotiquin);
                else if (File.Exists(fileEnBackup))
                    parseOrder(fileEnBackup, fileDestiXML, dtTP, dtOV, dtCD, dtMat, dtCli, _sCodBotiquin);
            }

            return true;
        }


        public bool parseOrder(string fitxerOrigen, string fitxerXML, DataTable dtTP, DataTable dtOV, DataTable dtCD, DataTable dtMat, DataTable dtCli, string _sCodBotiquin) //DataTable dtTPos, DataTable dtOV, DataTable dtCD)
        {
            StreamReader sr = new StreamReader(@fitxerOrigen);

            string K_DistChannel = "03"; // En les autocomandes el valor del canal de distribució sempre serà el mateix

            // Determina el número de materiales
            string file = sr.ReadToEnd();
            string[] allLines = file.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            int numLin = allLines.Length - 4;
            int iLin = 1;
            string lin = "";

            sr.Dispose();
            sr.Close();


            // Pedido XML
            Order xmlOrder = new Order();

            // Cabecera
            xmlOrder.OrderHeader = new OrderOrderHeader();
            xmlOrder.OrderHeader.Dates = new OrderOrderHeaderDates();
            xmlOrder.OrderHeader.Partners = new OrderOrderHeaderPartner[2];
            xmlOrder.OrderHeader.Partners[0] = new OrderOrderHeaderPartner();
            xmlOrder.OrderHeader.Partners[1] = new OrderOrderHeaderPartner();

            // Lineas
            xmlOrder.OrderDetail = new OrderOrderItem[numLin];


            StreamReader sr2 = new StreamReader(@fitxerOrigen);
            string valor = "";

            while ((lin = sr2.ReadLine()) != null)
            {
                if (lin.Substring(0, 4) == "0101" || lin.Substring(0, 4) == "0102")
                {
                    xmlOrder.OrderHeader.OrderNumber = lin.Substring(18, 15).Trim();
                    xmlOrder.OrderHeader.DistrChannel = K_DistChannel;
                    xmlOrder.OrderHeader.Dates.OrderDate = lin.Substring(4, 8);
                }
                else if (lin.Substring(0, 4) == "1010")
                {
                    xmlOrder.OrderHeader.OrderNumber = lin.Substring(20, 10).Trim() + xmlOrder.OrderHeader.OrderNumber;    //---- GSG (27/11/2019)

                    //---- GSG (06/12/2019) Cuando venga un KB (algunas fcias lo tienen configurado como KB en lugar de ZKBW) poner ZKBW
                    //xmlOrder.OrderHeader.DocumentType = lin.Substring(30, 4);
                    string tipo = lin.Substring(30, 4);
                    if (tipo.Trim() == "KB")
                        tipo = "ZKBW";

                    xmlOrder.OrderHeader.DocumentType = tipo;
                    //---- FI GSG


                    try
                    {
                        valor = "";
                        DataRow[] DRov = dtOV.Select("sID = '" + lin.Substring(70, 4) + "' AND (idInterfaz = 4 OR idInterfaz = 0) AND iEstado = 0");
                        if (DRov.Length > 0)
                            valor = DRov[0].ItemArray[2].ToString();
                    }
                    catch (Exception error) { valor = ""; }
                    //xmlOrder.OrderHeader.SalesOrganization = lin.Substring(70, 4); 
                    xmlOrder.OrderHeader.SalesOrganization = valor;


                    //xmlOrder.OrderHeader.Dates.RequestedDeliveryDate = lin.Substring(60, 8);
                    xmlOrder.OrderHeader.Dates.RequestedDeliveryDate = "";

                    xmlOrder.OrderHeader.Partners[0].PartnerType = "AG";
                    //---- GSG (10/06/2019)
                    // En AG va el destinatario de mercancías que es el cliente pero en el caso de haber botiquín ponemos el código del botiquín
                    try
                    {
                        //DataRow[] DRAG = dtCli.Select("sIdCliente = '" + lin.Substring(4, 16).Trim().PadLeft(10, '0') + "'"); // La stored entiende nuestro código SAP con 0's y sin la S
                        DataRow[] DRAG;

                        //if (_sCodBotiquin == "0000000") //---- GSG (19/02/2020)
                        if (_sCodBotiquin == "000000")
                            DRAG = dtCli.Select("sIdCliente = '" + lin.Substring(4, 16).Trim().PadLeft(10, '0') + "'"); // La stored entiende nuestro código SAP con 0's y sin la S
                        else
                            DRAG = dtCli.Select("sIdCliente = '" + _sCodBotiquin.PadLeft(10, '0') + "'"); // La stored entiende nuestro código SAP con 0's y sin la S


                        valor = "";
                        if (DRAG.Length > 0)
                        {
                            valor = DRAG[0].ItemArray[1].ToString();
                            if (valor.Length == 0)
                                valor = lin.Substring(4, 16).Trim(); // Si no tenemos el código ponemos el nuestro
                        }
                        else
                            valor = lin.Substring(4, 16).Trim(); // Si no tenemos la relación en la tabla ponemos el nuestro
                    }
                    catch (Exception error) { valor = ""; }
                    xmlOrder.OrderHeader.Partners[0].PartnerID = valor;



                    // En el caso de pedidos directos, WE irá vacío
                    try
                    {
                        if (lin.Substring(30, 4).Trim() == "ZDIW" || lin.Substring(30, 4).Trim() == "ZKBW" || lin.Substring(30, 4).Trim() == "KB")
                        {
                            xmlOrder.OrderHeader.Partners[1].PartnerType = "";
                            valor = "";
                        }
                        else // transfers
                        {
                            xmlOrder.OrderHeader.Partners[1].PartnerType = "WE";
                            DataRow[] DRWE = dtCli.Select("sIdCliente = '" + lin.Substring(36, 6).Trim().PadLeft(10, '0') + "'");
                            valor = "";
                            if (DRWE.Length > 0)
                            {
                                valor = DRWE[0].ItemArray[1].ToString();
                                if (valor.Length == 0)
                                    valor = lin.Substring(36, 6).Trim(); // Si no tenemos el código ponemos el nuestro
                            }
                            else
                                valor = lin.Substring(36, 6).Trim(); // Si no tenemos la relación en la tabla ponemos el nuestro
                        }

                    }
                    catch (Exception error) { valor = ""; }
                    xmlOrder.OrderHeader.Partners[1].PartnerID = valor;



                    // La conversión de la condición de pago no va a ser de código a código sino que 
                    // nuestros código lo vamos a relacionar con los días de pago que tenemos que es lo que quieren en el XML
                    // Reunión David/Boris 24/05/2018 - Se añade este campo que no estaba inicialmente en el XML

                    // En este punto ya tenemos todos los datos necesarios para obtener la condicion de pago:
                    SRConvert srConv = new SRConvert();
                    DataTable dtCP = new DataTable("SRCondPago");
                    // Els codis que entén l'strored són sense convertir
                    try
                    {
                        dtCP = srConv.CargarSRCondicionPago(1, lin.Substring(36, 6).Trim().TrimStart('0'), lin.Substring(30, 4).Trim(), lin.Substring(70, 4).Trim().TrimStart('0'), lin.Substring(4, 16).Trim());
                        valor = "";
                        DataRow[] DRo = dtCP.Select("codCondPago = 'XXXX'");
                        if (DRo.Length > 0)
                            valor = DRo[0].ItemArray[1].ToString();
                    }
                    catch (Exception error) { valor = ""; }
                    xmlOrder.OrderHeader.PaymentTerms = valor;



                }
                else if (lin.Substring(0, 4) == "1020" || lin.Substring(0, 4) == "1030")
                {
                    xmlOrder.OrderDetail[iLin - 1] = new OrderOrderItem();

                    xmlOrder.OrderDetail[iLin - 1].LineItemNumber = iLin.ToString();

                    try
                    {
                        valor = "";
                        DataRow[] DRMats = dtMat.Select("sCodNacional = '" + lin.Substring(10, 6).Trim() + "'"); // Per la conversió cod nacional sense d´´igit de control
                        valor = "";
                        if (DRMats.Length > 0)
                            valor = DRMats[0].ItemArray[2].ToString();
                        else
                            //---- GSG (06/12/2019) El cambio implica que si no tenemos el cod. alemán, enviaremos el código SAP en lugar del código nacional
                            //valor = lin.Substring(10, 6).Trim(); // Si no tenemos el código de alemania enviamos el código nacional (en interfaz CRM se retorna el codsap nuestro si el alemán no está)
                            valor = DRMats[0].ItemArray[0].ToString();
                    }
                    catch (Exception error) { valor = ""; }

                    xmlOrder.OrderDetail[iLin - 1].ProductCode = valor;

                    xmlOrder.OrderDetail[iLin - 1].OrderQty = lin.Substring(17, 4).TrimStart('0');

                    iLin++;
                }

            }

            sr2.Dispose();
            sr2.Close();

            if (xmlOrder != null)
                OrderToXML(xmlOrder, fitxerXML);

            return true;
        }


        public bool OrderToXML(Order ped, string fit)
        {
            string path = System.Configuration.ConfigurationManager.AppSettings["TargetXMLFolder"].ToString();

            DirectoryInfo diXml = new DirectoryInfo(path);
            if (!diXml.Exists)
                diXml.Create();

            string sXML = GetXMLFromObject(fit, ped);

            return true;
        }


        public static string GetXMLFromObject(string path, object o)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(o.GetType());
                XmlWriterSettings settings = new XmlWriterSettings();

                settings.Indent = true;
                settings.NewLineOnAttributes = true;


                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");

                using (XmlWriter writer = XmlWriter.Create(path, settings))
                {
                    serializer.Serialize(writer, o, ns);
                }

            }
            catch (Exception ex)
            {
                return "error";
            }

            return "";

        }
    }
}
