
/****** Object:  Table [dbo].[Empleados]  ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Empleados](
	[IdEmpleado] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](50) NOT NULL,
	[Apellido] [varchar](50) NOT NULL,
	[FechaNacimiento] [datetime] NOT NULL,
	[EstadoCivil] [tinyint] NOT NULL,
	[Imagen] [image] NULL,
 CONSTRAINT [PK_Empleados] PRIMARY KEY CLUSTERED 
(
	[IdEmpleado] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[Empleados] ON
INSERT [dbo].[Empleados] ([IdEmpleado], [Nombre], [Apellido], [FechaNacimiento], [EstadoCivil], [Imagen]) VALUES (1, N'Andres', N'Gomez', CAST(0x0000562600000000 AS DateTime), 1, NULL)
INSERT [dbo].[Empleados] ([IdEmpleado], [Nombre], [Apellido], [FechaNacimiento], [EstadoCivil], [Imagen]) VALUES (2, N'Carolina', N'Suarez', CAST(0x0000724E0172C788 AS DateTime), 1, NULL)
SET IDENTITY_INSERT [dbo].[Empleados] OFF


/****** Object:  Table [dbo].[Estudios]  ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Estudios](
	[IdEstudio] [int] IDENTITY(1,1) NOT NULL,
	[Descripcion] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Estudios] PRIMARY KEY CLUSTERED 
(
	[IdEstudio] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[Estudios] ON
INSERT [dbo].[Estudios] ([IdEstudio], [Descripcion]) VALUES (1, N'Primario')
INSERT [dbo].[Estudios] ([IdEstudio], [Descripcion]) VALUES (2, N'Secundario')
INSERT [dbo].[Estudios] ([IdEstudio], [Descripcion]) VALUES (3, N'Terciario')
INSERT [dbo].[Estudios] ([IdEstudio], [Descripcion]) VALUES (4, N'Universitario')
SET IDENTITY_INSERT [dbo].[Estudios] OFF


/****** Object:  Table [dbo].[EmpleadosEstudios] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmpleadosEstudios](
	[IdEmpleado] [int] NOT NULL,
	[IdEstudio] [int] NOT NULL,
 CONSTRAINT [PK_EmpleadosEstudios] PRIMARY KEY CLUSTERED 
(
	[IdEmpleado] ASC,
	[IdEstudio] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[EmpleadosEstudios] ([IdEmpleado], [IdEstudio]) VALUES (1, 1)
INSERT [dbo].[EmpleadosEstudios] ([IdEmpleado], [IdEstudio]) VALUES (2, 1)
INSERT [dbo].[EmpleadosEstudios] ([IdEmpleado], [IdEstudio]) VALUES (2, 2)
INSERT [dbo].[EmpleadosEstudios] ([IdEmpleado], [IdEstudio]) VALUES (2, 4)

/****** Object:  ForeignKey [FK_EmpleadosEstudios_Empleados]   ******/
ALTER TABLE [dbo].[EmpleadosEstudios]  WITH CHECK ADD  CONSTRAINT [FK_EmpleadosEstudios_Empleados] FOREIGN KEY([IdEmpleado])
REFERENCES [dbo].[Empleados] ([IdEmpleado])
GO
ALTER TABLE [dbo].[EmpleadosEstudios] CHECK CONSTRAINT [FK_EmpleadosEstudios_Empleados]
GO

/****** Object:  ForeignKey [FK_EmpleadosEstudios_Estudios]  ******/
ALTER TABLE [dbo].[EmpleadosEstudios]  WITH CHECK ADD  CONSTRAINT [FK_EmpleadosEstudios_Estudios] FOREIGN KEY([IdEstudio])
REFERENCES [dbo].[Estudios] ([IdEstudio])
GO
ALTER TABLE [dbo].[EmpleadosEstudios] CHECK CONSTRAINT [FK_EmpleadosEstudios_Estudios]
GO
