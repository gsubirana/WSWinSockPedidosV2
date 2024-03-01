using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WSWinSockPedidos.Funciones
{
    class FuncionesIncidencias
    {
        Constantes constantes = new Constantes();
        GuardarDatos guardarDatos = new GuardarDatos();
                
        #region Funciones Incidencias

        public void SendIncidenciaRechazoTransmision(string msg, Socket _m_clientSocket)
        {
            string resposta = msg;
            if (resposta.Length > 50)
                resposta = resposta.Substring(0, 50);
            resposta = constantes.codIncidenciaRechazoTransmision + resposta.PadRight(50 - resposta.Length, ' ');
            resposta += "\r\n";
            byte[] buffer = Encoding.UTF8.GetBytes(resposta);
            _m_clientSocket.Send(buffer);
        }

        // Incidencias 2010, 2011 O 2015
        public void SendIncidenciasToClient(Socket _m_clientSocket, int _numberOfMessages, List<string> _missatgesFinals, string _sNomfitxerIncidencies_traza)
        {
            for (int i = 0; i < _numberOfMessages; i++)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(_missatgesFinals[i]);
                _m_clientSocket.Send(buffer);

                //SendTrazaToLogs("SendIncidenciasToClient Enviando Respuesta: " + _missatgesFinals[i]); 
                SendTrazaToLogs(_missatgesFinals[i], _sNomfitxerIncidencies_traza);
            }
        }

        public void SendIncidenciasToLogs(bool _bHayTextoLibre, string _sCodPed, int _numberOfMessages, List<string> _missatgesFinals,
            string _sNomfitxerDesti, string DEFAULT_FILE_STORE_LOC, int _iPedIdR)
        {
            string lin = "";
            string sCodCli = "";
            string sDescError = "";
            string sTipoLin = "";

            //---- PER GUANYAR VELOCITAT NO ENVIEM A FITXER
            /*            string sNomfitxerIncidencies = INCIDENCIAS_FILE_STORE_LOC + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') +
                          DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Hour.ToString().PadLeft(2, '0') +
                          DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0') + "_LOG.txt";


            StreamWriter fileLog = null;
            fileLog = new StreamWriter(sNomfitxerIncidencies);
            */
            //------------------------------------------------------------

            //ESTATS:
            // - Refusada
            // - No refusada amb incidències
            // - No refusada

            //for (int i = 0; i < _numberOfMessages; i++)
            foreach(string sMsgFinalLine in _missatgesFinals)
            {
                //lin = _missatgesFinals[i];
                lin = sMsgFinalLine;

                if (lin.Substring(0, 4) == "2010" && lin.Substring(30, 1) == "1") //Cabecera de pedido rechazado
                {
                    sCodCli = lin.Substring(4, 16);
                    sDescError = lin.Substring(42, lin.Length - 44); //42 - /r/n
                    sTipoLin = constantes.K_LINCAP_LOG;
                }
                else if (lin.Substring(0, 4) == "2010" && lin.Substring(30, 1) == "0") //Cabecera de pedido NO rechazado
                {
                    // Mirar si hay incidencias
                    if (_bHayTextoLibre)
                    {
                        sCodCli = lin.Substring(4, 16);
                        sDescError = "Revisar incidencias"; //pq hi ha incidències però no s'ha rebutjat la comanda
                        sTipoLin = constantes.K_LINCAP_LOG;
                    }
                    else
                    {
                        //continue;
                        sCodCli = lin.Substring(4, 16);
                        sDescError = lin.Substring(42, lin.Length - 44); //42 - /r/n
                        sTipoLin = constantes.K_LINCAP_LOG;
                    }
                }
                else if (lin.Substring(0, 4) == "2011") //Detall
                {
                    sDescError = lin.Substring(4, lin.Length - 6);
                    sTipoLin = constantes.K_LINCAP_DET;
                }

                //Envia a BD tanto las cabeceras como las 2011 (las 2015 no pq son para envío a cliente. Las mismas ya están reflejadas como 2011 también - para Monitor Pedidos - )
                if (lin.Substring(0, 4) != "2015")
                {
                    //---- GSG (18/10/2019) Para saber las unidades sin stock . en general guarda para todas pero ya tendremos ua consuta para rechazadas por stock
                    //GuardarLinLogBD(s,CodCli, _sCodPed, sDescError, sTipoLin);
                    string unidades = "0";
                    if (lin.IndexOf("sin existencias") > 0)
                        unidades = lin.Substring(lin.Length - 4, 4);
                    guardarDatos.GuardarLinLogBD(sCodCli, _sCodPed, sDescError, sTipoLin, unidades, _sNomfitxerDesti,DEFAULT_FILE_STORE_LOC,_iPedIdR);
                }

                //---- PER GUANYAR VELOCITAT NO ENVIEM A FITXER

                //Envia a fitxer log
                /*DateTime dAra = DateTime.Now;
                string linToFile = "[" + dAra.ToString() + "]: " + sCodCli + "-" + _sCodPed + "-" + sDescError;
                fileLog.WriteLine(linToFile);
                fileLog.Flush();
                */
                //---------------------------------------------------

            }

            //---- PER GUANYAR VELOCITAT NO ENVIEM A FITXER
            /*
            fileLog.Close();
            fileLog = null;
            */
            //---------------------------------------------------

        }

        public void SendTrazaToLogs(string sTraza,string _sNomfitxerIncidencies_traza)
        {
            StreamWriter fileLog = null;
            fileLog = new StreamWriter(_sNomfitxerIncidencies_traza, true);

            DateTime dAra = DateTime.Now;
            string linToFile = "[" + dAra.ToString() + "]: " + sTraza;
            fileLog.WriteLine(linToFile);
            fileLog.Flush();

            fileLog.Close();
            fileLog = null;
        }

        #endregion
    }
}
