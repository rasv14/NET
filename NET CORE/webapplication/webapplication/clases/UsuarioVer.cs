using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webapplication.clases
{
    public class UsuarioVer
    {
        public  string id { get; set; }

        

        public string?integration_id { get; set; }

        public string name { get; set; }

        public string fecha_migracion { get; set; }
        public string? sortable_name { get; set; }
        public string? short_name { get; set; }

        public string sis_user_id { get; set; }

        public string? login_id { get; set; }

        public string? email { get; set; }

        public string usuario_migro { get; set; }

        public string tipo_migracion { get; set; }

        public string? imagen_url { get; set; }


        public string? id_cuenta_canvas { get; set; }

       

        public List<UsuarioCursoVer>? lst_cursos { get; set; }
    }


    

}
