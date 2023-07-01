Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace Entidades
    Public Class EmpleadoEntity

        Public Sub New()
            Me.Estudios = New List(Of EstudioEntity)()
        End Sub

        Public Property IdEmpleado() As Integer
            Get
                Return m_IdEmpleado
            End Get
            Set(ByVal value As Integer)
                m_IdEmpleado = Value
            End Set
        End Property
        Private m_IdEmpleado As Integer
        Public Property Nombre() As String
            Get
                Return m_Nombre
            End Get
            Set(ByVal value As String)
                m_Nombre = Value
            End Set
        End Property

        Private m_Nombre As String
        Public Property Apellido() As String
            Get
                Return m_Apellido
            End Get
            Set(ByVal value As String)
                m_Apellido = Value
            End Set
        End Property
        Private m_Apellido As String
        Public Property FechaNacimiento() As DateTime
            Get
                Return m_FechaNacimiento
            End Get
            Set(ByVal value As DateTime)
                m_FechaNacimiento = Value
            End Set
        End Property
        Private m_FechaNacimiento As DateTime
        Public Property EstadoCivil() As Short
            Get
                Return m_EstadoCivil
            End Get
            Set(ByVal value As Short)
                m_EstadoCivil = Value
            End Set
        End Property
        Private m_EstadoCivil As Short
        Public Property Imagen() As Byte()
            Get
                Return m_Imagen
            End Get
            Set(ByVal value As Byte())
                m_Imagen = Value
            End Set
        End Property
        Private m_Imagen As Byte()

        Public ReadOnly Property NombreCompleto() As String
            Get
                Return [String].Format("{0}, {1}", Me.Apellido, Me.Nombre)
            End Get
        End Property

        Public Property Estudios() As List(Of EstudioEntity)
            Get
                Return m_Estudios
            End Get
            Set(ByVal value As List(Of EstudioEntity))
                m_Estudios = Value
            End Set
        End Property
        Private m_Estudios As List(Of EstudioEntity)
    End Class
End Namespace
