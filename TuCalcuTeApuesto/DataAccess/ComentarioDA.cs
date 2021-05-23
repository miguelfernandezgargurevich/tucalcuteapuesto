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
    public class ComentarioDA
    {
        dbTuCalcuEntities _db;

        public string getExceptionEntityValidation(DbEntityValidationException error, string controlador = "", string accion = "")
        {
            string msg = string.Empty;
            foreach (var eve in error.EntityValidationErrors)
            {
                msg = msg + "Entity of type \"{0}\" in state \"{1}\" has the following validation errors:" +
                    eve.Entry.Entity.GetType().Name + eve.Entry.State;
                foreach (var ve in eve.ValidationErrors)
                {
                    msg = msg + "- Property: \"{0}\", Error: \"{1}\"" +
                        ve.PropertyName + " " + ve.ErrorMessage;
                }
            }

            var comentario = $@"Se ejecutó la accion: [{controlador}/{accion}] - MensajeError: {msg}";
            var logErrorFinal = string.Format("{0} | {1}", comentario, error.StackTrace);
            //log.ErrorFormat("{0} | {1}", comentario, error.StackTrace);

            var logError = String.Concat("ERROR-", "EquiposDA", ".txt");
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

            var logError = String.Concat("ERROR-","EquiposDA", ".txt");
            var pathLog = System.IO.Path.Combine("~/", "Files", "Programa", "SSIS", logError);

            using (FileStream fs = new FileStream(pathLog, FileMode.Create))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(logErrorFinal);
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();
            }


            return string.Format(logErrorFinal);
        }

        public bool GrabarComentario(Comentarios obj)
        {
            try
            {
                _db = new dbTuCalcuEntities();
                _db.Comentarios.Add(obj);
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

    }
}