using Comuns;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSWinSockPedidos.Funciones
{
    class Regalos
    {
        Utils U = new Utils();
        Constantes constantes = new Constantes();

        #region regalos

        public List<ArrayList> initTablaContadorRegalos()
        {
            return U.GetListaCondicionesRegalo();
        }

        public void setContadorRegalo(string psCodNacional, int qty, List<ArrayList> _tablaContadorRegalos)
        {
            //regals en els que intervé el material 
            List<string> codsRegalo = new List<string>();
            codsRegalo = U.getGruposRegalo(psCodNacional);

            //a la taula busca per a quins regals cal sumar
            for (int i = 0; i < _tablaContadorRegalos.Count; i++)
            {
                ArrayList lin = _tablaContadorRegalos[i];

                if (codsRegalo.IndexOf(lin[0].ToString()) >= 0)
                {
                    //cal sumar a la taula
                    int valor = int.Parse(lin[constantes.K_POS_TREGAL_CONTADOR].ToString());
                    valor += qty;
                    lin[constantes.K_POS_TREGAL_CONTADOR] = valor;
                }
            }
        }

        public List<ArrayList> RegalosAAnyadir(string psTipoPedido, List<ArrayList> _tablaContadorRegalos)
        {
            int numRegalos = 0;
            string sFit = "";
            List<ArrayList> _tablaRegalosAnyadir = new List<ArrayList>();

            for (int i = 0; i < _tablaContadorRegalos.Count; i++)
            {
                ArrayList lin = _tablaContadorRegalos[i];

                //Mira si el pedido es del tipo adecuado para el regalo
                if (lin[constantes.K_POS_TREGAL_TIPO].ToString() == psTipoPedido || lin[constantes.K_POS_TREGAL_TIPO].ToString().Trim() == "" || lin[constantes.K_POS_TREGAL_TIPO] == null)
                {
                    sFit = "";

                    //Mira si cumple mínimos de cantidad 
                    if (int.Parse(lin[constantes.K_POS_TREGAL_CONTADOR].ToString()) >= int.Parse(lin[constantes.K_POS_TREGAL_MIN].ToString()))
                    {
                        sFit += lin[constantes.K_POS_TREGAL_GRUPO]; // Aquí afegeix 0440 0 0446 segons i és genèric o ch

                        if (bool.Parse(lin[constantes.K_POS_TREGAL_PARTE].ToString())) //parte?
                            // Si parteix els regals aniran al fitxer indicat a la taula
                            sFit += lin[constantes.K_POS_TREGAL_FITXER];
                        else
                            sFit += "";  // Va al fichero de genéricos o ch no V (lo dejo escrito para que se lea mejor el código)

                        //al final de la llista afegiré un item que indiqui a quin fitxer ha d'anar
                        lin.Add(sFit);

                        numRegalos++;
                        _tablaRegalosAnyadir.Add(lin);
                    }
                }
            }

            return _tablaRegalosAnyadir;
        }

        #endregion
    }
}
