Partial Class EditarEmpleado
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
		Me.label1 = New System.Windows.Forms.Label()
		Me.btnGuardar = New System.Windows.Forms.Button()
		Me.btnCancelar = New System.Windows.Forms.Button()
		Me.groupBox1 = New System.Windows.Forms.GroupBox()
		Me.dgvEstudios = New System.Windows.Forms.DataGridView()
		Me.btnBuscarImagen = New System.Windows.Forms.Button()
		Me.picImagenEmpleado = New System.Windows.Forms.PictureBox()
		Me.label6 = New System.Windows.Forms.Label()
		Me.label5 = New System.Windows.Forms.Label()
		Me.cbEstadoCivil = New System.Windows.Forms.ComboBox()
		Me.dtpFechaNacimiento = New System.Windows.Forms.DateTimePicker()
		Me.label4 = New System.Windows.Forms.Label()
		Me.label3 = New System.Windows.Forms.Label()
		Me.txtApellido = New System.Windows.Forms.TextBox()
		Me.label2 = New System.Windows.Forms.Label()
		Me.txtNombre = New System.Windows.Forms.TextBox()
		Me.IdEstudio = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.Seleccion = New System.Windows.Forms.DataGridViewCheckBoxColumn()
		Me.Estudios = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.groupBox1.SuspendLayout()
		DirectCast(Me.dgvEstudios, System.ComponentModel.ISupportInitialize).BeginInit()
		DirectCast(Me.picImagenEmpleado, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		' 
		' label1
		' 
		Me.label1.BackColor = System.Drawing.Color.FromArgb(CInt(CByte(64)), CInt(CByte(64)), CInt(CByte(64)))
		Me.label1.Dock = System.Windows.Forms.DockStyle.Top
		Me.label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CByte(0))
		Me.label1.ForeColor = System.Drawing.Color.White
		Me.label1.Location = New System.Drawing.Point(0, 0)
		Me.label1.Name = "label1"
		Me.label1.Size = New System.Drawing.Size(637, 28)
		Me.label1.TabIndex = 2
		Me.label1.Text = "Editar Empleado"
		Me.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		' 
		' btnGuardar
		' 
		Me.btnGuardar.Location = New System.Drawing.Point(437, 321)
		Me.btnGuardar.Name = "btnGuardar"
		Me.btnGuardar.Size = New System.Drawing.Size(81, 23)
		Me.btnGuardar.TabIndex = 3
		Me.btnGuardar.Text = "Guardar"
		Me.btnGuardar.UseVisualStyleBackColor = True
		AddHandler Me.btnGuardar.Click, New System.EventHandler(AddressOf Me.btnGuardar_Click)
		' 
		' btnCancelar
		' 
		Me.btnCancelar.Location = New System.Drawing.Point(535, 321)
		Me.btnCancelar.Name = "btnCancelar"
		Me.btnCancelar.Size = New System.Drawing.Size(81, 23)
		Me.btnCancelar.TabIndex = 4
		Me.btnCancelar.Text = "Cancelar"
		Me.btnCancelar.UseVisualStyleBackColor = True
		AddHandler Me.btnCancelar.Click, New System.EventHandler(AddressOf Me.btnCancelar_Click)
		' 
		' groupBox1
		' 
		Me.groupBox1.Controls.Add(Me.dgvEstudios)
		Me.groupBox1.Controls.Add(Me.btnBuscarImagen)
		Me.groupBox1.Controls.Add(Me.picImagenEmpleado)
		Me.groupBox1.Controls.Add(Me.label6)
		Me.groupBox1.Controls.Add(Me.label5)
		Me.groupBox1.Controls.Add(Me.cbEstadoCivil)
		Me.groupBox1.Controls.Add(Me.dtpFechaNacimiento)
		Me.groupBox1.Controls.Add(Me.label4)
		Me.groupBox1.Controls.Add(Me.label3)
		Me.groupBox1.Controls.Add(Me.txtApellido)
		Me.groupBox1.Controls.Add(Me.label2)
		Me.groupBox1.Controls.Add(Me.txtNombre)
		Me.groupBox1.Location = New System.Drawing.Point(12, 42)
		Me.groupBox1.Name = "groupBox1"
		Me.groupBox1.Size = New System.Drawing.Size(606, 273)
		Me.groupBox1.TabIndex = 11
		Me.groupBox1.TabStop = False
		' 
		' dgvEstudios
		' 
		Me.dgvEstudios.AllowUserToAddRows = False
		Me.dgvEstudios.AllowUserToDeleteRows = False
		Me.dgvEstudios.AllowUserToOrderColumns = True
		Me.dgvEstudios.AllowUserToResizeColumns = False
		Me.dgvEstudios.AllowUserToResizeRows = False
		Me.dgvEstudios.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.IdEstudio, Me.Seleccion, Me.Estudios})
		Me.dgvEstudios.Location = New System.Drawing.Point(133, 135)
		Me.dgvEstudios.Name = "dgvEstudios"
		Me.dgvEstudios.RowHeadersVisible = False
		Me.dgvEstudios.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
		Me.dgvEstudios.Size = New System.Drawing.Size(185, 123)
		Me.dgvEstudios.TabIndex = 26
		' 
		' btnBuscarImagen
		' 
		Me.btnBuscarImagen.Location = New System.Drawing.Point(425, 211)
		Me.btnBuscarImagen.Name = "btnBuscarImagen"
		Me.btnBuscarImagen.Size = New System.Drawing.Size(105, 23)
		Me.btnBuscarImagen.TabIndex = 25
		Me.btnBuscarImagen.Text = "Buscar Imagen"
		Me.btnBuscarImagen.UseVisualStyleBackColor = True
		AddHandler Me.btnBuscarImagen.Click, New System.EventHandler(AddressOf Me.btnBuscarImagen_Click)
		' 
		' picImagenEmpleado
		' 
		Me.picImagenEmpleado.Location = New System.Drawing.Point(425, 30)
		Me.picImagenEmpleado.Name = "picImagenEmpleado"
		Me.picImagenEmpleado.Size = New System.Drawing.Size(147, 166)
		Me.picImagenEmpleado.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
		Me.picImagenEmpleado.TabIndex = 24
		Me.picImagenEmpleado.TabStop = False
		' 
		' label6
		' 
		Me.label6.AutoSize = True
		Me.label6.Location = New System.Drawing.Point(80, 135)
		Me.label6.Name = "label6"
		Me.label6.Size = New System.Drawing.Size(47, 13)
		Me.label6.TabIndex = 23
		Me.label6.Text = "Estudios"
		' 
		' label5
		' 
		Me.label5.AutoSize = True
		Me.label5.Location = New System.Drawing.Point(65, 116)
		Me.label5.Name = "label5"
		Me.label5.Size = New System.Drawing.Size(62, 13)
		Me.label5.TabIndex = 18
		Me.label5.Text = "Estado Civil"
		' 
		' cbEstadoCivil
		' 
		Me.cbEstadoCivil.FormattingEnabled = True
		Me.cbEstadoCivil.Location = New System.Drawing.Point(133, 108)
		Me.cbEstadoCivil.Name = "cbEstadoCivil"
		Me.cbEstadoCivil.Size = New System.Drawing.Size(145, 21)
		Me.cbEstadoCivil.TabIndex = 17
		' 
		' dtpFechaNacimiento
		' 
		Me.dtpFechaNacimiento.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
		Me.dtpFechaNacimiento.Location = New System.Drawing.Point(133, 82)
		Me.dtpFechaNacimiento.Name = "dtpFechaNacimiento"
		Me.dtpFechaNacimiento.Size = New System.Drawing.Size(106, 20)
		Me.dtpFechaNacimiento.TabIndex = 16
		' 
		' label4
		' 
		Me.label4.AutoSize = True
		Me.label4.Location = New System.Drawing.Point(37, 86)
		Me.label4.Name = "label4"
		Me.label4.Size = New System.Drawing.Size(90, 13)
		Me.label4.TabIndex = 15
		Me.label4.Text = "FechaNacimiento"
		' 
		' label3
		' 
		Me.label3.AutoSize = True
		Me.label3.Location = New System.Drawing.Point(83, 59)
		Me.label3.Name = "label3"
		Me.label3.Size = New System.Drawing.Size(44, 13)
		Me.label3.TabIndex = 14
		Me.label3.Text = "Apellido"
		' 
		' txtApellido
		' 
		Me.txtApellido.Location = New System.Drawing.Point(133, 56)
		Me.txtApellido.Name = "txtApellido"
		Me.txtApellido.Size = New System.Drawing.Size(225, 20)
		Me.txtApellido.TabIndex = 13
		' 
		' label2
		' 
		Me.label2.AutoSize = True
		Me.label2.Location = New System.Drawing.Point(83, 33)
		Me.label2.Name = "label2"
		Me.label2.Size = New System.Drawing.Size(44, 13)
		Me.label2.TabIndex = 12
		Me.label2.Text = "Nombre"
		' 
		' txtNombre
		' 
		Me.txtNombre.Location = New System.Drawing.Point(133, 30)
		Me.txtNombre.Name = "txtNombre"
		Me.txtNombre.Size = New System.Drawing.Size(225, 20)
		Me.txtNombre.TabIndex = 11
		' 
		' IdEstudio
		' 
		Me.IdEstudio.DataPropertyName = "IdEstudio"
		Me.IdEstudio.HeaderText = "IdEstudio"
		Me.IdEstudio.Name = "IdEstudio"
		Me.IdEstudio.[ReadOnly] = True
		Me.IdEstudio.Visible = False
		' 
		' Seleccion
		' 
		Me.Seleccion.HeaderText = "Seleccion"
		Me.Seleccion.Name = "Seleccion"
		Me.Seleccion.Width = 60
		' 
		' Estudios
		' 
		Me.Estudios.DataPropertyName = "Descripcion"
		Me.Estudios.HeaderText = "Estudios"
		Me.Estudios.Name = "Estudios"
		Me.Estudios.[ReadOnly] = True
		Me.Estudios.Width = 120
		' 
		' EditarEmpleado
		' 
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6F, 13F)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(637, 357)
		Me.Controls.Add(Me.groupBox1)
		Me.Controls.Add(Me.btnCancelar)
		Me.Controls.Add(Me.btnGuardar)
		Me.Controls.Add(Me.label1)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
		Me.MaximizeBox = False
		Me.MinimizeBox = False
		Me.Name = "EditarEmpleado"
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
		Me.Text = "EditarEmpleado"
		AddHandler Me.Load, New System.EventHandler(AddressOf Me.EditarEmpleado_Load)
		Me.groupBox1.ResumeLayout(False)
		Me.groupBox1.PerformLayout()
		DirectCast(Me.dgvEstudios, System.ComponentModel.ISupportInitialize).EndInit()
		DirectCast(Me.picImagenEmpleado, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)

	End Sub

	#End Region

	Private label1 As System.Windows.Forms.Label
	Private btnGuardar As System.Windows.Forms.Button
	Private btnCancelar As System.Windows.Forms.Button
	Private groupBox1 As System.Windows.Forms.GroupBox
	Private dtpFechaNacimiento As System.Windows.Forms.DateTimePicker
	Private label4 As System.Windows.Forms.Label
	Private label3 As System.Windows.Forms.Label
	Private txtApellido As System.Windows.Forms.TextBox
	Private label2 As System.Windows.Forms.Label
	Private txtNombre As System.Windows.Forms.TextBox
	Private label6 As System.Windows.Forms.Label
	Private label5 As System.Windows.Forms.Label
	Private cbEstadoCivil As System.Windows.Forms.ComboBox
	Private btnBuscarImagen As System.Windows.Forms.Button
	Private picImagenEmpleado As System.Windows.Forms.PictureBox
	Private dgvEstudios As System.Windows.Forms.DataGridView
	Private IdEstudio As System.Windows.Forms.DataGridViewTextBoxColumn
	Private Seleccion As System.Windows.Forms.DataGridViewCheckBoxColumn
	Private Estudios As System.Windows.Forms.DataGridViewTextBoxColumn
End Class
