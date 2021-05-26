using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using TuCalcuTeApuesto.Models;

namespace TuCalcuTeApuesto.DataAccess
{
    public class TorneosFavDA
    {
        dbTuCalcuEntities _db;

        public bool EliminarTorneoFav(TorneosFavoritos obj) {
            try
            {
                _db = new dbTuCalcuEntities();

                var entidad = _db.TorneosFavoritos.Where(m => m.CodTorneo == obj.CodTorneo && m.CodUsuario == obj.CodUsuario).ToList().FirstOrDefault();
                _db.TorneosFavoritos.Remove(entidad);
                _db.SaveChanges();

                return true;
            }
            catch (DbEntityValidationException e)
            {
                var error = getExceptionEntityValidation(e);
                return false;
            }
            catch (Exception ex)
            {
                var error = getException(ex);
                return false;
            }

        }

        public bool GrabarTorneoFav(TorneosFavoritos obj)
        {
            try
            {
                _db = new dbTuCalcuEntities();
                _db.TorneosFavoritos.Add(obj);
                _db.SaveChanges();

                return true;
            }
            catch (DbEntityValidationException e)
            {
                var error = getExceptionEntityValidation(e);
                return false;
            }
            catch (Exception ex)
            {
                var error = getException(ex);
                return false;
            }

        }

        public List<TorneosFavModel> ListarTorneosFavoritos(string codUsuario)
        {
            _db = new dbTuCalcuEntities();
            IQueryable<TorneosFavModel> queryResult = from r in _db.TorneosFavoritos
                                                      join tblUsuarios in _db.AspNetUsers on r.CodUsuario equals tblUsuarios.Id
                                                      join tblTorneos in _db.Torneos on r.CodTorneo equals tblTorneos.CodTorneo into gj
                                                      from x in gj.DefaultIfEmpty()
                                                      where tblUsuarios.Id.Equals(codUsuario)
                                                      select new TorneosFavModel
                                                      {
                                                          NombreCorto = x.NombreCorto,
                                                          Text = x.DesTorneo,
                                                          Value = x.CodTorneo,
                                                          Imagen = x.Flag
                                                      };

            return queryResult.ToList();
        }

        public string getExceptionEntityValidation(DbEntityValidationException error, string controlador = "", string accion = "")
        {
            string msg = string.Empty;
            //foreach (var eve in error.EntityValidationErrors)
            //{
            //    msg = msg + "Entity of type \"{0}\" in state \"{1}\" has the following validation errors:" +
            //        eve.Entry.Entity.GetType().Name + eve.Entry.State;
            //    foreach (var ve in eve.ValidationErrors)
            //    {
            //        msg = msg + "- Property: \"{0}\", Error: \"{1}\"" +
            //            ve.PropertyName + " " + ve.ErrorMessage;
            //    }
            //}

            //var comentario = $@"Se ejecutó la accion: [{controlador}/{accion}] - MensajeError: {msg}";
            //var logErrorFinal = string.Format("{0} | {1}", comentario, error.StackTrace);
            ////log.ErrorFormat("{0} | {1}", comentario, error.StackTrace);


            StringBuilder sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine();
            foreach (var eve in error.EntityValidationErrors)
            {
                sb.AppendLine(string.Format("- Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                    eve.Entry.Entity.GetType().FullName, eve.Entry.State));
                foreach (var ve in eve.ValidationErrors)
                {
                    sb.AppendLine(string.Format("-- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                        ve.PropertyName,
                        eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                        ve.ErrorMessage));
                }
            }
            sb.AppendLine();
            var logErrorFinal = sb.ToString();
            
            var logError = String.Concat("ERROR-", "TorneosFavDA", ".txt");
            var pathLog = System.IO.Path.Combine("~/", "Files", "Programa", "SSIS", logError);

            using (FileStream fs = new FileStream(pathLog, FileMode.Create))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(logErrorFinal);
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();
            }

            return string.Format(logErrorFinal);
        }

        public string getException(Exception error, string controlador = "", string accion = "")
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

            var logError = String.Concat("ERROR-", "EquiposFavDA", ".txt");
            var pathLog = System.IO.Path.Combine("~/", "Files", "Programa", "SSIS", logError);

            using (FileStream fs = new FileStream(pathLog, FileMode.Create))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(logErrorFinal);
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();
            }


            return string.Format(logErrorFinal);
        }

    }
}