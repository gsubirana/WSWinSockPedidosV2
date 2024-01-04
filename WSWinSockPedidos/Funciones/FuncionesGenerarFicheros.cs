using Comuns;
using DAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSWinSockPedidos.Funciones
{
    class FuncionesGenerarFicheros
    {
        private const string K_FITXER_GEN = "0440";
        private const string K_FITXER_CH = "0446";

        Constantes constantes = new Constantes();
        Utils U = new Utils();
        FuncionesListener funciones = new FuncionesListener();
        XML xml = new XML();

        #region generación de ficheros (esta versión separa en x ficheros, genéricos y CH que pueden desdoblarse en varios según tabla)

        // 0440 y 0446: Los materiales CH se separarán en un fichero aparte si el pedido es TRANSFER (si el pedido es directo se mantienen en el mismo fichero que los genéricos)
        // 0440VX y 0446VX: Los materiales definidos en la tabla MaterialesAparte se separarán en otro fichero. En el caso que a su vez sean materiales CH se separán en otro fichero si el pedido es transfer
        public void GenerarFicheros(int _numberOfLinesPerOrder, int _iCodsNacionalsCH, int _iCodsNacionalsAparte, int _iCodsNacionalsAparteCH, int _numRegalosPedido, string _sNomfitxerDesti, string _sNomfitxerDestiNOK, 
            List<ArrayList> _tablaRegalosAnyadir, List<string> _fitsAparte, bool _esDirecte, string DEFAULT_FILE_STORE_LOC, string DEFAULT_FILE_STORE_LOC_DIR, string _sCodPed, List<dsSPR.ListaMaterialesAparteRow> _lListaMaterialesAparte, 
            string _sTipoPedido, List<string> _codsNacionalsCH, string _sCodBotiquin, string DEFAULT_FILE_STORE_XML_NOK_LOC, string DEFAULT_FILE_STORE_XML_LOC, string DEFAULT_FILE_STORE_XML_LOC_DIR, string DEFAULT_FILE_STORE_BACKUP_LOC)
        {
            bool bGen = false;
            bool bCH = false;
            //---- GSG (26/05/2015)
            //bool bGenV2 = false;
            //bool bCHV2 = false;
            bool bGenVX = false;
            bool bCHVX = false;

            if (_numberOfLinesPerOrder - _iCodsNacionalsCH - _iCodsNacionalsAparte - _iCodsNacionalsAparteCH > 0)
                bGen = true;
            if (_iCodsNacionalsCH > 0)
                bCH = true;
            if (_iCodsNacionalsAparte > 0)
                bGenVX = true;
            if (_iCodsNacionalsAparteCH > 0)
                bCHVX = true;

            // Falta mirar si hi ha algún regal que vagi a algun fitxer que només té el regal
            // Si no ho determinem quedarà bé la línia de regal però no les línies de capçalera, total i final de fitxer que no es crearan
            if (_numRegalosPedido > 0)
            {
                for (int i = 0; i < _numRegalosPedido; i++)
                {
                    if (_tablaRegalosAnyadir[i][constantes.K_POS_TREGAL_FITXER].ToString() == "" && _tablaRegalosAnyadir[i][constantes.K_POS_TREGAL_GRUPO].ToString() == K_FITXER_GEN && !bGen)
                        bGen = true;
                    else if (_tablaRegalosAnyadir[i][constantes.K_POS_TREGAL_FITXER].ToString() == "" && _tablaRegalosAnyadir[i][constantes.K_POS_TREGAL_GRUPO].ToString() == K_FITXER_CH && !bCH)
                        bCH = true;
                    else if (_tablaRegalosAnyadir[i][constantes.K_POS_TREGAL_FITXER].ToString() != "" && _tablaRegalosAnyadir[i][constantes.K_POS_TREGAL_GRUPO].ToString() == K_FITXER_GEN && !bGenVX)
                        bGenVX = true;
                    else if (_tablaRegalosAnyadir[i][constantes.K_POS_TREGAL_FITXER].ToString() != "" && _tablaRegalosAnyadir[i][constantes.K_POS_TREGAL_GRUPO].ToString() == K_FITXER_CH && !bCHVX)
                        bCHVX = true;
                }
            }

            // Per als ok
            if (File.Exists(_sNomfitxerDesti))
                generaFitxers(bGen, bCH, bGenVX, bCHVX, true, _fitsAparte, _sNomfitxerDesti, _esDirecte, DEFAULT_FILE_STORE_LOC, DEFAULT_FILE_STORE_LOC_DIR, _sNomfitxerDestiNOK, _sCodPed, 
                    _lListaMaterialesAparte, _sTipoPedido, _codsNacionalsCH, _numRegalosPedido, _tablaRegalosAnyadir,
                    _sCodBotiquin, DEFAULT_FILE_STORE_XML_NOK_LOC, DEFAULT_FILE_STORE_XML_LOC, DEFAULT_FILE_STORE_XML_LOC_DIR, DEFAULT_FILE_STORE_BACKUP_LOC);

            // Per als nok
            if (File.Exists(_sNomfitxerDestiNOK))
                generaFitxers(bGen, bCH, bGenVX, bCHVX, false, _fitsAparte, _sNomfitxerDesti, _esDirecte, DEFAULT_FILE_STORE_LOC, DEFAULT_FILE_STORE_LOC_DIR, _sNomfitxerDestiNOK, _sCodPed,
                    _lListaMaterialesAparte, _sTipoPedido, _codsNacionalsCH, _numRegalosPedido, _tablaRegalosAnyadir,
                    _sCodBotiquin, DEFAULT_FILE_STORE_XML_NOK_LOC, DEFAULT_FILE_STORE_XML_LOC, DEFAULT_FILE_STORE_XML_LOC_DIR, DEFAULT_FILE_STORE_BACKUP_LOC);
        }

        public void generaFitxers(
            bool bGen, bool bCH, bool bGenVX, bool bCHVX, bool bOK, List<string> _fitsAparte, string _sNomfitxerDesti, bool _esDirecte, string DEFAULT_FILE_STORE_LOC, string DEFAULT_FILE_STORE_LOC_DIR,
            string _sNomfitxerDestiNOK, string _sCodPed, List<dsSPR.ListaMaterialesAparteRow> _lListaMaterialesAparte,string _sTipoPedido,List<string> _codsNacionalsCH,int _numRegalosPedido, List<ArrayList> _tablaRegalosAnyadir,
            string _sCodBotiquin, string DEFAULT_FILE_STORE_XML_NOK_LOC, string DEFAULT_FILE_STORE_XML_LOC, string DEFAULT_FILE_STORE_XML_LOC_DIR, string DEFAULT_FILE_STORE_BACKUP_LOC)
        {
            string sNomfitxerDesti = "";
            string sNomfitxerDestiGenerics = "";
            string sNomfitxerDestiCH = "";
            List<string> lsNomsfitxerDestiGenericsVX = new List<string>();
            List<string> lsNomsfitxerDestiCHVX = new List<string>();
            int iNumFitsAparte = _fitsAparte.Count();

            //Noms dels fitxers
            if (bOK)
            {
                sNomfitxerDesti = _sNomfitxerDesti;

                //---- GSG (31/10/2019)
                //sNomfitxerDestiGenerics = _sNomfitxerDesti.Replace(".txt", "");
                //sNomfitxerDestiCH = _sNomfitxerDesti.Replace(".txt", "");
                if (_esDirecte)
                {
                    sNomfitxerDestiGenerics = _sNomfitxerDesti.Replace(".txt", "").Replace(DEFAULT_FILE_STORE_LOC, DEFAULT_FILE_STORE_LOC_DIR);
                    sNomfitxerDestiCH = _sNomfitxerDesti.Replace(".txt", "").Replace(DEFAULT_FILE_STORE_LOC, DEFAULT_FILE_STORE_LOC_DIR);
                }
                else
                {
                    sNomfitxerDestiGenerics = _sNomfitxerDesti.Replace(".txt", "");
                    sNomfitxerDestiCH = _sNomfitxerDesti.Replace(".txt", "");
                }
                //---- FI GSG

                for (int j = 0; j < iNumFitsAparte; j++)
                {
                    //---- GSG (31/10/2019)           
                    //lsNomsfitxerDestiGenericsVX.Add(_sNomfitxerDesti.Replace(".txt", ""));
                    //lsNomsfitxerDestiCHVX.Add(_sNomfitxerDesti.Replace(".txt", ""));
                    if (_esDirecte)
                    {
                        lsNomsfitxerDestiGenericsVX.Add(_sNomfitxerDesti.Replace(".txt", "").Replace(DEFAULT_FILE_STORE_LOC, DEFAULT_FILE_STORE_LOC_DIR));
                        lsNomsfitxerDestiCHVX.Add(_sNomfitxerDesti.Replace(".txt", "").Replace(DEFAULT_FILE_STORE_LOC, DEFAULT_FILE_STORE_LOC_DIR));
                    }
                    else
                    {
                        lsNomsfitxerDestiGenericsVX.Add(_sNomfitxerDesti.Replace(".txt", ""));
                        lsNomsfitxerDestiCHVX.Add(_sNomfitxerDesti.Replace(".txt", ""));
                    }
                    //---- FI GSG
                }
            }
            else
            {
                sNomfitxerDesti = _sNomfitxerDestiNOK;

                sNomfitxerDestiGenerics = _sNomfitxerDestiNOK.Replace(".txt", "");
                sNomfitxerDestiCH = _sNomfitxerDestiNOK.Replace(".txt", "");
                for (int j = 0; j < iNumFitsAparte; j++)
                {
                    lsNomsfitxerDestiGenericsVX.Add(_sNomfitxerDestiNOK.Replace(".txt", ""));
                    lsNomsfitxerDestiCHVX.Add(_sNomfitxerDestiNOK.Replace(".txt", ""));
                }
            }

            sNomfitxerDestiGenerics = sNomfitxerDestiGenerics + K_FITXER_GEN + ".txt";
            sNomfitxerDestiCH = sNomfitxerDestiCH + K_FITXER_CH + ".txt";
            for (int j = 0; j < iNumFitsAparte; j++)
            {
                lsNomsfitxerDestiGenericsVX[j] = lsNomsfitxerDestiGenericsVX[j] + K_FITXER_GEN + _fitsAparte[j] + ".txt";
                lsNomsfitxerDestiCHVX[j] = lsNomsfitxerDestiCHVX[j] + K_FITXER_CH + _fitsAparte[j] + ".txt";
            }


            //Llegeix el fitxer original línia a línia i va generant X fitxers. Al final borra l'original

            if (File.Exists(sNomfitxerDesti))
            {
                //Vars per a les línies de material
                int totalLins = 0, totalLinsCH = 0;
                int totalCantidad = 0, totalCantidadCH = 0;
                int totalBonifs = 0, totalBonifsCH = 0;
                int[] totalLinsVX = new int[iNumFitsAparte];
                int[] totalCantidadVX = new int[iNumFitsAparte];
                int[] totalBonifsVX = new int[iNumFitsAparte];
                int[] totalLinsCHVX = new int[iNumFitsAparte];
                int[] totalCantidadCHVX = new int[iNumFitsAparte];
                int[] totalBonifsCHVX = new int[iNumFitsAparte]; //(en pricipi els VX no tenen bonif, però ho tracto normal)


                StreamReader sr = new StreamReader(@sNomfitxerDesti);
                StreamWriter swGen = new StreamWriter(@sNomfitxerDestiGenerics);
                StreamWriter swCH = new StreamWriter(@sNomfitxerDestiCH);
                StreamWriter[] swGenVX = new StreamWriter[iNumFitsAparte];
                StreamWriter[] swCHVX = new StreamWriter[iNumFitsAparte];

                for (int i = 0; i < iNumFitsAparte; i++)
                {
                    string fit = lsNomsfitxerDestiGenericsVX[i];
                    StreamWriter sw = new StreamWriter(@fit);
                    swGenVX[i] = sw;

                    fit = lsNomsfitxerDestiCHVX[i];

                    StreamWriter swch = new StreamWriter(@fit);
                    swCHVX[i] = swch;
                }

                // Va a ver en que streamwriter guardar la línea
                string lin = "";
                string liniaModificada = "";

                while ((lin = sr.ReadLine()) != null)
                {
                    //La última línia és igual per a tots i no rep cap modificació. 
                    if (lin.Substring(0, 4) == "0199")
                    {
                        if (bGen)
                            swGen.WriteLine(lin);
                        if (bCH)
                            swCH.WriteLine(lin);
                        if (bGenVX)
                        {
                            foreach (StreamWriter sw in swGenVX)
                                sw.WriteLine(lin);
                        }
                        if (bCHVX)
                        {
                            foreach (StreamWriter sw in swCHVX)
                                sw.WriteLine(lin);
                        }
                    }

                    //---- GSG (14/05/2014) Canvi en la construcció de les 16 posicions
                    // La primera línia, varia perquè aprofitarem les 16 posicions del usuari per afegir-hi la identificació de comanda CH i VX (dels 16 l'usuari nútilitza 11 i ens queden 5 posicions)    
                    // SAP del fitxer uneix aquestes 16 posicions amb les 10 posicions corresponents al codi de comanda de la línia de capçalera 
                    // per identificar la comanda.
                    // Codi client sense 0 esquerra però mantenint la S + 'A' + dos digits finals de l'any + identificació CH i VX
                    // S + n dígits cod client + 'A' + yy + identificació CH i VX + m blancs 

                    //---- GSG (05/02/2016)
                    //else if (lin.Substring(0, 4) == "0101")
                    else if (lin.Substring(0, 4) == "0101" || lin.Substring(0, 4) == "0102")
                    {
                        char[] zero = new char[1];
                        zero[0] = '0';
                        string anyo = DateTime.Today.Year.ToString().Substring(2, 2);

                        if (bGen) //(ex: 'S9179A140440    ')
                        {
                            //---- GSG (05/02/2016)
                            //liniaModificada = lin.Substring(0, 18);
                            liniaModificada = "0101";
                            liniaModificada += lin.Substring(4, 14);
                            //---- FI GSG

                            //usuari sense 0's esquerra 
                            //---- GSG (25/05/2015)
                            // En el pack stada algunos delegados ponen el código sap de la farmacia con la S y los 0's Y NO DEBERÍAN
                            // Para evitar que el servicio se pare detectamos el caso 
                            //string codusu = long.Parse(lin.Substring(19, 15).Trim()).ToString();
                            string cod = lin.Substring(19, 15).Trim();
                            if (cod.Substring(0, 1) == "S")
                                cod = cod.Substring(1, cod.Length - 1);
                            string codusu = long.Parse(cod).ToString();

                            codusu = lin.Substring(18, 1) + codusu + "A" + anyo + K_FITXER_GEN;
                            liniaModificada += codusu.PadRight(16);

                            liniaModificada += lin.Substring(34, lin.Length - 34);

                            swGen.WriteLine(liniaModificada);
                        }
                        if (bCH) //(ex: 'S9179A140446    ')
                        {
                            //---- GSG (05/02/2016)
                            //liniaModificada = lin.Substring(0, 18);
                            liniaModificada = "0101";
                            liniaModificada += lin.Substring(4, 14);
                            //---- FI GSG

                            //---- GSG (25/05/2015)
                            // En el pack stada algunos delegados ponen el código sap de la farmacia con la S y los 0's Y NO DEBERÍAN
                            // Para evitar que el servicio se pare detectamos el caso 
                            //string codusu = long.Parse(lin.Substring(19, 15).Trim()).ToString();
                            string cod = lin.Substring(19, 15).Trim();
                            if (cod.Substring(0, 1) == "S")
                                cod = cod.Substring(1, cod.Length - 1);
                            string codusu = long.Parse(cod).ToString();

                            codusu = lin.Substring(18, 1) + codusu + "A" + anyo + K_FITXER_CH;
                            liniaModificada += codusu.PadRight(16);
                            liniaModificada += lin.Substring(34, lin.Length - 34);

                            swCH.WriteLine(liniaModificada);
                        }
                        if (bGenVX) //(ex: 'S9179A140440V2  ')
                        {
                            //---- GSG (05/02/2016)
                            //string iniciLin = lin.Substring(0, 18);
                            string iniciLin = "0101";
                            iniciLin += lin.Substring(4, 14);
                            //---- FI GSG

                            string cod = lin.Substring(19, 15).Trim();
                            if (cod.Substring(0, 1) == "S")
                                cod = cod.Substring(1, cod.Length - 1);
                            string codusu = long.Parse(cod).ToString();

                            int i = 0;
                            foreach (StreamWriter sw in swGenVX)
                            {
                                liniaModificada = iniciLin;
                                liniaModificada += (lin.Substring(18, 1) + codusu + "A" + anyo + K_FITXER_GEN + _fitsAparte[i]).PadRight(16);
                                liniaModificada += lin.Substring(34, lin.Length - 34);

                                sw.WriteLine(liniaModificada);

                                i++;
                            }
                        }
                        if (bCHVX) //(ex: 'S9179A140446V2  ')
                        {
                            //---- GSG (05/02/2016)
                            //string iniciLin = lin.Substring(0, 18);
                            string iniciLin = "0101";
                            iniciLin += lin.Substring(4, 14);
                            //---- FI GSG

                            string cod = lin.Substring(19, 15).Trim();
                            if (cod.Substring(0, 1) == "S")
                                cod = cod.Substring(1, cod.Length - 1);
                            string codusu = long.Parse(cod).ToString();

                            int i = 0;
                            foreach (StreamWriter sw in swCHVX)
                            {
                                liniaModificada = iniciLin;
                                liniaModificada += (lin.Substring(18, 1) + codusu + "A" + anyo + K_FITXER_CH + _fitsAparte[i]).PadRight(16);
                                liniaModificada += lin.Substring(34, lin.Length - 34);

                                sw.WriteLine(liniaModificada);

                                i++;
                            }
                        }
                    }

                    // A la segona línia varia un afegit al final i el codi de comanda si client sense codsped i en particions 
                    // ---- GSG (17/07/2013) El codi comanda NO igual per totes les particions:
                    // cas aparte: configuració i anar incrementant
                    // cas normal: cod comanda al primer i configuració + increment per a la resta
                    //---- GSG (22/05/2018)
                    // Accedir a la taula de conversió de majoristes perquè potser cal canviarlo
                    else if (lin.Substring(0, 4) == "1010")
                    {
                        // lletres per a tenir diferents codis de comanda quan hi ha particions (com a molt posaré abcedari, queda limitat però....)
                        string[] lletres = { "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
                        int indexLletra = 0;
                        string afegito = "4140";

                        //---- GSG (22/05/2018)
                        List<Item> listaMays = new List<Item>();
                        string may = lin.Substring(36, 6).Trim();
                        listaMays = U.getConversionMayoristas(null);
                        Item item = null;
                        if (listaMays != null)
                            item = listaMays.Find(x => x.s1 == may);
                        //-----

                        if (bGen)
                        {
                            liniaModificada = lin.Substring(0, 20);
                            liniaModificada += (_sCodPed.Trim() + 'A').PadRight(10, ' ');
                            //---- GSG (22/05/2018)
                            //liniaModificada += lin.Substring(30, lin.Length - 30);
                            liniaModificada += lin.Substring(30, 6); //tipo pedido

                            //mayorista (6)
                            // buscarlo en tabla de conversión: si no está dejar el que viene en caso contrario poner el otro
                            if (item == null)
                                liniaModificada += lin.Substring(36, 6);
                            else
                                liniaModificada += item.s2.PadRight(6, ' ');

                            liniaModificada += lin.Substring(42, lin.Length - 42);
                            //-----

                            liniaModificada += (K_FITXER_GEN + afegito);
                            swGen.WriteLine(liniaModificada);
                        }
                        if (bCH)
                        {
                            liniaModificada = lin.Substring(0, 20);
                            liniaModificada += (_sCodPed.Trim() + 'B').PadRight(10, ' ');

                            //---- GSG (22/05/2018)
                            //liniaModificada += lin.Substring(30, lin.Length - 30);

                            liniaModificada += lin.Substring(30, 6); //tipo pedido
                            //mayorista (6)
                            // buscarlo en tabla de conversión: si no está dejar el que viene en caso contrario poner el otro
                            if (item == null)
                                liniaModificada += lin.Substring(36, 6);
                            else
                                liniaModificada += item.s2.PadRight(6, ' ');

                            liniaModificada += lin.Substring(42, lin.Length - 42);
                            //-----

                            liniaModificada += (K_FITXER_CH + afegito);
                            swCH.WriteLine(liniaModificada);
                        }
                        if (bGenVX)
                        {
                            foreach (StreamWriter sw in swGenVX)
                            {
                                liniaModificada = lin.Substring(0, 20);
                                liniaModificada += (_sCodPed.Trim() + lletres[indexLletra]).PadRight(10, ' ');
                                indexLletra++;
                                //---- GSG (22/05/2018)
                                //liniaModificada += lin.Substring(30, lin.Length - 30);
                                liniaModificada += lin.Substring(30, 6); //tipo pedido
                                //mayorista (6)
                                // buscarlo en tabla de conversión: si no está dejar el que viene en caso contrario poner el otro
                                if (item == null)
                                    liniaModificada += lin.Substring(36, 6);
                                else
                                    liniaModificada += item.s2.PadRight(6, ' ');

                                liniaModificada += lin.Substring(42, lin.Length - 42);
                                //-----

                                liniaModificada += (K_FITXER_GEN + afegito);
                                sw.WriteLine(liniaModificada);
                            }
                        }
                        if (bCHVX)
                        {
                            foreach (StreamWriter sw in swCHVX)
                            {
                                liniaModificada = lin.Substring(0, 20);
                                liniaModificada += (_sCodPed.Trim() + lletres[indexLletra]).PadRight(10, ' ');
                                indexLletra++;
                                //---- GSG (22/05/2018)
                                //liniaModificada += lin.Substring(30, lin.Length - 30);

                                liniaModificada += lin.Substring(30, 6); //tipo pedido
                                //mayorista (6)
                                // buscarlo en tabla de conversión: si no está dejar el que viene en caso contrario poner el otro
                                if (item == null)
                                    liniaModificada += lin.Substring(36, 6);
                                else
                                    liniaModificada += item.s2.PadRight(6, ' ');

                                liniaModificada += lin.Substring(42, lin.Length - 42);


                                //-----
                                liniaModificada += (K_FITXER_CH + afegito);
                                sw.WriteLine(liniaModificada);
                            }
                        }
                    }

                    // Les 1020 i 1030 es queden igual però necessitem guardar-nos les quantitats i bonificacions per a utilitzar-los després a la línia 1050
                    // En aquest cas, la línia sols va a un dels fitxers
                    //---- GSG (05/02/2016)
                    //else if (lin.Substring(0, 4) == "1020" || lin.Substring(0, 4) == "1030")
                    else if (lin.Substring(0, 4) == "1020" || lin.Substring(0, 4) == "1030" || lin.Substring(0, 4) == "1040")
                    {
                        int qty = int.Parse(lin.Substring(17, 4));
                        int bonif = 0;
                        string cod = "";
                        // ---- GSG (06/07/2016)
                        // Tratar líneas de regalo que vienen con cn inventado y que no cumplen con standar
                        //string codProvisional = lin.Substring(10, 7);                        
                        string codProvisional = lin.Substring(4, 13).Trim();
                        if (!Utils.IsNumeric(codProvisional) || (codProvisional.Length != 6 && codProvisional.Length != 7 && codProvisional.Length != 13))
                        {
                            // En este caso en la tabla de sustitutos tenemos el ean-13 inventado que nos va bien para el fichero (poner GAD003 en el fichero no nos gustaba)
                            string elCN = "";
                            if (funciones.getCNRegalo(codProvisional, out elCN))
                                cod = elCN;
                            else
                                cod = codProvisional;
                        }
                        else
                        {
                            codProvisional = lin.Substring(10, 7);

                            if (codProvisional.Substring(0, 1) == "0") // No viene el dígito de control
                                cod = lin.Substring(11, 6).Trim();
                            else
                                cod = lin.Substring(10, 6).Trim();
                        }

                        int iTipoAparte = funciones.EsCodNacionalAparte(cod,_lListaMaterialesAparte, _sTipoPedido);
                        string fit = "";
                        int posFit = -1;
                        if (iTipoAparte != -1)
                        {
                            fit = funciones.GetFicheroAparte(cod, _lListaMaterialesAparte);
                            for (int i = 0; i < iNumFitsAparte; i++)
                            {
                                if (_fitsAparte[i] == fit)
                                    posFit = i;
                            }
                        }


                        if (iTipoAparte == -1 && funciones.esLinCH(lin,_codsNacionalsCH)) //ch normal
                        {
                            totalLinsCH++;
                            totalCantidadCH += qty;
                            if (lin.Substring(0, 4) == "1030")
                            {
                                bonif = int.Parse(lin.Substring(21, 4));
                                totalBonifsCH += bonif;
                            }

                            swCH.WriteLine(lin);
                        }
                        else if (iTipoAparte == 0) //Aparte gen
                        {
                            totalLinsVX[posFit]++;
                            totalCantidadVX[posFit] += qty;
                            if (lin.Substring(0, 4) == "1030") //en principi no ho serà mai
                            {
                                bonif = int.Parse(lin.Substring(21, 4));
                                totalBonifsVX[posFit] += bonif;
                            }

                            swGenVX[posFit].WriteLine(lin);
                        }
                        else if (iTipoAparte == 1) //Aparte ch
                        {
                            totalLinsCHVX[posFit]++;
                            totalCantidadCHVX[posFit] += qty;
                            if (lin.Substring(0, 4) == "1030") //en principi no ho serà mai
                            {
                                bonif = int.Parse(lin.Substring(21, 4));
                                totalBonifsCHVX[posFit] += bonif;
                            }

                            swCHVX[posFit].WriteLine(lin);
                        }
                        else //línia genèric normal
                        {
                            totalLins++;
                            totalCantidad += qty;
                            if (lin.Substring(0, 4) == "1030") //en principi no ho serà mai
                            {
                                bonif = int.Parse(lin.Substring(21, 4));
                                totalBonifs += bonif;
                            }

                            swGen.WriteLine(lin);
                        }
                    }

                    // A la línia 1050 hem de posar els totals de cada fitxer però abans hem d'afegir les línies del productes de regal que hi pugui haber
                    // Si un regal no parteix pot anar a genéric o ch, i si parteix el posarem al fitxer indicat a la taula MaterialRegalo
                    else if (lin.Substring(0, 4) == "1050")
                    {
                        // Afegir les línies de regal
                        // Un regal sols va a un fitxer
                        if (_numRegalosPedido > 0)
                        {
                            for (int i = 0; i < _numRegalosPedido; i++)
                            {
                                ArrayList linr = _tablaRegalosAnyadir[i];
                                string lineRegalo = "1020";

                                //código artículo
                                //---- GSG (14/07/2016)
                                // lineRegalo += linr[K_POS_TREGAL_COD].ToString().PadLeft(13); 
                                string codProvisional = linr[constantes.K_POS_TREGAL_COD].ToString();
                                string elCN = "";
                                if (funciones.getCNRegalo(codProvisional, out elCN))
                                    lineRegalo += elCN.PadLeft(13);
                                else
                                    lineRegalo += linr[constantes.K_POS_TREGAL_COD].ToString().PadLeft(13);



                                //cantidad
                                int qtyRegalo = int.Parse(linr[constantes.K_POS_TREGAL_UNIDADES].ToString());
                                if (bool.Parse(linr[constantes.K_POS_TREGAL_MULTIPLO].ToString()))
                                {
                                    if (int.Parse(linr[constantes.K_POS_TREGAL_MIN].ToString()) != 0)
                                        qtyRegalo = (int.Parse(linr[constantes.K_POS_TREGAL_CONTADOR].ToString()) / int.Parse(linr[constantes.K_POS_TREGAL_MIN].ToString())) * int.Parse(linr[constantes.K_POS_TREGAL_UNIDADES].ToString()); //packs de min * qty a regalar
                                }

                                lineRegalo += qtyRegalo.ToString().PadLeft(4, '0');

                                switch (linr[constantes.K_POS_TREGAL_GRUPIFIT].ToString())
                                {
                                    case K_FITXER_GEN:
                                        swGen.WriteLine(lineRegalo);
                                        totalLins++;
                                        totalCantidad += qtyRegalo;
                                        break;
                                    case K_FITXER_CH:
                                        swCH.WriteLine(lineRegalo);
                                        totalLinsCH++;
                                        totalCantidadCH += qtyRegalo;
                                        break;
                                    default: //Línias que van a otras particiones Vx 
                                        // Buscar la posición del array correspondiente a la partición
                                        string f = linr[constantes.K_POS_TREGAL_FITXER].ToString(); //vx
                                        int pos = 0;
                                        for (int j = 0; j < iNumFitsAparte; j++)
                                        {
                                            if (_fitsAparte[j] == f)
                                                pos = j;
                                        }
                                        string sTip = linr[constantes.K_POS_TREGAL_GRUPO].ToString(); // generico o ch
                                        if (sTip == K_FITXER_GEN)
                                        {
                                            swGenVX[pos].WriteLine(lineRegalo);
                                            totalLinsVX[pos]++;
                                            totalCantidadVX[pos] += qtyRegalo;
                                        }
                                        else if (sTip == K_FITXER_CH)
                                        {
                                            swCHVX[pos].WriteLine(lineRegalo);
                                            totalLinsCHVX[pos]++;
                                            totalCantidadCHVX[pos] += qtyRegalo;
                                        }
                                        break;
                                }
                            }
                        }

                        //Afegim línea 1050
                        if (bGen)
                        {
                            string linG = lin.Substring(0, 4) + totalLins.ToString().PadLeft(4, '0') + totalCantidad.ToString().PadLeft(6, '0') + totalBonifs.ToString().PadLeft(6, '0');
                            swGen.WriteLine(linG);
                        }
                        if (bCH)
                        {
                            liniaModificada = lin.Substring(0, 4) + totalLinsCH.ToString().PadLeft(4, '0') + totalCantidadCH.ToString().PadLeft(6, '0') + totalBonifsCH.ToString().PadLeft(6, '0');
                            swCH.WriteLine(liniaModificada);
                        }
                        if (bGenVX)
                        {
                            //---- GSG (26/05/2015)
                            //liniaModificada = lin.Substring(0, 4) + totalLinsV2.ToString().PadLeft(4, '0') + totalCantidadV2.ToString().PadLeft(6, '0') + totalBonifsV2.ToString().PadLeft(6, '0');
                            //swGenV2.WriteLine(liniaModificada);
                            for (int j = 0; j < iNumFitsAparte; j++)
                            {
                                if (totalLinsVX[j] > 0)
                                {
                                    liniaModificada = lin.Substring(0, 4) + totalLinsVX[j].ToString().PadLeft(4, '0') + totalCantidadVX[j].ToString().PadLeft(6, '0') + totalBonifsVX[j].ToString().PadLeft(6, '0');
                                    swGenVX[j].WriteLine(liniaModificada);
                                }
                            }
                        }
                        if (bCHVX)
                        {
                            //---- GSG (26/05/2015)
                            //liniaModificada = lin.Substring(0, 4) + totalLinsCHV2.ToString().PadLeft(4, '0') + totalCantidadCHV2.ToString().PadLeft(6, '0') + totalBonifsCHV2.ToString().PadLeft(6, '0');
                            //swCHV2.WriteLine(liniaModificada);
                            for (int j = 0; j < iNumFitsAparte; j++)
                            {
                                if (totalLinsCHVX[j] > 0)
                                {
                                    liniaModificada = lin.Substring(0, 4) + totalLinsCHVX[j].ToString().PadLeft(4, '0') + totalCantidadCHVX[j].ToString().PadLeft(6, '0') + totalBonifsCHVX[j].ToString().PadLeft(6, '0');
                                    swCHVX[j].WriteLine(liniaModificada);
                                }
                            }
                        }
                    }
                }


                sr.Close();
                swGen.Close();
                swCH.Close();
                sr = null;
                swGen = null;
                swCH = null;

                for (int j = 0; j < iNumFitsAparte; j++)
                {
                    swGenVX[j].Close();
                    swCHVX[j].Close();
                    swGenVX[j] = null;
                    swCHVX[j] = null;
                }

                // Una vez separados los materiales, podemos borrar el fichero original que los contenía todos
                File.Delete(sNomfitxerDesti);

                // Se han creado todos los StreamWriter posibles y ahora hay que borrar los que no se han utilizado
                if (!bGen)
                    File.Delete(sNomfitxerDestiGenerics);
                if (!bCH)
                    File.Delete(sNomfitxerDestiCH);

                for (int j = 0; j < iNumFitsAparte; j++)
                {
                    if (totalLinsVX[j] == 0)
                        File.Delete(lsNomsfitxerDestiGenericsVX[j]);
                    if (totalLinsCHVX[j] == 0)
                        File.Delete(lsNomsfitxerDestiCHVX[j]);
                }


                //---- GSG (17/04/2018) SAP interfaz SAP rollout

                // Recuperar solo los ficheros creados para generar sus correspondientes XML
                // Habrá que mirar en la carpeta destino y también en Backup por si coincide que se procesan

                //if (bOK)
                //    GenerarXMLs(sNomfitxerDestiGenerics, sNomfitxerDestiCH, lsNomsfitxerDestiGenericsVX, lsNomsfitxerDestiCHVX);
                xml.GenerarXMLs(sNomfitxerDestiGenerics, sNomfitxerDestiCH, lsNomsfitxerDestiGenericsVX, lsNomsfitxerDestiCHVX, bOK, 
                    _sCodBotiquin, DEFAULT_FILE_STORE_XML_NOK_LOC, DEFAULT_FILE_STORE_XML_LOC, DEFAULT_FILE_STORE_XML_LOC_DIR, _esDirecte, DEFAULT_FILE_STORE_BACKUP_LOC, DEFAULT_FILE_STORE_LOC);
                //---------------------------------------------------------------------

                #region codiComentat

                //---- GSG (07/07/2016)
                // Una vegada tenim els fitxers creats, afegim un nou tractament:
                // Quan a una partició, directe i CH, l'import és inferior a 300 € PVP --> convertir la comanda a transfer (canviar ZTRW i condició de servei )
                // ---- GSG (26/09/2016)
                // Hem de fer que si la comanda és directe la partició, si és de CH, no es converteixi a transfer encara que no arribi al mínim
                // PER TANT, EN CAP CAS UNA PARTICIÓ DE CH CANVIARÀ A TRANSFER

                /*
                if (bCHVX) // hi ha particions de CH
                {
                    string[] liniesR = new string[2000];
                    int numLinsR = 0;

                    // Per a cada una de les que hem generat, mirar si alguna és directe
                    //foreach (StreamWriter sw in swCHVX)
                    for (int indexFit = 0; indexFit < lsNomsfitxerDestiCHVX.Count; indexFit++)
                    {
                        string linr = "";
                        numLinsR = 0;
                        liniesR.Initialize();

                        string sNomfitxerOrigen = lsNomsfitxerDestiCHVX[indexFit];

                        if (File.Exists(sNomfitxerOrigen))
                        {
                            StreamReader srAux = new StreamReader(@sNomfitxerOrigen);

                            while ((linr = srAux.ReadLine()) != null)
                            {
                                liniesR[numLinsR] = linr;
                                numLinsR++;
                            }

                            srAux.Close();
                            srAux = null;

                            if (numLinsR > 0) // El fichero existe y tiene líneas
                            {
                                // Mirar si el pedido es directo
                                string tipoPed = liniesR[1].Substring(30, 6).Trim(); // 1010
                                //if (tipoPed == "ZDIW" || tipoPed == "KB")
                                if (tipoPed == "ZDIW") //---- GSG (21/09/2016) El KB continuará siendo kb
                                {
                                    // Mirar si importe inferior a 300 € PVP
                                    float fImpMinDir = float.Parse(U.getConfguracion(K_CONF_IMPMINDIR));
                                    string importe = liniesR[numLinsR - 2].Substring(8, 6).Trim(); // 1050
                                    if (float.Parse(importe) < fImpMinDir)
                                    {
                                        // Buscar condición de servicio
                                        string cliente = liniesR[1].Substring(4, 16).Trim(); // 1010
                                        cliente = "S" + cliente.PadLeft(10, '0');
                                        string condServ = U.getCondicionServicio(cliente);
                                        condServ = int.Parse(condServ.Replace('S', '0')).ToString().PadRight(6, ' ');

                                        // Modificar líneas afectadas (1010)
                                        string linOriginal = liniesR[1];
                                        liniesR[1] = linOriginal.Substring(0, 30);
                                        liniesR[1] += "ZTRW".PadRight(6, ' ');
                                        liniesR[1] += condServ.PadRight(6, ' ');
                                        liniesR[1] += linOriginal.Substring(42, linOriginal.Length - 42);

                                        // Borramos el fichero de origen para poderlo crear con el mismo nombre y los datos modificados
                                        File.Delete(sNomfitxerOrigen);

                                        // Generar fichero con los datos cambiados
                                        StreamWriter swAux = new StreamWriter(@sNomfitxerOrigen);
                                        for (int i = 0; i < numLinsR; i++)
                                            swAux.WriteLine(liniesR[i]);

                                        swAux.Close();
                                        swAux = null;
                                    }
                                }
                            }

                        }
                    }
                }
                else if (bCH) // en el fitxer separat de CH encara que no sigui V també hem de comprobar import
                {
                    string[] liniesR = new string[2000];
                    int numLinsR = 0;
                    string sNomfitxerOrigen = sNomfitxerDestiCH;

                    if (File.Exists(sNomfitxerOrigen))
                    {
                        StreamReader srAux = new StreamReader(@sNomfitxerOrigen);
                        string linr = "";

                        while ((linr = srAux.ReadLine()) != null)
                        {
                            liniesR[numLinsR] = linr;
                            numLinsR++;
                        }

                        srAux.Close();
                        srAux = null;

                        if (numLinsR > 0) // El fichero existe y tiene líneas
                        {
                            // Mirar si el pedido es directo
                            string tipoPed = liniesR[1].Substring(30, 6).Trim(); // 1010
                            //if (tipoPed == "ZDIW" || tipoPed == "KB")
                            if (tipoPed == "ZDIW") //---- GSG (21/09/2016)
                            {
                                // Mirar si importe inferior a 300 € PVP
                                float fImpMinDir = float.Parse(U.getConfguracion(K_CONF_IMPMINDIR));
                                string importe = liniesR[numLinsR - 2].Substring(8, 6).Trim(); // 1050
                                if (float.Parse(importe) < fImpMinDir)
                                {
                                    // Buscar condición de servicio
                                    string cliente = liniesR[1].Substring(4, 16).Trim(); // 1010
                                    cliente = "S" + cliente.PadLeft(10, '0');
                                    string condServ = U.getCondicionServicio(cliente);
                                    condServ = int.Parse(condServ.Replace('S', '0')).ToString().PadRight(6, ' ');

                                    // Modificar líneas afectadas (1010)
                                    string linOriginal = liniesR[1];
                                    liniesR[1] = linOriginal.Substring(0, 30);
                                    liniesR[1] += "ZTRW".PadRight(6, ' ');
                                    liniesR[1] += condServ.PadRight(6, ' ');
                                    liniesR[1] += linOriginal.Substring(42, linOriginal.Length - 42);

                                    // Borramos el fichero de origen para poderlo crear con el mismo nombre y los datos modificados
                                    File.Delete(sNomfitxerOrigen);

                                    // Generar fichero con los datos cambiados
                                    StreamWriter swAux = new StreamWriter(@sNomfitxerOrigen);
                                    for (int i = 0; i < numLinsR; i++)
                                        swAux.WriteLine(liniesR[i]);

                                    swAux.Close();
                                    swAux = null;
                                }
                            }
                        }
                    }
                }
                */
                //---- FI GSG
                #endregion
            }
        }


        #endregion

    }
}
