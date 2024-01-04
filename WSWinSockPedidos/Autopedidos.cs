using Comuns;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSWinSockPedidos
{
    class WinSockPedidos
    {
        public class Pedido
        {
            public Pedido()
            {
                this.listPedidosOriginal = new List<string>();
                this.listPedidosModify = new List<string>(); 
                this.listPedidosWithoutRepe = new List<string>();
                this.Cliente = new Cliente();
                this.ListasPedidos = new ListasPedidos();
                this.ExtraListasPedidos = new ExtraListasPedidos();
                this.bIsDirecto = false;
            }
            public string sTipoPedido { get; set; }
            public bool bIsDirecto { get; set; }
            public string sCodBotiquin { get; set; }
            public string codPedAux { get; set; }
            public List<string> listPedidosOriginal { get; set; }
            public List<string> listPedidosModify { get; set; }
            public List<string> listPedidosWithoutRepe { get; set; }
            public Cliente Cliente { get; set; }
            public ListasPedidos ListasPedidos { get; set; }
            public ExtraListasPedidos ExtraListasPedidos { get; set; }
            public Variables Variables { get; set; }
           

        }


        public class Cliente
        {
            public string codClienteLine { get; set; }
            public string sCodCliente { get; set; }
        }

        public class ExtraListasPedidos
        {
            public ExtraListasPedidos()
            {
                this.listSubstituciones = new List<Item>();
                this.listContadorRegalos = new List<ArrayList>();
                this.listNoComercializados = new List<string>();
                this.listlBloqueados = new List<string>();
                this.listConSustituto = new List<string>();
                this.listRegalosAnyadir = new List<ArrayList>();
            }
            public List<Item> listSubstituciones { get; set; }
            public List<ArrayList> listContadorRegalos { get; set; }
            public List<string> listNoComercializados { get; set; }
            public List<string> listlBloqueados { get; set; }
            public List<string> listConSustituto { get; set; }
            public List<ArrayList> listRegalosAnyadir { get; set; }
        }

        public class Variables
        {
            public Variables()
            {
                _numberOfOrders = 0; 
                _numberOfLinesPerOrder = 0;
                _qtyPerOrder = 0;
                _bonificacionsPerOrder = 0;
                _iSuperanStock = 0;
                _totalBrutComanda = 0;
                _bHayTextoLibre = false;
                _qtyNoComerc = 0;
                _bonifNoComercPerOrder = 0;
                _bonifBloquejatsPerOrder = 0;
                _qtyBloquejats = 0;
                _qtyDeMenysAlSubstituirPerOrder = 0;
                _qtyNOStada = 0;
                _bonifNOStadaPerOrder = 0;
                _qtyNOKPorInactivos = 0;
                _bonifNOKPorInactivosPerOrder = 0;
                _sCodPed = "";
            }
            public int _numberOfOrders { get; set; } //Nombre de comandes rebudes
            public int _numberOfLinesPerOrder { get; set; } //Nombre de línies a una comanda
            public int _qtyPerOrder { get; set; }
            public int _bonificacionsPerOrder { get; set; } //Suma de les bonificacions de cada línia 1030
            public int _iSuperanStock { get; set; }
            public float _totalBrutComanda { get; set; }
            public bool _bHayTextoLibre { get; set; }
            public int _qtyNoComerc { get; set; }
            public int _bonifNoComercPerOrder { get; set; }
            public int _bonifBloquejatsPerOrder { get; set; }
            public int _qtyBloquejats { get; set; }
            public int _qtyDeMenysAlSubstituirPerOrder { get; set; }
            public int _qtyNOStada { get; set; }
            public int _bonifNOStadaPerOrder { get; set; }
            public int _qtyNOKPorInactivos { get; set; }
            public int _bonifNOKPorInactivosPerOrder { get; set; }
            public string _sCodPed { get; set; }
        }

        public class ListasPedidos
        {
            public ListasPedidos()
            {

                this.pedLines = new List<string>();
                this.missatgesFinals = new List<string>();
                this.codsNacionals = new List<string>();
                this.codsNacionalsCH = new List<string>();
                this.codsNacionalsSencers = new List<string>();
                this.codsNacionalsSustitucion = new List<string[]>();
                this.codsNacionalsNOStada = new List<string>();
                this.codsNacionalsNOK = new List<string>();
                this.codsNacionalsNOKPorInactivos = new List<string>();
                this.codsNacionalsNoComerc = new List<string>();
                this.codsNacionalsBloquejats = new List<string>();
                this.qtysNacionalsBloquejats = new List<string>();
                this.codsNacionalsStock = new List<string>();
                this.qtysNacionalsStock = new List<string>();
                this.stocksNacionalsStock = new List<string>();
            }
            public List<string> pedLines { get; set; }
            public List<string> missatgesFinals { get; set; }
            public List<string> codsNacionals { get; set; }
            public List<string> codsNacionalsCH { get; set; }
            public List<string> codsNacionalsSencers { get; set; }
            public List<string[]> codsNacionalsSustitucion { get; set; }
            public List<string> codsNacionalsNOStada { get; set; }
            public List<string> codsNacionalsNOKPorInactivos { get; set; }
            public List<string> codsNacionalsNOK { get; set; }
            public List<string> codsNacionalsNoComerc { get; set; }
            public List<string> codsNacionalsBloquejats { get; set; }
            public List<string> qtysNacionalsBloquejats { get; set; }
            public List<string> codsNacionalsStock { get; set; }
            public List<string> qtysNacionalsStock { get; set; }
            public List<string> stocksNacionalsStock { get; set; }

        }

    }
}
