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
    public class EquiposDA
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

        public List<EquiposModel> ListarEquipos()
        {
            //AspNetUsers user = _db.AspNetUsers.Where(m => m.Email == User.Identity.Name).ToList().FirstOrDefault();
            //List<Equipos> equiposFav = _db.Equipos.ToList();
            _db = new dbTuCalcuEntities();
            IQueryable<EquiposModel> queryResult = from x in _db.Equipos
                                                   select new EquiposModel
                                                   {
                                                       Text = x.DesEquipo,
                                                       Value = x.CodEquipo,
                                                       Imagen = x.Flag
                                                   };

            return queryResult.ToList();
        }

    }
}