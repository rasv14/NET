using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class ClientesEntidad
    {
        private string codigo, nombres, apellidos, correo;
        private int estado;
        public string codigoCliente
        {
            get { return codigo; }
            set { codigo = value; }
        }
        public string nombresCliente
        {
            get { return nombres; }
            set { nombres = value; }
        }
        public string apellidosCliente
        {
            get { return apellidos; }
            set { apellidos = value; }
        }
        public string correoCliente
        {
            get { return correo; }
            set { correo = value; }
        }
        public int estadoCliente
        {
            get { return estado; }
            set { estado = value; }
        }
    }
}
