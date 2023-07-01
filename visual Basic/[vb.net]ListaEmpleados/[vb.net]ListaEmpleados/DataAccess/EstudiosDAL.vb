Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports DataAccess.Entidades
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Data

Public NotInheritable Class EstudiosDAL


    Public Shared Function ObtenerTodos() As List(Of EstudioEntity)
        Dim lista As New List(Of EstudioEntity)()

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("default").ToString())
            conn.Open()

            Dim query As String = "SELECT E.IdEstudio, E.Descripcion " & _
                                    "FROM Estudios E"

            Dim cmd As New SqlCommand(query, conn)

            Dim reader As SqlDataReader = cmd.ExecuteReader()

            While reader.Read()
                lista.Add(ConvertirEstudio(reader))

            End While
        End Using

        Return lista
    End Function

    Public Shared Function ObtenerAsignadoEmpleado(ByVal idEmpleado As Integer) As List(Of EstudioEntity)
        Dim lista As New List(Of EstudioEntity)()

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("default").ToString())
            conn.Open()

            Dim query As String = "SELECT E.IdEstudio, E.Descripcion " & _
                                    "FROM Estudios E INNER JOIN EmpleadosEstudios EE " & _
                                    "ON E.IdEstudio = EE.IdEstudio " & _
                                    "WHERE EE.IdEmpleado = @idempleado"

            Dim cmd As New SqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@idempleado", idEmpleado)

            Dim reader As SqlDataReader = cmd.ExecuteReader()

            While reader.Read()
                lista.Add(ConvertirEstudio(reader))

            End While
        End Using

        Return lista
    End Function

    Private Shared Function ConvertirEstudio(ByVal reader As IDataReader) As EstudioEntity
        Dim estudio As New EstudioEntity()

        estudio.IdEstudio = Convert.ToInt32(reader("IdEstudio"))
        estudio.Descripcion = Convert.ToString(reader("Descripcion"))

        Return estudio
    End Function

    Public Shared Sub EliminarPorEmpleado(ByVal empleado As EmpleadoEntity)
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("default").ToString())
            conn.Open()

            Dim query As String = "DELETE FROM EmpleadosEstudios " & _
                                    "WHERE IdEmpleado = @idEmpleado"

            Dim cmd As New SqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@idEmpleado", empleado.IdEmpleado)


            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Public Shared Sub RelacionarConEmpleado(ByVal empleado As EmpleadoEntity, ByVal estudio As EstudioEntity)
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("default").ToString())
            conn.Open()

            Dim query As String = "INSERT INTO EmpleadosEstudios (IdEmpleado, IdEstudio) " & _
                                    "VALUES (@IdEmpleado, @IdEstudio)"

            Dim cmd As New SqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@IdEmpleado", empleado.IdEmpleado)
            cmd.Parameters.AddWithValue("@IdEstudio", estudio.IdEstudio)


            cmd.ExecuteNonQuery()
        End Using
    End Sub


End Class
