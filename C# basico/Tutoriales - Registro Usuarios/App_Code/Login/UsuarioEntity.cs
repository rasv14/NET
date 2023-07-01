using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class UsuarioEntity
{
    public UsuarioEntity()
    {
        UsuarioID = 0;
        Username = "";
        Contraseña = "";
        Fecharegistro = DateTime.Now;
        Estado = 0;
        Email = "";
        UltimoAcceso = DateTime.Now;
        IPAcceso = "";
    }
    public int UsuarioID { get; set; }
    public string Username { get; set; }
    public string Contraseña { get; set; } 
    public DateTime Fecharegistro { get; set; }
    public int Estado { get; set; }
    public string Email { get; set; }

    /* Dentro de la base de datos existe una tabla llamada UsuarioSecurity se decidió no crear una clase para esa tabla
     * y ponerlas en esta misma en ella se encontrarán datos para saber de que IP y de que maquina y asi como en que fecha se
     * ingresó a cada usuario
    */
    public int UsuarioSecurityID { get; set; }
    public DateTime UltimoAcceso { get; set; }
    public string IPAcceso { get; set; }
}

