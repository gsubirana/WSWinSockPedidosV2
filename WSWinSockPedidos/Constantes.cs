using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSWinSockPedidos
{
    class Constantes
	{
		#region Constantes

		private const string _codIncidenciaCabecera = "2010";
        private const string _codIncidenciaTextoLibre = "2011";
        private const string _codIncidenciaLinea = "2015";
        private const string _codIncidenciaCabeceraAnteriores = "2020"; //No ho farem servir
        private const string _codIncidenciaTextoLibreAnteriores = "2021"; //No ho farem servir
        private const string _codIncidenciaLineaAnteriores = "2025"; //No ho farem servir
		private const string _codIncidenciaFaltaVerificacion = "2030"; //No ho farem servir
		private const string _codIncidenciaRechazoTransmision = "9999";

		private const string _codLineaRegalo = "1020";

		private const string _K_LINCAP_LOG = "C";
		private const string _K_LINCAP_DET = "D";

		private const string _K_CONF_IMPMINDIR = "fImporteMinDIR";
		private const string _K_CONF_IMPMINTRA = "fImporteMinTRA";
		private const string _K_CONF_IMPMINKB = "fImporteMinKB"; //---- GSG (21/09/2016)
		private const string _K_CONF_ULTPEDAPA = "sUltimoPedidoAparte";

		//private const string _K_FITXER_GEN = "0440";
		//private const string _K_FITXER_CH = "0446";
        //---- GSG (26/05/2015)
        //private const  string K_FITXER_GENV2 = "0440V2";
        //private const  string K_FITXER_CHV2 = "0446V2";       
        //private const  string K_V2 = "V2";        

        private const int _K_POS_TREGAL_COD = 0;
        private const int _K_POS_TREGAL_MIN = 1;
        private const int _K_POS_TREGAL_CONTADOR = 2;
        private const int _K_POS_TREGAL_UNIDADES = 3;
        private const int _K_POS_TREGAL_MULTIPLO = 4;
        private const int _K_POS_TREGAL_ESREGALO = 5;
        private const int _K_POS_TREGAL_TIPO = 6;
        private const int _K_POS_TREGAL_PARTE = 7;
        private const int _K_POS_TREGAL_GRUPO = 8;
		private const int _K_POS_TREGAL_FITXER = 9;
		private const int _K_POS_TREGAL_GRUPIFIT = 10;

        //---- GSG (26/05/2015)
        private const int _K_POS_TAPARTE_CODSAP = 0;
        private const int _K_POS_TAPARTE_DESCMAT = 1;
        private const int _K_POS_TAPARTE_CODNAC = 2;
        private const int _K_POS_TAPARTE_ESCH = 3;
        private const int _K_POS_TAPARTE_TIPOPED = 4;
        private const int _K_POS_TAPARTE_FIT = 5;
        ////---- GSG (17/06/2019)
        ////private const  int K_POS_TAPARTE_TIPOPEDDEST = 6;

        //---- GSG (27/10/2015)
        private const int _K_LEN_LIN_CAB = 72;

        //
        private const string _K_INCID_SINEXISTENCIAS = "01";     // NO HAY EXISTENCIAS
        private const string _K_INCID_NOTRABAJADO = "03";        // NO TRABAJADO
        private const string _K_INCID_NUEVAESPECIALIDAD = "09";  // NUEVA ESPECIALIDAD
        private const string _K_INCID_BAJA = "11";               // BAJA
        private const string _K_INCID_ESTUPEFASINVALE = "15";    // ESTUPEFACIENTE SIN VALE
        private const string _K_INCID_ESTUPEFANOES = "16";       // NO ES ESTUPEFACIENTE
        private const string _K_INCID_ESTUPEFALINNOK = "17";     // ESTUPEFACIENTE EN LÍNEA NO 1040

        #endregion

        public string codIncidenciaCabecera
        {
            get { return _codIncidenciaCabecera; }
        }
        public string codIncidenciaTextoLibre
        {
            get { return _codIncidenciaTextoLibre; }
        }
        public string codIncidenciaLinea
        {
            get { return _codIncidenciaLinea; }
        }
        public string codIncidenciaCabeceraAnteriores
        {
            get { return _codIncidenciaCabeceraAnteriores; }
        }
        public string codIncidenciaTextoLibreAnteriores
        {
            get { return _codIncidenciaTextoLibreAnteriores; }
        }
        public string codIncidenciaLineaAnteriores
        {
            get { return _codIncidenciaLineaAnteriores; }
        }
        public string codIncidenciaFaltaVerificacion
        {
            get { return _codIncidenciaFaltaVerificacion; }
        }
        public string codIncidenciaRechazoTransmision
        {
            get { return _codIncidenciaRechazoTransmision; }
        }
        public string codLineaRegalo
        {
            get { return _codLineaRegalo; }
        }
        public string K_LINCAP_LOG
        {
            get { return _K_LINCAP_LOG; }
        }
        public string K_LINCAP_DET
        {
            get { return _K_LINCAP_DET; }
        }
        public string K_CONF_IMPMINDIR
        {
            get { return _K_CONF_IMPMINDIR; }
        }
        public string K_CONF_IMPMINTRA
        {
            get { return _K_CONF_IMPMINTRA; }
        }
        public string K_CONF_IMPMINKB
        {
            get { return _K_CONF_IMPMINKB; }
        }
        public string K_CONF_ULTPEDAPA
        {
            get { return _K_CONF_ULTPEDAPA; }
        }
        //public string K_FITXER_GEN
        //{
        //    get { return _K_FITXER_GEN; }
        //}
        //public string K_FITXER_CH
        //{
        //    get { return _K_FITXER_CH; }
        //}
        public int K_POS_TREGAL_COD
        {
            get { return _K_POS_TREGAL_COD; }
        }
        public int K_POS_TREGAL_MIN
        {
            get { return _K_POS_TREGAL_MIN; }
        }
        public int K_POS_TREGAL_CONTADOR
        {
            get { return _K_POS_TREGAL_CONTADOR; }
        }
        public int K_POS_TREGAL_UNIDADES
        {
            get { return _K_POS_TREGAL_UNIDADES; }
        }
        public int K_POS_TREGAL_MULTIPLO
        {
            get { return _K_POS_TREGAL_MULTIPLO; }
        }
        public int K_POS_TREGAL_ESREGALO
        {
            get { return _K_POS_TREGAL_ESREGALO; }
        }
        public int K_POS_TREGAL_TIPO
        {
            get { return _K_POS_TREGAL_TIPO; }
        }
        public int K_POS_TREGAL_PARTE
        {
            get { return _K_POS_TREGAL_PARTE; }
        }
        public int K_POS_TREGAL_GRUPO
        {
            get { return _K_POS_TREGAL_GRUPO; }
        }
        public int K_POS_TREGAL_FITXER
        {
            get { return _K_POS_TREGAL_FITXER; }
        }
        public int K_POS_TREGAL_GRUPIFIT
        {
            get { return _K_POS_TREGAL_GRUPIFIT; }
        }
        public int K_POS_TAPARTE_CODSAP
        {
            get { return _K_POS_TAPARTE_CODSAP; }
        }
        public int K_POS_TAPARTE_DESCMAT
        {
            get { return _K_POS_TAPARTE_DESCMAT; }
        }
        public int K_POS_TAPARTE_CODNAC
        {
            get { return _K_POS_TAPARTE_CODNAC; }
        }
        public int K_POS_TAPARTE_ESCH
        {
            get { return _K_POS_TAPARTE_ESCH; }
        }
        public int K_POS_TAPARTE_TIPOPED
        {
            get { return _K_POS_TAPARTE_TIPOPED; }
        }
        public int K_POS_TAPARTE_FIT
        {
            get { return _K_POS_TAPARTE_FIT; }
        }
        public int K_LEN_LIN_CAB
        {
            get { return _K_LEN_LIN_CAB; }
        }
        public string K_INCID_SINEXISTENCIAS
        {
            get { return _K_INCID_SINEXISTENCIAS; }
        }
        public string K_INCID_NOTRABAJADO
        {
            get { return _K_INCID_NOTRABAJADO; }
        }
        public string K_INCID_NUEVAESPECIALIDAD
        {
            get { return _K_INCID_NUEVAESPECIALIDAD; }
        }
        public string K_INCID_BAJA
        {
            get { return _K_INCID_BAJA; }
        }
        public string K_INCID_ESTUPEFASINVALE
        {
            get { return _K_INCID_ESTUPEFASINVALE; }
        }
        public string K_INCID_ESTUPEFANOES
        {
            get { return _K_INCID_ESTUPEFANOES; }
        }
        public string K_INCID_ESTUPEFALINNOK
        {
            get { return _K_INCID_ESTUPEFALINNOK; }
        }


    }
}
