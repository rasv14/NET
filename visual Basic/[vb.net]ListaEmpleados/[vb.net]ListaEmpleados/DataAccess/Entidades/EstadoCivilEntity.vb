Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace Entidades
	Public Class EstadoCivilEntity
		Public Sub New(id As Integer, desc As String)
			Me.IdEstadoCivil = id
			Me.Descripcion = desc
		End Sub

		Public Property IdEstadoCivil() As Integer
			Get
				Return m_IdEstadoCivil
			End Get
			Set
				m_IdEstadoCivil = Value
			End Set
		End Property
		Private m_IdEstadoCivil As Integer
		Public Property Descripcion() As String
			Get
				Return m_Descripcion
			End Get
			Set
				m_Descripcion = Value
			End Set
		End Property
		Private m_Descripcion As String
	End Class
End Namespace
