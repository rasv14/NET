using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using CapaEntidad;
using CapaDatos;
namespace CapaNegocio
{
    public class ClientesNegocio
    {
        ClientesDatos _ClienteDatos = new ClientesDatos();

        public bool InsertarCliente(ClientesEntidad CliNegocio)
        {
            return _ClienteDatos.InsertarCliente(CliNegocio);
        }

        public bool ActualizarCliente(ClientesEntidad CliNegocio)
        {
            return _ClienteDatos.ActualizarCliente(CliNegocio);
        }

        public bool EliminarCliente(ClientesEntidad CliNegocio)
        {
            return _ClienteDatos.EliminarCliente(CliNegocio);
        }

        public DataTable ListarClientes(string parametro)
        {
            return _ClienteDatos.ListarClientes(parametro);
        }
        public ClientesEntidad ConsultarCliente(string codigo)
        {
            return _ClienteDatos.ConsultarCliente(codigo);
        }
    }
}
