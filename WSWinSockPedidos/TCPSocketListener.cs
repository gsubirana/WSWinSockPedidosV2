using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Configuration;
using Comuns;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.Data;
using DAL;
using WSWinSockPedidos.Funciones;


namespace WSWinSockPedidos
{
    class TCPSocketListener
    {


        WinSockPedidos.Pedido pedido = new WinSockPedidos.Pedido();
        FuncionesIncidencias funcionesIncidencias = new FuncionesIncidencias();
        FuncionesListener funciones = new FuncionesListener();
        Regalos regalos = new Regalos();
        Constantes constantes = new Constantes();
        Utils U = new Utils();
        FuncionesGenerarFicheros funcionesFicheros = new FuncionesGenerarFicheros();
        GuardarDatos guardarDatos = new GuardarDatos();

        public static String DEFAULT_FILE_STORE_LOC = "";
        public static String DEFAULT_FILE_STORE_LOC_DIR = ""; //---- GSG (31/10/2019)
        public static String DEFAULT_FILE_STORE_NOK_LOC = "";
        public static String INCIDENCIAS_FILE_STORE_LOC = "";
        //---- GSG (17/04/2018) xml sap rollout
        public static String DEFAULT_FILE_STORE_BACKUP_LOC = "";
        public static String DEFAULT_FILE_STORE_XML_LOC = "";
        public static String DEFAULT_FILE_STORE_XML_LOC_DIR = ""; //---- GSG (31/10/2019)
        public static String DEFAULT_FILE_STORE_XML_NOK_LOC = "";


        private bool _m_stopClient = false;
        private bool _m_markedForDeletion = false;
        private Thread _m_clientListenerThread = null;
        private Socket _m_clientSocket = null;
        private DateTime _m_lastReceiveDateTime;
        private DateTime _m_currentReceiveDateTime;


        private string _sNomfitxerDesti = "";
        private string _sNomfitxerDestiNOK = "";
        private string _sNomfitxerIncidencies_traza = "";
        private string _sNomfitxerIncidencies = "";

        private StringBuilder _m_oneLineBuf = new StringBuilder();


        #region Variables Utilizadas

        bool _bProcesaYa = false;
        //bool _bHayRepe = false; --SCS 09/01/2023
        bool _bRechazado = false;
        private StreamWriter _m_cfgFile = null;
        private StreamWriter _m_cfgFileNOK = null;


        public enum STATE { FILE_NAME_READ, DATA_READ, FILE_CLOSED };
        private STATE _m_processState = STATE.FILE_NAME_READ;


        private string _firstLine = ""; //Primera línia per a missatge final d'incidències
        IEnumerable<string> _sortAscendingQueryByCodNac = null;
        private string _capcaLine = ""; //Capçalera per a missatge final d'incidències
        private int _numberOfOrders = 0; //Nombre de comandes rebudes
        private int _numberOfLinesPerOrder = 0; //Nombre de línies a una comanda
        private int _qtyPerOrder = 0;
        private int _bonificacionsPerOrder = 0; //Suma de les bonificacions de cada línia 1030
        private int _iSuperanStock = 0;
        private float _totalBrutComanda = 0;
        private int _iNumRegalos = 0;
        private bool _bHayTextoLibre = false;
        private int _qtyNoComerc = 0;
        private int _bonifNoComercPerOrder = 0;
        private int _bonifBloquejatsPerOrder = 0;
        private int _qtyBloquejats = 0;
        private int _qtyDeMenysAlSubstituirPerOrder = 0;
        private int _qtyNOStada = 0;
        private int _bonifNOStadaPerOrder = 0;
        private int _qtyNOKPorInactivos = 0;
        private int _bonifNOKPorInactivosPerOrder = 0;
        private long _m_totalClientDataSizeNOK = 0;
        private int _numberOfMessages = 0; //Nombre de missatges a enviar al final
        private int _iPedIdR = -1; //---- GSG (22/02/2016)
        private string _totalsLine = ""; //Línia de totals per a missatge final d'incidències
        string _msgLibre = "";
        private long _m_totalClientDataSize = 0;
        private int _iCodsNacionalsCH = 0;
        private int _iCodsNacionalsAparte = 0;
        private int _iCodsNacionalsAparteCH = 0;
        int _numRegalosPedido = 0;

        private string _sIniciCodNacional = "";
        private int _iCodsNacionalsNOStada = 0;
        private int _iCodsNacionalsNOKPorInactivos = 0;
        private int _iCodsNacionalsNoComerc = 0;
        private int _iCodsNacionalsBloquejats = 0;

        private List<dsSPR.ListaMaterialesAparteRow> _lListaMaterialesAparte; //Tabla MaterialesAParte completa
        private List<string> _fitsAparte;
        private List<List<string>> _lCodsNacionalsAparte = new List<List<string>>();
        private List<List<string>> _lCodsNacionalsAparteCH = new List<List<string>>();


        private List<string> _missatgesFinals = new List<string>();
        private List<string> _missatgesFinalsPerAfegir = new List<string>();


        #endregion

        public TCPSocketListener(Socket clientSocket)
        {
            _m_clientSocket = clientSocket;
            DEFAULT_FILE_STORE_LOC = ConfigurationManager.AppSettings["TargetFolder"].ToString();
            DEFAULT_FILE_STORE_LOC_DIR = ConfigurationManager.AppSettings["TargetFolderDIR"].ToString(); //---- GSG (31/10/2019)
            DEFAULT_FILE_STORE_NOK_LOC = ConfigurationManager.AppSettings["TargetFolderNOK"].ToString();
            INCIDENCIAS_FILE_STORE_LOC = ConfigurationManager.AppSettings["IncidenciasFolder"].ToString();
            //---- GSG (17/04/2018) xml sap rollout
            DEFAULT_FILE_STORE_BACKUP_LOC = ConfigurationManager.AppSettings["TargetFolderBackup"].ToString();
            DEFAULT_FILE_STORE_XML_LOC = ConfigurationManager.AppSettings["TargetXMLFolder"].ToString();
            DEFAULT_FILE_STORE_XML_LOC_DIR = ConfigurationManager.AppSettings["TargetXMLFolderDIR"].ToString();//---- GSG (31/10/2019)
            DEFAULT_FILE_STORE_XML_NOK_LOC = ConfigurationManager.AppSettings["TargetXMLFolderNOK"].ToString();
        }

        ~TCPSocketListener()
        {
            StopSocketListener();
        }

        // Inicia el thread associant-lo amb el mètode 'SocketListenerThreadStart'
        public void StartSocketListener()
        {
            if (_m_clientSocket != null)
            {
                _m_clientListenerThread = new Thread(new ThreadStart(SocketListenerThreadStart));
                _m_clientListenerThread.Start();
            }
        }

        // Inicia algunes variables que no depenent de la informació rebuda i es queda esperant les dades de client
        private void SocketListenerThreadStart()
        {
            int size = 0;
            Byte[] byteBuffer = new Byte[1024];

            //_m_lastReceiveDateTime = DateTime.Now;
            //_m_lastReceiveDateTime = DateTime.Now.AddSeconds(-5); //---- GSG (20/01/2014)
            _m_currentReceiveDateTime = DateTime.Now;
            _m_lastReceiveDateTime = _m_currentReceiveDateTime.AddSeconds(-5);

            // comproba si hi ha crides del client (CheckClientCommInterval) cada 5 minuts començant en 6 segundos
            Timer t = new Timer(new TimerCallback(CheckClientCommInterval), null, 6000, 300000);

            // No poso el .txt encara ja hauré d'afegir abans el codi del distribuidor perquè volem que aparegui al nom del fitxer

            string sDiaHora = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') +
                              DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0');

            // Construeix el nom del fitxer destí
            _sNomfitxerDesti = DEFAULT_FILE_STORE_LOC + sDiaHora;

            // Construeix el nom del fitxer destí per a les comandes rebutjades
            _sNomfitxerDestiNOK = DEFAULT_FILE_STORE_NOK_LOC + sDiaHora;

            // Fichero donde guardaremos las incidencias
            _sNomfitxerIncidencies = INCIDENCIAS_FILE_STORE_LOC + sDiaHora + "_LOG.txt";

            // Fichero con la traza del programa
            _sNomfitxerIncidencies_traza = INCIDENCIAS_FILE_STORE_LOC + sDiaHora + "_LOG_traza.txt";


            _bProcesaYa = false;

            // Escolta
            while (!_m_stopClient)
            {
                //Thread.Sleep(1000);
                try
                {
                    size = _m_clientSocket.Receive(byteBuffer);
                    _m_currentReceiveDateTime = DateTime.Now;

                    // Inici implementació --Alex Diaz 05/01/2024--
                    // Comproba que el valor de size no sigui 0 perquè quan pren aquest valor la funció ParseReceiveBuffer entra en un bucle infinit
                    // En cas de que el size sigui 0 vol dir que s'ha realitzat la connexió sense enviar cap dada
                    // Per tant es marca el thread per borrar i es tanca la connexió
                    if (size == 0)
                    {
                        _m_markedForDeletion = true;
                        TancarConnexio();
                    } 
                    else
                    {
                        ParseReceiveBuffer(byteBuffer, size);

                        if (_bProcesaYa)
                        {
                            //---- GSG (06/02/2017)
                            RetocarLineasSegunSustitucion();
                            GuardaLinsSinRepe();

                            if (pedido.listPedidosWithoutRepe.Count > 0)
                                ProcesaPedido();
                        }
                    }
                    // Final implementació --Alex Diaz 05/01/2024--


                    //---- GSG (03/02/2017)
                    // A veces la farmacia envía un pedido con líneas de un material y líneas del sustituto de ese material
                    // En estos casos también debemos sumar las unidades y que quede una única línea.
                    // Para que posteriormente el proceso encuentre que debe sumar las cantidades y demaás, deben llevar el mismo código de material
                    // No pongo el código del material bueno porque si no no saldría después el mensaje de que un material ha sido sustituido por otro
                    // Pondré el código antiguo pero solo cuando también exista el nuevo (si la fcia solo envía el nuevo no hay que decirle nada)


                    //---- GSG (12/07/2016)

                }
                catch (SocketException se)
                {
                    _m_stopClient = true;
                    _m_markedForDeletion = true;

                    //SendTrazaToLogs(se.Message); 
                }    
            }

            //vaciamos variables
            cleanVariables();

            t.Change(Timeout.Infinite, Timeout.Infinite);
            t = null;
        }

        // Para el thread d'escolta
        public void StopSocketListener()
        {
            if (_m_clientSocket != null)
            {
                _m_stopClient = true;
                _m_clientSocket.Close();

                _m_clientListenerThread.Join(1000);

                if (_m_clientListenerThread.IsAlive)
                {
                    _m_clientListenerThread.Abort();
                }

                _m_clientListenerThread = null;
                _m_clientSocket = null;
                _m_markedForDeletion = true;
            }
        }

        // Method that returns the state of this object i.e. whether this
        // object is marked for deletion or not.
        public bool IsMarkedForDeletion()
        {
            return _m_markedForDeletion;
        }

        // Method that checks whether there are any client calls for the
        // last x seconds or not. If not this client SocketListener will
        // be closed.
        private void CheckClientCommInterval(object o)
        {
            if (_m_lastReceiveDateTime.Equals(_m_currentReceiveDateTime))
            {
                this.StopSocketListener();
            }
            else
            {
                _m_lastReceiveDateTime = _m_currentReceiveDateTime;
            }
        }

        public void ParseReceiveBuffer(Byte[] byteBuffer, int size)
        {
            string data = Encoding.ASCII.GetString(byteBuffer, 0, size);
            int lineEndIndex = 0;

            do
            {
                // COMPROBAR QUE EL SALTO DE LINEA SEA DE WINDOWS PORQUE SI ES DE LINUX O MAC PETA (BUCLE INFINITO)
                lineEndIndex = data.IndexOf("\r\n");

                if (lineEndIndex != -1)
                {
                    // Agafa una línia
                    _m_oneLineBuf = _m_oneLineBuf.Append(data, 0, lineEndIndex + 2);

                    int iBucle = 1;
                    if (_m_oneLineBuf.ToString().IndexOf("\r\n") != _m_oneLineBuf.ToString().LastIndexOf("\r\n"))
                        iBucle = 2;

                    for (int i = 0; i < iBucle; i++)
                    {
                        int posfin = _m_oneLineBuf.ToString().IndexOf("\r\n");
                        string lin = _m_oneLineBuf.ToString().Substring(0, posfin + 2);

                        //---- GSG (06/02/2017)
                        //GuardaLinsSinRepe(lin);
                        pedido.listPedidosOriginal.Add(lin);
                        funcionesIncidencias.SendTrazaToLogs("linia recibida " + lin, _sNomfitxerIncidencies_traza);

                        // Inici implementació --Alex Diaz 19/12/2023--
                        // Funció implementada perque el servei no caigui quan es reben peticions que no segueixen el protocol fedicom
                        // Funció que comprova si lin es diferent als codis del protocol, i si es diferent ha de tancar la conexio
                        if (lin.Length < 4)
                        {
                            _m_markedForDeletion = true;
                            TancarConnexio();
                        }
                        else if (lin.Substring(0, 4) != "0101" && lin.Substring(0, 4) != "0102" && lin.Substring(0, 4) != "0199" && lin.Substring(0, 4) != "1010" && lin.Substring(0, 4) != "1020" && lin.Substring(0, 4) != "1030" && lin.Substring(0, 4) != "1050" && lin.Substring(0, 4) != "1040")                       
                        {
                            _m_markedForDeletion = true;
                            TancarConnexio();
                        }
                        // Final implementació --Alex Diaz 19/12/2023--

                        if (lin.Substring(0, 4) == "0199")
                            _bProcesaYa = true;

                        // Del que hem rebut elimina el que ja està processat
                        _m_oneLineBuf.Remove(0, lin.Length);

                    }

                    /////////////////////////////////////////////////////////////////

                    data = data.Substring(lineEndIndex + 2, data.Length - lineEndIndex - 2);
                }
                else
                {
                    _m_oneLineBuf = _m_oneLineBuf.Append(data);

                    //SendTrazaToLogs("ParseReceiveBuffer: linia recibida " + _m_oneLineBuf.ToString());
                }

            } while (lineEndIndex != -1);

            var a = 1;
        }

        //---- GSG (06/02/2017)
        // Revisa les línies rebudes i si hi ha un material substitut i a la vegada també hi ha el material substituit
        // canvia el substitut pel substituit (i.e. el nou pel vell)
        private void RetocarLineasSegunSustitucion()
        {
            string tipoLin = "";
            string materialComArriba = "";
            string materialTractat = "";

            // s1 = sustituido o OLD
            // s2 = sustituto o NEW


            // Buscar
            foreach (string sLin in pedido.listPedidosOriginal)
            {
                tipoLin = sLin.Substring(0, 4);

                switch (tipoLin)
                {
                    case "1010":
                        pedido.sTipoPedido = sLin.Substring(30, 6).Trim();
                        pedido.Cliente.codClienteLine = sLin.Substring(4, 16).Trim();
                        pedido.Cliente.sCodCliente = "S" + pedido.Cliente.codClienteLine.PadLeft(10, '0');

                        // Obtener tabla de sustitutos (teniendo en cuenta el cliente y el tipo de pedido para respetar los bloqueos)
                        pedido.ExtraListasPedidos.listSubstituciones = funciones.getListaSustitucionMateriales(pedido.sTipoPedido, pedido.Cliente.sCodCliente);
                        pedido.listPedidosModify.Add(sLin);
                        break;
                    case "1020":
                        // En la tabla de materiales los sustitutos pueden estar sólo con 6 dígitos o bien con el EAN-13

                        materialComArriba = sLin.Substring(4, 13); // Código tal cual llega (pot venir en EAN-13 o no)                    
                        materialTractat = sLin.Substring(10, 7);
                        if (materialTractat.Substring(0, 1) == "0") // No viene el dígito de control --> nos quedamos con los 6 de la derecha
                            materialTractat = materialTractat.Substring(1, 6);
                        else // Viene el dígito de control (tanto si el último dígito es 0 como si no nos quedamos con los 6 primeros pq no importa el dc)
                            materialTractat = materialTractat.Substring(0, 6);

                        // Es material sustituto?
                        var item = pedido.ExtraListasPedidos.listSubstituciones.Where(x => x.s2 == materialComArriba || x.s2 == materialTractat);

                        // Si sí --> mirar si el sustituido está en el pedido
                        if (item.Count() > 0)
                        {
                            string s1 = item.FirstOrDefault().s1;

                            var materialRepeSub = pedido.listPedidosOriginal
                                .Where(x => x.Substring(0, 4) == "1020" || x.Substring(0, 4) == "1030")
                                .Where(x => x.Substring(10, 7).Substring(1, 6) == s1 || x.Substring(10, 7).Substring(0, 6) == s1);

                            if (materialRepeSub.Count() >= 1)
                                pedido.listPedidosModify.Add(sLin.Substring(0, 4) + materialRepeSub.FirstOrDefault().Substring(4, 13) + sLin.Substring(17));
                            else
                                pedido.listPedidosModify.Add(sLin);
                        }
                        else
                            pedido.listPedidosModify.Add(sLin);
                        break;
                    case "1030":
                        // En la tabla de materiales los sustitutos pueden estar sólo con 6 dígitos o bien con el EAN-13

                        materialComArriba = sLin.Substring(4, 13); // Código tal cual llega (pot venir en EAN-13 o no)                    
                        materialTractat = sLin.Substring(10, 7);
                        if (materialTractat.Substring(0, 1) == "0") // No viene el dígito de control --> nos quedamos con los 6 de la derecha
                            materialTractat = materialTractat.Substring(1, 6);
                        else // Viene el dígito de control (tanto si el último dígito es 0 como si no nos quedamos con los 6 primeros pq no importa el dc)
                            materialTractat = materialTractat.Substring(0, 6);

                        // Es material sustituto?
                        var item2 = pedido.ExtraListasPedidos.listSubstituciones.Where(x => x.s2 == materialComArriba || x.s2 == materialTractat);

                        // Si sí --> mirar si el sustituido está en el pedido
                        if (item2.Count() > 0)
                        {
                            string s1 = item2.FirstOrDefault().s1;

                            var materialRepeSub = pedido.listPedidosOriginal
                                .Where(x => x.Substring(0, 4) == "1020" || x.Substring(0, 4) == "1030")
                                .Where(x => x.Substring(10, 7).Substring(1, 6) == s1 || x.Substring(10, 7).Substring(0, 6) == s1);

                            if (materialRepeSub.Count() >= 1)
                                pedido.listPedidosModify.Add(sLin.Substring(0, 4) + materialRepeSub.FirstOrDefault().Substring(4, 13) + sLin.Substring(17));
                            else
                                pedido.listPedidosModify.Add(sLin);
                        }
                        else
                            pedido.listPedidosModify.Add(sLin);
                        break;
                    default:
                        pedido.listPedidosModify.Add(sLin);
                        break;
                }
            }
        }

        // Unir líneas del mismo material
        private void GuardaLinsSinRepe()
        {
            string sTipoLin = "", sMaterial = "";
            int iCantidad = 0, iBonificacion = 0;
            string sLineRepe = "", sFinalLine = "";

            int iTotalCantidad = 0, iTotalBonificacion = 0;

            foreach (string sLine in pedido.listPedidosModify)
            {
                sTipoLin = sLine.Substring(0, 4);

                switch (sTipoLin)
                {
                    case "1050":
                        sFinalLine = sLine.Substring(0, 8) + iTotalCantidad.ToString().PadLeft(6, '0') + iTotalBonificacion.ToString().PadLeft(6, '0') + sLine.Substring(20, sLine.Length - 20);
                        pedido.listPedidosWithoutRepe.Add(sFinalLine);
                        break;
                    case "1020":
                        // AGREGADO POR ALEX DIAZ 17/01/2024 -- INICI --
                        sMaterial = sLine.Substring(4, 13); // Código tal cual llega
                        iCantidad = int.Parse(sLine.Substring(17, 4));

                        if (sTipoLin == "1030")
                            iBonificacion = int.Parse(sLine.Substring(21, 4));
                        else
                            iBonificacion = 0;

                        if (pedido.listPedidosWithoutRepe.Where(x => x.Substring(0, 4) == "1020" || x.Substring(0, 4) == "1030").Where(x => x.Substring(4, 13) == sMaterial).Count() == 0)
                        {
                            iTotalCantidad += iCantidad;
                            iTotalBonificacion += iBonificacion;
                            pedido.listPedidosWithoutRepe.Add(sLine);
                        }
                        else
                        {
                            sLineRepe = pedido.listPedidosWithoutRepe.Where(x => x.Substring(4, 13) == sMaterial).FirstOrDefault();

                            iTotalCantidad += iCantidad;

                            iCantidad = iCantidad + int.Parse(sLineRepe.Substring(17, 4));

                            sFinalLine = sLineRepe.Substring(0, 17) + iCantidad.ToString().PadLeft(4, '0');

                            if (sLineRepe.Substring(0, 4) == "1030" && sLineRepe.Substring(0, 4) == sTipoLin)
                            {
                                iTotalBonificacion += iBonificacion;

                                iBonificacion = iBonificacion + int.Parse(sLineRepe.Substring(21, 4));

                                sFinalLine = sFinalLine + iBonificacion.ToString().PadLeft(4, '0') + sLineRepe.Substring(25, sLineRepe.Length - 25);
                            }
                            else
                            {
                                sFinalLine += sLineRepe.Substring(21, sLineRepe.Length - 21);
                            }

                            pedido.listPedidosWithoutRepe[pedido.listPedidosWithoutRepe.FindIndex(x => x.Substring(4, 13) == sMaterial)] = sFinalLine;

                        }
                        break;
                        // AGREGADO POR ALEX DIAZ 17/01/2024 -- FIN -- 
                    case "1030":
                        sMaterial = sLine.Substring(4, 13); // Código tal cual llega
                        iCantidad = int.Parse(sLine.Substring(17, 4));

                        if (sTipoLin == "1030")
                            iBonificacion = int.Parse(sLine.Substring(21, 4));
                        else
                            iBonificacion = 0;

                        if (pedido.listPedidosWithoutRepe.Where(x=>x.Substring(0,4)=="1020" || x.Substring(0,4)=="1030").Where(x => x.Substring(4, 13) == sMaterial).Count() == 0)
                        {
                            iTotalCantidad += iCantidad;
                            iTotalBonificacion += iBonificacion;
                            pedido.listPedidosWithoutRepe.Add(sLine);
                        }
                        else
                        {
                            sLineRepe = pedido.listPedidosWithoutRepe.Where(x => x.Substring(4, 13) == sMaterial).FirstOrDefault();

                            iTotalCantidad += iCantidad;

                            iCantidad = iCantidad + int.Parse(sLineRepe.Substring(17, 4));

                            sFinalLine = sLineRepe.Substring(0, 17) + iCantidad.ToString().PadLeft(4, '0');

                            if (sLineRepe.Substring(0, 4) == "1030" && sLineRepe.Substring(0, 4) == sTipoLin)
                            {
                                iTotalBonificacion += iBonificacion;

                                iBonificacion = iBonificacion + int.Parse(sLineRepe.Substring(21, 4));

                                sFinalLine = sFinalLine + iBonificacion.ToString().PadLeft(4, '0') + sLineRepe.Substring(25, sLineRepe.Length - 25);
                            }
                            else
                            {
                                sFinalLine += sLineRepe.Substring(21, sLineRepe.Length - 21);
                            }

                            pedido.listPedidosWithoutRepe[pedido.listPedidosWithoutRepe.FindIndex(x => x.Substring(4, 13) == sMaterial)] = sFinalLine;

                        }
                        break;
                    default:
                        pedido.listPedidosWithoutRepe.Add(sLine);
                        break;
                }

            }
        }

        private void ProcesaPedido()
        {
            foreach(string sLine in pedido.listPedidosWithoutRepe)
            {
                ProcessClientData(sLine);
            }
            cleanVariables();
        }

        // Mètode que procesa les dades rebudes del client segons el protocol establert

        // El protocol treballa de la següent manera:
        // 1. Construeix el nom del fitxer i llegeix la primera línia rebuda (inici de sessió 0101....) // a partir de 2016 l'inici de sessió és 0102
        // 2. Si és ok continua sino aborta amb missatge final de transmissió (9999) i es tancará la connexió.
        // 3. Cada línia rebuda ha d'acabar amb "CRLF".
        // 4. Guarda cada línia rebuda en un arxiu de text (determinat al punt 1)
        // 5. Acaba quan rebem del client "0199\r\n" 
        // 6. Es retorna al client línies de confirmació i incidències (si cal)
        private void ProcessClientData(String oneLine)
        {
            switch (_m_processState)
            {
                case STATE.FILE_NAME_READ:
                    #region línia 0101 (0102)
                    int length = oneLine.Length;
                    if (length <= 4)
                    {
                        _m_processState = STATE.FILE_CLOSED;
                        length = -1;
                        //Envia missatge de rebuig de connexió
                        //SendTrazaToLogs("ProcessClientData: Fallo en línea de inicio de sesión."); 
                        funcionesIncidencias.SendIncidenciaRechazoTransmision("Fallo en línea de inicio de sesión.",_m_clientSocket);
                        TancarConnexio();
                    }
                    else
                    {
                        try
                        {
                            //Ha llegit la primera línia (0101..)
                            //Si és ok continuar sinó tancar transmissió
                            // Aqueta línia s'envia igual als dos StreamWriters (ok i nok)
                            //---- GSG (05/02/2016)
                            //if (oneLine.Substring(0, 4) == "0101")
                            if (oneLine.Substring(0, 4) == "0101" || oneLine.Substring(0, 4) == "0102")
                            {
                                // Afegeix el nom del distribuidor i l'extensió al nom del fitxer
                                string finalNomFit = "_" + oneLine.Substring(18, 16).Trim() + ".txt";
                                _sNomfitxerDesti += finalNomFit;
                                _sNomfitxerDestiNOK += finalNomFit;
                                _m_cfgFile = new StreamWriter(_sNomfitxerDesti);
                                _m_cfgFileNOK = new StreamWriter(_sNomfitxerDestiNOK);

                                _m_processState = STATE.DATA_READ;
                                _firstLine = oneLine; //Guarda primera línea rebuda
                            }
                            else
                            {
                                _m_processState = STATE.FILE_CLOSED;
                                //Tanca fitxer
                                _m_cfgFile.Close();
                                _m_cfgFile = null;
                                _m_cfgFileNOK.Close();
                                _m_cfgFileNOK = null;
                                //Envia missatge de rebuig de connexió
                                //SendTrazaToLogs("ProcessClientData: Fallo en línea de inicio de sesión."); 
                                funcionesIncidencias.SendIncidenciaRechazoTransmision("Error en la línea de inicio de sesión.", _m_clientSocket);
                                //Tanca la connexió
                                TancarConnexio();
                            }
                        }
                        catch (SocketException se)
                        {
                            //SendTrazaToLogs("ProcessClientData: " + se.Message); 
                            _m_processState = STATE.FILE_CLOSED;
                            //Tanca fitxer
                            _m_cfgFile.Close();
                            _m_cfgFile = null;
                            _m_cfgFileNOK.Close();
                            _m_cfgFileNOK = null;
                            TancarConnexio();
                        }
                    }
                    #endregion
                    break;

                case STATE.DATA_READ:
                    if (oneLine.ToUpper().Equals("0199\r\n")) //Darrera línia: el client ja no enviará més dades
                    {
                        #region línia 0199
                        try
                        {
                            //---- GSG (10/02/2016)
                            // Triga molt a partir els fitxers després de llegir la línea 1050, per tant enviaré aquestes incidencies abans per a que el client no hagi d'esperar tant
                            //// Envío de la respuesta al cliemte, mesajes 2010, 2011 y 2015
                            //envioRespuestaACliente();

                            //---- FI GSG (10/02/2016)

                            // Envia les incidències al fitxer i a la taula de logs (el rebuig tb ha de sortir al Monitor de Pedidos com una incidència)
                            funcionesIncidencias.SendIncidenciasToLogs(_bHayTextoLibre, pedido.codPedAux, _numberOfMessages, _missatgesFinals, _sNomfitxerDesti, DEFAULT_FILE_STORE_LOC, _iPedIdR);

                            if (_bRechazado)
                            {
                                #region Escriu a fitxer NOK

                                //En aquest cas escribint totes les línies al final (quan ha anat ok ja s'han anat escribint a mida que es llegien)

                                //Escriu la primera línia rebuda tal qual
                                _m_totalClientDataSizeNOK += _firstLine.Length;
                                _m_cfgFileNOK.Write(_firstLine);
                                _m_cfgFileNOK.Flush();

                                //Escriu la capçalera (pot estar modificada respecte de la inicial)
                                _m_totalClientDataSizeNOK += _capcaLine.Length;
                                _m_cfgFileNOK.Write(_capcaLine);
                                _m_cfgFileNOK.Flush();

                                //Escriu les línies de la comanda
                                foreach (string l in _sortAscendingQueryByCodNac)
                                {
                                    _m_totalClientDataSizeNOK += l.Length;
                                    _m_cfgFileNOK.Write(l);
                                    _m_cfgFileNOK.Flush();
                                }

                                //Escriu la línia de totals
                                _m_totalClientDataSizeNOK += _totalsLine.Length;
                                _m_cfgFileNOK.Write(_totalsLine);
                                _m_cfgFileNOK.Flush();

                                // Envía la darrera línia al fitxer nok
                                _m_cfgFileNOK.Write(oneLine); //"0199\r\n"
                                _m_cfgFileNOK.Flush();

                                //Tanca fitxers
                                _m_cfgFile.Close();
                                _m_cfgFile = null;
                                _m_cfgFileNOK.Close();
                                _m_cfgFileNOK = null;

                                #endregion

                                length = -1;

                                // Envío de la respuesta al cliemte, mesaje 9999
                                funcionesIncidencias.SendIncidenciaRechazoTransmision(_msgLibre,_m_clientSocket); //---- GSG (24/07/2013)

                                //SendTrazaToLogs("ProcessClientData: Pedido rechazado.");

                                //Esborra el que sobra (será el ok)
                                if (_m_totalClientDataSize == 0 && File.Exists(_sNomfitxerDesti))
                                    File.Delete(_sNomfitxerDesti);
                                if (_m_totalClientDataSizeNOK == 0 && File.Exists(_sNomfitxerDestiNOK))
                                    File.Delete(_sNomfitxerDestiNOK);
                            }
                            else
                            {
                                // Ecriu la darrera línia als StreamWriters (ok i nok) i els tanca. 
                                if (_m_totalClientDataSize > 0)
                                {
                                    _m_totalClientDataSize += oneLine.Length;
                                    _m_cfgFile.Write(oneLine);
                                    _m_cfgFile.Flush();

                                    _m_cfgFile.Close();
                                    _m_cfgFile = null;
                                }
                                else if (_m_cfgFile != null)
                                {
                                    _m_cfgFile.Close();
                                    _m_cfgFile = null;
                                }

                                if (_m_totalClientDataSizeNOK > 0)
                                {
                                    _m_totalClientDataSizeNOK += oneLine.Length;
                                    _m_cfgFileNOK.Write(oneLine);
                                    _m_cfgFileNOK.Flush();

                                    _m_cfgFileNOK.Close();
                                    _m_cfgFileNOK = null;
                                }
                                else if (_m_cfgFileNOK != null)
                                {
                                    _m_cfgFileNOK.Close();
                                    _m_cfgFileNOK = null;
                                }
                            }

                            //Abans de generar els fitxers esborra un dels dos originals, el ok o el nok, segons quin estigui buit
                            if (_m_totalClientDataSize == 0 && File.Exists(_sNomfitxerDesti))
                                File.Delete(_sNomfitxerDesti);

                            if (_m_totalClientDataSizeNOK == 0 && File.Exists(_sNomfitxerDestiNOK))
                                File.Delete(_sNomfitxerDestiNOK);

                            // Genera els fitxers (a partir del que ho té tot)
                            funcionesFicheros.GenerarFicheros(_numberOfLinesPerOrder, _iCodsNacionalsCH, _iCodsNacionalsAparte, _iCodsNacionalsAparteCH, _numRegalosPedido, _sNomfitxerDesti, _sNomfitxerDestiNOK,
                                pedido.ExtraListasPedidos.listRegalosAnyadir,_fitsAparte,pedido.bIsDirecto, DEFAULT_FILE_STORE_LOC, DEFAULT_FILE_STORE_LOC_DIR, pedido.codPedAux,_lListaMaterialesAparte, pedido.sTipoPedido,pedido.ListasPedidos.codsNacionalsCH,
                                pedido.sCodBotiquin, DEFAULT_FILE_STORE_XML_NOK_LOC, DEFAULT_FILE_STORE_XML_LOC, DEFAULT_FILE_STORE_XML_LOC_DIR, DEFAULT_FILE_STORE_BACKUP_LOC);

                        }
                        catch (SocketException se)
                        {
                            //SendTrazaToLogs("ProcessClientData: " + se.Message);
                        }


                        //Actualitza estat del procés 
                        _m_processState = STATE.FILE_CLOSED;

                        _m_markedForDeletion = true;
                        TancarConnexio();


                        #endregion
                    }
                    else if (oneLine.Substring(0, 4) == "1010") //Capçalera de la comanda
                    {
                        #region línia 1010

                        _bRechazado = false;
                        _sortAscendingQueryByCodNac = null;

                        // A diferència de les línies 0101 i 0199 no l'enviem al fitxer pq faltarà determinar si anirà al ok i/o al nok (encara no tenim els valors de _m_totalClientDataSize i _m_totalClientDataSizeNOK)

                        if (oneLine.Length != constantes.K_LEN_LIN_CAB) //---- GSG (27/10/2015)
                        {
                            _m_processState = STATE.FILE_CLOSED;
                            length = -1;
                            //Envia missatge de rebuig de connexió
                            //SendTrazaToLogs("ProcessClientData: Fallo en línea de cabecera (Longitud incorrecta).");
                            funcionesIncidencias.SendIncidenciaRechazoTransmision("Fallo en línea de cabecera (Longitud incorrecta).",_m_clientSocket);
                            TancarConnexio();
                        }
                        else
                        {
                            _capcaLine = oneLine; //La guardo per al final poder fer validacions

                            //init variables globals
                            _numberOfOrders++;
                            _numberOfLinesPerOrder = 0;
                            _qtyPerOrder = 0;
                            _bonificacionsPerOrder = 0;
                            pedido.ListasPedidos.pedLines = new List<string>();
                            _iSuperanStock = 0;
                            pedido.sTipoPedido = _capcaLine.Substring(30, 6).Trim();


                            // ---- GSG (31/10/2019) Los directos van a otra carpeta
                            pedido.bIsDirecto= false;
                            if (pedido.sTipoPedido != "ZTRW")
                                pedido.bIsDirecto = true;



                            pedido.Cliente.codClienteLine = _capcaLine.Substring(4, 16).Trim(); //---- GSG (12/05/2016) (És el codi SAP de client sense la S i sense els 0's))
                            //_sCodBotiquin = _capcaLine.Substring(42, 7).Trim(); //---- GSG (10/06/2019) És el codi de botiquín si n'hi ha sense la S i sense els 0's
                            pedido.sCodBotiquin = _capcaLine.Substring(43, 6).Trim();   //--- GSG (18/02/2020) El primer dígit no forma part i Zarafarma aquí hi posa un 1 per defecte cosa que fa que el codi de botiquín no sigui el correcte (no troba conversió a SR)
                            _totalBrutComanda = 0;
                            pedido.ExtraListasPedidos.listContadorRegalos = regalos.initTablaContadorRegalos();
                            _iNumRegalos = pedido.ExtraListasPedidos.listContadorRegalos.Count();
                            _bHayTextoLibre = false; //---- GSG (17/07/2013)
                            //---- GSG (25/11/2015)
                            _qtyNoComerc = 0;
                            _bonifNoComercPerOrder = 0;
                            //---- GSG (12/05/2016)                            
                            _bonifBloquejatsPerOrder = 0;
                            _qtyBloquejats = 0;
                            _qtyDeMenysAlSubstituirPerOrder = 0;
                            //---- GSG (17/10/2016
                            _qtyNOStada = 0;
                            _bonifNOStadaPerOrder = 0;
                            _qtyNOKPorInactivos = 0;
                            _bonifNOKPorInactivosPerOrder = 0;

                            /*  COMENTAT A L'ESPERA DE POSAR-HO EN MARXA (FUNCIONA OK)
                            //---- GSG (05/02/2016)
                            _iIncidEstupefacientes = 0; //número de incidencias en estupefacientes
                            _lEstupefacientes.Clear();  //lista de todos los estupefacientes existentes
                            _lEstupefacientes = getEstupefacientes();
                             */
                            pedido.ExtraListasPedidos.listNoComercializados = new List<string>();
                            pedido.ExtraListasPedidos.listNoComercializados = funciones.getNoComercializados(pedido.sTipoPedido);


                            //---- GSG (25/07/2013)
                            //Hay casos en que el código de pedido ya tiene 10 posiciones que es el máximo. 
                            //Intentaremos reducirlo quitando ceros de la izquierda y espacios. Si continua siendo de 10 cambiaremos el código por el que toque en configuración.
                            bool bCanviarCodPedido = false;
                            string codPedAux = _capcaLine.Substring(20, 10);
                            if (codPedAux.Length == 10)
                            {
                                codPedAux = _capcaLine.Substring(20, 10).Trim();
                                char[] charsQuitar = { '0' };
                                codPedAux = codPedAux.TrimStart(charsQuitar); //quita 0's de la izquierda
                                if (codPedAux.Length == 10)
                                    bCanviarCodPedido = true;
                            }

                            //---- GSG (10/06/2013) (14/05/2014) Apareix un tercer tipus
                            // Hi ha tres tipus de clients:
                            // 1.- Els que envien números de comanda diferents --> mirar si ja existeix un número de comanda igual per al mateix client
                            // 2.- Els que repeteixen números de comanda --> Prescindim del seu número de comanda i la construïm  
                            // 3.- Els que repeteixen números de comanda però que comencen cada any
                            // El cas 3, nou, el tractarem igual que el 1 però en tots dos casos quan comprobem si existeix la comanda ho farem mirant en l'any actual (a la taula el codi de comanda no és la clau :) )

                            if (funciones.esClienteSinCodsPedido(pedido.Cliente.codClienteLine) || bCanviarCodPedido) // ---- GSG (25/07/2013)
                            {
                                int iCodLastPedAparte = int.Parse(U.getConfguracion(constantes.K_CONF_ULTPEDAPA));
                                iCodLastPedAparte++;
                                pedido.codPedAux = iCodLastPedAparte.ToString().PadRight(10, ' ');
                                U.updConfiguracion(constantes.K_CONF_ULTPEDAPA, iCodLastPedAparte.ToString());
                                //---- FI GSG
                            }
                            else
                                pedido.codPedAux = codPedAux.PadRight(10, ' ');


                            //---- GSG (12/05/2016)
                            
                            pedido.ExtraListasPedidos.listlBloqueados = funciones.getCodsBloqueados(pedido.sTipoPedido, pedido.Cliente.sCodCliente);

                            pedido.ExtraListasPedidos.listConSustituto = funciones.getCodsConSustituto(pedido.sTipoPedido, pedido.Cliente.sCodCliente);


                            //---- FI GSG


                            // Modifica _capcaline amb el nou codi de comanda (en alguns casos quedarà igual)
                            _capcaLine = _capcaLine.Substring(0, 20) + pedido.codPedAux + _capcaLine.Substring(30, _capcaLine.Length - 30);

                        }
                        #endregion
                    }
                    else if (oneLine.Substring(0, 4) == "1030") //Línies de la comanda AMB bonificació
                    {
                        #region línia 1030
                        //TratarLineas1020i1030(oneLine, "1030");
                        TratarLineas1020i1030i1040(oneLine, "1030");
                        #endregion
                    }
                    else if (oneLine.Substring(0, 4) == "1020") //Línies de la comanda SENSE bonificació
                    {
                        #region línia 1020
                        //TratarLineas1020i1030(oneLine, "1020");
                        TratarLineas1020i1030i1040(oneLine, "1020");
                        #endregion
                    }
                    else if (oneLine.Substring(0, 4) == "1040") //Línies de la comanda ESTUPEFACIENTE   //---- GSG (05/02/2016)
                    {
                        #region línia 1040
                        TratarLineas1020i1030i1040(oneLine, "1040");
                        #endregion
                    }
                    else if (oneLine.Substring(0, 4) == "1050") //Línia de final amb totals
                    {
                        // Guarda la línea de totales
                        _totalsLine = oneLine;

                        // Ordena les línies de la comanda segons codi nacional
                        string[] pedLinesPerOrdernar = new string[_numberOfLinesPerOrder];
                        for (int i = 0; i < _numberOfLinesPerOrder; i++)
                        {
                            pedLinesPerOrdernar[i] = pedido.ListasPedidos.pedLines[i];
                        }

                        IEnumerable<string> sortAscendingQueryByCodNac =
                            from lin in pedLinesPerOrdernar
                            orderby lin.Substring(4, 13)
                            select lin;


                        _sortAscendingQueryByCodNac = sortAscendingQueryByCodNac;

                        //(EN LA NUEVA VERSIÓN TODAS LAS PARTICIONES)
                        codsNacionalsAparte();
                        codsNacionalsCH(); // Debe ir después de codsNacionalsAparte() pq NO hay que coger los CH que estén en la tabla Aparte


                        #region mensajes de error e incidencia

                        // Inspecciona la capçalera per veure si hi ha alguna incidència de capçalera
                        string msgLibre = "";
                        bool bRechazado = false;
                        string respCapca = respostaCapcalera(out msgLibre, out bRechazado);
                        // Guarda la línia 2010 (respCapca tiene mensaje indicando aceptada o rechazo)
                        _missatgesFinals.Add(respCapca);
                        _numberOfMessages++;

                        //Añade los mensajes de incidencia que vienen de la cabecera (se añaden como 2011)
                        if (msgLibre != "") // Hay errores detectados en cabecera
                        {
                            string sMissParcial = "";
                            int index = 0;
                            int len = 0;

                            while (index < msgLibre.Length)
                            {
                                len = msgLibre.IndexOf("#", index) - index;
                                sMissParcial = msgLibre.Substring(index, len);
                                index += len + 1;

                                _missatgesFinals.Add(constantes.codIncidenciaTextoLibre + sMissParcial + "\r\n");
                                _numberOfMessages++;
                            }

                            _bHayTextoLibre = true;
                        }

                        //Añade los mensajes de incidencia que vienen de las líneas (se añaden como 2015)
                        //Errores tratados:
                        // - No comercializado (CASO 1)
                        // - Bloqueados (CASO 2) Se comportan como los No comercializados pero el mensaje que hay que indicar es que no hay stock
                        // - Material inexistente en Stada (CASO 3)
                        // - Material existente pero inactivo (CASO 4)
                        // - Sin existencias (CASO 5)
                        // - Materiales sustituidos (CASO 6)
                        // - Incidencias con estupefacientes (CASO 7)


                        var msgFinal = "";
                        #region  CAS 1: NO COMERCIALITZAT

                        if (_iCodsNacionalsNoComerc > 0)
                        {
                            int index = 0;
                            string sMissPar = "";
                            while (index < _iCodsNacionalsNoComerc)
                            {
                                msgFinal = constantes.codIncidenciaLinea;
                                if (pedido.ListasPedidos.codsNacionalsNoComerc[index].Length == 13)
                                    msgFinal += pedido.ListasPedidos.codsNacionalsNoComerc[index]; //codNacionalSencer calcula l'index de control i l'afegeix al codi
                                else
                                    msgFinal += (_sIniciCodNacional + funciones.codNacionalSencer(pedido.ListasPedidos.codsNacionalsNoComerc[index])); //codNacionalSencer calcula l'index de control i l'afegeix al codi
                                msgFinal += "0".PadLeft(16, '0');
                                msgFinal += constantes.K_INCID_NUEVAESPECIALIDAD; // "09"; // NUEVA ESPECIALIDAD
                                msgFinal += _sIniciCodNacional.PadRight(13, '0');
                                msgFinal += "\r\n";


                                //_missatgesFinals[_numberOfMessages] = constantes.codIncidenciaLinea;
                                //if (pedido.codsNacionalsNoComerc[index].Length == 13)
                                //    _missatgesFinals[_numberOfMessages] += pedido.codsNacionalsNoComerc[index]; //codNacionalSencer calcula l'index de control i l'afegeix al codi
                                //else
                                //    _missatgesFinals[_numberOfMessages] += (_sIniciCodNacional + funciones.codNacionalSencer(pedido.codsNacionalsNoComerc[index])); //codNacionalSencer calcula l'index de control i l'afegeix al codi
                                //_missatgesFinals[_numberOfMessages] += "0".PadLeft(16, '0');
                                //_missatgesFinals[_numberOfMessages] += constantes.K_INCID_NUEVAESPECIALIDAD; // "09"; // NUEVA ESPECIALIDAD
                                //_missatgesFinals[_numberOfMessages] += _sIniciCodNacional.PadRight(13, '0');
                                //_missatgesFinals[_numberOfMessages] += "\r\n";

                                _missatgesFinals.Add(msgFinal);
                                _numberOfMessages++;
                                msgFinal = "";

                                sMissPar = " Cod. nacional " + pedido.ListasPedidos.codsNacionalsNoComerc[index] + " no comercializado";
                                _missatgesFinals.Add(constantes.codIncidenciaTextoLibre + sMissPar + "\r\n");
                                _numberOfMessages++;
                                msgFinal = "";

                                _bHayTextoLibre = true;
                                index++;
                            }
                        }
                        #endregion

                        //_codsNacionalsBloquejats

                        #region  CAS 2: BLOQUEADOS

                        if (_iCodsNacionalsBloquejats > 0)
                        {
                            string sMissPar = "";
                            int index = 0;

                            while (index < _iCodsNacionalsBloquejats)
                            {
                                if (!pedido.ListasPedidos.codsNacionalsNoComerc.Contains(pedido.ListasPedidos.codsNacionalsBloquejats[index]))
                                {
                                    msgFinal = constantes.codIncidenciaLinea;
                                    if (pedido.ListasPedidos.codsNacionalsBloquejats[index].Length != 13)
                                        msgFinal += (_sIniciCodNacional + funciones.codNacionalSencer(pedido.ListasPedidos.codsNacionalsBloquejats[index])); //codNacionalSencer calcula l'index de control i l'afegeix al codi
                                    else
                                        msgFinal += pedido.ListasPedidos.codsNacionalsBloquejats[index];

                                    msgFinal += pedido.ListasPedidos.qtysNacionalsBloquejats[index]; //Qty pedida
                                    msgFinal += pedido.ListasPedidos.qtysNacionalsBloquejats[index]; //Qty Falta (Ponemos el total porque no servimos nada)
                                    msgFinal += "0".PadLeft(8, '0'); //Per bonificacions
                                    msgFinal += constantes.K_INCID_SINEXISTENCIAS; // "01"; // NO HAY EXISTENCIAS
                                    msgFinal += _sIniciCodNacional.PadRight(13, '0'); // No procede poner un material de sustitución y como no afecta lo que pongamos dejamos este valor 0000000847000 o bien 0000000000000 según venía EAN-13 o no en línia
                                    msgFinal += "\r\n";

                                    //_missatgesFinals[_numberOfMessages] = constantes.codIncidenciaLinea;

                                    //if (pedido.codsNacionalsBloquejats[index].Length != 13)
                                    //    _missatgesFinals[_numberOfMessages] += (_sIniciCodNacional + funciones.codNacionalSencer(pedido.codsNacionalsBloquejats[index])); //codNacionalSencer calcula l'index de control i l'afegeix al codi
                                    //else
                                    //    _missatgesFinals[_numberOfMessages] += pedido.codsNacionalsBloquejats[index];

                                    //_missatgesFinals[_numberOfMessages] += pedido.qtysNacionalsBloquejats[index]; //Qty pedida
                                    //_missatgesFinals[_numberOfMessages] += pedido.qtysNacionalsBloquejats[index]; //Qty Falta (Ponemos el total porque no servimos nada)
                                    //_missatgesFinals[_numberOfMessages] += "0".PadLeft(8, '0'); //Per bonificacions
                                    //_missatgesFinals[_numberOfMessages] += constantes.K_INCID_SINEXISTENCIAS; // "01"; // NO HAY EXISTENCIAS
                                    //_missatgesFinals[_numberOfMessages] += _sIniciCodNacional.PadRight(13, '0'); // No procede poner un material de sustitución y como no afecta lo que pongamos dejamos este valor 0000000847000 o bien 0000000000000 según venía EAN-13 o no en línia
                                    //_missatgesFinals[_numberOfMessages] += "\r\n";

                                    _missatgesFinals.Add(msgFinal);
                                    _numberOfMessages++;
                                    msgFinal = "";

                                    // Per a que aquests missatges al nostre log quedin reflectits igual que els 2011 hem de guardar-los i afegir-los al final quan ja haguemenviat a client
                                    //---- GSG (18/10/2019)
                                    //sMissPar = " Cod. nacional " + _codsNacionalsBloquejats[index] + " sin existencias";
                                    sMissPar = " Cod. nacional " + pedido.ListasPedidos.codsNacionalsBloquejats[index] + " sin existencias - " + pedido.ListasPedidos.qtysNacionalsBloquejats[index].PadLeft(4, '0');
                                    _missatgesFinals.Add(constantes.codIncidenciaTextoLibre + sMissPar + "\r\n");
                                    _numberOfMessages++;
                                    msgFinal = "";

                                    _bHayTextoLibre = true;

                                }

                                index++;
                            }
                        }


                        #endregion

                        #region CAS 3 i 4: MATERIALS NO EXISTENTS I MATERIALS INACTIUS

                        //// Rellena _codsNacionalsNOK y _codsNacionalsNOKPorInactivos
                        //codsNacionalsOK(); 

                        //// Omple els missatges
                        ////if (_iCodsNacionalsNOK > 0 || _iCodsNacionalsNOKPorInactivos > 0)
                        //if ( _iCodsNacionalsNOK > 0 || _iCodsNacionalsNOKPorInactivos > 0)
                        //{
                        //    string sMissPar = "";
                        //    int indice = 0;
                        //    while (indice < _iCodsNacionalsNOK)
                        //    {
                        //        if (!_codsNacionalsNoComerc.Contains(_codsNacionalsNOK[indice]) && !_codsNacionalsBloquejats.Contains(_codsNacionalsNOK[indice])) 
                        //        {
                        //            // Quan substituim per un material amb codi EAN-13 no mostrarem el missatge de que no existeix
                        //            if (_codsNacionalsNOK[indice].Length != 13)
                        //            {
                        //                _missatgesFinals[_numberOfMessages] = codIncidenciaLinea;

                        //                //if (_codsNacionalsNOK[indice].Length != 13)
                        //                    _missatgesFinals[_numberOfMessages] += (_sIniciCodNacional + codNacionalSencer(_codsNacionalsNOK[indice])); //codNacionalSencer calcula l'index de control i l'afegeix al codi
                        //                //else
                        //                //    _missatgesFinals[_numberOfMessages] += _codsNacionalsNOK[indice];

                        //                _missatgesFinals[_numberOfMessages] += "0".PadLeft(16, '0');
                        //                _missatgesFinals[_numberOfMessages] += K_INCID_NOTRABAJADO; // "03"; // NO TRABAJADO
                        //                _missatgesFinals[_numberOfMessages] += _sIniciCodNacional.PadRight(13, '0');  // No procede poner un material de sustitución y como no afecta lo que pongamos dejamos este valor 0000000847000 o bien 0000000000000 según venía EAN-13 o no en línia
                        //                _missatgesFinals[_numberOfMessages] += "\r\n";
                        //                _numberOfMessages++;

                        //                // Per a que aquests missatges al nostre log quedin reflectits igual que els 2011 hem de guardar-los i afegir-los al final quan ja haguemenviat a client
                        //                sMissPar = " El cod. nacional " + _codsNacionalsNOK[indice] + " no existe";
                        //                _missatgesFinals[_numberOfMessages] = codIncidenciaTextoLibre + sMissPar + "\r\n";
                        //                _numberOfMessages++;

                        //                _bHayTextoLibre = true; 
                        //            }
                        //        }

                        //        indice++;
                        //    }

                        //    indice = 0;
                        //    while (indice < _iCodsNacionalsNOKPorInactivos)
                        //    {
                        //        if (!_codsNacionalsNoComerc.Contains(_codsNacionalsNOKPorInactivos[indice]) && !_codsNacionalsBloquejats.Contains(_codsNacionalsNOKPorInactivos[indice]) && !_codsNacionalsNOK.Contains(_codsNacionalsNOKPorInactivos[indice])) 
                        //        {
                        //            // Aquests missatges s'han d'enviar com a 2015 
                        //            _missatgesFinals[_numberOfMessages] = codIncidenciaLinea;

                        //            if (_codsNacionalsNOKPorInactivos[indice].Length != 13)
                        //                _missatgesFinals[_numberOfMessages] += (_sIniciCodNacional + codNacionalSencer(_codsNacionalsNOKPorInactivos[indice]));
                        //            else
                        //                _missatgesFinals[_numberOfMessages] += _codsNacionalsNOKPorInactivos[indice];

                        //            _missatgesFinals[_numberOfMessages] += "0".PadLeft(16, '0');
                        //            _missatgesFinals[_numberOfMessages] += K_INCID_BAJA; // "11"; // BAJA
                        //            _missatgesFinals[_numberOfMessages] += _sIniciCodNacional.PadRight(13, '0'); // No procede poner un material de sustitución y como no afecta lo que pongamos dejamos este valor 0000000847000 o bien 0000000000000 según venía EAN-13 o no en línia
                        //            _missatgesFinals[_numberOfMessages] += "\r\n";
                        //            _numberOfMessages++;

                        //            // Per a que aquests missatges al nostre log quedin reflectits igual que els 2011 hem de guardar-los i afegir-los al final quan ja haguemenviat a client
                        //            sMissPar = " El cod. nacional " + _codsNacionalsNOKPorInactivos[indice] + " no está activo";
                        //            _missatgesFinals[_numberOfMessages] = codIncidenciaTextoLibre + sMissPar + "\r\n";
                        //            _numberOfMessages++;

                        //            _bHayTextoLibre = true; 
                        //        }

                        //        indice++;
                        //    }
                        //}

                        #endregion

                        //---- GSG (17/10/2016)
                        #region CAS 3: MATERIALS NO EXISTENTS (NO SON DE STADA)
                        if (_iCodsNacionalsNOStada > 0)
                        {
                            string sMissPar = "";
                            int indice = 0;
                            while (indice < _iCodsNacionalsNOStada)
                            {
                                msgFinal = constantes.codIncidenciaLinea; //2015 

                                if (pedido.ListasPedidos.codsNacionalsNOStada[indice].Length != 13)
                                    msgFinal += (_sIniciCodNacional + funciones.codNacionalSencer(pedido.ListasPedidos.codsNacionalsNOStada[indice]));
                                else
                                    msgFinal += pedido.ListasPedidos.codsNacionalsNOStada[indice];

                                msgFinal += "0".PadLeft(16, '0');
                                msgFinal += constantes.K_INCID_NOTRABAJADO; // "03"; // NO TRABAJADO
                                msgFinal += _sIniciCodNacional.PadRight(13, '0');  // No procede poner un material de sustitución y como no afecta lo que pongamos dejamos este valor 0000000847000 o bien 0000000000000 según venía EAN-13 o no en línia
                                msgFinal += "\r\n";
                                //_missatgesFinals[_numberOfMessages] = constantes.codIncidenciaLinea; //2015 

                                //if (pedido.codsNacionalsNOStada[indice].Length != 13)
                                //    _missatgesFinals[_numberOfMessages] += (_sIniciCodNacional + funciones.codNacionalSencer(pedido.codsNacionalsNOStada[indice]));
                                //else
                                //    _missatgesFinals[_numberOfMessages] += pedido.codsNacionalsNOStada[indice];

                                //_missatgesFinals[_numberOfMessages] += "0".PadLeft(16, '0');
                                //_missatgesFinals[_numberOfMessages] += constantes.K_INCID_NOTRABAJADO; // "03"; // NO TRABAJADO
                                //_missatgesFinals[_numberOfMessages] += _sIniciCodNacional.PadRight(13, '0');  // No procede poner un material de sustitución y como no afecta lo que pongamos dejamos este valor 0000000847000 o bien 0000000000000 según venía EAN-13 o no en línia
                                //_missatgesFinals[_numberOfMessages] += "\r\n";

                                _missatgesFinals.Add(msgFinal);
                                _numberOfMessages++;
                                msgFinal = "";

                                // Per a que aquests missatges al nostre log quedin reflectits igual que els 2011 hem de guardar-los i afegir-los al final quan ja haguemenviat a client
                                sMissPar = " El cod. nacional " + pedido.ListasPedidos.codsNacionalsNOStada[indice] + " no existe";
                                _missatgesFinals.Add(constantes.codIncidenciaTextoLibre + sMissPar + "\r\n");
                                _numberOfMessages++;
                                msgFinal = "";

                                _bHayTextoLibre = true;


                                indice++;
                            }
                        }
                        #endregion

                        #region CAS 4: MATERIALS INACTIUS
                        if (_iCodsNacionalsNOKPorInactivos > 0)
                        {
                            string sMissPar = "";
                            int indice = 0;
                            while (indice < _iCodsNacionalsNOKPorInactivos)
                            {
                                if (!pedido.ListasPedidos.codsNacionalsNoComerc.Contains(pedido.ListasPedidos.codsNacionalsNOKPorInactivos[indice]) && !pedido.ListasPedidos.codsNacionalsBloquejats.Contains(pedido.ListasPedidos.codsNacionalsNOKPorInactivos[indice]) && !pedido.ListasPedidos.codsNacionalsNOStada.Contains(pedido.ListasPedidos.codsNacionalsNOKPorInactivos[indice])) // prevalecen el resto de mensajes que le puedan afectar
                                {
                                    // Aquests missatges s'han d'enviar com a 2015
                                    msgFinal = constantes.codIncidenciaLinea;

                                    if (pedido.ListasPedidos.codsNacionalsNOKPorInactivos[indice].Length != 13)
                                        msgFinal += (_sIniciCodNacional + funciones.codNacionalSencer(pedido.ListasPedidos.codsNacionalsNOKPorInactivos[indice]));
                                    else
                                        msgFinal += pedido.ListasPedidos.codsNacionalsNOKPorInactivos[indice];

                                    msgFinal += "0".PadLeft(16, '0');
                                    msgFinal += constantes.K_INCID_BAJA; // "11"; // BAJA
                                    msgFinal += _sIniciCodNacional.PadRight(13, '0'); // No procede poner un material de sustitución y como no afecta lo que pongamos dejamos este valor 0000000847000 o bien 0000000000000 según venía EAN-13 o no en línia
                                    msgFinal += "\r\n";
                                    //_missatgesFinals[_numberOfMessages] = constantes.codIncidenciaLinea;

                                    //if (pedido.codsNacionalsNOKPorInactivos[indice].Length != 13)
                                    //    _missatgesFinals[_numberOfMessages] += (_sIniciCodNacional + funciones.codNacionalSencer(pedido.codsNacionalsNOKPorInactivos[indice]));
                                    //else
                                    //    _missatgesFinals[_numberOfMessages] += pedido.codsNacionalsNOKPorInactivos[indice];

                                    //_missatgesFinals[_numberOfMessages] += "0".PadLeft(16, '0');
                                    //_missatgesFinals[_numberOfMessages] += constantes.K_INCID_BAJA; // "11"; // BAJA
                                    //_missatgesFinals[_numberOfMessages] += _sIniciCodNacional.PadRight(13, '0'); // No procede poner un material de sustitución y como no afecta lo que pongamos dejamos este valor 0000000847000 o bien 0000000000000 según venía EAN-13 o no en línia
                                    //_missatgesFinals[_numberOfMessages] += "\r\n";

                                    _missatgesFinals.Add(msgFinal);
                                    _numberOfMessages++;
                                    msgFinal = "";

                                    // Per a que aquests missatges al nostre log quedin reflectits igual que els 2011 hem de guardar-los i afegir-los al final quan ja haguemenviat a client
                                    sMissPar = " El cod. nacional " + pedido.ListasPedidos.codsNacionalsNOKPorInactivos[indice] + " no está activo";
                                    _missatgesFinals.Add(constantes.codIncidenciaTextoLibre + sMissPar + "\r\n");
                                    _numberOfMessages++;
                                    msgFinal = "";

                                    _bHayTextoLibre = true;
                                }

                                indice++;
                            }
                        }
                        #endregion

                        #region  CAS 5: NO HI HA STOCK

                        if (_iSuperanStock > 0)
                        {
                            int index = 0;
                            string sMissPar = "";
                            while (index < _iSuperanStock)
                            {
                                //---- GSG (03/03/2016)
                                // Vamos a distinguir dos casos:
                                // - En pedidos directos cuando un material está sin stock (*) --> siempte se muestra " sin existencias" 
                                // - En pedidos tranfers tenemos dos casos: 
                                //      - No hay stock pero el material está activo --> en este caso no mostraremos ningún mensaje 
                                //      - No hay stock pero está comercializado (Material que está inactivo temporalmente por falta de stock) -->  " sin existencias" 
                                // (*) Nota 1: un material está en _codsNacionalsStock cuando no hay stock independientemente del tipo de pedido y de si el material está activo o no
                                // Nota 2: Un material está en _codsNacionalsNOKPorInactivos cuando está realmente inactivo (no están los inactivos por falta de stock)
                                // ESTO YA ESTÁ CONTROLADO A LA HORA DE PONER LOS MATERIALES EN LOS ARRAYS

                                if (!pedido.ListasPedidos.codsNacionalsNoComerc.Contains(pedido.ListasPedidos.codsNacionalsStock[index]) && !pedido.ListasPedidos.codsNacionalsBloquejats.Contains(pedido.ListasPedidos.codsNacionalsStock[index]) && !pedido.ListasPedidos.codsNacionalsNOK.Contains(pedido.ListasPedidos.codsNacionalsStock[index]) && !pedido.ListasPedidos.codsNacionalsNOKPorInactivos.Contains(pedido.ListasPedidos.codsNacionalsStock[index]))
                                {
                                    // Quan substituim per un material amb codi EAN-13 no mostrarem el missatge de que no hi ha existencies
                                    if (pedido.ListasPedidos.codsNacionalsStock[index].Length != 13)
                                    {
                                        msgFinal = constantes.codIncidenciaLinea;

                                        //if (_codsNacionalsStock[index].Length != 13) //Amb la condició anterior afegida no caldria però ho deixo per si hem de desfer en un futur, no ens oblidem de posar-ho
                                        msgFinal += (_sIniciCodNacional + funciones.codNacionalSencer(pedido.ListasPedidos.codsNacionalsStock[index])); //codNacionalSencer calcula l'index de control i l'afegeix al codi
                                        //else
                                        //    _missatgesFinals[_numberOfMessages] += _codsNacionalsStock[index];

                                        msgFinal += pedido.ListasPedidos.qtysNacionalsStock[index]; //Qty pedida
                                                                            //_missatgesFinals[_numberOfMessages] += (int.Parse(_qtysNacionalsStock[index]) - int.Parse(_stocksNacionalsStock[index])).ToString().PadLeft(4, '0'); //Qty falta
                                        msgFinal += pedido.ListasPedidos.qtysNacionalsStock[index]; //Qty Falta (Ponemos el total porque no servimos nada)
                                        msgFinal += "0".PadLeft(8, '0'); //Per bonificacions
                                        msgFinal += constantes.K_INCID_SINEXISTENCIAS; // "01"; // NO HAY EXISTENCIAS
                                        msgFinal += _sIniciCodNacional.PadRight(13, '0'); // No procede poner un material de sustitución y como no afecta lo que pongamos dejamos este valor 0000000847000 o bien 0000000000000 según venía EAN-13 o no en línia
                                        msgFinal += "\r\n";

                                        //_missatgesFinals[_numberOfMessages] = constantes.codIncidenciaLinea;

                                        ////if (_codsNacionalsStock[index].Length != 13) //Amb la condició anterior afegida no caldria però ho deixo per si hem de desfer en un futur, no ens oblidem de posar-ho
                                        //_missatgesFinals[_numberOfMessages] += (_sIniciCodNacional + funciones.codNacionalSencer(pedido.codsNacionalsStock[index])); //codNacionalSencer calcula l'index de control i l'afegeix al codi
                                        ////else
                                        ////    _missatgesFinals[_numberOfMessages] += _codsNacionalsStock[index];

                                        //_missatgesFinals[_numberOfMessages] += pedido.qtysNacionalsStock[index]; //Qty pedida
                                        ////_missatgesFinals[_numberOfMessages] += (int.Parse(_qtysNacionalsStock[index]) - int.Parse(_stocksNacionalsStock[index])).ToString().PadLeft(4, '0'); //Qty falta
                                        //_missatgesFinals[_numberOfMessages] += pedido.qtysNacionalsStock[index]; //Qty Falta (Ponemos el total porque no servimos nada)
                                        //_missatgesFinals[_numberOfMessages] += "0".PadLeft(8, '0'); //Per bonificacions
                                        //_missatgesFinals[_numberOfMessages] += constantes.K_INCID_SINEXISTENCIAS; // "01"; // NO HAY EXISTENCIAS
                                        //_missatgesFinals[_numberOfMessages] += _sIniciCodNacional.PadRight(13, '0'); // No procede poner un material de sustitución y como no afecta lo que pongamos dejamos este valor 0000000847000 o bien 0000000000000 según venía EAN-13 o no en línia
                                        //_missatgesFinals[_numberOfMessages] += "\r\n";

                                        _missatgesFinals.Add(msgFinal);
                                        _numberOfMessages++;
                                        msgFinal = "";

                                        // Per a que aquests missatges al nostre log quedin reflectits igual que els 2011 hem de guardar-los i afegir-los al final quan ja haguemenviat a client
                                        //---- GSG (18/10/2019)
                                        //sMissPar = " Cod. nacional " + _codsNacionalsStock[index] + " sin existencias";
                                        sMissPar = " Cod. nacional " + pedido.ListasPedidos.codsNacionalsStock[index] + " sin existencias - " + pedido.ListasPedidos.qtysNacionalsStock[index].PadLeft(4, '0');
                                        _missatgesFinals.Add(constantes.codIncidenciaTextoLibre + sMissPar + "\r\n");
                                        _numberOfMessages++;
                                        msgFinal = "";

                                        _bHayTextoLibre = true;
                                    }
                                }

                                index++;
                            }
                        }

                        #endregion

                        //---- GSG (05/03/2014)
                        #region CAS 6: MATERIALS SUSTITUITS

                        if (pedido.ListasPedidos.codsNacionalsSustitucion.Count > 0) //Mostrar como incidencia 
                        {
                            int indice = 0;
                            string sMissPar = "";

                            while (indice < pedido.ListasPedidos.codsNacionalsSustitucion.Count)
                            {
                                if (!pedido.ListasPedidos.codsNacionalsNoComerc.Contains(pedido.ListasPedidos.codsNacionalsSustitucion[indice][1]) && !pedido.ListasPedidos.codsNacionalsBloquejats.Contains(pedido.ListasPedidos.codsNacionalsSustitucion[indice][1]) && !pedido.ListasPedidos.codsNacionalsNOK.Contains(pedido.ListasPedidos.codsNacionalsSustitucion[indice][1]) &&
                                    !pedido.ListasPedidos.codsNacionalsNOKPorInactivos.Contains(pedido.ListasPedidos.codsNacionalsSustitucion[indice][1]) && !pedido.ListasPedidos.codsNacionalsStock.Contains(pedido.ListasPedidos.codsNacionalsSustitucion[indice][1])) //---- GSG (19/11/2015)
                                {
                                    // Quan substituim per un material amb codi EAN-13 no mostrarem el missatge de substitució
                                    if (pedido.ListasPedidos.codsNacionalsSustitucion[indice][1].Length != 13)
                                    {

                                        msgFinal = constantes.codIncidenciaLinea; //2015
                                        msgFinal += (_sIniciCodNacional + funciones.codNacionalSencer(pedido.ListasPedidos.codsNacionalsSustitucion[indice][0]));
                                        msgFinal += "0".PadLeft(16, '0');
                                        msgFinal += constantes.K_INCID_NOTRABAJADO; // "03"; // NO TRABAJADO
                                        msgFinal += (_sIniciCodNacional + funciones.codNacionalSencer(pedido.ListasPedidos.codsNacionalsSustitucion[indice][1]));
                                      



                                        //_missatgesFinals[_numberOfMessages] = constantes.codIncidenciaLinea; //2015

                                        ////if (_codsNacionalsSustitucion[indice][0].Length != 13)
                                        //_missatgesFinals[_numberOfMessages] += (_sIniciCodNacional + funciones.codNacionalSencer(pedido.codsNacionalsSustitucion[indice][0]));
                                        ////else
                                        ////    _missatgesFinals[_numberOfMessages] += _codsNacionalsSustitucion[indice][0];

                                        //_missatgesFinals[_numberOfMessages] += "0".PadLeft(16, '0');
                                        //_missatgesFinals[_numberOfMessages] += constantes.K_INCID_NOTRABAJADO; // "03"; // NO TRABAJADO

                                        ////if (_codsNacionalsSustitucion[indice][1].Length != 13)
                                        //_missatgesFinals[_numberOfMessages] += (_sIniciCodNacional + funciones.codNacionalSencer(pedido.codsNacionalsSustitucion[indice][1]));
                                        ////else
                                        ////    _missatgesFinals[_numberOfMessages] += codNacionalSencer(_codsNacionalsSustitucion[indice][1]);

                                        _missatgesFinals.Add(msgFinal);
                                        _numberOfMessages++;
                                        msgFinal = "";

                                        // Per a que aquests missatges al nostre log quedin reflectits igual que els 2011 hem de guardar-los i afegir-los al final quan ja haguemenviat a client
                                        sMissPar = " Cod. nacional " + pedido.ListasPedidos.codsNacionalsSustitucion[indice][0] + " sustituido por " + pedido.ListasPedidos.codsNacionalsSustitucion[indice][1];
                                        _missatgesFinals.Add(constantes.codIncidenciaTextoLibre + sMissPar + "\r\n");
                                        _numberOfMessages++;
                                        msgFinal = "";

                                        _bHayTextoLibre = true;
                                    }
                                }

                                indice++;
                            }
                        }
                        #endregion

                        //---- GSG (05/02/2016)
                        #region CAS 7: ESTUPEFACIENTE SIN VALE - NO ES ESTUPEFACIENTE - ESTUPEFACIENTE EN LINEA INCORRECTA (NO 1040)
                        /*  COMENTAT A L'ESPERA DE POSAR-HO EN MARXA (FUNCIONA OK)
                        if (_iIncidEstupefacientes > 0)
                        {
                            int index = 0;
                            string sMissPar = "";
                            while (index < _iIncidEstupefacientes)
                            {
                                //mira si este material ha dado incidencia por sustitucion
                                int indiceS = 0;
                                bool bYaTiene = false;
                                while (indiceS < _codsNacionalsSustitucion.Count)
                                {
                                    if (_codsNacionalsSustitucion[indiceS][1] == _IncidEstupefacientes[index][0])
                                    {
                                        bYaTiene = true;
                                        break;
                                    }
                                    indiceS++;
                                }

                                if (!_codsNacionalsNoComerc.Contains(_IncidEstupefacientes[index][0]) && !_codsNacionalsNOK.Contains(_IncidEstupefacientes[index][0]) &&
                                    !_codsNacionalsNOKPorInactivos.Contains(_IncidEstupefacientes[index][0]) && !_codsNacionalsStock.Contains(_IncidEstupefacientes[index][0]) &&
                                    !bYaTiene)
                                {
                                    // no miro si no estan comercialitzats perquè ja ho he fet abans d'omplir la llista d'incidències relacionades amb estupefaents
                                    _missatgesFinals[_numberOfMessages] = codIncidenciaLinea; // 2015

                                    if (_IncidEstupefacientes[index][0].Length != 13)
                                        _missatgesFinals[_numberOfMessages] += (_sIniciCodNacional + codNacionalSencer(_IncidEstupefacientes[index][0]));
                                    else
                                        _missatgesFinals[_numberOfMessages] += _IncidEstupefacientes[index][0];

                                    _missatgesFinals[_numberOfMessages] += "0".PadLeft(16, '0');
                                    _missatgesFinals[_numberOfMessages] += _IncidEstupefacientes[index][1]; // "15" o "16" o "17"
                                    _missatgesFinals[_numberOfMessages] += _sIniciCodNacional.PadRight(13, '0');
                                    _missatgesFinals[_numberOfMessages] += "\r\n";
                                    _numberOfMessages++;

                                    // Per a que aquests missatges al nostre log quedin reflectits igual que els 2011 hem de guardar-los i afegir-los al final quan ja haguemenviat a client
                                    sMissPar = " Cod. nacional " + _IncidEstupefacientes[index][0];
                                    switch (_IncidEstupefacientes[index][1])
                                    {
                                        case K_INCID_ESTUPEFASINVALE: sMissPar += " estupefaciente sin vale";
                                            break;
                                        case K_INCID_ESTUPEFANOES: sMissPar += " no es estupefaciente";
                                            break;
                                        case K_INCID_ESTUPEFALINNOK: sMissPar += " estupef. en linea no ok";
                                            break;
                                    }

                                    _missatgesFinals[_numberOfMessages] = codIncidenciaTextoLibre + sMissPar + "\r\n";
                                    _numberOfMessages++;

                                    _bHayTextoLibre = true;
                                }

                                index++;
                            }
                        }
                        */
                        #endregion

                        //---- FI GSG


                        //---- GSG (10/02/2016)
                        // Triga molt a partir els fitxers després de llegir la línea 1050, per tant enviaré aquestes incidencies abans per a que el client no hagi d'esperar tant
                        // ho he eliminat de 0199
                        // Envío de la respuesta al cliemte, mesajes 2010, 2011 y 2015
                        envioRespuestaACliente();
                        //---- FI GSG (10/02/2016)

                        _iPedIdR = -1; //---- GSG (22/02/2016)

                        if (bRechazado)
                        {
                            _bRechazado = true;
                            _msgLibre = msgLibre;

                            //---- GSG (24/11/2015)
                            //Guarda capçalera de la comanda rebutjada                            
                            int iPedIdR = guardarDatos.GuardarCapPedidoBD(_capcaLine, _numberOfLinesPerOrder, _qtyPerOrder, _bonificacionsPerOrder, false, false, false, false,_sNomfitxerDesti, DEFAULT_FILE_STORE_LOC,pedido.codPedAux);
                            _iPedIdR = iPedIdR; //---- GSG (22/02/2016)

                            //Guarda Línies
                            for (int i = 0; i < _numberOfLinesPerOrder; i++)
                            {
                                int iLinId = -1;

                                // Insertar en BD la líneas del pedido que no son regalos
                                iLinId = guardarDatos.GuardarLinPedidoBD(pedido.ListasPedidos.pedLines[i], iPedIdR, false);
                            }
                        }
                        else
                        {
                            #region Mirar si hay que añadir regalos
                            pedido.ExtraListasPedidos.listRegalosAnyadir = regalos.RegalosAAnyadir(pedido.sTipoPedido, pedido.ExtraListasPedidos.listContadorRegalos);
                            _numRegalosPedido = pedido.ExtraListasPedidos.listRegalosAnyadir.Count();

                            #endregion


                            #region Guarda a BD la comanda

                            int iPedId = -1;
                            string lineRegalo = "";

                            //A quantitat no sumo les quantitats de regal, no cal. Al fitxer sí que les hi posaré però a BD no les guardem
                            // Tot i que la comanda es pot haber dividit fins i tot en X fitxers, a BD la mantindrem com una sola
                            if (_numberOfLinesPerOrder > 0)
                            {
                                //Guarda capçalera
                                //---- GSG (26/05/2015)
                                //iPedId = GuardarCapPedidoBD(_capcaLine, _numberOfLinesPerOrder + _iApendToToGen + _iApendToToCH, _qtyPerOrder, _bonificacionsPerOrder, false, false, false);
                                //---- GSG (24/11/2015)
                                //iPedId = GuardarCapPedidoBD(_capcaLine, _numberOfLinesPerOrder, _qtyPerOrder, _bonificacionsPerOrder, false, false, false);
                                iPedId = guardarDatos.GuardarCapPedidoBD(_capcaLine, _numberOfLinesPerOrder, _qtyPerOrder, _bonificacionsPerOrder, false, false, false, true, _sNomfitxerDesti, DEFAULT_FILE_STORE_LOC, pedido.codPedAux);
                                _iPedIdR = iPedId; //---- GSG (22/02/2016)
                                //Guarda Línies
                                for (int i = 0; i < _numberOfLinesPerOrder; i++)
                                {
                                    int iLinId = -1;

                                    // Insertar en BD la líneas del pedido que no son regalos
                                    //---- GSG (24/11/2015)
                                    //iLinId = GuardarLinPedidoBD(_pedLines[i], iPedId);
                                    iLinId = guardarDatos.GuardarLinPedidoBD(pedido.ListasPedidos.pedLines[i], iPedId, true);
                                }



                                //Una vez añadidas las líneas normales añadimos las de regalo
                                if (_numRegalosPedido > 0)
                                {
                                    for (int i = 0; i < _numRegalosPedido; i++)
                                    {
                                        ArrayList lin = pedido.ExtraListasPedidos.listRegalosAnyadir[i];
                                        // Código y subcódigo línea (1020)
                                        lineRegalo = constantes.codLineaRegalo;
                                        //código artículo
                                        lineRegalo += lin[constantes.K_POS_TREGAL_COD].ToString().PadLeft(13);

                                        //---- GSG (14/07/2016)
                                        // AQUÍ NO HACE FALTA PORQUE ES PARA GUARDAR EN BD Y SOLO ME INTERESA CAMBIARLO CUANDO VA AL FICHERO
                                        //string codProvisional = lin[K_POS_TREGAL_COD].ToString();
                                        //string elCN = "";
                                        //if (getCNRegalo(codProvisional, out elCN))
                                        //    lineRegalo += elCN.PadLeft(13);
                                        //else
                                        //    lineRegalo += lin[K_POS_TREGAL_COD].ToString().PadLeft(13);


                                        //cantidad
                                        int qtyRegalo = int.Parse(lin[constantes.K_POS_TREGAL_UNIDADES].ToString()); //---- GSG (21/01/2014)
                                        if (bool.Parse(lin[constantes.K_POS_TREGAL_MULTIPLO].ToString()))
                                        {
                                            //---- GSG (21/01/2014) Control de la divisió per 0
                                            if (int.Parse(lin[constantes.K_POS_TREGAL_MIN].ToString()) != 0)
                                                qtyRegalo = (int.Parse(lin[constantes.K_POS_TREGAL_CONTADOR].ToString()) / int.Parse(lin[constantes.K_POS_TREGAL_MIN].ToString())) * int.Parse(lin[constantes.K_POS_TREGAL_UNIDADES].ToString()); //packs de min * qty a regalar
                                        }
                                        lineRegalo += qtyRegalo.ToString().PadLeft(4, '0');
                                        //final de línea
                                        lineRegalo += "\r\n";

                                        //---- GSG (24/11/2015)
                                        //GuardarLinPedidoBD(lineRegalo, iPedId);
                                        guardarDatos.GuardarLinPedidoBD(lineRegalo, iPedId, true);

                                    }
                                }
                            }
                            #endregion

                            #region Escriu a fitxer ok

                            //Escriu la primera línia 
                            _m_totalClientDataSize += _firstLine.Length;
                            _m_cfgFile.Write(_firstLine);
                            _m_cfgFile.Flush();
                            //Escriu la capçalera
                            _m_totalClientDataSize += _capcaLine.Length;
                            _m_cfgFile.Write(_capcaLine);
                            _m_cfgFile.Flush();
                            //Escriu les línies de la comanda
                            foreach (string l in sortAscendingQueryByCodNac)
                            {
                                _m_totalClientDataSize += l.Length;
                                _m_cfgFile.Write(l);
                                _m_cfgFile.Flush();
                            }

                            //Escriu la línia de totals
                            _m_totalClientDataSize += _totalsLine.Length;
                            _m_cfgFile.Write(_totalsLine);
                            _m_cfgFile.Flush();

                            #endregion
                        }
                        #endregion

                    }
                    else // Error código + subcodigo
                    {
                        _m_processState = STATE.FILE_CLOSED;
                        length = -1;
                        //Envia missatge de rebuig de connexió
                        funcionesIncidencias.SendIncidenciaRechazoTransmision("Error en línea de datos (código y/o subcódigo de línea incorrectos). Línea: " + oneLine, _m_clientSocket);
                        TancarConnexio();

                        //SendTrazaToLogs("ProcessClientData: Inicio de línea incorrecto. No se reconocen el código y subcódigo indicados. Línea: " + oneLine); 
                    }
                    break;
                case STATE.FILE_CLOSED:

                    break;
                default:
                    break;
            }
        }

        private void TratarLineas1020i1030i1040(string oneLine, string tipoLin)
        {//AQUI ESTA EL PROBLEMA
            int iIndexCodNacional = 0, iIndexCodNacionalSencer=0;

            //SCS 05/12/2022 booleano que avisa si es necesario sumar en el contador de pedidosLin
            bool? bSumarCantidadLineasPerOrder = true;
            //---- GSG (22/08/2016)
            // Detectar que una línea se ha eliminado de pedLines[]
            bool bEliminada = false;

            // En la versión 2 del protocolo Fedicom, las líneas 1020, 1030 y 1040 vienen con 50 posiciones de texto libre al final.
            // Como no nos interesan para nada y parece que la escritura a fichero se ralentiza, las eliminamos
            switch (tipoLin)
            {
                case "1020":
                    oneLine = oneLine.Substring(0, 21) + "\r\n";
                    break;
                case "1030":
                    oneLine = oneLine.Substring(0, 29) + "\r\n";
                    break;
                case "1040":
                    oneLine = oneLine.Substring(0, 50) + "\r\n";
                    break;
            }


            // guarda les línies llegides per al final 
            pedido.ListasPedidos.pedLines.Add(oneLine);

            // Guardem els codis de material per al final poder mirar si existeixem a CE. 
            // A vegades els codis nacionals venen amb 7 dígits enlloc de 6. 
            // Si dels 7 el primer és 0, vol dir que no ve el dígit de control i l'hem de calcular
            // Si no, tenim dos casos:
            // 1.- ETRON: L'últim dígit és 0 --> Cal calcular el dígit de control (ETRON posa un 0 al final i no al inici com la resa de majoristes quan sols envía 6 dígits)
            // 2.- La resta (cas normal): L'últim dígit és el de control 
            // Nota: un cas normal amb dígit 0 l'estem tractant com ETRON però no passa res perquè el càlcul donarà 0

            //REVISAR ALEX - 22/01/2024

            string codProvisional = oneLine.Substring(10, 7);
            string sCodNacional = "", sCodNacionalSencer="";
            if (codProvisional.Substring(0, 1) == "0") // No viene el dígito de control y tenemos que calcularlo
            {
                sCodNacional = long.Parse(oneLine.Substring(11, 6).Trim()).ToString();
                sCodNacionalSencer = funciones.codNacionalSencer(oneLine.Substring(11, 6).Trim());
            }
            else // Viene el dígito de control
            {
                sCodNacional = long.Parse(oneLine.Substring(10, 6).Trim()).ToString();

                if (codProvisional.Substring(6, 1) == "0") //cas ETRON
                    sCodNacionalSencer = funciones.codNacionalSencer(oneLine.Substring(10, 6).Trim());
                else //cas normal
                    // Hem vist que a vegades ens envien el dígit de control malament --> Encara que ens el posin el calcularem igualment  
                    sCodNacionalSencer = funciones.codNacionalSencer(oneLine.Substring(10, 6).Trim());
            }

            pedido.ListasPedidos.codsNacionals.Add(sCodNacional);
            pedido.ListasPedidos.codsNacionalsSencers.Add(sCodNacionalSencer);



            // EAN-13 o 6+1 Ho necesitaré per a les línies d'incidència. Sols cal que ho agafi una vegada pq és igual per a tots els materials
            if (_sIniciCodNacional == "")
                _sIniciCodNacional = oneLine.Substring(4, 6);


            // Unitats a la línia
            int qtyLin = int.Parse(oneLine.Substring(17, 4).Trim());

            // Primer he de tractar substituts, ja que si el material té substitut hauré de fer la resta de comprobacions amb ell i no amb l'original
            bool bConSubst = false;
            if (pedido.ExtraListasPedidos.listConSustituto != null && pedido.ExtraListasPedidos.listConSustituto.Count() > 0)
            {
                int indice2 = pedido.ExtraListasPedidos.listConSustituto.Where(x => x == sCodNacional).Count(); // Ho mira sols amb els 6 dígits normals del CN
                bConSubst = indice2 >= 1 ? true : false;
            }


            // Tenemos que detectar qué materiales sustitutos viene con EAN-13 por dos motivos:
            //-- 1.- Hay que enviarlos con este formato a SAP
            //-- 2.- Como son códigos nacionales no reales, no enviaremos el mensaje de NO TRABAJADO (el dígito de control en este caso puede estar mal y lo detectaría como inexistente)
            // _codsNacionals y _codsNacionalsSencers llevarán los 13 dígitos sólo en este caso

            string sustituto = null;
            int iUnidadesSustituto = 0;
            if (bConSubst)
            {
                if (funciones.esMaterialSustituido(sCodNacional, out sustituto))
                {
                    if (sustituto.Length != 13)
                        sustituto = sustituto.Substring(0, 6); //Para asegurar que no cogemos el dígito de control si lo trae
                                                               //else tiene formato EAN-13 (caso especial)



                    //Apuntamos el cambio para poder, después, mostrar incidencias (aquí porque en expositor no hay que mostrar el mensaje)
                    string[] cambio = new string[2];
                    cambio[0] = sCodNacional;
                    cambio[1] = sustituto;
                    pedido.ListasPedidos.codsNacionalsSustitucion.Add(cambio);




                    // Miramos si el material lleva unidades base
                    // Un expositor puede sustituir a un material con n unidades pedidas

                    // ---- GSG (21/12/2016) Cambio. Se dará el expositor cuando las unidades sean iguales o inferirores a las del expositor
                    // Ejemplo: expositor de 16 unidades:
                    // Si 5 u --> 1 exp
                    //   16 u --> 1 exp
                    //   20 u --> 2 exp
                    //   32 u --> 2 exp
                    //   60 u --> 3 exp

                    ////////// Calcular el número de expositores a servir según unidades pedidas redondeando
                    ////////// Si las unidades pedidas no llegan a las de 1 solo expositor --> servir unidades de forma individual


                    int iUB = funciones.getUnidadesBaseSustituto(sCodNacional);

                    if (iUB > 0)
                    {
                        // ---- GSG (21/12/2016)
                        //if (qtyLin >= iUB)
                        //    iUnidadesSustituto = int.Parse(Math.Round(double.Parse(qtyLin.ToString()) / double.Parse(iUB.ToString())).ToString());  // Math.Round(19.00/7.00) = 3.00 i Math.Round(19.00/8.00) = 2.00
                        iUnidadesSustituto = int.Parse(Math.Ceiling(double.Parse(qtyLin.ToString()) / double.Parse(iUB.ToString())).ToString());
                    }

                    // Debido a la incorporación de las unidades en sustituto, no siempre que un material tenga sustituto quedará sustituido --> cambiamos sólo cuando sea necesario
                    if (iUB == 0) // Caso normal (no expositor)
                    {
                        // LO QUITAMOS DE AQUÍ Y LO PONEMOS MÁS ARRIBA PORQUE AHORA SÍ QUEREMOS QUE SE MUESTRE EL MENSAJE EN EL CASO DE EXPOSITOR
                        ////Apuntamos el cambio para poder, después, mostrar incidencias (aquí porque en expositor no hay que mostrar el mensaje)
                        //string[] cambio = new string[2];
                        //cambio[0] = _codsNacionals[_numberOfLinesPerOrder - 1];
                        //cambio[1] = sustituto;
                        //_codsNacionalsSustitucion.Add(cambio);


                        //Cambiamos el código nacional por el nuevo
                        iIndexCodNacional = pedido.ListasPedidos.codsNacionals.FindIndex(ind => ind.Equals(sCodNacional));
                        iIndexCodNacionalSencer = pedido.ListasPedidos.codsNacionalsSencers.FindIndex(ind => ind.Equals(sCodNacionalSencer));


                        pedido.ListasPedidos.codsNacionals[iIndexCodNacional] = sustituto;
                        sCodNacional = sustituto;

                        //Guardamos el nuevo con su correspondiente dígito de control
                        if (sustituto.Length != 13)
                            sCodNacionalSencer = funciones.codNacionalSencer(sustituto);
                        else
                            sCodNacionalSencer = sustituto;

                        //pedido.ListasPedidos.codsNacionalsSencers[iIndexCodNacionalSencer] = sCodNacional;--COMENTADO POR ALEX - 19/01/2024
                        pedido.ListasPedidos.codsNacionals[iIndexCodNacionalSencer] = sCodNacional;
                        pedido.ListasPedidos.codsNacionalsSencers[iIndexCodNacionalSencer] = sCodNacionalSencer;

                    }
                    else if (iUnidadesSustituto > 0) // Caso expositor --> hay cambio de material y cambio en las unidades
                    {
                        _qtyDeMenysAlSubstituirPerOrder += (qtyLin - iUnidadesSustituto); //Cal abans de canviar el valor de qtyLin
                        qtyLin = iUnidadesSustituto;


                        //Cambiamos el código nacional por el nuevo
                        sCodNacional = sustituto;

                        //Guardamos el nuevo con su correspondiente dígito de control
                        //iIndexCodNacional = pedido.ListasPedidos.codsNacionals.FindIndex(ind => ind.Equals(sCodNacional));//AGREGADO POR ALEX - 19/01/2024
                        iIndexCodNacionalSencer = pedido.ListasPedidos.codsNacionalsSencers.FindIndex(ind => ind.Equals(sCodNacionalSencer));

                        if (sustituto.Length != 13)
                            sCodNacionalSencer = funciones.codNacionalSencer(sustituto);
                        else
                            sCodNacionalSencer = sustituto;

                        //pedido.ListasPedidos.codsNacionalsSencers[iIndexCodNacionalSencer] = sCodNacional;--COMENTADO POR ALEX - 19/01/2024
                        pedido.ListasPedidos.codsNacionals[iIndexCodNacionalSencer] = sCodNacional;
                        pedido.ListasPedidos.codsNacionalsSencers[iIndexCodNacionalSencer] = sCodNacionalSencer;

                        // En el caso expositor no debe salir el mensaje de sustitución.
                    }
                    //else  No hacemos nada porque los códigos són los originales y la cantidad también                   
                }
            }

            // Guardamos los 6 dígitos de cod. nacional ya que en _codsNacionals pueden venir 13 cuando se trata de un sustituto "especial" con EAN-13
            string codNac6D = sCodNacional;
            if (codNac6D.Length == 13)
                codNac6D = codNac6D.Substring(6, 6);


            //---- GSG (17/10/2016) 
            //-- Nos damos cuenta de que los materiales detectados como que no son de STADA (cod nac xxxxxx no existe) se están enviado al fichero al igual que los inactivos :(
            // Arrrgggg!!! Los borraban a mano? Carmen?
            // Bueno, a partir de ahora los trataremos de la misma manera que los bloqueados, los no comercializados, ...

            // Volem distingir entre els que no són d'Stada (mai) i els que ho han estat alguna vegada i ara estan inactius
            // Retorn de EnQueEstadoExisteCodNacional:
            //      0: No existe, no es material de Stada
            //     -1: Existe pero está inactivo (y no es un material desactivado temporalmente por falta de stock)
            //      1: Existe y está activo

            int estadoExiste = U.EnQueEstadoExisteCodNacional(codNac6D);
            if (estadoExiste == 0)
            {
                pedido.ListasPedidos.codsNacionalsNOStada.Add(sCodNacional);
                _iCodsNacionalsNOStada++;

                //Resto ja que la línea afegida a _pedLines al pas anterior no s'ha de tenir en compte     
                pedido.ListasPedidos.pedLines.RemoveAt(_numberOfLinesPerOrder);
                bSumarCantidadLineasPerOrder = false;
                //bEliminada = true;
                //ALEX - INICIO - 22/01/2024
                pedido.ListasPedidos.codsNacionals.RemoveAt(_numberOfLinesPerOrder);
                pedido.ListasPedidos.codsNacionalsSencers.RemoveAt(_numberOfLinesPerOrder);
                //ALEX - FIN - 22/01/2024
                _qtyNOStada += qtyLin;

                if (tipoLin == "1030")
                {
                    int bonifLin = int.Parse(oneLine.Substring(21, 4).Trim());
                    _bonifNOStadaPerOrder += bonifLin;
                }
            }
            else if (estadoExiste == -1)
            {
                pedido.ListasPedidos.codsNacionalsNOKPorInactivos.Add(sCodNacional);
                _iCodsNacionalsNOKPorInactivos++;

                //Resto ja que la línea afegida a _pedLines al pas anterior no s'ha de tenir en compte       
                pedido.ListasPedidos.pedLines.RemoveAt(_numberOfLinesPerOrder);
                bSumarCantidadLineasPerOrder = false;
                //ALEX - INICIO - 22/01/2024
                pedido.ListasPedidos.codsNacionals.RemoveAt(_numberOfLinesPerOrder);
                pedido.ListasPedidos.codsNacionalsSencers.RemoveAt(_numberOfLinesPerOrder);
                //ALEX - FIN - 22/01/2024
                //bEliminada = true;
                _qtyNOKPorInactivos += qtyLin;

                if (tipoLin == "1030")
                {
                    int bonifLin = int.Parse(oneLine.Substring(21, 4).Trim());
                    _bonifNOKPorInactivosPerOrder += bonifLin;
                }
            }
            else
            {
                // (*) ATENCIÓ!!!!!!!!!!!!!!!!!!!
                // Com a resultat de la reunió del dia 21/06/2016 no mirarem si un material està comercialitzat o no


                // És un material No Comercialitzat?
                // (aquests materials és com si no existisin, no van al fitxer, no sumen import i brut i no es tenen en compte per a comprovacions)
                bool bNoComerc = false;

                // (*) ---- GSG (11/07/2016)
                //if (_lNoComerc != null && _lNoComerc.Count() > 0)
                //{
                //    int indice = _lNoComerc.FindIndex(s => s.Equals(codNac6D)); 
                //    bNoComerc = indice >= 0 ? true : false;
                //}


                if (bNoComerc)
                {
                    pedido.ListasPedidos.codsNacionalsNoComerc.Add(sCodNacional); // Guarda 6 dígits o EAN-13 si es substitut especial (sabem que el dígit de control que porta pot no estar bé --> no el podem calcular)
                    _iCodsNacionalsNoComerc++;

                    //Resto ja que la línea afegida a _pedLines al pas anterior no s'ha de tenir en compte       
                    pedido.ListasPedidos.pedLines.RemoveAt(_numberOfLinesPerOrder);
                    bSumarCantidadLineasPerOrder = false;
                    //ALEX - INICIO - 22/01/2024
                    pedido.ListasPedidos.codsNacionals.RemoveAt(_numberOfLinesPerOrder);
                    pedido.ListasPedidos.codsNacionalsSencers.RemoveAt(_numberOfLinesPerOrder);
                    //ALEX - FIN - 22/01/2024
                    //bEliminada = true; //---- GSG (22/08/2016)

                    _qtyNoComerc += qtyLin;

                    if (tipoLin == "1030")
                    {
                        int bonifLin = int.Parse(oneLine.Substring(21, 4).Trim());
                        _bonifNoComercPerOrder += bonifLin;
                    }
                }
                else
                {
                    //-- Entren en joc els materials bloquejats
                    //-- Un material por estar bloquejat per tipus de comanda i/o segons si és de Canàries o no
                    //-- No anirán al fitxer, no sumaran a import però sí mostrarán missatge de bloqueig dient que no hi ha stock (es comporten igual que els no comercialitzats)
                    bool bBloqueado = false;
                    if (pedido.ExtraListasPedidos.listlBloqueados != null && pedido.ExtraListasPedidos.listlBloqueados.Count() > 0)
                    {
                        int indice = pedido.ExtraListasPedidos.listlBloqueados.Where(s => s.Equals(codNac6D)).Count();
                        bBloqueado = indice >= 1 ? true : false;
                    }

                    if (bBloqueado)
                    {
                        pedido.ListasPedidos.codsNacionalsBloquejats.Add(sCodNacional); // Guarda 6 dígits o EAN-13 si es substitut especial (sabem que el dígit de control que porta pot no estar bé --> no el podem calcular)
                        pedido.ListasPedidos.qtysNacionalsBloquejats.Add(qtyLin.ToString().PadLeft(4, ' '));
                        _iCodsNacionalsBloquejats++;

                        //Resto ja que la línea afegida a _pedLines al pas anterior no s'ha de tenir en compte                   
                        pedido.ListasPedidos.pedLines.RemoveAt(_numberOfLinesPerOrder);
                        bSumarCantidadLineasPerOrder = false;
                        //bEliminada = true; //---- GSG (22/08/2016)
                        //REVISAR ALEX - 22/01/2024
                        
                        //ALEX - INICIO - 22/01/2024
                        pedido.ListasPedidos.codsNacionals.RemoveAt(_numberOfLinesPerOrder);
                        pedido.ListasPedidos.codsNacionalsSencers.RemoveAt(_numberOfLinesPerOrder);
                        //ALEX - FIN - 22/01/2024

                        _qtyBloquejats += qtyLin;

                        if (tipoLin == "1030")
                        {
                            int bonifLin = int.Parse(oneLine.Substring(21, 4).Trim());
                            _bonifBloquejatsPerOrder += bonifLin;
                        }

                        //---- GSG (22/07/2019) -- Ara també tindrem el compte les línies despreciades per a condició d'import
                        float brutLiniesDespreciades = U.PVPMaterial(null, codNac6D, DateTime.Today) * qtyLin;
                        _totalBrutComanda += brutLiniesDespreciades;
                    }
                    else
                    {
                        //va claculant els totals globals
                        _qtyPerOrder += qtyLin;

                        // La bonificación es la única diferencia entre las 1020 y 1030
                        if (tipoLin == "1030")
                        {
                            int bonifLin = int.Parse(oneLine.Substring(21, 4).Trim());
                            _bonificacionsPerOrder += bonifLin;
                        }


                        // La línea supera el stock? Sólo para pedidos directos. 
                        // A partir de julio 2013 también para transfers
                        // En octubre de 2014 volvemos a dejarlo sólo para directos
                        // En mayo 2015 Añadimos KB (Envío producto)
                        // En marzo de 2016 vuelvo a añadir transfer per sólo si el material está comercializado y a la vez inactivo (temporal por falta de existencias no es un inactivo real)              
                        // En octubre 2017 KB cambia por ZKBW
                        //if (_sTipoPedido == "ZDIW" || _sTipoPedido == "KB" || (_sTipoPedido == "ZTRW" && U.EsComercializadoInactivo(codNac6D) == 1))
                        if (pedido.sTipoPedido == "ZDIW" || pedido.sTipoPedido == "KB" || pedido.sTipoPedido == "ZKBW" || (pedido.sTipoPedido == "ZTRW" && U.EsComercializadoInactivo(codNac6D) == 1))
                        {
                            int stockLin = U.StockMaterial(codNac6D);

                            if (stockLin < qtyLin)
                            {
                                //---- GSG (05/07/2016)
                                // Las líneas sin stock no bloqueadas, en pedidos directos se van a despreciar
                                //if (_sTipoPedido == "ZDIW" || _sTipoPedido == "KB")
                                if (pedido.sTipoPedido == "ZDIW" || pedido.sTipoPedido == "KB" || pedido.sTipoPedido == "ZKBW")
                                {
                                    //Resto ja que la línea afegida a _pedLines al pas anterior no s'ha de tenir en compte                
                                    // i al igual que els bloquejats direm que no tenen stock. Per 
                                    pedido.ListasPedidos.codsNacionalsBloquejats.Add(sCodNacional); // Guarda 6 dígits o EAN-13 si es substitut especial (sabem que el dígit de control que porta pot no estar bé --> no el podem calcular)
                                    pedido.ListasPedidos.qtysNacionalsBloquejats.Add(qtyLin.ToString().PadLeft(4, ' '));
                                    _iCodsNacionalsBloquejats++;

                                    pedido.ListasPedidos.pedLines.RemoveAt(_numberOfLinesPerOrder);
                                    bSumarCantidadLineasPerOrder = false;
                                    //ALEX - INICIO - 22/01/2024
                                    pedido.ListasPedidos.codsNacionals.RemoveAt(_numberOfLinesPerOrder);
                                    pedido.ListasPedidos.codsNacionalsSencers.RemoveAt(_numberOfLinesPerOrder);
                                    //ALEX - FIN - 22/01/2024
                                    bEliminada = true; //---- GSG (22/08/2016)         

                                    //---- GSG (22/07/2019) -- Ara també tindrem el compte les línies despreciades per a condició d'import
                                    float brutLiniesDespreciades = U.PVPMaterial(null, codNac6D, DateTime.Today) * qtyLin;
                                    _totalBrutComanda += brutLiniesDespreciades;

                                }
                                else
                                {
                                    //esto lo he dejado igual pero dentro de else
                                    pedido.ListasPedidos.codsNacionalsStock.Add(sCodNacional);
                                    pedido.ListasPedidos.qtysNacionalsStock.Add(qtyLin.ToString().PadLeft(4, ' '));
                                    pedido.ListasPedidos.stocksNacionalsStock.Add(stockLin.ToString().PadLeft(4, ' '));

                                    _iSuperanStock++;
                                }
                            }
                        }

                        if (!bEliminada) //---- GSG (22/08/2016))
                        {
                            //Preu brut línia calculat a partir del PVP de la BD de preus. Al final cal veure si per total brut rebutgem o no la comanda
                            float brutLinia = U.PVPMaterial(null, codNac6D, DateTime.Today) * qtyLin;

                            _totalBrutComanda += brutLinia;


                            // Afegeix les unitats del material a la taula de regals si és que cal
                            // setContadorRegalo Va sumant les quantitats per producte de regal i així, al final, sabrem si hem d'afegir línies de regal al fitxer
                            if (_iNumRegalos > 0)
                                regalos.setContadorRegalo(codNac6D, qtyLin, pedido.ExtraListasPedidos.listContadorRegalos);


                            // Cal canviar el codi de material a la línia original en dos casos:
                            // 1.- Si hi ha substitut
                            // 2.- Si venia sense dígit de control (cal posar-lo)
                            // Cal tenir en compte que si venia en format EAN-13 l'hem de mantenir i que si el substitut és EAN-13 també (en el darrer cas _codsNacionalsSencers ja té els 13 dígits, en el primer no y ho agafarem de la línia on ja ve)
                            // También se ha de controlar si el número de unidades ha cambiado en la sustitución de materiales

                            if (_numberOfLinesPerOrder >= 0) //---- GSG (07/07/2016) per evitar error quan la primera línea es desprecia ja que _numberOfLinesPerOrder és 0
                            {
                                string oneLineModif = "";

                                if (sCodNacionalSencer.Length == 13)
                                {   // 4 + 13 + el resto
                                    oneLineModif = oneLine.Substring(0, 4);
                                    oneLineModif += sCodNacionalSencer;
                                }
                                else
                                {   // 10 + 7 + el resto
                                    oneLineModif = oneLine.Substring(0, 10);
                                    oneLineModif += sCodNacionalSencer;
                                }

                                if (iUnidadesSustituto > 0)
                                {
                                    oneLineModif += qtyLin.ToString().PadLeft(4, '0');
                                    oneLineModif += oneLine.Substring(21, oneLine.Length - 21);
                                }
                                else
                                    oneLineModif += oneLine.Substring(17, oneLine.Length - 17);

                                pedido.ListasPedidos.pedLines[_numberOfLinesPerOrder] = oneLineModif;
                            }
                        }
                    }
                }
            }

            if (bSumarCantidadLineasPerOrder==true)
                _numberOfLinesPerOrder++;
        }
        
        private void TancarConnexio()
        {
            byte[] buffer = Encoding.UTF8.GetBytes("0199\r\n");
            _m_clientSocket.Send(buffer);
            funcionesIncidencias.SendTrazaToLogs("0199\r\n",_sNomfitxerIncidencies_traza);

            //---- GSG (02/03/2016) Tanquem la connexió inmediatament després d'enviar final de resposta
            Thread.Sleep((int)TimeSpan.FromSeconds(2).TotalMilliseconds);
            StopSocketListener();
        }

        public void codsNacionalsAparte()
        {
            int iTipoAparte = -1;
            string cod = "";
            string fit = "";
            int iPosFit = -1;
            int iNumFitsAparte = 0;


            _lListaMaterialesAparte = U.GetListaMaterialesAparte(); //Totes les línies de la taula  MaterialesAparte amb tots els camps per aL tipus de comanda

            _fitsAparte = U.GetFicherosAparte(); //Posibles ficheros a generar (sin tener en cuenta si ch o gen)
            iNumFitsAparte = _fitsAparte.Count;


            for (int i = 0; i < iNumFitsAparte; i++)
            {
                _lCodsNacionalsAparte.Add(new List<string>());
                _lCodsNacionalsAparteCH.Add(new List<string>());
            }

            // Revisa todas la líneas que han llegada y detrmina en que fichero van a ir
            for (int i = 0; i < _numberOfLinesPerOrder; i++)
            {
                cod = pedido.ListasPedidos.codsNacionals[i];
                if (cod != null && cod.Length == 13)  // Alguns són EAN-13
                    cod = pedido.ListasPedidos.codsNacionals[i].Substring(6, 6);
                iTipoAparte = funciones.EsCodNacionalAparte(cod, _lListaMaterialesAparte, pedido.sTipoPedido);
                if (iTipoAparte != -1) // Si són aparte mirar de quin fitxer
                {
                    fit = funciones.GetFicheroAparte(cod, _lListaMaterialesAparte);
                    for (int j = 0; j < iNumFitsAparte; j++) // Busca la posición que ocupa el fichero en el array
                    {
                        if (_fitsAparte[j] == fit)
                        {
                            iPosFit = j;

                            switch (iTipoAparte)
                            {
                                case 0:
                                    _lCodsNacionalsAparte[j].Add(pedido.ListasPedidos.codsNacionals[i]); // La posición j tiene la lista de códigos del fichero xj (puede pasar que quede alguna de las listas vacías)
                                    _iCodsNacionalsAparte++;
                                    break;
                                case 1:
                                    _lCodsNacionalsAparteCH[j].Add(pedido.ListasPedidos.codsNacionals[i]);
                                    _iCodsNacionalsAparteCH++;
                                    break;
                            }

                            j = iNumFitsAparte; //Para acabar una vez encontrado
                        }
                    }
                }
            }


            //for (int i = 0; i < iNumFitsAparte; i++)
            //{
            //    _lCodsNacionalsAparte.Add(new List<string>());
            //    _lCodsNacionalsAparteCH.Add(new List<string>());
            //}

            //// Revisa todas la líneas que han llegada y detrmina en que fichero van a ir
            //for (int i = 0; i < _numberOfLinesPerOrder; i++)
            //{
            //    cod = _codsNacionals[i];
            //    if (cod != null && cod.Length == 13)  // Alguns són EAN-13
            //        cod = _codsNacionals[i].Substring(6, 6);
            //    iTipoAparte = EsCodNacionalAparte(cod);
            //    if (iTipoAparte != -1) // Si són aparte mirar de quin fitxer
            //    {
            //        fit = GetFicheroAparte(cod);
            //        for (int j = 0; j < iNumFitsAparte; j++) // Busca la posición que ocupa el fichero en el array
            //        {
            //            if (_fitsAparte[j] == fit)
            //            {
            //                iPosFit = j;

            //                switch (iTipoAparte)
            //                {
            //                    case 0:
            //                        _lCodsNacionalsAparte[j].Add(_codsNacionals[i]); // La posición j tiene la lista de códigos del fichero xj (puede pasar que quede alguna de las listas vacías)
            //                        _iCodsNacionalsAparte++;
            //                        break;
            //                    case 1:
            //                        _lCodsNacionalsAparteCH[j].Add(_codsNacionals[i]);
            //                        _iCodsNacionalsAparteCH++;
            //                        break;
            //                }

            //                j = iNumFitsAparte; //Para acabar una vez encontrado
            //            }
            //        }
            //    }
            //}
        }

        public bool codsNacionalsCH()
        {
            bool ret = false;
            _iCodsNacionalsCH = 0;

            //---- GSG (12/05/2014)
            // Los CH también se van a separar para pedidos directos

            //if (_sTipoPedido != "ZDIW") //---- GSG (11/06/2013) Per les directes no se separan els CH
            //{
            //---- GSG (12/11/2014) alguns són EAN-13
            string cod = "";

            for (int i = 0; i < _numberOfLinesPerOrder; i++)
            {
                cod = pedido.ListasPedidos.codsNacionals[i];
                if (cod != null && cod.Length == 13)
                    cod = cod.Substring(6, 6);

                //---- GSG (26/05/2015)
                ////if (U.EsCodNacionalCH(_codsNacionals[i]) != 0 && !U.EstaEnLista(_codsNacionals[i], _codsNacionalsAparteCH))
                //if (U.EsCodNacionalCH(cod) != 0)
                //{
                //    ret = true;
                //    _codsNacionalsCH[_iCodsNacionalsCH] = _codsNacionals[i];
                //    _iCodsNacionalsCH++;
                //}

                if (_lCodsNacionalsAparteCH != null)
                {
                    //---- GSG (22/07/2019)
                    //for (int j = 0; j < _lCodsNacionalsAparteCH.Count; j++)
                    //{
                    //    if (U.EsCodNacionalCH(cod) != 0 && !U.EstaEnLista(_codsNacionals[i], _lCodsNacionalsAparteCH[j].ToArray()))
                    //    {
                    //        ret = true;
                    //        _codsNacionalsCH[_iCodsNacionalsCH] = _codsNacionals[i];
                    //        _iCodsNacionalsCH++;
                    //    }
                    //}

                    bool bEsta = false;
                    for (int j = 0; j < _lCodsNacionalsAparteCH.Count; j++)
                    {
                        bEsta = U.EstaEnLista(pedido.ListasPedidos.codsNacionals[i], _lCodsNacionalsAparteCH[j].ToArray());
                        if (bEsta)
                            break;
                    }

                    if (!bEsta && U.EsCodNacionalCH(cod) != 0)
                    {
                        ret = true;
                        pedido.ListasPedidos.codsNacionalsCH.Add(pedido.ListasPedidos.codsNacionals[i]);
                        _iCodsNacionalsCH++;
                    }


                }
                else if (U.EsCodNacionalCH(cod) != 0)
                {
                    ret = true;
                    pedido.ListasPedidos.codsNacionalsCH.Add(pedido.ListasPedidos.codsNacionals[i]);
                    _iCodsNacionalsCH++;
                }
            }

            //}

            return ret;
        }

        private void envioRespuestaACliente()
        {
            //Línea inicial: cabecera del fichero tal cual
            byte[] buffer = Encoding.UTF8.GetBytes(_firstLine);
            _m_clientSocket.Send(buffer);

            //SendTrazaToLogs("ProcessClientData Enviando Respuesta. Cabecera: " + _firstLine); 
            funcionesIncidencias.SendTrazaToLogs(_firstLine,_sNomfitxerIncidencies_traza);

            //Líneas de incidencias (de cabecera (2010) + libres (2011) + línea (2015))
            funcionesIncidencias.SendIncidenciasToClient(_m_clientSocket,_numberOfMessages,_missatgesFinals,_sNomfitxerIncidencies_traza);

            //--- en aquest punt queda per enviar el 9999
        }

        private string respostaCapcalera(out string msg, out bool bRechazado)
        {
            string ret = constantes.codIncidenciaCabecera; //Código y subcódigo 2010
            string rechazo = "0";               //Por defecto no rechazado

            msg = "";
            bRechazado = false;

            //Codigo cliente
            string sCodCliWithNoS = pedido.Cliente.codClienteLine;
            string sCodCli = "S" + pedido.Cliente.codClienteLine.PadLeft(10, '0');

            // Comprueba la existencia y estado del cliente en CE
            int existe = U.ExisteCliente(sCodCli);
            sCodCli = sCodCli.PadRight(16, ' ');

            ret += sCodCli;
            switch (existe)
            {
                case 0: //No existe
                    msg += "Cliente no existe.#";
                    rechazo = "1";
                    break;
                case 1: //Existe i ok
                    break;
                case 2: //Existe pero no está activo
                    msg += "Cliente no activo.#";
                    rechazo = "1";
                    break;
            }

            //Codigo pedido

            // Comproba si la comanda existeix a recepció per a un client
            // (No cal tenir en compte que si la comanda està partida el codi de comanda és diferent perquè en aquesta última versió encara que el fitxer es parteixi sols tenim una comanda a BD recepció)
            //_sCodPed inicializado al leer la línea 1010

            DateTime fechaEnvio = DateTime.Parse(_firstLine.Substring(10, 2) + "/" + _firstLine.Substring(8, 2) + "/" + _firstLine.Substring(4, 4));

            //---- GSG (03/06/2016)
            //int totalLineas = int.Parse(_totalsLine.Substring(4, 4));
            //int totalUnidades = int.Parse(_totalsLine.Substring(8, 6));
            int totalLineas = _numberOfLinesPerOrder;
            int totalUnidades = _qtyPerOrder;

            existe = U.ExistePedidoEnRecepcion(pedido.codPedAux, sCodCliWithNoS.Trim(), fechaEnvio, totalLineas, totalUnidades);

            ret += pedido.codPedAux;
            switch (existe)
            {
                case 0: //No existe --> ok
                    break;
                case 1: //Existe --> nok
                    msg += "El pedido ya existe.#";
                    rechazo = "1";
                    break;
            }

            //Rechazo pedido
            ret += rechazo; //Para considerar los posibles errores en el resto de campos podría tener que cambiarse el valor de esta posición al final de las comprobacioes.


            //Tipo pedido
            string sTipoPedido = pedido.sTipoPedido; //_capcaLine.Substring(30, 6); 
            //if (sTipoPedido.Trim() == "ZDIW" || sTipoPedido.Trim() == "ZTRW" || sTipoPedido.Trim() == "KB")
            if (sTipoPedido.Trim() == "ZDIW" || sTipoPedido.Trim() == "ZTRW" || pedido.sTipoPedido == "KB" || sTipoPedido.Trim() == "ZKBW")
                ret += "0";
            else
            {
                ret += "1";
                //msg += "Tipo distinto a ZDIW, ZTRW, KB.#";
                msg += "Tipo distinto a ZDIW, ZTRW, KB, ZKBW.#";
                rechazo = "1";
            }

            //Condiciones de servicio
            ret += "0";
            // Cargo cooperativo
            ret += "0";
            //Aplazamiento de cargo
            ret += "0";
            //Aplazamiento de cargo
            ret += "0";
            //Decuento/cargo adicional
            ret += "0";
            //Empresa facturadora
            ret += "0";
            //Almacén de servicio
            ret += "0";
            //Fecha envío pedido
            ret += "0";
            //Día envío pedido
            ret += "0";

            //Suma de totales
            bool bErrTotals = false;

            //Número de materiales
            // No hay que contar las línias repetidas
            //int numLins = int.Parse(_totalsLine.Substring(4, 4).Trim());
            int numLins = pedido.listPedidosWithoutRepe.Count - 4; // Resto las dos líneas de cabecera y las dos líneas de totales y final de fichero

            //if (numLins != _numberOfLinesPerOrder + _iCodsNacionalsNoComerc + _iCodsNacionalsBloquejats)  //---- GSG (12/05/2016) añade bloqueados
            if (numLins != _numberOfLinesPerOrder + _iCodsNacionalsNoComerc + _iCodsNacionalsBloquejats + _iCodsNacionalsNOStada + _iCodsNacionalsNOKPorInactivos)  //---- GSG (17/10/2016) añade materiales no stada                    
            {
                //CAMBIO PROVISIONAL PARA VER SI DESCUBRO PORQUE A VECES DA ESTE ERROR (ya está solucionado, creo pero lod dejo)
                //msg += "Total líneas incorrecto.#";
                //msg += "T. lín. NOK " + numLins.ToString() + "<>" + _numberOfLinesPerOrder.ToString() + " " + _iCodsNacionalsNoComerc + " " + _iCodsNacionalsBloquejats.ToString() + "#";
                msg += "T. lín. NOK " + numLins.ToString() + "<>" + _numberOfLinesPerOrder.ToString() + " " + _iCodsNacionalsNoComerc + " " + _iCodsNacionalsBloquejats.ToString() + " " + _iCodsNacionalsNOStada.ToString() + " " + _iCodsNacionalsNOKPorInactivos.ToString() + "#";

                rechazo = "1";
                bErrTotals = true;
            }

            //Total cantidad pedida
            int totalQty = int.Parse(_totalsLine.Substring(8, 6).Trim());

            //if (totalQty != _qtyPerOrder + _qtyNoComerc + _qtyBloquejats + _qtyDeMenysAlSubstituirPerOrder) //---- GSG (12/05/2016) añade bloqueados y los que hemos quitado en sustituciones (expositor)
            if (totalQty != _qtyPerOrder + _qtyNoComerc + _qtyBloquejats + _qtyDeMenysAlSubstituirPerOrder + _qtyNOStada + _qtyNOKPorInactivos) //---- GSG (17/10/2016) añade materiales que no son de stada                    
            {
                msg += "Total cantidad incorrecto.#";
                rechazo = "1";
                bErrTotals = true;
            }

            //Total bonificaciones
            int totalBonif = int.Parse(_totalsLine.Substring(14, 6).Trim());

            //if (totalBonif != _bonificacionsPerOrder + _bonifNoComercPerOrder + _bonifBloquejatsPerOrder) //---- GSG (12/05/2016) añade bloqueados   
            if (totalBonif != _bonificacionsPerOrder + _bonifNoComercPerOrder + _bonifBloquejatsPerOrder + _bonifNOStadaPerOrder + _bonifNOKPorInactivosPerOrder) //---- GSG (17/10/2016) añade los que no son de stada                       
            {
                msg += "Total bonificaciones incorrecto.#";
                rechazo = "1";
                bErrTotals = true;
            }

            if (bErrTotals)
                ret += "1";
            else
                ret += "0";

            //Se van a rechazar los pedidos inferiores a X euros cuando el pedido sea directo y los inferiores a Y (X e Y están en la tabla de configuración) 
            if (sTipoPedido.Trim() == "ZDIW")
            {
                float fImpMinDir = float.Parse(U.getConfguracion(constantes.K_CONF_IMPMINDIR));
                if (_totalBrutComanda < fImpMinDir)
                {
                    msg += "Importe " + _totalBrutComanda.ToString() + " < " + fImpMinDir.ToString() + ".#";
                    rechazo = "1";
                }
            }
            else if (sTipoPedido.Trim() == "ZTRW")
            {
                float fImpMinTra = float.Parse(U.getConfguracion(constantes.K_CONF_IMPMINTRA));
                if (_totalBrutComanda < fImpMinTra)
                {
                    msg += "Imp " + _totalBrutComanda.ToString() + " < " + fImpMinTra.ToString() + " Tramitar por mayorista habitual#";
                    rechazo = "1";
                }
            }
            //---- GSG (21/09/2016)
            //else if (sTipoPedido.Trim() == "KB")
            else if (sTipoPedido.Trim() == "ZKBW" || pedido.sTipoPedido == "KB")
            {
                float fImpMinKB = float.Parse(U.getConfguracion(constantes.K_CONF_IMPMINKB));
                if (_totalBrutComanda < fImpMinKB)
                {
                    msg += "Imp " + _totalBrutComanda.ToString() + " < " + fImpMinKB.ToString() + " .#";
                    rechazo = "1";
                }
            }


            // ZDIW no pot portar condiciones de servicio --> li treurem si en porta i,
            // ZTRW sempre n'ha de portar. Quan no en porta es rebutja la comanda.
            //if (sTipoPedido.Trim() == "ZDIW")
            //if (sTipoPedido.Trim() == "ZDIW" || sTipoPedido.Trim() == "KB") //---- GSG (21/09/2016)
            if (sTipoPedido.Trim() == "ZDIW" || pedido.sTipoPedido == "KB" || sTipoPedido.Trim() == "ZKBW")
            {
                if (_capcaLine.Substring(36, 6).Trim() != "")
                    _capcaLine = _capcaLine.Substring(0, 36) + "      " + _capcaLine.Substring(42, _capcaLine.Length - 42);
            }
            else if (sTipoPedido.Trim() == "ZTRW")
            {
                if (_capcaLine.Substring(36, 6).Trim() == "")
                {
                    msg += "ZTRW sin condic. servicio.#";
                    rechazo = "1";
                }
            }


            // Per a ZTRW també comprobarem si el majorista existeix a ce. Si no existeix rebutjarem la comanda.
            if (sTipoPedido.Trim() == "ZTRW")
            {
                string sDistribuidor = _capcaLine.Substring(36, 6).Trim();
                existe = U.EsDistribuidorOK(sDistribuidor);
                switch (existe)
                {
                    case 0: //No existe --> nok
                        msg += "Distribuidor no existe.#";
                        rechazo = "1";
                        break;
                    case 1: //Existe --> ok
                        break;
                }
            }


            if (rechazo == "1")
            {
                ret = ret.Substring(0, 30) + rechazo + ret.Substring(31, ret.Length - 31) + "Rechazo: Consulte en 934738889".PadRight(30, ' ');
                bRechazado = true;
            }
            else
                ret += "Recepcionado correctamente".PadRight(30, ' ');

            ret += "\r\n";

            if (msg != "")
            {
                if (msg.Length > 50)
                    msg = msg.Substring(0, 50);  //Sólo caben 50 carácteres

                msg = msg.Substring(0, msg.Length - 1) + "#"; //Per garantir que al final hi ha un #
            }

            return ret;
        }

        /*SCS 09/01/2023*/
        //Vaciamos todas las variables para no mantenerlas en la memoria
        private void cleanVariables()
        {
            pedido = null;
            _sortAscendingQueryByCodNac = null;
            _lListaMaterialesAparte = null;
            _fitsAparte = null;
            _lCodsNacionalsAparte = null;
            _lCodsNacionalsAparteCH = null;
            _missatgesFinals = null;
            _missatgesFinalsPerAfegir = null;
        }

    }
}

