using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitucion.Models
{
    public class CursoClass
    {
        const string NOMBRE_DEF_CURSO = "NO ASIGNADO";
        private string curso;

        public string Curso
        {
            get { return curso; }
            set { curso = value; }
        }

        public short max_capacidad ;

        public CursoClass(short max_cap)
        {
            max_capacidad = max_cap;
            curso = NOMBRE_DEF_CURSO;
        }
    }
}
