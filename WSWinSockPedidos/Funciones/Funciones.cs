using Comuns;
using DAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSWinSockPedidos.Funciones
{
    class FuncionesListener
    {
        Utils U = new Utils();
        Constantes constantes = new Constantes();

        //---- GSG (06/02/2017)
        public List<Item> getListaSustitucionMateriales(string sTipoPedido, string sCodCliente)
        {
            List<Item> lRet = new List<Item>();

            lRet = U.getListaSustitucion(sTipoPedido, sCodCliente);

            return lRet;
        }

        public List<string> getNoComercializados(string _sTipoPedido)
        {
            List<string> lRet = new List<string>();

            lRet = U.getNoComercializados(_sTipoPedido);

            return lRet;
        }

        public  bool esClienteSinCodsPedido(string sCodCliente)
        {
            bool bRet = false;

            if (U.EsClienteSinCodsPedido(sCodCliente))
            {
                bRet = true;
            }

            return bRet;
        }

        //---- GSG (12/05/2016)
        public List<string> getCodsBloqueados(string sTipoPedido, string sCodCliente)
        {
            List<string> lRet = new List<string>();

            lRet = U.getCodsBloqueados(sTipoPedido, sCodCliente);

            return lRet;
        }

        public List<string> getCodsConSustituto(string sTipoPedido, string sCodCliente)
        {
            List<string> lRet = new List<string>();

            lRet = U.getCodsConSustituto(sTipoPedido, sCodCliente);

            return lRet;
        }

        //Funció que calcula el dígit de control del material
        public string codNacionalSencer(string codNacional)
        {
            string sRet = "";
            int iTotal = 0;
            int iComplemento = 0;
            bool bPar = false;

            for (int i = 0; i < codNacional.Length; i++)
            {
                if (!bPar)
                    iTotal += int.Parse(codNacional.Substring(i, 1));
                else
                    iTotal += int.Parse(codNacional.Substring(i, 1)) * 3;

                bPar = !bPar;
            }

            iTotal += 27;

            iComplemento = iTotal % 10;
            if (iComplemento != 0)
                iComplemento = 10 - iComplemento;

            sRet = codNacional + iComplemento.ToString();

            return sRet;
        }

        public bool esMaterialSustituido(string sCodNacional, out string sCodNacionalSustituto)
        {
            bool bRet = false;

            sCodNacionalSustituto = U.EsMaterialSustituido(sCodNacional);

            if (sCodNacionalSustituto != null && sCodNacionalSustituto != "")
            {
                bRet = true;
            }

            return bRet;
        }

        public int getUnidadesBaseSustituto(string sCodNacional)
        {
            return U.GetUnidadesBaseSustituto(sCodNacional);
        }

        public int EsCodNacionalAparte(string sCodNacional, List<dsSPR.ListaMaterialesAparteRow> _lListaMaterialesAparte, string _sTipoPedido)
        {
            int iRet = -1;
            List<dsSPR.ListaMaterialesAparteRow> lA = _lListaMaterialesAparte.Where(x=>x.sCodNacional==sCodNacional).ToList();
            //AGREGADO POR JOSE DURAND - INICIO - 12/01/2024
            if (lA.Count > 0)
            {
                iRet = lA.Where(x => x.sTipoPedido == _sTipoPedido || x.sTipoPedido == "XXXX" || x.sTipoPedido == "" || x.sTipoPedido == null)
               .Where(x => x.bEsCH == true)
               .Count() >= 1 ? 1 : 0;
            }            
            //AGREGADO POR JOSE DURAND - FIN - 12/01/2024 

            //COMENTADO POR JOSE DURAND - INICIO - 12/01/2024
            //iRet = lA.Where(x => x.sTipoPedido == _sTipoPedido || x.sTipoPedido == "XXXX" || x.sTipoPedido == "" || x.sTipoPedido == null)
            //    .Where(x => x.bEsCH == true)
            //    .Count() >= 1 ? 1 : -1;
            //COMENTADO POR JOSE DURAND - FIN - 12/01/2024

            return iRet;

            //int iRet = -1;
            //List<string[]> lA = _lListaMaterialesAparte.FindAll(li => FindRegistroInArray(li, K_POS_TAPARTE_CODNAC, sCodNacional));

            //for (int i = 0; i < lA.Count; i++)
            //{
            //    if (lA[i][K_POS_TAPARTE_TIPOPED] == _sTipoPedido || lA[i][K_POS_TAPARTE_TIPOPED].ToUpper() == "XXXX" || lA[i][K_POS_TAPARTE_TIPOPED] == "" || lA[i][K_POS_TAPARTE_TIPOPED] == null)
            //    {
            //        //if (lA[i][K_POS_TAPARTE_ESCH] == "1") //---- GSG (07/07/2016)
            //        if (lA[i][K_POS_TAPARTE_ESCH] == "1" || lA[i][K_POS_TAPARTE_ESCH].ToLower() == "true")
            //            iRet = 1;
            //        else
            //            iRet = 0;

            //        i = lA.Count + 1; // per sortir
            //    }
            //}


            //return iRet;
        }

        private static bool FindRegistroInArray(string[] reg, int field, string value)
        {
            if (reg[field] != null && reg[field].ToString() == value)
                return true;
            else
                return false;
        }

        public string GetFicheroAparte(string sCodNacional, List<dsSPR.ListaMaterialesAparteRow> _lListaMaterialesAparte)
        {
            string sRet = "";
            List<dsSPR.ListaMaterialesAparteRow> lA = _lListaMaterialesAparte.Where(x => x.sCodNacional == sCodNacional).ToList();

            if (lA.Count > 0)
                sRet = lA.Select(x=>x.sFichero).FirstOrDefault();

            return sRet;

            //string sRet = "";
            //List<string[]> lA = _lListaMaterialesAparte.FindAll(li => FindRegistroInArray(li, K_POS_TAPARTE_CODNAC, sCodNacional));

            //if (lA.Count > 0)
            //    sRet = lA[0][K_POS_TAPARTE_FIT];

            //return sRet;
        }

        public bool getCNRegalo(string sCodNacional, out string sCodNacionalRegalo)
        {
            bool bRet = false;

            sCodNacionalRegalo = U.getCNRegalo(sCodNacional);

            if (sCodNacionalRegalo != null && sCodNacionalRegalo != "")
            {
                bRet = true;
            }

            return bRet;
        }


        #region determinar tipo de línia

        public bool esLinCH(string lin, List<string> _codsNacionalsCH)
        {
            bool ret = false;

            //Obtenir el codi nacional de la línia
            string codProvisional = long.Parse(lin.Substring(10, 7).Trim()).ToString();
            string codNac = "";
            if (codProvisional.Substring(0, 1) == "0")
                codNac = long.Parse(lin.Substring(11, 6).Trim()).ToString();
            else
                codNac = long.Parse(lin.Substring(10, 6).Trim()).ToString();

            //Mirem si és un dels de CH
            for (int j = 0; j < _codsNacionalsCH.Count; j++)
            {
                if (_codsNacionalsCH[j] == codNac)
                {
                    ret = true;
                    break;
                }
            }

            return ret;
        }

        #endregion

    }
}
