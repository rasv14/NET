using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitucion.Models
{
    public class Profesor: Persona
    {
        public string Catedra { get; set; }

        public override string ConstruirResumen()
        {
            return $"{NombreCompleto}, {Catedra}, {Edad}";

        }
    }
}

