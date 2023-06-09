﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using TuCalcuTeApuesto.Models;
using System.Diagnostics;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text;
using System.Configuration;
using TuCalcuTeApuesto.DataAccess;

namespace TuCalcuTeApuesto.Controllers
{
    public class HomeController : Controller
    {
        dbTuCalcuEntities _db;

        public ActionResult Index(string f)

        {
            CrearArchivos();
            ListaModel listaFinal = CargaDataModelo(f);

            return View(listaFinal);

        }

        [HttpPost]
        public ActionResult Upload2(HttpPostedFileBase files)
        {

            string rutaCompleta = GetFile(files);

            ListaModel listaFinal = CargarDataFinal(rutaCompleta);

            //HttpContext.Session.SetObject("ObjetoComplejo", listaFinal);

            return View("Index", listaFinal);
        }

        [HttpPost]
        public string EnviarComentario(ComentarioModel model)
        {
            var rpta = "true";
            try
            {
                Comentarios entidad = new Comentarios();
                entidad.DesComentario = model.Mensaje;
                entidad.FechaCreacion = DateTime.Now;
                entidad.Puntaje = model.Puntaje;
                entidad.CodUsuario = string.Empty;

                bool isAuth = User.Identity.IsAuthenticated;
                if (isAuth)
                {
                    _db = new dbTuCalcuEntities();
                    AspNetUsers user = _db.AspNetUsers.Where(m => m.Email == User.Identity.Name).ToList().FirstOrDefault();
                    entidad.CodUsuario = user.Id;
                }

                ComentarioDA _da = new ComentarioDA();
                _da.GrabarComentario(entidad);

                return rpta;

            }
            catch (Exception e)
            {
                rpta = CapturarError(e, "HomeController", "EnviarComentario");
                return "false";

            }

        }

        public ActionResult Privacy()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        #region "Equipos Favoritos"
 
        [HttpPost]
        public string EliminarEquipoFav(EquiposFavModel model)
        {
            var rpta = "true";
            try
            {
                bool isAuth = User.Identity.IsAuthenticated;
                if (isAuth)
                {
                    _db = new dbTuCalcuEntities();
                    AspNetUsers user = _db.AspNetUsers.Where(m => m.Email == User.Identity.Name).ToList().FirstOrDefault();

                    EquiposFavoritos entidad = new EquiposFavoritos();
                    entidad.CodEquipo = model.Value;
                    entidad.CodUsuario = user.Id;

                    EquiposFavDA _da = new EquiposFavDA();
                    _da.EliminarEquipoFav(entidad);

                }

                return rpta;

            }
            catch (Exception e)
            {
                rpta = CapturarError(e, "HomeController", "EliminarEquipoFav");
                return "false";
                //return View("ERROR");

            }
        }

        [HttpPost]
        public string GrabarEquipoFav(EquiposFavModel model)
        {
            var rpta = "true";
            try
            {
                bool isAuth = User.Identity.IsAuthenticated;
                if (isAuth)
                {
                    _db = new dbTuCalcuEntities();
                    AspNetUsers user = _db.AspNetUsers.Where(m => m.Email == User.Identity.Name).ToList().FirstOrDefault();

                    EquiposFavoritos entidad = new EquiposFavoritos();
                    entidad.CodEquipo = model.Value;
                    entidad.CodUsuario = user.Id;

                    EquiposFavDA _da = new EquiposFavDA();
                    _da.GrabarEquipoFav(entidad);
                }

                return rpta;

            }
            catch (Exception e)
            {
                rpta = CapturarError(e, "HomeController", "EliminarEquipoFav");
                return "false";

            }
        }

        #endregion

        #region "Equipos Favoritos"

        [HttpPost]
        public string EliminarTorneoFav(TorneosFavModel model)
        {
            var rpta = "true";
            try
            {
                bool isAuth = User.Identity.IsAuthenticated;
                if (isAuth)
                {
                    _db = new dbTuCalcuEntities();
                    AspNetUsers user = _db.AspNetUsers.Where(m => m.Email == User.Identity.Name).ToList().FirstOrDefault();

                    TorneosFavoritos entidad = new TorneosFavoritos();
                    entidad.CodTorneo = model.Value;
                    entidad.CodUsuario = user.Id;

                    TorneosFavDA _da = new TorneosFavDA();
                    _da.EliminarTorneoFav(entidad);

                }

                return rpta;

            }
            catch (Exception e)
            {
                rpta = CapturarError(e, "HomeController", "EliminarTorneoFav");
                return "false";
                //return View("ERROR");

            }
        }

        [HttpPost]
        public string GrabarTorneoFav(TorneosFavModel model)
        {
            var rpta = "true";
            try
            {
                bool isAuth = User.Identity.IsAuthenticated;
                if (isAuth)
                {
                    _db = new dbTuCalcuEntities();
                    AspNetUsers user = _db.AspNetUsers.Where(m => m.Email == User.Identity.Name).ToList().FirstOrDefault();

                    TorneosFavoritos entidad = new TorneosFavoritos();
                    entidad.CodTorneo = model.Value;
                    entidad.CodUsuario = user.Id;

                    TorneosFavDA _da = new TorneosFavDA();
                    _da.GrabarTorneoFav(entidad);
                }

                return rpta;

            }
            catch (Exception e)
            {
                rpta = CapturarError(e, "HomeController", "EliminarTorneoFav");
                return "false";

            }
        }

        #endregion

        #region "Metodos"
        /// <summary>
        /// CapturarError: llena el ViewBag.ErrorMessage y devuelve una cadena con la descripcion del error 
        /// </summary>
        /// <param name="error"></param>
        /// <param name="controlador"></param>
        /// <param name="accion"></param>
        /// <returns></returns>
        public string CapturarError(Exception error, string controlador = "", string accion = "")
        {
            var msg = error.Message;
            if (error.InnerException != null)
            {
                msg = msg + "/;/" + error.InnerException.Message;
                if (error.InnerException.InnerException != null)
                {
                    msg = msg + "/;/" + error.InnerException.InnerException.Message;
                    if (error.InnerException.InnerException.InnerException != null)
                        msg = msg + "/;/" + error.InnerException.InnerException.InnerException.Message;
                }
            }


            var comentario = $@"Se ejecutó la accion: [{controlador}/{accion}] - MensajeError: {msg}";
            var logErrorFinal = string.Format("{0} | {1}", comentario, error.StackTrace);
            //log.ErrorFormat("{0} | {1}", comentario, error.StackTrace);

            //PARA LOG ERROR
            var fileUnicoNameSSIS = String.Concat("ERROR", ".txt");
            var fileUnicoPathSSIS = System.IO.Path.Combine(Server.MapPath("~/"), "Files", "Programa", "SSIS", fileUnicoNameSSIS);

            using (FileStream fs = new FileStream(fileUnicoPathSSIS, FileMode.Create))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(logErrorFinal);
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();
            }

            ViewBag.ErrorMessage = logErrorFinal;
            
            return string.Format(logErrorFinal);
        }

        public void CrearArchivos()
        {
            string path = ConfigurationManager.AppSettings["urlArchivo"];
            string pathCarpeta = System.IO.Path.Combine(Server.MapPath("~/"), "Files");

            System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(pathCarpeta);
            List<FileInfo> listaFileInfo = directory.GetFiles().ToList();

            DateTime fechaHoy = DateTime.Today;
            var diaHoy = GetDia(fechaHoy);
            var mesHoy = GetMes(fechaHoy);
            var mydia = DateTime.Today.Day.ToString().PadLeft(2, '0');
            var cad = diaHoy + " " + mydia + " DE " + mesHoy;

            //si no hay archivo de hoy crea
            var fileModel = listaFileInfo.Where(w => w.Name.Contains(cad)).FirstOrDefault();
            if (fileModel == null)
            {
                ExtractTextFromPdf(path);
            }
            else
            {
                //ultima modificacion es menor a hoy actualiza           
                var pathFile = System.IO.Path.Combine(Server.MapPath("~/"), "Files", fileModel.Name);
                var strLastModified = Convert.ToDateTime(System.IO.File.GetLastWriteTime(pathFile).ToShortDateString());
                //var fechaCreacionFile = Convert.ToDateTime(fileModel.CreationTime.ToShortDateString());
                var fechaHoyCadena = Convert.ToDateTime(fechaHoy.ToShortDateString());
                var diasDiff = (fechaHoyCadena - strLastModified).Days;

                if (diasDiff > 0)
                {
                    ExtractTextFromPdf(path);
                }

                ////crea files
                //ExtractTextFromPdf(path);
            }
        }

        public ListaModel CargaDataModelo(string fileName = "")
        {
            string pathCarpeta = System.IO.Path.Combine(Server.MapPath("~/"), "Files");
            ListaModel listaFinal = new ListaModel();
            listaFinal.Lista = new List<ModeloModel>();

            List<int> listaCabecerasFav = new List<int>();
            List<int> listaCabecerasMin = new List<int>();
            List<CabeceraModel> listaCabeceras = new List<CabeceraModel>();
            List<TorneoModel> listaTorneosHoy = new List<TorneoModel>();
            List<FileModel> listaFileModel = new List<FileModel>();
            FileModel fileModel = new FileModel();
            List<EquiposFavModel> listaEquiposFav = new List<EquiposFavModel>();
            List<EquiposModel> listaEquipos = new List<EquiposModel>();
            List<TorneosFavModel> listaTorneosFav = new List<TorneosFavModel>();
            List<TorneoModel> listaTorneos = new List<TorneoModel>();

            System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(pathCarpeta);
            List<FileInfo> listaFileInfo = directory.GetFiles().ToList();
            List<FileInfo> listaFilter = new List<FileInfo>();
            var rutaCompleta = string.Empty;

            if (listaFileInfo.Count > 0)
            {
                foreach (FileInfo lista in listaFileInfo)
                {
                    FileModel file = new FileModel();
                    var index = lista.Name.ToUpper().IndexOf("PROGRAMA");
                    var cod = lista.Name.Substring(index).Replace("PROGRAMA", string.Empty).Replace(".txt", string.Empty).Trim();

                    file.Codigo = cod;
                    file.Extension = lista.Extension;
                    file.Name = lista.Name;
                    file.NameShow = lista.Name.Replace(".txt", string.Empty);
                    file.Length = lista.Length.ToString();
                    file.CreationTimeUtc = lista.LastWriteTime.ToString();
                    listaFileModel.Add(file);
                }

                listaFileModel.Sort((x, y) => x.Codigo.CompareTo(y.Codigo));

                if (!String.IsNullOrEmpty(fileName))
                {
                    fileModel = listaFileModel.Where(w => w.Name.Contains(fileName)).FirstOrDefault();
                    rutaCompleta = System.IO.Path.Combine(pathCarpeta, fileModel.Name);
                }
                else
                {
                    DateTime fechaHoy = DateTime.Today;
                    var diaHoy = GetDia(fechaHoy);
                    var mesHoy = GetMes(fechaHoy);

                    var mydia = DateTime.Today.Day.ToString().PadLeft(2, '0');

                    var cad = diaHoy + " " + mydia + " DE " + mesHoy;

                    fileModel = listaFileModel.Where(w => w.Name.Contains(cad)).FirstOrDefault();
                    if (fileModel.Name == null)
                        fileModel = listaFileModel.FirstOrDefault();

                    rutaCompleta = System.IO.Path.Combine(pathCarpeta, fileModel.Name);
                }

                List<string> lstFinal = FormateaData(rutaCompleta);

                DataTable dt = FormateaDataFinal(lstFinal, ref listaCabeceras, ref listaTorneosHoy, ref listaCabecerasFav, ref listaCabecerasMin);
                listaFinal = CargarListaFinal(dt);

            }

            //Favoritos
            bool isAuth = User.Identity.IsAuthenticated;
            if (isAuth)
            {
                //Usuarios
                UsuariosDA _daUsuarios = new UsuariosDA();
                UsuariosModel user = _daUsuarios.ObtenerUsuario(User.Identity.Name);

                //Equipos
                EquiposFavDA _daEquiposFav = new EquiposFavDA();
                listaEquiposFav = _daEquiposFav.ListarEquiposFavoritos(user.Id);

                EquiposDA _daEquipos = new EquiposDA();
                listaEquipos = _daEquipos.ListarEquipos();

                //elimina los favoritos de la lista final
                foreach (var item in listaEquiposFav) {
                    var codEquipoFav = Convert.ToInt32(item.Value);
                    EquiposModel resultFind = listaEquipos.Where(x => x.Value == codEquipoFav).FirstOrDefault();
                    if (resultFind != null)
                    {
                        listaEquipos.Remove(resultFind);
                    }
                }

                //Torneos
                TorneosFavDA _daTorneosFav = new TorneosFavDA();
                listaTorneosFav = _daTorneosFav.ListarTorneosFavoritos(user.Id);

                TorneosDA _daTorneos = new TorneosDA();
                listaTorneos = _daTorneos.ListarTorneos();

                //elimina los favoritos de la lista final
                foreach (var item in listaTorneosFav)
                {
                    int codEquipoFav = Convert.ToInt32(item.Value);
                    var nombreCorto = item.NombreCorto.Trim(); ;
                    TorneoModel resultFind = listaTorneos.Where(x => x.Value == codEquipoFav).FirstOrDefault();
                    if (resultFind != null)
                    {
                        listaTorneos.Remove(resultFind);
                    }

                    //solo marca los torneos fav de la lista de torneos de hoy
                    foreach (var itemHoy in listaTorneosHoy)
                    {
                        if (nombreCorto == itemHoy.Text)
                        {
                            itemHoy.IsChecked = false;
                            itemHoy.IsSelected = "";
                        }
                            
                    }
                }
                
            }

            //Final
            listaFinal.ListaCabeceras = listaCabeceras;
            listaFinal.ListaTorneosHoy = listaTorneosHoy;
            listaFinal.ListaCabecerasFav = listaCabecerasFav;
            listaFinal.ListaCabecerasMin = listaCabecerasMin;
            listaFinal.ListaFiles = listaFileModel;
            listaFinal.Archivo = fileModel;
            listaFinal.ListaEquiposFav = listaEquiposFav;
            listaFinal.ListaEquipos = listaEquipos;
            listaFinal.ListaTorneosFav = listaTorneosFav;
            listaFinal.ListaTorneos = listaTorneos;

            return listaFinal;
        }
        
        public string GetDia(DateTime fecha)
        {
            switch (fecha.DayOfWeek)
            {
                case System.DayOfWeek.Sunday:
                    return "DOMINGO";
                case System.DayOfWeek.Monday:
                    return "LUNES";
                case System.DayOfWeek.Tuesday:
                    return "MARTES";
                case System.DayOfWeek.Wednesday:
                    return "MIÉRCOLES";
                case System.DayOfWeek.Thursday:
                    return "JUEVES";
                case System.DayOfWeek.Friday:
                    return "VIERNES";
                case System.DayOfWeek.Saturday:
                    return "SÁBADO";
                default:
                    return string.Empty;
            }
        }

        public string GetMes(DateTime fecha)
        {
            switch (fecha.Month)
            {
                case 1:
                    return "ENERO";
                case 2:
                    return "FEBRERO";
                case 3:
                    return "MARZO";
                case 4:
                    return "ABRIL";
                case 5:
                    return "MAYO";
                case 6:
                    return "JUNIO";
                case 7:
                    return "JULIO";
                case 8:
                    return "AGOSTO";
                case 9:
                    return "SEPTIEMBRE";
                case 10:
                    return "OCTUBRE";
                case 11:
                    return "NOVIEMBRE";
                case 12:
                    return "DICIEMBRE";
                default:
                    return string.Empty;
            }
        }

        private DataTable CargarTabla()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("TORNEO");
            dt.Columns.Add("HORA");
            dt.Columns.Add("COD");
            dt.Columns.Add("MINIMO");
            dt.Columns.Add("LOCAL");
            dt.Columns.Add("L");
            dt.Columns.Add("E");
            dt.Columns.Add("V");
            dt.Columns.Add("VISITA");
            dt.Columns.Add("LoE");
            dt.Columns.Add("LoV");
            dt.Columns.Add("EoV");
            dt.Columns.Add("1TL");
            dt.Columns.Add("1TE");
            dt.Columns.Add("1TV");
            dt.Columns.Add("2TL");
            dt.Columns.Add("2TE");
            dt.Columns.Add("2TV");
            dt.Columns.Add("L/L");
            dt.Columns.Add("E/L");
            dt.Columns.Add("V/L");
            dt.Columns.Add("L/E");
            dt.Columns.Add("E/E");
            dt.Columns.Add("V/E");
            dt.Columns.Add("L/V");
            dt.Columns.Add("E/V");
            dt.Columns.Add("V/V");
            dt.Columns.Add("-1.5");
            dt.Columns.Add("+1.5");
            dt.Columns.Add("-2.5");
            dt.Columns.Add("+2.5");
            dt.Columns.Add("-3.5");
            dt.Columns.Add("+3.5");
            dt.Columns.Add("0-1");
            dt.Columns.Add("2-3");
            dt.Columns.Add("4+");
            dt.Columns.Add("A");
            dt.Columns.Add("NA");
            dt.Columns.Add("I");
            dt.Columns.Add("P");
            dt.Columns.Add("FLAG");
            return dt;
        }

        private List<CabeceraModel> CargarCabeceras(DataTable dt)
        {

            string[] columnNames = dt.Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName)
                                 .ToArray();

            List<CabeceraModel> listaCabeceras = new List<CabeceraModel>();

            var i = 0;
            foreach (var item in columnNames)
            {
                if (item != "FLAG")
                {
                    CabeceraModel objCabecera = new CabeceraModel();
                    objCabecera.Id = i + 1;
                    objCabecera.Text = item;
                    objCabecera.Value = i;
                    objCabecera.IsChecked = true;
                    objCabecera.IdGrupo = EvalcolumnNames(item);
                    listaCabeceras.Add(objCabecera);
                    i++;
                }
            }

            return listaCabeceras;

        }

        private int EvalcolumnNames(string colName)
        {
            var ret = 0;
            if (colName == ("TORNEO")) ret = 0;
            else if (colName == ("HORA")) ret = 0;
            else if (colName == ("COD")) ret = 0;
            else if (colName == ("MINIMO")) ret = 0;
            else if (colName == ("LOCAL")) ret = 0;
            else if (colName == ("L")) ret = 0;
            else if (colName == ("E")) ret = 0;
            else if (colName == ("V")) ret = 0;
            else if (colName == ("VISITA")) ret = 0;
            else if (colName == ("LoE")) ret = 1;
            else if (colName == ("LoV")) ret = 1;
            else if (colName == ("EoV")) ret = 1;
            else if (colName == ("1TL")) ret = 2;
            else if (colName == ("1TE")) ret = 2;
            else if (colName == ("1TV")) ret = 2;
            else if (colName == ("2TL")) ret = 3;
            else if (colName == ("2TE")) ret = 3;
            else if (colName == ("2TV")) ret = 3;
            else if (colName == ("L/L")) ret = 4;
            else if (colName == ("E/L")) ret = 4;
            else if (colName == ("V/L")) ret = 4;
            else if (colName == ("L/E")) ret = 4;
            else if (colName == ("E/E")) ret = 4;
            else if (colName == ("V/E")) ret = 4;
            else if (colName == ("L/V")) ret = 4;
            else if (colName == ("E/V")) ret = 4;
            else if (colName == ("V/V")) ret = 4;
            else if (colName == ("-1.5")) ret = 5;
            else if (colName == ("+1.5")) ret = 5;
            else if (colName == ("-2.5")) ret = 5;
            else if (colName == ("+2.5")) ret = 5;
            else if (colName == ("-3.5")) ret = 5;
            else if (colName == ("+3.5")) ret = 5;
            else if (colName == ("0-1")) ret = 6;
            else if (colName == ("2-3")) ret = 6;
            else if (colName == ("4+")) ret = 6;
            else if (colName == ("A")) ret = 7;
            else if (colName == ("NA")) ret = 7;
            else if (colName == ("I")) ret = 8;
            else if (colName == ("P")) ret = 8;

            return ret;
        }

        private List<TorneoModel> CargarTorneos(DataTable dt)
        {
            TorneosDA _daTorneos = new TorneosDA();
            List<TorneoModel> listaTorneosDB = _daTorneos.ListarTorneos();

            DataView view = new DataView(dt);
            DataTable dtTorneos = view.ToTable(true, "TORNEO");

            string[] torneos = dtTorneos.AsEnumerable().Select(r => r.Field<string>("TORNEO")).ToArray();

            List<TorneoModel> listaTorneos = new List<TorneoModel>();

            var i = 0;
            foreach (var item in torneos)
            {
                TorneoModel objTorneo = new TorneoModel();
                objTorneo.Text = item;
                objTorneo.Value = i;
                objTorneo.IsChecked = true;
                objTorneo.Imagen = ObtenerUrlImagenDB(item, listaTorneosDB);
                objTorneo.IsSelected = "selected";
                listaTorneos.Add(objTorneo);
                i++;
            }

            return listaTorneos;
        }

        private string ObtenerUrlImagenDB(string torneo, List<TorneoModel> listaTorneosDB) {
            string url = "";
            var tab = "\t";
            TorneoModel existeTorneo = listaTorneosDB.Where(x => x.NombreCorto.Contains(torneo)).FirstOrDefault();
            if (existeTorneo != null) 
                url = existeTorneo.Imagen.Replace(tab, "");
            else
                url = "europeanunion.png";

            return url;
        }

        private string ObtenerUrlImagen(string torneo)
        {
            string url = "";
            
            if (torneo == "ARG" || torneo == "ARG2" || torneo == "ARGC")
                url = "ar.png";
            else if (torneo == "PER" || torneo == "PER2" || torneo == "PE2")
                url = "pe.png";
            else if (torneo == "ESP" || torneo == "ESP2" || torneo == "ESP3")
                url = "es.png";
            else if (torneo == "ALE" || torneo == "AL2" || torneo == "AL3" || torneo == "G4SW" || torneo == "ALC" || torneo == "G4WE")
                url = "de.png";
            else if (torneo == "CHN" || torneo == "CH2")
                url = "cn.png";
            else if (torneo == "URU" || torneo == "URU2")
                url = "uy.png";
            else if (torneo == "PAR" || torneo == "PAR")
                url = "py.png";
            else if (torneo == "ECU" || torneo == "ECU2")
                url = "ec.png";
            else if (torneo == "PAN" || torneo == "PAN2")
                url = "pa.png";
            else if (torneo == "NOR" || torneo == "NOR2")
                url = "no.png";
            else if (torneo == "COL" || torneo == "COL2")
                url = "co.png";
            else if (torneo == "BOL" || torneo == "BOL2")
                url = "bo.png";
            else if (torneo == "BRA" || torneo == "BRA2" || torneo == "BRP")
                url = "br.png";
            else if (torneo == "POR" || torneo == "POR2")
                url = "pt.png";
            else if (torneo == "POL" || torneo == "POL2" || torneo == "POL3")
                url = "pl.png";
            else if (torneo == "SUE" || torneo == "SUE2" || torneo == "SU3N" || torneo == "SU3S")
                url = "se.png";
            else if (torneo == "USA" || torneo == "USA2" || torneo == "CCC")
                url = "us.png";
            else if (torneo == "RUS" || torneo == "RUS2" || torneo == "RUC")
                url = "ru.png";
            else if (torneo == "ROM" || torneo == "ROM2" || torneo == "ROC")
                url = "ro.png";
            else if (torneo == "DEN" || torneo == "DEN2" || torneo == "DK3A" || torneo == "DK3B" || torneo == "DINC")
                url = "dk.png";
            else if (torneo == "PAR" || torneo == "PAR2")
                url = "py.png";
            else if (torneo == "FRA" || torneo == "FRA2" || torneo == "FRA3" || torneo == "FRC")
                url = "fr.png";
            else if (torneo == "BEL" || torneo == "BEL2")
                url = "be.png";
            else if (torneo == "ISR" || torneo == "ISR2")
                url = "il.png";
            else if (torneo == "INPL" || torneo == "ING1" || torneo == "ING2" || torneo == "INPL2" || torneo == "INCH" || torneo == "ICL" || torneo == "INC")
                url = "england.png";
            else if (torneo == "JAP" || torneo == "JAP2" || torneo == "JLC")
                url = "jp.png";
            else if (torneo == "COR" || torneo == "COR2")
                url = "kr.png";
            else if (torneo == "CZE" || torneo == "CZE2" || torneo == "CZEC")
                url = "cz.png";
            else if (torneo == "ITA" || torneo == "ITA2" || torneo == "ITC" || torneo == "LPPO" || torneo == "IT3P")
                url = "it.png";
            else if (torneo == "HOL" || torneo == "HOL2")
                url = "nl.png";
            else if (torneo == "AUT" || torneo == "AUT2" || torneo == "AUS")
                url = "au.png";
            else if (torneo == "BEL" || torneo == "BEL2")
                url = "au.png";
            else if (torneo == "BUL" || torneo == "BUL2")
                url = "bg.png";
            else if (torneo == "FIN" || torneo == "FIN2" || torneo == "FIC")
                url = "fi.png";
            else if (torneo == "CHI" || torneo == "CHI2")
                url = "cl.png";
            else if (torneo == "SUE" || torneo == "SUE2")
                url = "se.png";
            else if (torneo == "SUI" || torneo == "SUI2")
                url = "ch.png";
            else if (torneo == "GRE" || torneo == "GRE2")
                url = "gr.png";
            else if (torneo == "HUN" || torneo == "HUN2")
                url = "hu.png";
            else if (torneo == "TUR" || torneo == "TUR2")
                url = "tr.png";
            else if (torneo == "CAN" || torneo == "CAN2")
                url = "ca.png";
            else if (torneo == "CRO" || torneo == "CRO2")
                url = "hr.png";
            else if (torneo == "MEXP" || torneo == "MEX2")
                url = "mx.png";
            else if (torneo == "ISL" || torneo == "ISL2")
                url = "is.png";
            else if (torneo == "SVK" || torneo == "SVK2")
                url = "sk.png";
            else if (torneo == "VEN" || torneo == "VEN2")
                url = "ve.png";
            else if (torneo == "KUW" || torneo == "KUW2")
                url = "kw.png";
            else if (torneo == "ARM" || torneo == "ARM2")
                url = "am.png";
            else if (torneo == "UCK" || torneo == "UCK2" || torneo == "UKC")
                url = "ua.png";
            else if (torneo == "SPL" || torneo == "SPL2" || torneo == "SCH" || torneo == "SC1P" || torneo == "SCHP" || torneo == "SC1P" || torneo == "SC1" || torneo == "SC2" || torneo == "SCC")
                url = "scotland.png";
            else if (torneo == "SVNC" || torneo == "SVNC2" || torneo == "SVKC")
                url = "sk.png";
            else if (torneo == "LIT" || torneo == "LIT2" || torneo == "LTU")
                url = "li.png";
            else if (torneo == "CLA")
                url = "cla.png";
            else if (torneo == "CSA")
                url = "csa.png";
            else if (torneo == "CEU")
                url = "cha.png";
            else if (torneo == "LDE")
                url = "eul.png";
            else if (torneo == "BIH")
                url = "ba.png";
            else if (torneo == "BLR")
                url = "by.png";
            else if (torneo == "CYP")
                url = "cy.png";
            else if (torneo == "IRL")
                url = "ia.png";
            else if (torneo == "UAE")
                url = "ae.png";
            else if (torneo == "KSA")
                url = "kz.png";
            else if (torneo == "BHR")
                url = "ws.png";
            else if (torneo == "SAF")
                url = "za.png";
            else if (torneo == "ELI")
                url = "eli.png";
            else
                url = "europeanunion.png";

            return url;

        }

        private List<int> CargarCabecerasVisiblesFav(DataTable dt)
        {
            List<int> lstHideColumnasLoad = new List<int>();

            for (int i = 0; i <= 39; i++)
            {
                lstHideColumnasLoad.Add(i);
            }
            lstHideColumnasLoad.Remove(0);
            lstHideColumnasLoad.Remove(1);
            //lstHideColumnasLoad.Remove(2); //cod
            //lstHideColumnasLoad.Remove(3);//min
            lstHideColumnasLoad.Remove(4);
            lstHideColumnasLoad.Remove(5);
            lstHideColumnasLoad.Remove(6);
            lstHideColumnasLoad.Remove(7);
            lstHideColumnasLoad.Remove(8);
            lstHideColumnasLoad.Remove(18);
            lstHideColumnasLoad.Remove(19);
            lstHideColumnasLoad.Remove(25);
            lstHideColumnasLoad.Remove(26);
            lstHideColumnasLoad.Remove(29);
            lstHideColumnasLoad.Remove(33);
            lstHideColumnasLoad.Remove(34);
            lstHideColumnasLoad.Remove(35);
            lstHideColumnasLoad.Remove(36);

            return lstHideColumnasLoad;
        }

        private List<int> CargarCabecerasVisiblesMin(DataTable dt)
        {
            List<int> lstHideColumnasLoad = new List<int>();

            for (int i = 0; i <= 39; i++)
            {
                lstHideColumnasLoad.Add(i);
            }
            lstHideColumnasLoad.Remove(0); //torneo
            lstHideColumnasLoad.Remove(1); //hora
            //lstHideColumnasLoad.Remove(2); //cod
            //lstHideColumnasLoad.Remove(3); //min
            lstHideColumnasLoad.Remove(4); //local
            lstHideColumnasLoad.Remove(5); //L
            lstHideColumnasLoad.Remove(6); //E
            lstHideColumnasLoad.Remove(7); //V
            lstHideColumnasLoad.Remove(8); //visita
            //lstHideColumnasLoad.Remove(18);
            //lstHideColumnasLoad.Remove(19);
            //lstHideColumnasLoad.Remove(25);
            //lstHideColumnasLoad.Remove(26);
            //lstHideColumnasLoad.Remove(29);
            //lstHideColumnasLoad.Remove(33);
            //lstHideColumnasLoad.Remove(34);
            //lstHideColumnasLoad.Remove(35);
            //lstHideColumnasLoad.Remove(36);

            return lstHideColumnasLoad;
        }

        private ListaModel CargarListaFinal(DataTable dt)
        {
            ListaModel listaFinal = new ListaModel();
            listaFinal.Lista = new List<ModeloModel>();
            var i = 1;

            foreach (DataRow row in dt.Rows)
            {
                ModeloModel myModelo = new ModeloModel();
                myModelo.ID = i.ToString();
                myModelo.TORNEO = row["TORNEO"].ToString();
                myModelo.HORA = row["HORA"].ToString();
                myModelo.COD = row["COD"].ToString();
                myModelo.MINIMO = row["MINIMO"].ToString();
                myModelo.LOCAL = row["LOCAL"].ToString();
                myModelo.L = row["L"].ToString();
                myModelo.E = row["E"].ToString();
                myModelo.V = row["V"].ToString();
                myModelo.VISITA = row["VISITA"].ToString();
                myModelo.LoE = row["LoE"].ToString();
                myModelo.LoV = row["LoV"].ToString();
                myModelo.EoV = row["EoV"].ToString();
                myModelo.PTL = row["1TL"].ToString();
                myModelo.PTE = row["1TE"].ToString();
                myModelo.PTV = row["1TV"].ToString();
                myModelo.STL = row["2TL"].ToString();
                myModelo.STE = row["2TE"].ToString();
                myModelo.STV = row["2TV"].ToString();
                myModelo.LL = row["L/L"].ToString();
                myModelo.EL = row["E/L"].ToString();
                myModelo.VL = row["V/L"].ToString();
                myModelo.LE = row["L/E"].ToString();
                myModelo.EE = row["E/E"].ToString();
                myModelo.VE = row["V/E"].ToString();
                myModelo.LV = row["L/V"].ToString();
                myModelo.EV = row["E/V"].ToString();
                myModelo.VV = row["V/V"].ToString();
                myModelo.MenosUnoPuntoCinco = row["-1.5"].ToString();
                myModelo.MasUnoPuntoCinco = row["+1.5"].ToString();
                myModelo.MenosDosPuntoCinco = row["-2.5"].ToString();
                myModelo.MasDosPuntoCinco = row["+2.5"].ToString();
                myModelo.MenosTresPuntoCinco = row["-3.5"].ToString();
                myModelo.MasTresPuntoCinco = row["+3.5"].ToString();
                myModelo.CeroUnGol = row["0-1"].ToString();
                myModelo.DosTresGol = row["2-3"].ToString();
                myModelo.CuatroMasGol = row["4+"].ToString();
                myModelo.AmbosAnotan = row["A"].ToString();
                myModelo.NingunoAnota = row["NA"].ToString();
                myModelo.Impar = row["I"].ToString();
                myModelo.Par = row["P"].ToString();
                myModelo.Imagen = row["FLAG"].ToString();
                listaFinal.Lista.Add(myModelo);
                i++;
            }

            return listaFinal;
        }

        private ListaModel CargarDataFinal(string path)
        {
            ListaModel listaFinal = new ListaModel();
            List<int> listaCabecerasFav = new List<int>();
            List<int> listaCabecerasMin = new List<int>();
            List<CabeceraModel> listaCabeceras = new List<CabeceraModel>();
            List<TorneoModel> listaTorneos = new List<TorneoModel>();
            FileModel fileModel = new FileModel();

            listaFinal.Lista = new List<ModeloModel>();

            List<string> lstFinal = FormateaData(path);

            DataTable dt = FormateaDataFinal(lstFinal, ref listaCabeceras, ref listaTorneos, ref listaCabecerasFav, ref listaCabecerasMin);

            //var pathCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "Files");
            var pathCarpeta = System.IO.Path.Combine(Server.MapPath("~/"), "Files");

            listaFinal = CargaDataModelo(pathCarpeta);

            return listaFinal;
        }

        private string GetFile(HttpPostedFileBase files)
        {
            var filepathName = string.Empty;
            if (files != null)
            {
                if (files.ContentLength > 0)
                {
                    //Getting FileName
                    var fileName = System.IO.Path.GetFileName(files.FileName);

                    //Assigning Unique Filename (Guid)
                    var myUniqueFileName = Convert.ToString(Guid.NewGuid());

                    //Getting file Extension
                    var fileExtension = System.IO.Path.GetExtension(fileName);

                    var hoy = String.Concat(DateTime.Today.ToString("yyyyMMdd"), DateTime.Now.ToString("HHmmss"));

                    // concatenating  FileName + FileExtension
                    var newFileName = String.Concat(hoy, myUniqueFileName, fileExtension);

                    // Combines two strings into a path.
                    var filepath = System.IO.Path.Combine(Server.MapPath("~/"), "Files", newFileName);
                    //var filepath = Path.Combine(Directory.GetCurrentDirectory(), "Files", newFileName);

                    using (FileStream fs = System.IO.File.Create(filepath))
                    {
                        files.InputStream.CopyTo(fs);
                        fs.Flush();
                    }

                    filepathName = filepath;
                }
            }
            return filepathName;
        }

        private string EvalNombresEquipos(int index, string[] campos, string line)
        {
            string local;
            for (int i = 0; i <= 10; i++)
            {
                double irpta1 = 0;
                string valor1 = campos[4 + index + i].ToString();
                bool result1 = false;
                if (valor1.Contains("."))
                {
                    result1 = double.TryParse(valor1, out irpta1);
                }
                if (valor1.Contains("-")) result1 = true;

                if (result1 == false)
                {
                    local = valor1;
                    double irpta2 = 0;
                    string valor2 = campos[4 + index + i + 1].ToString();
                    bool result2 = false;
                    if (valor2.Contains("."))
                    {
                        result2 = double.TryParse(valor2, out irpta2);
                    }
                    if (valor2.Contains("-")) result2 = true;

                    if (result2 == false)
                    {
                        local = local + "+" + valor2;
                        double irpta3 = 0;
                        string valor3 = campos[4 + index + i + 2].ToString();
                        bool result3 = false;
                        if (valor3.Contains("."))
                        {
                            result3 = double.TryParse(valor3, out irpta3);
                        }
                        if (valor3.Contains("-")) result3 = true;

                        if (result3 == false)
                        {
                            local = local + "+" + valor3;
                            double irpta4 = 0;
                            string valor4 = campos[4 + index + i + 3].ToString();
                            bool result4 = false;
                            if (valor4.Contains("."))
                            {
                                result4 = double.TryParse(valor4, out irpta4);
                            }
                            if (valor4.Contains("-")) result4 = true;

                            if (result4 == false)
                            {
                                local = local + "+" + valor4;
                                double irpta5 = 0;
                                string valor5 = campos[4 + index + i + 4].ToString();
                                bool result5 = false;
                                if (valor5.Contains("."))
                                {
                                    result5 = double.TryParse(valor5, out irpta5);
                                }
                                if (valor5.Contains("-")) result5 = true;

                                if (result5 == false)
                                {
                                    local = local + "+" + valor5;
                                    double irpta6 = 0;
                                    string valor6 = campos[4 + index + i + 5].ToString();
                                    bool result6 = false;
                                    if (valor6.Contains("."))
                                    {
                                        result6 = double.TryParse(valor6, out irpta6);
                                    }
                                    if (valor6.Contains("-")) result6 = true;

                                    if (result6 == false)
                                    {
                                        local = local + "+" + valor6;
                                        double irpta7 = 0;
                                        string valor7 = campos[4 + index + i + 6].ToString();
                                        bool result7 = false;
                                        if (valor7.Contains("."))
                                        {
                                            result7 = double.TryParse(valor7, out irpta7);
                                        }
                                        if (valor7.Contains("-")) result7 = true;
                                    }
                                    else
                                    {
                                        string valorOld = valor1 + " " + valor2 + " " + valor3 + " " + valor4 + " " + valor5;
                                        line = line.Replace(valorOld, local);
                                        break;
                                    }
                                }
                                else
                                {
                                    string valorOld = valor1 + " " + valor2 + " " + valor3 + " " + valor4;
                                    line = line.Replace(valorOld, local);
                                    break;
                                }

                            }
                            else
                            {
                                string valorOld = valor1 + " " + valor2 + " " + valor3;
                                line = line.Replace(valorOld, local);
                                break;
                            }

                        }
                        else
                        {
                            string valorOld = valor1 + " " + valor2;
                            line = line.Replace(valorOld, local);
                            break;
                        }

                    }
                    else
                    {
                        string valorOld = valor1;
                    }
                }
                else
                {
                    string valorOld = valor1;
                }

            }

            campos = line.Split(' ');

            string visita;
            for (int i = 0; i <= 10; i++)
            {
                double irpta1 = 0;
                string valor1 = campos[8 + index + i].ToString();
                bool result1 = false;
                if (valor1.Contains("."))
                {
                    result1 = double.TryParse(valor1, out irpta1);
                }
                if (valor1.Contains("-")) result1 = true;

                if (result1 == false)
                {
                    visita = valor1;
                    double irpta2 = 0;
                    string valor2 = campos[8 + index + i + 1].ToString();
                    bool result2 = false;
                    if (valor2.Contains("."))
                    {
                        result2 = double.TryParse(valor2, out irpta2);
                    }
                    if (valor2.Contains("-")) result2 = true;

                    if (result2 == false)
                    {
                        visita = visita + "+" + valor2;
                        double irpta3 = 0;
                        string valor3 = campos[8 + index + i + 2].ToString();
                        bool result3 = false;
                        if (valor3.Contains("."))
                        {
                            result3 = double.TryParse(valor3, out irpta3);
                        }
                        if (valor3.Contains("-")) result3 = true;

                        if (result3 == false)
                        {
                            visita = visita + "+" + valor3;
                            double irpta4 = 0;
                            string valor4 = campos[8 + index + i + 3].ToString();
                            bool result4 = false;
                            if (valor4.Contains("."))
                            {
                                result4 = double.TryParse(valor4, out irpta4);
                            }
                            if (valor4.Contains("-")) result4 = true;

                            if (result4 == false)
                            {
                                visita = visita + "+" + valor4;
                                double irpta5 = 0;
                                string valor5 = campos[8 + index + i + 4].ToString();
                                bool result5 = false;
                                if (valor5.Contains("."))
                                {
                                    result5 = double.TryParse(valor5, out irpta5);
                                }
                                if (valor5.Contains("-")) result5 = true;

                                if (result5 == false)
                                {
                                    visita = visita + "+" + valor5;
                                    double irpta6 = 0;
                                    string valor6 = campos[8 + index + i + 5].ToString();
                                    bool result6 = false;
                                    if (valor6.Contains("."))
                                    {
                                        result6 = double.TryParse(valor6, out irpta6);
                                    }
                                    if (valor6.Contains("-")) result6 = true;

                                    if (result6 == false)
                                    {
                                        visita = visita + "+" + valor6;
                                        double irpta7 = 0;
                                        string valor7 = campos[8 + index + i + 6].ToString();
                                        bool result7 = false;
                                        if (valor7.Contains("."))
                                        {
                                            result7 = double.TryParse(valor7, out irpta7);
                                        }
                                        if (valor7.Contains("-")) result7 = true;
                                    }
                                    else
                                    {
                                        string valorOld = valor1 + " " + valor2 + " " + valor3 + " " + valor4 + " " + valor5;
                                        line = line.Replace(valorOld, visita);
                                        break;
                                    }
                                }
                                else
                                {
                                    string valorOld = valor1 + " " + valor2 + " " + valor3 + " " + valor4;
                                    line = line.Replace(valorOld, visita);
                                    break;
                                }
                            }
                            else
                            {
                                string valorOld = valor1 + " " + valor2 + " " + valor3;
                                line = line.Replace(valorOld, visita);
                                break;
                            }

                        }
                        else
                        {
                            string valorOld = valor1 + " " + valor2;
                            line = line.Replace(valorOld, visita);
                            break;
                        }
                    }
                    else
                    {
                        string valorOld = valor1;
                    }
                }
                else
                {
                    string valorOld = valor1;
                }

            }

            //lstFinal.Add(line);
            return line;
        }

        private List<string> FormateaData(string rutaCompleta)
        {
            List<string> lstFinal = new List<string>();
            if (!String.IsNullOrEmpty(rutaCompleta))
            {
                string[] fileArray = System.IO.File.ReadAllLines(rutaCompleta);
                char[] x = { ' ' }; // delimitador

                string line = "";
                lstFinal = new List<string>();
                for (int i = 0; i < fileArray.Length; i++)
                {
                    line = fileArray[i].ToString();
                    if (line != "")
                    {
                        //elimina la columna en vivo
                        if (line.Substring(0, 2).ToUpper() == "L ")
                        {
                            line = line.Substring(2, line.Length - 2);
                        }

                        string[] campos = line.Split(x);
                        line = EvalNombresEquipos(0, campos, line);
                        lstFinal.Add(line);

                    }
                }
            }
            return lstFinal;
        }

        private DataTable FormateaDataFinal(List<string> lstFinal, ref List<CabeceraModel> listaCabeceras, ref List<TorneoModel> listaTorneos, ref List<int> listaCabecerasFav, ref List<int> listaCabecerasMin)
        {
            char[] x = { ' ' }; // delimitador

            DataTable dt = CargarTabla();

            listaCabeceras = CargarCabeceras(dt);
            listaCabecerasFav = CargarCabecerasVisiblesFav(dt);
            listaCabecerasMin = CargarCabecerasVisiblesMin(dt);

            for (int i = 0; i < lstFinal.Count; i++)
            {
                var columns = lstFinal[i].Split(x);
                DataRow row = dt.NewRow();
                for (int ii = 0; ii < columns.Length; ii++)
                {
                    string valor = columns[ii].ToString();
                    row[ii] = valor;
                }
                dt.Rows.Add(row);
            }

            TorneosDA _daTorneos = new TorneosDA();
            List<TorneoModel> listaTorneosDB = _daTorneos.ListarTorneos();

            for (int k = 0; k <= dt.Rows.Count - 1; k++)
            {
                string torneo = dt.Rows[k]["TORNEO"].ToString();
                string local = dt.Rows[k]["LOCAL"].ToString().Replace("+", " ");
                string visita = dt.Rows[k]["VISITA"].ToString().Replace("+", " ");

                dt.Rows[k]["LOCAL"] = local;
                dt.Rows[k]["VISITA"] = visita;

                string imagen = ObtenerUrlImagenDB(torneo, listaTorneosDB);
                dt.Rows[k]["FLAG"] = imagen;

            }

            listaTorneos = CargarTorneos(dt);

            for (int i = 0; i < listaCabecerasFav.Count; i++)
            {
                var Index = listaCabecerasFav[i].ToString();
                listaCabeceras[Convert.ToInt32(Index)].IsChecked = false;
            }

            for (int i = 0; i < listaCabecerasMin.Count; i++)
            {
                var Index = listaCabecerasMin[i].ToString();
                listaCabeceras[Convert.ToInt32(Index)].IsChecked = false;
            }

            return dt;
        }

        public void ExtractTextFromPdf(string path)
        {
            var newFileNameOriginal = String.Concat("TA-ED-Regular-Inicial", ".txt");
            var filepath = System.IO.Path.Combine(Server.MapPath("~/"), "Files", "Programa", newFileNameOriginal);

            try
            {
                using (PdfReader reader = new PdfReader(path))
                {
                    StringBuilder text = new StringBuilder();

                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                    }

                    System.IO.File.WriteAllText(filepath, text.ToString());

                    List<string> listaProgramas = new List<string>();
                    //crear un nuevo file limpio
                    DataTable listaFinal = new DataTable();
                    listaFinal.Columns.Add("Codigo");
                    listaFinal.Columns.Add("Nombre");
                    listaFinal.Columns.Add("Fecha");
                    listaFinal.Columns.Add("Descripcion");

                    string codigo = string.Empty;
                    string nombreProg = string.Empty;
                    string fechaProg = string.Empty;

                    var newFileName = String.Concat("TA-ED-Regular", ".txt");
                    var filepathNew = System.IO.Path.Combine(Server.MapPath("~/"), "Files", "Programa", newFileName);

                    string[] empty = new string[0];
                    System.IO.File.WriteAllLines(filepathNew, empty);
                    StreamWriter fileClean = new StreamWriter(filepathNew, append: true);

                    string[] fileArray = System.IO.File.ReadAllLines(filepath);

                    string nomProgramaInicial = string.Empty;
                    string line = "";
                    var lstFinal = new List<string>();
                    for (int i = 0; i < fileArray.Length; i++)
                    {
                        DataRow row = listaFinal.NewRow();

                        line = fileArray[i].ToString();
                        if (ValidaTextoEnLinea(line) == true)
                        {
                            if (line.Contains("NUEVAS APUESTAS"))
                                break;

                            if (line.Contains("PROGRAMA"))
                            {
                                if (nomProgramaInicial != line)
                                {
                                    fileClean.WriteLine(line);

                                    nomProgramaInicial = line;
                                    listaProgramas.Add(line);

                                    var index = line.ToString().ToUpper().IndexOf("PROGRAMA");
                                    codigo = line.ToString().Substring(index).Replace("PROGRAMA", string.Empty).Replace(".txt", string.Empty).Trim();
                                    nombreProg = line.ToString().Replace(".txt", string.Empty).Trim();
                                    fechaProg = line.ToString().Split('-').GetValue(0).ToString().Trim();

                                }
                            }
                            else
                            {
                                if (line.Contains("V-"))
                                    line = line.Replace("V-", "V ");

                                if (line.Contains("AL-"))
                                    line = line.Replace("AL-", "AL ");

                                if (line.Contains("VILLEFRANCHE-BEAUJOLAIS"))
                                    line = line.Replace("VILLEFRANCHE-BEAUJOLAIS", "VILLEFRANCHE BEAUJOLAIS");

                                if (line.Contains("'"))
                                    line = line.Replace("'", "");

                                fileClean.WriteLine(line);

                                row["Codigo"] = codigo;
                                row["Nombre"] = line;
                                row["Descripcion"] = nombreProg.Replace(" ", "|").Replace("Á", "A").Replace("É", "E").Replace("Í", "I").Replace("Ó", "O").Replace("Ú", "U");
                                row["Fecha"] = GetFechaPrograma(fechaProg);
                                listaFinal.Rows.Add(row);
                            }
                        }
                    }

                    fileClean.Close();

                    string addResultSSIS = string.Empty;

                    //crear files unicos
                    for (int j = 0; j <= listaProgramas.Count - 1; j++)
                    {

                        var index = listaProgramas[j].ToString().ToUpper().IndexOf("PROGRAMA");
                        codigo = listaProgramas[j].ToString().Substring(index).Replace("PROGRAMA", string.Empty).Replace(".txt", string.Empty).Trim();

                        DataRow[] result = listaFinal.Select("Codigo = " + codigo);
                        string addResult = string.Empty;
                        foreach (DataRow row in result)
                        {
                            addResult = addResult + row["Nombre"] + "\n";
                            addResultSSIS = addResultSSIS + codigo + " " + row["Descripcion"] + " " + row["Fecha"] + " " + row["Nombre"] + "\n";
                        }

                        var fileUnicoName = String.Concat(listaProgramas[j], ".txt");
                        var fileUnicoPath = System.IO.Path.Combine(Server.MapPath("~/"), "Files", fileUnicoName);

                        using (FileStream fs = new FileStream(fileUnicoPath, FileMode.Create))
                        {
                            byte[] bytes = Encoding.UTF8.GetBytes(addResult);
                            fs.Write(bytes, 0, bytes.Length);
                            fs.Close();
                        }

                    }

                    //PARA SSIS
                    var fileUnicoNameSSIS = String.Concat("TA-ED-Regular-SSIS", ".txt");
                    var fileUnicoPathSSIS = System.IO.Path.Combine(Server.MapPath("~/"), "Files", "Programa", "SSIS", fileUnicoNameSSIS);

                    using (FileStream fs = new FileStream(fileUnicoPathSSIS, FileMode.Create))
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(addResultSSIS);
                        fs.Write(bytes, 0, bytes.Length);
                        fs.Close();
                    }

                    //crea txt formateado para SSIS
                    DataTable dtSSIS = CargaDataModeloSSIS();
                }
            }
            catch (Exception e)
            {
                CapturarError(e, "ExtractTExtFromPdf");
                //ViewBag.ErrorMessage = e.Message.ToString();
                //ret = false;
            }
        }

        public bool ValidaTextoEnLinea(string texto)
        {
            bool ret = true;
            if (texto.Contains("www.teapuesto.com.pe"))
                ret = false;
            else if (texto.Contains("www.teapuesto.com.pe"))
                ret = false;
            else if (texto.Contains("GOL"))
                ret = false;
            else if (texto.Contains("LOCAL"))
                ret = false;
            else if (texto.Contains("VISITA"))
                ret = false;
            else if (texto.Contains("L E V"))
                ret = false;
            else if (texto.Contains("0-1 2-3 4+"))
                ret = false;
            else if (texto.Contains("LoE LoV EoV"))
                ret = false;
            else if (texto.Contains("VIVO"))
                ret = false;
            else if (texto.Contains("TORNEO"))
                ret = false;
            else if (texto.Contains("HORA"))
                ret = false;
            else if (texto.Contains("COD"))
                ret = false;
            else if (texto.Contains("MÍNIMO"))
                ret = false;
            else if (texto.Contains("POS."))
                ret = false;
            else if (texto.Contains("PRIMER"))
                ret = false;
            else if (texto.Contains("- 1.5 + 1.5 - 2.5 + 2.5 - 3.5 + 3.5"))
                ret = false;
            else if (texto.Contains("TIEMPO"))
                ret = false;
            else if (texto.Contains("- 1.5- 1.5 + 1.5+ 1.5 - 2.5- 2.5 + 2.5+ 2.5 - 3.5- 3.5 + 3.5+ 3.5"))
                ret = false;
            else if (texto.Contains("10(-) 20(+) 10(-) 30(+) 30(-) 40(+)"))
                ret = false;
            else if (texto.Contains("LL EE VV"))
                ret = false;
            else if (texto == "V")
                ret = false;
            else
            {
                int i = 0;
                bool result = int.TryParse(texto, out i);
                if (result == true)
                    ret = false;
            }

            return ret;
        }

        public List<string> ExtractProgramasFromPdfStrategy(string path = "")
        {
            List<string> listaProgramas = new List<string>();
            string nombrePrograma = string.Empty;

            //path = "https://www.intralot.com.pe/intralot/docs/teapuesto/edicion_regular/TA-ED-Regular.pdf";
            path = ConfigurationManager.AppSettings["urlArchivo"];

            ITextExtractionStrategy its = new iTextSharp.text.pdf.parser.LocationTextExtractionStrategy();

            using (PdfReader reader = new PdfReader(path))
            {
                StringBuilder text = new StringBuilder();

                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    string thePage = PdfTextExtractor.GetTextFromPage(reader, i, its);
                    string[] theLines = thePage.Split('\n');
                    foreach (var theLine in theLines)
                    {
                        bool isValidLine = ValidaTextoEnLinea(theLine);
                        if (isValidLine)
                        {
                            if (theLine.Contains("PROGRAMA") && theLine.Length < 50)
                            {
                                listaProgramas.Add(theLine);
                            }
                        }
                    }
                }

                List<string> listaProgramasUnicos = listaProgramas.Select(x => x).Distinct().ToList();
                return listaProgramasUnicos;

            }
        }

        #endregion


        #region "SSIS"

        public DataTable CargaDataModeloSSIS()
        {
            List<string> lstFinalSSIS = FormateaDataSSIS();
            DataTable dtFinalSSIS = FormateaDataFinalSSIS(lstFinalSSIS);

            //PARA SSIS
            var fileUnicoNameSSIS = String.Concat("TA-ED-Regular-SSIS-Final", ".txt");
            var fileUnicoPathSSIS = System.IO.Path.Combine(Server.MapPath("~/"), "Files", "Programa", "SSIS", fileUnicoNameSSIS);

            var tab = "\t";
            var saltolinea = "\n";

            string addResultSSISCol = string.Empty;
            foreach (DataColumn col in dtFinalSSIS.Columns)
            {
                addResultSSISCol = addResultSSISCol + col.Caption + tab;
            }
            addResultSSISCol = addResultSSISCol + saltolinea;

            string addResultSSIS = string.Empty;
            foreach (DataRow row in dtFinalSSIS.Rows)
            {
                foreach (DataColumn col in dtFinalSSIS.Columns)
                {
                    addResultSSIS = addResultSSIS + row[col.ColumnName] + tab;
                }
                addResultSSIS = addResultSSIS + saltolinea;
            }

            var fileFinal = addResultSSISCol + addResultSSIS;
            using (FileStream fs = new FileStream(fileUnicoPathSSIS, FileMode.Create))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(fileFinal);
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();
            }

            return dtFinalSSIS;
        }

        private List<string> FormateaDataSSIS()
        {
            var fileUnicoNameSSIS = String.Concat("TA-ED-Regular-SSIS", ".txt");
            var pathCarpetaSSIS = System.IO.Path.Combine(Server.MapPath("~/"), "Files", "Programa", "SSIS");
            var rutaCompleta = System.IO.Path.Combine(pathCarpetaSSIS, fileUnicoNameSSIS);

            List<string> lstFinal = new List<string>();
            if (!String.IsNullOrEmpty(rutaCompleta))
            {
                string[] fileArray = System.IO.File.ReadAllLines(rutaCompleta);
                char[] x = { ' ' }; // delimitador

                string line = "";
                lstFinal = new List<string>();
                for (int i = 0; i < fileArray.Length; i++)
                {
                    line = fileArray[i].ToString();
                    if (line != "")
                    {
                        //elimina la columna en vivo
                        if (line.Substring(0, 2).ToUpper() == "L ")
                        {
                            line = line.Substring(2, line.Length - 2);
                        }

                        string[] campos = line.Split(x);
                        line = EvalNombresEquiposSSIS(0, campos, line);
                        lstFinal.Add(line);

                    }
                }
            }
            return lstFinal;
        }

        private string EvalNombresEquiposSSIS(int index, string[] campos, string line)
        {
            string local;
            for (int i = 3; i <= 10; i++)
            {
                double irpta1 = 0;
                string valor1 = campos[4 + index + i].ToString();
                bool result1 = false;
                if (valor1.Contains("."))
                {
                    result1 = double.TryParse(valor1, out irpta1);
                }
                if (valor1.Contains("-")) result1 = true;

                if (result1 == false)
                {
                    local = valor1;
                    double irpta2 = 0;
                    string valor2 = campos[4 + index + i + 1].ToString();
                    bool result2 = false;
                    if (valor2.Contains("."))
                    {
                        result2 = double.TryParse(valor2, out irpta2);
                    }
                    if (valor2.Contains("-")) result2 = true;

                    if (result2 == false)
                    {
                        local = local + "+" + valor2;
                        double irpta3 = 0;
                        string valor3 = campos[4 + index + i + 2].ToString();
                        bool result3 = false;
                        if (valor3.Contains("."))
                        {
                            result3 = double.TryParse(valor3, out irpta3);
                        }
                        if (valor3.Contains("-")) result3 = true;

                        if (result3 == false)
                        {
                            local = local + "+" + valor3;
                            double irpta4 = 0;
                            string valor4 = campos[4 + index + i + 3].ToString();
                            bool result4 = false;
                            if (valor4.Contains("."))
                            {
                                result4 = double.TryParse(valor4, out irpta4);
                            }
                            if (valor4.Contains("-")) result4 = true;

                            if (result4 == false)
                            {
                                local = local + "+" + valor4;
                                double irpta5 = 0;
                                string valor5 = campos[4 + index + i + 4].ToString();
                                bool result5 = false;
                                if (valor5.Contains("."))
                                {
                                    result5 = double.TryParse(valor5, out irpta5);
                                }
                                if (valor5.Contains("-")) result5 = true;

                                if (result5 == false)
                                {
                                    local = local + "+" + valor5;
                                    double irpta6 = 0;
                                    string valor6 = campos[4 + index + i + 5].ToString();
                                    bool result6 = false;
                                    if (valor6.Contains("."))
                                    {
                                        result6 = double.TryParse(valor6, out irpta6);
                                    }
                                    if (valor6.Contains("-")) result6 = true;

                                    if (result6 == false)
                                    {
                                        local = local + "+" + valor6;
                                        double irpta7 = 0;
                                        string valor7 = campos[4 + index + i + 6].ToString();
                                        bool result7 = false;
                                        if (valor7.Contains("."))
                                        {
                                            result7 = double.TryParse(valor7, out irpta7);
                                        }
                                        if (valor7.Contains("-")) result7 = true;
                                    }
                                    else
                                    {
                                        string valorOld = valor1 + " " + valor2 + " " + valor3 + " " + valor4 + " " + valor5;
                                        line = line.Replace(valorOld, local);
                                        break;
                                    }
                                }
                                else
                                {
                                    string valorOld = valor1 + " " + valor2 + " " + valor3 + " " + valor4;
                                    line = line.Replace(valorOld, local);
                                    break;
                                }

                            }
                            else
                            {
                                string valorOld = valor1 + " " + valor2 + " " + valor3;
                                line = line.Replace(valorOld, local);
                                break;
                            }

                        }
                        else
                        {
                            string valorOld = valor1 + " " + valor2;
                            line = line.Replace(valorOld, local);
                            break;
                        }

                    }
                    else
                    {
                        string valorOld = valor1;
                    }
                }
                else
                {
                    string valorOld = valor1;
                }

            }

            campos = line.Split(' ');

            string visita;
            for (int i = 1; i <= 10; i++)
            {
                double irpta1 = 0;
                string valor1 = campos[8 + index + i].ToString();
                bool result1 = false;
                if (valor1.Contains("."))
                {
                    result1 = double.TryParse(valor1, out irpta1);
                }
                if (valor1.Contains("-")) result1 = true;

                if (result1 == false)
                {
                    visita = valor1;
                    double irpta2 = 0;
                    string valor2 = campos[8 + index + i + 1].ToString();
                    bool result2 = false;
                    if (valor2.Contains("."))
                    {
                        result2 = double.TryParse(valor2, out irpta2);
                    }
                    if (valor2.Contains("-")) result2 = true;

                    if (result2 == false)
                    {
                        visita = visita + "+" + valor2;
                        double irpta3 = 0;
                        string valor3 = campos[8 + index + i + 2].ToString();
                        bool result3 = false;
                        if (valor3.Contains("."))
                        {
                            result3 = double.TryParse(valor3, out irpta3);
                        }
                        if (valor3.Contains("-")) result3 = true;

                        if (result3 == false)
                        {
                            visita = visita + "+" + valor3;
                            double irpta4 = 0;
                            string valor4 = campos[8 + index + i + 3].ToString();
                            bool result4 = false;
                            if (valor4.Contains("."))
                            {
                                result4 = double.TryParse(valor4, out irpta4);
                            }
                            if (valor4.Contains("-")) result4 = true;

                            if (result4 == false)
                            {
                                visita = visita + "+" + valor4;
                                double irpta5 = 0;
                                string valor5 = campos[8 + index + i + 4].ToString();
                                bool result5 = false;
                                if (valor5.Contains("."))
                                {
                                    result5 = double.TryParse(valor5, out irpta5);
                                }
                                if (valor5.Contains("-")) result5 = true;

                                if (result5 == false)
                                {
                                    visita = visita + "+" + valor5;
                                    double irpta6 = 0;
                                    string valor6 = campos[8 + index + i + 5].ToString();
                                    bool result6 = false;
                                    if (valor6.Contains("."))
                                    {
                                        result6 = double.TryParse(valor6, out irpta6);
                                    }
                                    if (valor6.Contains("-")) result6 = true;

                                    if (result6 == false)
                                    {
                                        visita = visita + "+" + valor6;
                                        double irpta7 = 0;
                                        string valor7 = campos[8 + index + i + 6].ToString();
                                        bool result7 = false;
                                        if (valor7.Contains("."))
                                        {
                                            result7 = double.TryParse(valor7, out irpta7);
                                        }
                                        if (valor7.Contains("-")) result7 = true;
                                    }
                                    else
                                    {
                                        string valorOld = valor1 + " " + valor2 + " " + valor3 + " " + valor4 + " " + valor5;
                                        line = line.Replace(valorOld, visita);
                                        break;
                                    }
                                }
                                else
                                {
                                    string valorOld = valor1 + " " + valor2 + " " + valor3 + " " + valor4;
                                    line = line.Replace(valorOld, visita);
                                    break;
                                }
                            }
                            else
                            {
                                string valorOld = valor1 + " " + valor2 + " " + valor3;
                                line = line.Replace(valorOld, visita);
                                break;
                            }

                        }
                        else
                        {
                            string valorOld = valor1 + " " + valor2;
                            line = line.Replace(valorOld, visita);
                            break;
                        }
                    }
                    else
                    {
                        string valorOld = valor1;
                    }
                }
                else
                {
                    string valorOld = valor1;
                }

            }

            //lstFinal.Add(line);
            return line;
        }

        private DataTable FormateaDataFinalSSIS(List<string> lstFinal)
        {
            char[] x = { ' ' }; // delimitador

            DataTable dt = CargarTablaSSIS();

            for (int i = 0; i < lstFinal.Count; i++)
            {
                var columns = lstFinal[i].Split(x);
                DataRow row = dt.NewRow();
                for (int ii = 0; ii < columns.Length; ii++)
                {
                    string valor = columns[ii].ToString();
                    row[ii] = valor;
                }
                dt.Rows.Add(row);
            }

            TorneosDA _daTorneos = new TorneosDA();
            List<TorneoModel> listaTorneosDB = _daTorneos.ListarTorneos();

            for (int k = 0; k <= dt.Rows.Count - 1; k++)
            {
                string torneo = dt.Rows[k]["TORNEO"].ToString();
                string local = dt.Rows[k]["LOCAL"].ToString().Replace("+", " ");
                string visita = dt.Rows[k]["VISITA"].ToString().Replace("+", " ");
                string nombreProg = dt.Rows[k]["NOMBRE"].ToString().Replace("|", " ");
                string fechaProg = dt.Rows[k]["FECHA"].ToString().Replace("|", " ");

                dt.Rows[k]["LOCAL"] = local;
                dt.Rows[k]["VISITA"] = visita;
                dt.Rows[k]["NOMBRE"] = nombreProg;
                dt.Rows[k]["FECHA"] = fechaProg;
                dt.Rows[k]["FLAG"] = ObtenerUrlImagenDB(torneo, listaTorneosDB);

            }

            return dt;
        }

        private string GetFechaPrograma(string fechaProg)
        {
            string ret = string.Empty;
            var array = fechaProg.Split(' ');
            var dia = array.GetValue(1).ToString();
            var mes = EvaluaNombreMes(array.GetValue(3).ToString());
            ret = dia + "/" + mes + "/" + DateTime.Today.Year.ToString();
            return ret;
        }

        private string EvaluaNombreMes(string nombreMes)
        {
            string ret = string.Empty;
            if (nombreMes == "ENERO") ret = "01";
            else if (nombreMes == "FEBRERO") ret = "02";
            else if (nombreMes == "MARZO") ret = "03";
            else if (nombreMes == "ABRIL") ret = "04";
            else if (nombreMes == "MAYO") ret = "05";
            else if (nombreMes == "JUNIO") ret = "06";
            else if (nombreMes == "JULIO") ret = "07";
            else if (nombreMes == "AGOSTO") ret = "08";
            else if (nombreMes == "SEPTIEMBRE") ret = "09";
            else if (nombreMes == "OCTUBRE") ret = "10";
            else if (nombreMes == "NOVIEMBRE") ret = "11";
            else if (nombreMes == "DICIEMBRE") ret = "12";
            return ret;
        }

        private DataTable CargarTablaSSIS()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("PROGRAMA");
            dt.Columns.Add("NOMBRE");
            dt.Columns.Add("FECHA");
            dt.Columns.Add("TORNEO");
            dt.Columns.Add("HORA");
            dt.Columns.Add("COD");
            dt.Columns.Add("MINIMO");
            dt.Columns.Add("LOCAL");
            dt.Columns.Add("L");
            dt.Columns.Add("E");
            dt.Columns.Add("V");
            dt.Columns.Add("VISITA");
            dt.Columns.Add("LoE");
            dt.Columns.Add("LoV");
            dt.Columns.Add("EoV");
            dt.Columns.Add("1TL");
            dt.Columns.Add("1TE");
            dt.Columns.Add("1TV");
            dt.Columns.Add("2TL");
            dt.Columns.Add("2TE");
            dt.Columns.Add("2TV");
            dt.Columns.Add("L/L");
            dt.Columns.Add("E/L");
            dt.Columns.Add("V/L");
            dt.Columns.Add("L/E");
            dt.Columns.Add("E/E");
            dt.Columns.Add("V/E");
            dt.Columns.Add("L/V");
            dt.Columns.Add("E/V");
            dt.Columns.Add("V/V");
            dt.Columns.Add("-1.5");
            dt.Columns.Add("+1.5");
            dt.Columns.Add("-2.5");
            dt.Columns.Add("+2.5");
            dt.Columns.Add("-3.5");
            dt.Columns.Add("+3.5");
            dt.Columns.Add("0-1");
            dt.Columns.Add("2-3");
            dt.Columns.Add("4+");
            dt.Columns.Add("A");
            dt.Columns.Add("NA");
            dt.Columns.Add("I");
            dt.Columns.Add("P");
            dt.Columns.Add("FLAG");
            return dt;
        }

        #endregion

    }
}
