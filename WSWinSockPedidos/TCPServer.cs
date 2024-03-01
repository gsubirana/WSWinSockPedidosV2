using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.IO;
using System.Configuration;


namespace WSWinSockPedidos
{
    public partial class TCPServer
    {
        public static String INI_FILE = "";

        private Int32 _ipNport = 3001; //Està al fitxer ini. Valor per defecte
        private IPAddress localAddr = IPAddress.Parse("172.31.233.18"); //Està al fitxer ini. Valor per defecte

        private TcpListener _m_server = null;
        private bool _m_stopServer = false;
		private bool _m_stopPurging = false;

        private Thread _m_serverThread = null;
        private Thread _m_purgingThread = null;

        private ArrayList _m_socketListenersList = null;        


        public TCPServer()
        {
            Inits();
        }

        ~TCPServer()
        {
            StopServer();
        }

        
        private void Inits()
        {
            ////Per debbugar. Comentar-ho
            //FileStream fsDebug = new FileStream(@"C:\debug.txt", FileMode.OpenOrCreate, FileAccess.Write);
            //StreamWriter m_streamWriterDebug = new StreamWriter(fsDebug);
            //m_streamWriterDebug.WriteLine("INICI DEBUG");
            //m_streamWriterDebug.Close();
            ////----------------------------

            try
            {
                INI_FILE = System.Configuration.ConfigurationManager.AppSettings.Get("IniFolder").ToString();

                FileStream fs = new FileStream(@INI_FILE, FileMode.Open, FileAccess.Read);
                StreamReader m_streamReader = new StreamReader(fs);

                string sIP = m_streamReader.ReadLine();
                
                //IPAddress localAddr = IPAddress.Parse(sIP); //---- GSG (22/02/2013) migració

               
                string sPuerto = m_streamReader.ReadLine();
                m_streamReader.Close();

                _ipNport = int.Parse(sPuerto);

                //---- GSG (22/02/2013) migració NO VA
                _m_server = new TcpListener(_ipNport);
                //_m_server = new TcpListener(localAddr, _ipNport);

            }
            catch (Exception e)
            {
                _m_server = null;
            }

        }


        public void StartServer()
        {
            if (_m_server != null)
            {
                // Create a ArrayList for storing SocketListeners before
                // starting the server.
                _m_socketListenersList = new ArrayList();

                // Start the Server and start the thread to listen client 
                // requests.
                _m_server.Start();
                _m_serverThread = new Thread(new ThreadStart(ServerThreadStart));
                _m_serverThread.Start();

                // Create a low priority thread that checks and deletes client
                // SocktConnection objcts that are marked for deletion.
                _m_purgingThread = new Thread(new ThreadStart(PurgingThreadStart));
                _m_purgingThread.Priority = ThreadPriority.Lowest;
                _m_purgingThread.Start();
            }
        }




        public void StopServer()
        {
            if (_m_server != null)
            {
                // It is important to Stop the server first before doing
                // any cleanup. If not so, clients might being added as
                // server is running, but supporting data structures
                // (such as m_socketListenersList) are cleared. This might
                // cause exceptions.

                // Stop the TCP/IP Server.
                _m_stopServer = true;
                _m_server.Stop();

                // Wait for one second for the the thread to stop.
                _m_serverThread.Join(1000);

                // If still alive; Get rid of the thread.
                if (_m_serverThread.IsAlive)
                {
                    _m_serverThread.Abort();
                }
                _m_serverThread = null;

                _m_stopPurging = true;
                _m_purgingThread.Join(1000);

                if (_m_purgingThread.IsAlive)
                {
                    _m_purgingThread.Abort();
                }
                _m_purgingThread = null;

                // Free Server Object.
                _m_server = null;

                // Stop All clients.
                StopAllSocketListers();
            }
        }


        // Method that stops all clients and clears the list.
		private void StopAllSocketListers()
		{
			foreach (TCPSocketListener socketListener in _m_socketListenersList)
			{
				socketListener.StopSocketListener();
			}
			// Remove all elements from the list.
			_m_socketListenersList.Clear();
			_m_socketListenersList = null;
		}


        private void ServerThreadStart()
        {
            // Client Socket variable;
            Socket clientSocket = null;
            TCPSocketListener socketListener = null;

            while (!_m_stopServer)
            {
                try
                {
                    // Wait for any client requests and if there is any 
                    // request from any client accept it (Wait indefinitely).
                    clientSocket = _m_server.AcceptSocket();

                    // Create a SocketListener object for the client.
                    socketListener = new TCPSocketListener(clientSocket);

                    // Add the socket listener to an array list in a thread 
                    // safe fashon.
                    //Monitor.Enter(m_socketListenersList);
                    lock (_m_socketListenersList)
                    {
                        _m_socketListenersList.Add(socketListener);
                    }
                    //Monitor.Exit(m_socketListenersList);

                    // Start a communicating with the client in a different
                    // thread.
                    socketListener.StartSocketListener();
                }
                catch (SocketException se)
                {
                    _m_stopServer = true;
                }
            }
        }

       


		// Thread method for purging Client Listeneres that are marked for
		// deletion (i.e. clients with socket connection closed). This thead
		// is a low priority thread and sleeps for 10 seconds and then check
		// for any client SocketConnection obects which are obselete and 
		// marked for deletion.
        private void PurgingThreadStart()
        {
            while (!_m_stopPurging)
            {
                ArrayList deleteList = new ArrayList();

                // Check for any clients SocketListeners that are to be
                // deleted and put them in a separate list in a thread sage
                // fashon.
                // Monitor.Enter(m_socketListenersList);
                lock (_m_socketListenersList)
                {
                    foreach (TCPSocketListener socketListener in _m_socketListenersList)
                    {
                        if (socketListener.IsMarkedForDeletion())
                        {
                            deleteList.Add(socketListener);
                            socketListener.StopSocketListener();
                        }
                    }

                    // Delete all the client SocketConnection ojects which are
                    // in marked for deletion and are in the delete list.
                    for (int i = 0; i < deleteList.Count; ++i)
                    {
                        _m_socketListenersList.Remove(deleteList[i]);
                    }
                }
                //Monitor.Exit(m_socketListenersList);

                deleteList = null;
                Thread.Sleep(10000);
            }
        }
    }
}
