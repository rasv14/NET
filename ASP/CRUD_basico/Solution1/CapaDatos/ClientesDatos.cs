using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using CapaEntidad;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class ClientesDatos
    {
        SqlConnection cnx;
        ClientesEntidad mcEntidad = new ClientesEntidad();
        Conexion MiConexi = new Conexion();
        SqlCommand cmd = new SqlCommand();
        bool vexito;
        public ClientesDatos()
        {
            cnx = new SqlConnection(MiConexi.GetConex());
        }
        public bool InsertarCliente(ClientesEntidad mcEntidad)
        {
            cmd.Connection = cnx;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "proc_insertar";
            try
            {
                cmd.Parameters.Add(new SqlParameter("@codigo", SqlDbType.VarChar, 5));
                cmd.Parameters["@codigo"].Value = mcEntidad.codigoCliente;
                cmd.Parameters.Add(new SqlParameter("@nombres", SqlDbType.VarChar, 50));
                cmd.Parameters["@nombres"].Value = mcEntidad.nombresCliente;
                cmd.Parameters.Add(new SqlParameter("@apellidos", SqlDbType.VarChar, 100));
                cmd.Parameters["@apellidos"].Value = mcEntidad.apellidosCliente;
                cmd.Parameters.Add(new SqlParameter("@correo", SqlDbType.VarChar, 100));
                cmd.Parameters["@correo"].Value = mcEntidad.correoCliente;
                cmd.Parameters.Add(new SqlParameter("@estado", SqlDbType.Int));
                cmd.Parameters["@estado"].Value = mcEntidad.estadoCliente;
                cnx.Open();
                cmd.ExecuteNonQuery();
                vexito = true;
            }
            catch (SqlException)
            {
                vexito = false;
            }
            finally
            {
                if (cnx.State == ConnectionState.Open)
                {
                    cnx.Close();
                }
                cmd.Parameters.Clear();
            }
            return vexito;
        }
        public bool ActualizarCliente(ClientesEntidad mcEntidad)
        {
            cmd.Connection = cnx;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "proc_actualizar";
            try
            {
                cmd.Parameters.Add(new SqlParameter("@codigo", SqlDbType.VarChar, 5));
                cmd.Parameters["@codigo"].Value = mcEntidad.codigoCliente;
                cmd.Parameters.Add(new SqlParameter("@nombres", SqlDbType.VarChar, 50));
                cmd.Parameters["@nombres"].Value = mcEntidad.nombresCliente;
                cmd.Parameters.Add(new SqlParameter("@apellidos", SqlDbType.VarChar, 100));
                cmd.Parameters["@apellidos"].Value = mcEntidad.apellidosCliente;
                cmd.Parameters.Add(new SqlParameter("@correo", SqlDbType.VarChar, 100));
                cmd.Parameters["@correo"].Value = mcEntidad.correoCliente;
                cnx.Open();
                cmd.ExecuteNonQuery();
                vexito = true;
            }
            catch (SqlException)
            {
                vexito = false;
            }
            finally
            {
                if (cnx.State == ConnectionState.Open)
                {
                    cnx.Close();
                }
                cmd.Parameters.Clear();
            }
            return vexito;
        }
        public bool EliminarCliente(ClientesEntidad mcEntidad)
        {
            cmd.Connection = cnx;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "proc_eliminar";
            try
            {
                cmd.Parameters.Add(new SqlParameter("@codigo", SqlDbType.VarChar, 5));
                cmd.Parameters["@codigo"].Value = mcEntidad.codigoCliente;
                cmd.Parameters.Add(new SqlParameter("@estado", SqlDbType.Int));
                cmd.Parameters["@estado"].Value = mcEntidad.estadoCliente;
                cnx.Open();
                cmd.ExecuteNonQuery();
                vexito = true;
            }
            catch (SqlException)
            {
                vexito = false;
            }
            finally
            {
                if (cnx.State == ConnectionState.Open)
                {
                    cnx.Close();
                }
                cmd.Parameters.Clear();
            }
            return vexito;
        }
        public DataTable ListarClientes(string parametro)
        {
            DataSet dts = new DataSet();
            try
            {
                cmd.Connection = cnx;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "proc_listar";
                cmd.Parameters.Add(new SqlParameter("@apellidos", parametro));
                SqlDataAdapter miada;
                miada = new SqlDataAdapter(cmd);
                miada.Fill(dts, "clientes");
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                cmd.Parameters.Clear();
            }
            return (dts.Tables["clientes"]);
        }
        public ClientesEntidad ConsultarCliente(string codigo)
        {
            try
            {
                SqlDataReader dtr;
                cmd.Connection = cnx;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "proc_consultar";
                cmd.Parameters.Add(new SqlParameter("@codigo", SqlDbType.VarChar, 10));
                cmd.Parameters["@codigo"].Value = codigo;
                if (cnx.State == ConnectionState.Closed)
                {
                    cnx.Open();
                }
                dtr = cmd.ExecuteReader();
                if (dtr.HasRows == true)
                {
                    dtr.Read();
                    mcEntidad.codigoCliente = Convert.ToString(dtr[0]);
                    mcEntidad.nombresCliente = Convert.ToString(dtr[1]);
                    mcEntidad.apellidosCliente = Convert.ToString(dtr[2]);
                    mcEntidad.correoCliente = Convert.ToString(dtr[3]);
                }
                cnx.Close();
                cmd.Parameters.Clear();
                return mcEntidad;
            }
            catch (SqlException)
            {
                throw new Exception();
            }
            finally
            {
                if (cnx.State == ConnectionState.Open)
                {
                    cnx.Close();
                }
                cmd.Parameters.Clear();
            }
        }
    }
}
