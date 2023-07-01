using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcApplication3.Models;

namespace MvcApplication3.Controllers
{
    public class CargoController : Controller
    {
        //
        // GET: /Cargo/

        bdescuelaEntities entidad = new bdescuelaEntities();

        public ActionResult Index()
        {
            var listacargos = entidad.cargo;

            return View(listacargos.ToList());
        }

        public ActionResult ListadoMaestraCargos() {

            var listacargos = entidad.cargo;
            return View(listacargos.ToList());
        }

        public ActionResult UsuarioCargo(string cargo)
        {

            var modelo = from p in entidad.usuario where p.cargo.car_des == cargo select p;
            return View(modelo.ToList());
        }

        public ActionResult ListadoUsuarioconDescripcionCargo(string cargo)
        {

            var modelo = from p in entidad.usuario
                         join c in entidad.cargo
                             on p.car_cod equals c.car_cod

                         select new ElUsuario
                         {
                             codigo = p.usu_cod,
                             nombre = p.usu_nom,
                             cargo = c.car_des

                         };
            return View(modelo.ToList());
        }

    }
}
