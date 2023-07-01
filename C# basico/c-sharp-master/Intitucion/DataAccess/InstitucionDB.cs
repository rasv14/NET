using Intitucion.Models;
using System.Data.Entity;

namespace Intitucion.DataAccess
{
    public class InstitucionDB: DbContext
    {
        public DbSet<Profesor> Profesores { get; set; }
        public DbSet<Alumno> Alumnos { get; set; }

        public InstitucionDB(): base("platziSharp")
        {

        }
    }
}
