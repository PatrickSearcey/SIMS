Imports Microsoft.VisualBasic

Public Class Helpers
    Private s As String

    Public Sub New(ByVal compareval As String)
        s = compareval
    End Sub

    Public Function CompareTo(ByVal val As String) As Boolean
        If val = s Then
            Return True
        Else
            Return False
        End If
    End Function
End Class
