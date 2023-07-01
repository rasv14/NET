Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Windows.Forms
Imports DataAccess
Imports Helpers
Imports DataAccess.Entidades

Public Partial Class ListaEmpleados
	Inherits Form
	Public Sub New()
		InitializeComponent()
	End Sub

	Private Sub frmListaEmpleados_Load(sender As Object, e As EventArgs)
		CargarListaEmpleados()
	End Sub

	Private Sub CargarListaEmpleados()
		dgvEmpleados.AutoGenerateColumns = False
		dgvEmpleados.DataSource = EmpleadosDAL.ObtenerTodos()

		For Each row As DataGridViewRow In dgvEmpleados.Rows
			'se asigna el alto de la fila para poder ver la imagen correctamente
			row.Height = 120

			'se recupera la entidad que es asignada como dato
			Dim empleado As EmpleadoEntity = TryCast(row.DataBoundItem, EmpleadoEntity)

			If empleado.Imagen Is Nothing Then
				row.Cells("Imagen").Value = ImageHelper.ObtenerImagenNoDisponible()
			Else
				row.Cells("Imagen").Value = ImageHelper.ByteArrayToImage(empleado.Imagen)
			End If
		Next

	End Sub

	Private Sub btnNuevoEmpleado_Click(sender As Object, e As EventArgs)
		'
		' sino se le pasa un id ,el formulario entrara en modo alta
		'
		Dim frmEditar As New EditarEmpleado()
		AddHandler frmEditar.FormClosing, New FormClosingEventHandler(AddressOf frmEditar_FormClosing)

		frmEditar.Show()

	End Sub


	Private Sub frmEditar_FormClosing(sender As Object, e As FormClosingEventArgs)
		'
		' al cerrarse el form de edicion se ingresa a este evento 
		' para actualizar la informacion del listado
		'

		Dim frmEdit As EditarEmpleado = TryCast(sender, EditarEmpleado)

		If frmEdit.DialogResult = DialogResult.OK Then
			CargarListaEmpleados()
		End If

	End Sub


    Private Sub dgvEmpleados_CellContentDoubleClick(ByVal sender As Object, ByVal e As DataGridViewCellEventArgs)

        Dim IdEmpleado As Integer = Convert.ToInt32(dgvEmpleados.Rows(e.RowIndex).Cells("IdEmpleado").Value)

        '
        ' al pasarle un id de empleado este lo cargara para su edicion
        '
        Dim frmEditar As New EditarEmpleado(IdEmpleado)
        AddHandler frmEditar.FormClosing, New FormClosingEventHandler(AddressOf frmEditar_FormClosing)

        frmEditar.Show()

    End Sub


End Class
