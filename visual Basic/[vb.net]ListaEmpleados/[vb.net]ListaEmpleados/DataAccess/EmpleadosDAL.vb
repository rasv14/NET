Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Data.SqlClient
Imports System.Configuration
Imports DataAccess.Entidades
Imports System.Data
Imports System.Transactions

Public NotInheritable Class EmpleadosDAL

    Public Shared Function ObtenerTodos() As List(Of EmpleadoEntity)

        Dim lista As New List(Of EmpleadoEntity)()

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("default").ToString())
            conn.Open()

            Dim query As String = "SELECT IdEmpleado, Nombre, Apellido, FechaNacimiento, EstadoCivil, Imagen " & _
                                  "FROM Empleados"

            Dim cmd As New SqlCommand(query, conn)

            Dim reader As SqlDataReader = cmd.ExecuteReader()

            While reader.Read()
                lista.Add(ConvertirEmpleado(reader, False))
            End While

        End Using

        Return lista

    End Function

    Public Shared Function ObtenerById(ByVal idEmpleado As Integer) As EmpleadoEntity

        Dim empleado As EmpleadoEntity = Nothing

        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("default").ToString())
            conn.Open()

            Dim query As String = "SELECT IdEmpleado, Nombre, Apellido, FechaNacimiento, EstadoCivil, Imagen " & _
                                  "FROM Empleados WHERE IdEmpleado = @idEmpleado"

            Dim cmd As New SqlCommand(query, conn)
            cmd.Parameters.AddWithValue("@idEmpleado", idEmpleado)

            Dim reader As SqlDataReader = cmd.ExecuteReader()

            If reader.Read() Then
                empleado = ConvertirEmpleado(reader, True)
            End If

        End Using

        Return empleado

    End Function

    Private Shared Function ConvertirEmpleado(ByVal reader As IDataReader, ByVal cargarRelaciones As Boolean) As EmpleadoEntity

        Dim empleado As New EmpleadoEntity()

        empleado.IdEmpleado = Convert.ToInt32(reader("IdEmpleado"))
        empleado.Nombre = Convert.ToString(reader("Nombre"))
        empleado.Apellido = Convert.ToString(reader("Apellido"))
        empleado.FechaNacimiento = Convert.ToDateTime(reader("FechaNacimiento"))
        empleado.EstadoCivil = Convert.ToInt16(reader("EstadoCivil"))

        If reader("Imagen") IsNot DBNull.Value Then
            empleado.Imagen = DirectCast(reader("Imagen"), Byte())
        End If

        If cargarRelaciones Then
            empleado.Estudios = EstudiosDAL.ObtenerAsignadoEmpleado(empleado.IdEmpleado)
        End If

        Return empleado

    End Function

	Private Shared Function AgregarNuevo(empleado As EmpleadoEntity) As EmpleadoEntity
		Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("default").ToString())
			conn.Open()

            Dim query As String = "INSERT INTO Empleados (Nombre, Apellido, FechaNacimiento, EstadoCivil, Imagen) " & _
                                    "VALUES (@Nombre, @Apellido, @FechaNacimiento, @EstadoCivil, @Imagen); " & _
                                    "SELECT SCOPE_IDENTITY()"

			Dim cmd As New SqlCommand(query, conn)

			cmd.Parameters.AddWithValue("@Nombre", empleado.Nombre)
			cmd.Parameters.AddWithValue("@Apellido", empleado.Apellido)
			cmd.Parameters.AddWithValue("@FechaNacimiento", empleado.FechaNacimiento)
			cmd.Parameters.AddWithValue("@EstadoCivil", empleado.EstadoCivil)

			Dim imageParam As SqlParameter = cmd.Parameters.Add("@Imagen", System.Data.SqlDbType.Image)
			imageParam.Value = empleado.Imagen

			'
			' se recupera el id generado por la tabla
			'
			empleado.IdEmpleado = Convert.ToInt32(cmd.ExecuteScalar())
		End Using

		Return empleado
	End Function

	Private Shared Function Actualizar(empleado As EmpleadoEntity) As EmpleadoEntity
		Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("default").ToString())
			conn.Open()

            Dim query As String = "UPDATE Empleados SET " & _
                                        "Nombre = @Nombre, " & _
                                        "Apellido = @Apellido, " & _
                                        "FechaNacimiento = @FechaNacimiento, " & _
                                        "EstadoCivil = @EstadoCivil, " & _
                                        "Imagen = @Imagen " & _
                                    "WHERE IdEmpleado = @idEmpleado"

			Dim cmd As New SqlCommand(query, conn)

			cmd.Parameters.AddWithValue("@Nombre", empleado.Nombre)
			cmd.Parameters.AddWithValue("@Apellido", empleado.Apellido)
			cmd.Parameters.AddWithValue("@FechaNacimiento", empleado.FechaNacimiento)
			cmd.Parameters.AddWithValue("@EstadoCivil", empleado.EstadoCivil)

			Dim imageParam As SqlParameter = cmd.Parameters.Add("@Imagen", System.Data.SqlDbType.Image)
			imageParam.Value = empleado.Imagen

			cmd.Parameters.AddWithValue("@idEmpleado", empleado.IdEmpleado)

			cmd.ExecuteNonQuery()
		End Using

		Return empleado
	End Function

	Public Shared Function Save(empleado As EmpleadoEntity) As EmpleadoEntity
		Using scope As New TransactionScope()
			'
			' Graba datos empleado
			'
			If Existe(empleado.IdEmpleado) Then
				Actualizar(empleado)
			Else
				AgregarNuevo(empleado)
			End If

			'
			' graba los estudios relacionado con el empleado, 
			' se eliminan las opciones existentes
			' y se ingresan la nueva seleccion del usuario
			'
			EstudiosDAL.EliminarPorEmpleado(empleado)

			For Each estudio As EstudioEntity In empleado.Estudios
				EstudiosDAL.RelacionarConEmpleado(empleado, estudio)
			Next


			scope.Complete()
		End Using

		Return empleado

	End Function

	Public Shared Function Existe(idEmpleado As Integer) As Boolean
		Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("default").ToString())
			conn.Open()

            Dim query As String = "SELECT COUNT(*) FROM Empleados " & _
                                    "WHERE IdEmpleado = @idEmpleado"

			Dim cmd As New SqlCommand(query, conn)
			cmd.Parameters.AddWithValue("@idEmpleado", idEmpleado)

			Dim resultado As Integer = Convert.ToInt32(cmd.ExecuteScalar())

			If resultado = 0 Then
				Return False
			Else
				Return True
			End If
		End Using

	End Function


End Class
