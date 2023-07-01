using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitucion.Models
{
    class Salon: IEnteInstituto
    {
        public string CodigoInterno
        {
            get;

            set;
        }

        public int Id { get; set; }
        public string Nombre { get; set; }

        public string ConstruirLlaveSecreta(string nombreEnte)
        {
            return "SAL ON";
        }
    }
}
