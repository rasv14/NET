using Intitucion.DataAccess;
using Intitucion.Misc;
using Intitucion.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intitucion
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                foreach (var parametro in args)
                {
                    Console.WriteLine(parametro);
                    Console.WriteLine("===================");

                    switch (parametro)
                    {
                        case "Rutina2":
                            Rutina2();
                            break;
                        case "Rutina3":
                            Rutina3();
                            break;
                        case "Rutina5":
                            Rutina5();
                            break;
                        default:
                            break;
                    }
                }
            }

            Console.ReadLine();
        }

        private static void Runita8()
        {
            var listaProfesores = CrearLista();
            var db = new InstitucionDB();

            db.Profesores.AddRange(listaProfesores);
            db.SaveChanges();

            //var subconjunto = from prof in db.Profesores
            //                  where prof.Nombre.StartsWith("J")
            //                  select prof;

            //foreach (var item in subconjunto)
            //{
            //    item.CodigoInterno = "STARTS_WITH_J";
            //    Console.WriteLine(item.Nombre);
            //}

            //db.SaveChanges();

            Console.WriteLine("Cuantos JM Hay?" + db.Profesores.Where((p) => p.Nombre == "Jose Mauricio").Count());

            var profesorBorrable = (from p in db.Profesores
                                    where p.Nombre == "Jose Mauricio"
                                    select p).FirstOrDefault();
            db.Profesores.Remove(profesorBorrable);

            db.SaveChanges();

            Console.WriteLine("Cuantos JM Quedaron?" + db.Profesores.Where((p) => p.Nombre == "Jose Mauricio").Count());

        }

        private static void Random8()
        {
            // var listaProfesores = CrearLista();
            Random rnd = new Random();

            Profesor[] listaProfesores = new Profesor[5] {
                new Profesor() { Nombre = "Juan Carlos", Id = rnd.Next() },
                new Profesor() { Nombre = "Juan Gabriel", Id = rnd.Next(),Catedra = "Marketing" },
                new Profesor() { Nombre = "Carlos", Id = rnd.Next() },
                new Profesor() { Nombre = "Yohanna", Catedra = "Marketing", Id = rnd.Next() },
                new Profesor() { Nombre = "Sorey", Id = rnd.Next() },
            };

            var consulta = from profe in listaProfesores
                           where profe.Catedra == "Marketing"
                           || profe.Nombre.StartsWith("J")
                           select new
                           {
                               IDProfesor = profe.Id,
                               Nombre = profe.Nombre.ToUpper(),
                               Llave = Guid.NewGuid().ToString()
                           };

            foreach (var item in consulta)
            {
                Console.WriteLine(
                    $"IDProfesor ={item.IDProfesor}- Nombre={item.Nombre} - Llave{item.Llave}"
                    );
            }

            //foreach (var item in consulta)
            //{
            //    Console.WriteLine(item);
            //}
            //foreach (var profe in consulta)
            //{
            //    Console.WriteLine(profe.Nombre);
            //}
        }

        private static List<Profesor> CrearLista()
        {
            Random rnd = new Random();
            var lista = new List<Profesor>();

            lista.Add(new Profesor() { Nombre = "Juan Carlos", Id = rnd.Next() });
            lista.Add(new Profesor()
            {
                Nombre = "Jeronimo",
                Catedra = "Marketing",
                Id = rnd.Next()
            });
            lista.Add(new Profesor() { Nombre = "Yohanna", Id = rnd.Next() });
            lista.Add(new Profesor() { Nombre = "Martha", Catedra = "Marketing", Id = rnd.Next() });
            lista.Add(new Profesor() { Nombre = "Jose Mauricio", Id = rnd.Next() });
            lista.Add(new Profesor() { Nombre = "Angela", Id = rnd.Next() });
            lista.Add(new Profesor() { Nombre = "Walter", Id = rnd.Next() });
            lista.Add(new Profesor() { Nombre = "Marco", Id = rnd.Next() });
            lista.Add(new Profesor() { Nombre = "Satya", Id = rnd.Next() });
            lista.Add(new Profesor() { Nombre = "Terry", Catedra = "Marketing", Id = rnd.Next() });
            lista.Add(new Profesor() { Nombre = "Alexander", Id = rnd.Next() });
            lista.Add(new Profesor() { Nombre = "Sandra", Id = rnd.Next() });

            return lista;
        }

        private static void Rutina7()
        {
            var listaProfes = new List<Profesor>();

            string[] lineas = File.ReadAllLines("./Files/Profesores.txt");

            int localId = 0;
            foreach (var linea in lineas)
            {
                listaProfes.Add(new Profesor() { Nombre = linea, Id = localId++ });

            }

            foreach (var profe in listaProfes)
            {
                Console.WriteLine(profe.Nombre);
            }

            var archivo = File.Open("profesBinarios.bin", FileMode.OpenOrCreate);

            var binFile = new BinaryWriter(archivo);

            foreach (var profe in listaProfes)
            {
                binFile.Write(profe.Nombre);
                binFile.Write(profe.Id);
            }

            binFile.Close();
            archivo.Close();
        }

        private static void Rutina6()
        {
            var profesor = new Profesor() { Id = 12, Nombre = "Mateo", Apellido = "Pereira", CodigoInterno = "PROFE_SMART" };

            var transmitter = new TransmisorDeDatos();
            transmitter.InformacionEnviada += Transmitter_InformacionEnviada;
            transmitter.InformacionEnviada += (obj, evtarg) =>
            {
                Console.WriteLine("WOOOOOOOOAAAAAAAAAA");
            };

            transmitter.FormatearYEnviar(profesor, formatter, "ALEXTROIO");

            transmitter.FormatearYEnviar(profesor, ReverseFormatter, "ALEXTROIO");

            transmitter.FormatearYEnviar(profesor,

                (s) => new string(s.Reverse().ToArray<char>())

                , "ALEXTROIO");

            transmitter.InformacionEnviada -= Transmitter_InformacionEnviada;

            transmitter.FormatearYEnviar(profesor,
              (s) => new string(s.Reverse().ToArray<char>())

              , "ALEXTROIO");
        }

        private static string formatter(string input)
        {
            byte[] stringBytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(stringBytes);
        }

        private static void Transmitter_InformacionEnviada(object sender, EventArgs e)
        {
            Console.WriteLine("TRANSMISION DE INFORMACION");
        }

        private static string ReverseFormatter(string input) => new string(input.Reverse().ToArray<char>());


        private static void Rutina5()
        {
            List<Persona> listaPersonas = new List<Persona>();
            //var listaPersonas = new ArrayList();

            listaPersonas.Add(new Alumno("Pedro", "Fernandez") { NickName = "Pedrito" });
            listaPersonas.Add(new Profesor() { Nombre = "Profesor", Apellido = "X" });
            listaPersonas.Add(new Alumno("Fernando", "Pedroza"));
            listaPersonas.Add(new Profesor() { Nombre = "Mag", Apellido = "Neto" });
            listaPersonas.Add(new Alumno("Juan", "Es"));
            //listaPersonas.Add(new Salon());

            foreach (var obj in listaPersonas)
            {
                if (obj is Alumno)
                {
                    var al = (Alumno)obj;
                    Console.WriteLine(al.NickName != null ? al.NickName : al.NombreCompleto);
                }
                else
                {
                    var per = obj as Persona;
                    if (per != null)
                        Console.WriteLine(per.NombreCompleto);
                }
            }
        }

        private void Rutina4()
        {
            Persona[] arregloPersonas = new Persona[5];

            var tam = arregloPersonas.Length;

            arregloPersonas[0] = new Alumno("Pedro", "Fernandez") { NickName = "Pedrito" };
            arregloPersonas[1] = new Profesor() { Nombre = "Profesor", Apellido = "X" };
            arregloPersonas[2] = new Alumno("Fernando", "Pedroza");
            arregloPersonas[3] = new Profesor() { Nombre = "Mag", Apellido = "Neto" };
            arregloPersonas[4] = new Alumno("Juan", "Es");

            //arregloPersonas[5] = new Profesor() { Nombre = "Alberto", Apellido = "Piedra" };

            for (int i = 0; i < arregloPersonas.Length; i++)
            {
                if (arregloPersonas[i] is Alumno)
                {
                    var al = (Alumno)arregloPersonas[i];
                    Console.WriteLine(al.NickName != null ? al.NickName : al.NombreCompleto);
                }
                else
                {
                    Console.WriteLine(arregloPersonas[i].NombreCompleto);
                }

            }
        }

        private static void Rutina3()
        {
            var alumno = new Alumno("Victor", "Perez");
            //var profesor = new Profesor();
            //Persona persona = profesor;

            //alumno = (Alumno)persona;

            //if (persona is Profesor)
            //{
            //    var profe = (Profesor)persona;
            //    ///...
            //}

            //var tmpProfe = persona as Profesor;

            //if (tmpProfe != null)
            //{
            //    //...
            //}
        }

        private static void Rutina2()
        {
            // -32.000 +32.00 
            short s = 32000;
            int i = 33000;
            float f = 2.35f;
            double d = (double)0.00023m;

            Console.WriteLine(i);
            s = (short)i;
            Console.WriteLine(s);
            Console.WriteLine(f);
            i = (int)f;
            Console.WriteLine(i);
        }

        [Flags]
        enum Banderas
        {
            NOMBRE_COMPLETO = 1,
            CURSO_COMPLETO = 2,
            TODO_COMPLETO = 3,
            HABILITA_RESUMEN = 4
        }

        public void Rutina1()
        {
            Console.WriteLine("GESTION DE INTITUCION");

            Persona[] lista = new Persona[3];
            lista[0] = new Alumno("Juan Carlos", "Ruiz")
            {
                Id = 1,
                Edad = 36,
                Teléfono = "3111111",
                Email = "juank@platzi.com"
            };

            lista[1] = new Profesor()
            {
                Id = 2,
                Nombre = "Freddy",
                Apellido = "Vega",
                Edad = 86,
                Teléfono = "564564",
                Catedra = "Programación"
            };

            lista[2] = new Profesor()
            {
                Id = 3,
                Nombre = "William",
                Apellido = "Torvalds",
                Edad = 25,
                Teléfono = "911",
                Catedra = "Algebra"
            };

            Console.WriteLine(Persona.ContadorPersonas);
            Console.WriteLine("Resumenes");

            foreach (Persona p in lista)
            {
                Console.WriteLine($"Tipo {p.GetType()}");
                Console.WriteLine(p.ConstruirResumen());

                IEnteInstituto ente = p;

                ente.ConstruirLlaveSecreta("Hola");
            }

            Console.WriteLine("S T R U C T S");
            CursoStruct c = new CursoStruct(70);
            c.Curso = "101-B";

            var newC = new CursoStruct();
            newC.Curso = "564-A";

            var cursoFreak = c;
            cursoFreak.Curso = "666-G";

            Console.WriteLine($"Curso c = {c.Curso}");
            Console.WriteLine($"Curso Freak = {cursoFreak.Curso}");

            //Console.ReadLine();

            Console.WriteLine("C L A S E S");

            CursoClass c_class = new CursoClass(70);
            c_class.Curso = "101-B";

            var newCc_class = new CursoStruct();
            newCc_class.Curso = "564-A";

            var cursoFreakc_class = c_class;
            cursoFreak.Curso = "666-G";

            Console.WriteLine($"Curso c = {c_class.Curso}");
            Console.WriteLine($"Curso Freak = {cursoFreakc_class.Curso}");

            Console.WriteLine("E N U M E R A C I O N E S");

            var alumnoEst = new Alumno("Freddy", "Vega")
            {
                Id = 22,
                Edad = 86,
                Teléfono = "564564",
                Estado = EstadosAlumno.Activo
            };

            Persona personaX = alumnoEst;
            Console.WriteLine("Estado de alumno" + alumnoEst.Estado);

            IEnteInstituto iei = alumnoEst;

            Console.WriteLine($"Tipo: { typeof(EstadosAlumno)} ");
            Console.WriteLine($"Tipo: { typeof(Alumno)} ");
            Console.WriteLine($"Tipo: { iei.GetType()} ");

            Console.WriteLine($"Tipo: { alumnoEst.GetType() } ");
            Console.WriteLine($"Tipo: { personaX.GetType() } ");
            Console.WriteLine($"nombre: { nameof(Alumno)} ");
            Console.WriteLine($"Tamaño: { sizeof(int)} ");

            Banderas mis_banderas = Banderas.NOMBRE_COMPLETO
                                   | Banderas.CURSO_COMPLETO;

            if (mis_banderas == (Banderas.TODO_COMPLETO)
               )
            {
                //...
            }

            if (mis_banderas == Banderas.NOMBRE_COMPLETO)
            {
                //...
            }

            if (mis_banderas == Banderas.TODO_COMPLETO)
            {
                //...
            }
        }
    }
}