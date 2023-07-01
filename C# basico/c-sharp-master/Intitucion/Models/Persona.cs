using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitucion.Models
{
    public abstract class Persona: IEnteInstituto
    {
        public static int ContadorPersonas = 0;

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public short Edad { get; set; }
        public string Teléfono { get; set; }
        protected int Inasistencias { get; set; }

        public virtual string NombreCompleto
        {
            get
            {
                return $"{Nombre} {Apellido}";
            }
        }

        public string CodigoInterno
        {
            get;
            set;
        }

        public Persona()
        {
            ContadorPersonas++;
        }

        public abstract string ConstruirResumen();

        public string ConstruirLlaveSecreta(string nombreEnte)
        {
            nombreEnte = "nuevo Valor";
            var rnd = new Random();
            
            return rnd.Next(1, 9998945).ToString();
            
        }
    }
}
