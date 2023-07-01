using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webapplication.clases
{
    public class SeccionVer
    {
        public string nombre { get; set; }
        public string id { get; set; }
        public string sis { get; set; }
        public string codigo { get; set; }
        public string carrera { get; set; }
        public string periodo { get; set; }

        //public string estado { get; set; }
        public string usuario_migro { get; set; }
        public string fecha_migracion { get; set; }
        public string tipo_migracion { get; set; }

        public int? total_students { get; set; }

        public string nombre_padre { get; set; }

        public string sis_padre { get; set; }

        // public string cuenta { get; set; }
    }


    

}
