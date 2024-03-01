using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.IO;
using System.Net.Sockets;

//using System.Collections;
//using System.Configuration;



namespace WSWinSockPedidos
{
    public partial class WSWinSockPedidos : System.ServiceProcess.ServiceBase
    {
        TCPServer _miTCPServer = null;


        public WSWinSockPedidos()
        {
            InitializeComponent();

            /*SCS BORRAR AL TERMINAR */
            //this.OnStart(null);
        }

        static void Main()
        {
            #if DEBUG
                System.Diagnostics.Debugger.Launch();
            #endif

            System.ServiceProcess.ServiceBase[] ServicesToRun;

            // More than one user Service may run within the same process. To add
            // another service to this process, change the following line to
            // create a second service object. For example,
            //
            //   ServicesToRun = new System.ServiceProcess.ServiceBase[] {new TCPService(), new MySecondUserService()};
            //
            ServicesToRun = new System.ServiceProcess.ServiceBase[] { new WSWinSockPedidos() };

            System.ServiceProcess.ServiceBase.Run(ServicesToRun);
        }


        protected override void OnStart(string[] args)
        {
            // Create the Server Object ans Start it.
            _miTCPServer = new TCPServer();
            _miTCPServer.StartServer(); 

        }

        protected override void OnStop()
        {
            // Stop the Server. Release it.
            _miTCPServer.StopServer();
            _miTCPServer = null;

        }
    }
}
