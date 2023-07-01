Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Windows.Forms
Imports DataAccess
Imports DataAccess.Entidades
Imports Helpers

Public Partial Class EditarEmpleado
    Inherits Form

    Private _idEmpleado As Nullable(Of Integer) = Nothing

	Public Sub New()
		InitializeComponent()
	End Sub

	Public Sub New(idEmpleado As Integer)
		Me.New()
		_idEmpleado = idEmpleado
	End Sub

    Private Sub EditarEmpleado_Load(ByVal sender As Object, ByVal e As EventArgs)

        CargarEstadoCivil()
        CargarEstudio()

        'por defecto se carga una imagen de no disponible
        picImagenEmpleado.Image = ImageHelper.ObtenerImagenNoDisponible()

        '
        ' se carga la informacion del empleado que se quiere editar
        '
        If _idEmpleado.HasValue Then
            Dim empleado As EmpleadoEntity = EmpleadosDAL.ObtenerById(_idEmpleado.Value)

            _idEmpleado = empleado.IdEmpleado
            txtNombre.Text = empleado.Nombre
            txtApellido.Text = empleado.Apellido
            dtpFechaNacimiento.Value = empleado.FechaNacimiento

            cbEstadoCivil.SelectedValue = Convert.ToInt32(empleado.EstadoCivil)

            If empleado.Imagen Is Nothing Then
                picImagenEmpleado.Image = ImageHelper.ObtenerImagenNoDisponible()
            Else
                picImagenEmpleado.Image = ImageHelper.ByteArrayToImage(empleado.Imagen)
            End If

            '
            ' Se obtienen los estudios del empleado
            '

            AsignarEstudios(empleado)
        End If


    End Sub

    Private Sub CargarEstadoCivil()

        cbEstadoCivil.DisplayMember = "Descripcion"
        cbEstadoCivil.ValueMember = "IdEstadoCivil"
        cbEstadoCivil.DataSource = EstadoCivilDAL.ObtenerTodos()

    End Sub

	Private Sub CargarEstudio()
		dgvEstudios.AutoGenerateColumns = False
		dgvEstudios.DataSource = EstudiosDAL.ObtenerTodos()
	End Sub

	Private Sub AsignarEstudios(empleado As EmpleadoEntity)

        Dim rows As List(Of DataGridViewRow) = (From row In dgvEstudios.Rows.Cast(Of DataGridViewRow)() _
                                                 Let idEstudio = Convert.ToInt32(row.Cells("IdEstudio").Value) _
                                                 Join dd In empleado.Estudios On idEstudio Equals dd.IdEstudio _
                                                 Select row).ToList()


        'rows.ForEach(Function(o) o.Cells("Seleccion").Value = True)
        rows.ForEach(Function(o) InlineAssignHelper(o.Cells("Seleccion").Value, True))

	End Sub

    Private Sub btnBuscarImagen_Click(ByVal sender As Object, ByVal e As EventArgs)

        Dim fileDialog As New OpenFileDialog()
        fileDialog.Filter = "Archivo JPG|*.jpg"

        If fileDialog.ShowDialog() = DialogResult.OK Then
            picImagenEmpleado.Image = Image.FromFile(fileDialog.FileName)
        End If

    End Sub

	Private Sub btnGuardar_Click(sender As Object, e As EventArgs)
		'
		' Se crea la entidad
		'
        Dim empleado As New EmpleadoEntity() With { _
                                                   .IdEmpleado = _idEmpleado.GetValueOrDefault(), _
                                                   .Nombre = txtNombre.Text, _
                                                   .Apellido = txtApellido.Text, _
                                                   .FechaNacimiento = dtpFechaNacimiento.Value, _
                                                   .EstadoCivil = Convert.ToInt16(cbEstadoCivil.SelectedValue), _
                                                   .Imagen = ImageHelper.ImageToByteArray(picImagenEmpleado.Image) _
                                                  }

		'
		' Se asignan los estudios seleccionados, se unicializa la lista 
		' para cargar la selecciond el usuario
		'
		empleado.Estudios = New List(Of EstudioEntity)()

        Dim rowsSelected As IEnumerable(Of DataGridViewRow) = dgvEstudios.Rows.Cast(Of DataGridViewRow)().Where(Function(o) Convert.ToBoolean(o.Cells("Seleccion").Value))

        For Each row As DataGridViewRow In rowsSelected

            Dim estudio As New EstudioEntity() With { _
                                                     .IdEstudio = Convert.ToInt32(row.Cells("IdEstudio").Value) _
                                                    }

            empleado.Estudios.Add(estudio)

        Next

		'
		' se graba 
		'
		EmpleadosDAL.Save(empleado)

		Me.DialogResult = DialogResult.OK
        Me.Close()

	End Sub

    Private Sub btnCancelar_Click(ByVal sender As Object, ByVal e As EventArgs)

        Me.DialogResult = DialogResult.Cancel
        Me.Close()

    End Sub


    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, ByVal value As T) As T
        target = value
        Return value
    End Function

End Class
