Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.IO
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Reflection

Public Class ImageHelper

    Public Shared Function ByteArrayToImage(ByVal byteArrayIn As Byte()) As Image

        Dim ms As New MemoryStream(byteArrayIn)
        Return Image.FromStream(ms)

    End Function

    Public Shared Function ImageToByteArray(ByVal imageIn As Image) As Byte()

        Dim ms As New MemoryStream()
        imageIn.Save(ms, ImageFormat.Jpeg)
        Return ms.ToArray()

    End Function

    Public Shared Function ObtenerImagenNoDisponible() As Image

        Dim assembly As Assembly = System.Reflection.Assembly.GetExecutingAssembly()

        Dim file As Stream = assembly.GetManifestResourceStream("Helpers.NoDisponible.jpg")
        Return Image.FromStream(file)

    End Function

End Class

