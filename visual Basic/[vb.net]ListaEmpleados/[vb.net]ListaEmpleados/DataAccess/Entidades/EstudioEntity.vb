Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace Entidades
	Public Class EstudioEntity
		Public Property IdEstudio() As Integer
			Get
				Return m_IdEstudio
			End Get
			Set
				m_IdEstudio = Value
			End Set
		End Property
		Private m_IdEstudio As Integer
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
