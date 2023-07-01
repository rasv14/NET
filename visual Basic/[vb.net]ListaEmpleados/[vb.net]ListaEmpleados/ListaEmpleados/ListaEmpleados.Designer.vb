Partial Class ListaEmpleados
	''' <summary>
	''' Required designer variable.
	''' </summary>
	Private components As System.ComponentModel.IContainer = Nothing

	''' <summary>
	''' Clean up any resources being used.
	''' </summary>
	''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
	Protected Overloads Overrides Sub Dispose(disposing As Boolean)
		If disposing AndAlso (components IsNot Nothing) Then
			components.Dispose()
		End If
		MyBase.Dispose(disposing)
	End Sub

	#Region "Windows Form Designer generated code"

	''' <summary>
	''' Required method for Designer support - do not modify
	''' the contents of this method with the code editor.
	''' </summary>
	Private Sub InitializeComponent()
		Me.dgvEmpleados = New System.Windows.Forms.DataGridView()
		Me.IdEmpleado = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.NombreCompleto = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.Imagen = New System.Windows.Forms.DataGridViewImageColumn()
		Me.label1 = New System.Windows.Forms.Label()
		Me.btnNuevoEmpleado = New System.Windows.Forms.Button()
		DirectCast(Me.dgvEmpleados, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		' 
		' dgvEmpleados
		' 
		Me.dgvEmpleados.AllowUserToAddRows = False
		Me.dgvEmpleados.AllowUserToDeleteRows = False
		Me.dgvEmpleados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
		Me.dgvEmpleados.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.IdEmpleado, Me.NombreCompleto, Me.Imagen})
		Me.dgvEmpleados.Location = New System.Drawing.Point(12, 46)
		Me.dgvEmpleados.Name = "dgvEmpleados"
		Me.dgvEmpleados.[ReadOnly] = True
		Me.dgvEmpleados.Size = New System.Drawing.Size(506, 391)
		Me.dgvEmpleados.TabIndex = 0
		AddHandler Me.dgvEmpleados.CellContentDoubleClick, New System.Windows.Forms.DataGridViewCellEventHandler(AddressOf Me.dgvEmpleados_CellContentDoubleClick)
		' 
		' IdEmpleado
		' 
		Me.IdEmpleado.DataPropertyName = "IdEmpleado"
		Me.IdEmpleado.HeaderText = "IdEmpleado"
		Me.IdEmpleado.Name = "IdEmpleado"
		Me.IdEmpleado.Visible = False
		' 
		' NombreCompleto
		' 
		Me.NombreCompleto.DataPropertyName = "NombreCompleto"
		Me.NombreCompleto.HeaderText = "Nombre Completo"
		Me.NombreCompleto.Name = "NombreCompleto"
		Me.NombreCompleto.Width = 200
		' 
		' Imagen
		' 
		Me.Imagen.HeaderText = "imagen"
		Me.Imagen.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Stretch
		Me.Imagen.Name = "Imagen"
		' 
		' label1
		' 
		Me.label1.BackColor = System.Drawing.Color.FromArgb(CInt(CByte(64)), CInt(CByte(64)), CInt(CByte(64)))
		Me.label1.Dock = System.Windows.Forms.DockStyle.Top
		Me.label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CByte(0))
		Me.label1.ForeColor = System.Drawing.Color.White
		Me.label1.Location = New System.Drawing.Point(0, 0)
		Me.label1.Name = "label1"
		Me.label1.Size = New System.Drawing.Size(597, 28)
		Me.label1.TabIndex = 1
		Me.label1.Text = "Lista Empleados"
		Me.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		' 
		' btnNuevoEmpleado
		' 
		Me.btnNuevoEmpleado.Location = New System.Drawing.Point(532, 46)
		Me.btnNuevoEmpleado.Name = "btnNuevoEmpleado"
		Me.btnNuevoEmpleado.Size = New System.Drawing.Size(53, 31)
		Me.btnNuevoEmpleado.TabIndex = 2
		Me.btnNuevoEmpleado.Text = "Nuevo"
		Me.btnNuevoEmpleado.UseVisualStyleBackColor = True
		AddHandler Me.btnNuevoEmpleado.Click, New System.EventHandler(AddressOf Me.btnNuevoEmpleado_Click)
		' 
		' ListaEmpleados
		' 
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6F, 13F)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(597, 453)
		Me.Controls.Add(Me.btnNuevoEmpleado)
		Me.Controls.Add(Me.label1)
		Me.Controls.Add(Me.dgvEmpleados)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
		Me.MaximizeBox = False
		Me.MinimizeBox = False
		Me.Name = "ListaEmpleados"
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
		Me.Text = "Lista Empleados"
		AddHandler Me.Load, New System.EventHandler(AddressOf Me.frmListaEmpleados_Load)
		DirectCast(Me.dgvEmpleados, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)

	End Sub

	#End Region

	Private dgvEmpleados As System.Windows.Forms.DataGridView
	Private label1 As System.Windows.Forms.Label
	Private btnNuevoEmpleado As System.Windows.Forms.Button
	Private IdEmpleado As System.Windows.Forms.DataGridViewTextBoxColumn
	Private NombreCompleto As System.Windows.Forms.DataGridViewTextBoxColumn
	Private Imagen As System.Windows.Forms.DataGridViewImageColumn
End Class

