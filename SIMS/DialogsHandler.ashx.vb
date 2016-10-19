Imports System
Imports System.Web
Imports System.Data

Public Class DialogsHandler : Implements IHttpHandler

    Private p As RecordPeriod
    Private r As Record
    Private s As Site

    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Dim period_id As Integer = context.Request.QueryString("period_id")

        p = New RecordPeriod(period_id)
        Dim status As String = p.Status

        r = New Record(p.RMSRecordID)
        s = New Site(r.SiteID)

        Dim dtDialogs As DataTable = p.GetDialogs(period_id, status)

        Dim pOut As String = Nothing

        pOut = "Dialogs for " & s.NumberName & " - " & r.TypeDS & Chr(10) & _
            "File created on " & Now().ToShortDateString & Chr(10) & Chr(10)

        For Each row As DataRow In dtDialogs.Rows
            pOut = pOut & _
                "Dialog Date: " & row("dialog_dt").ToShortDateString & Chr(10) & _
                "Dialog Created by: " & row("dialog_uid").ToString & Chr(10) & _
                "Status of Record at creation: " & row("status_set_to_va").ToString & Chr(10) & _
                "Comments: " & row("comments_va").ToString & Chr(10) & Chr(10)
        Next

        context.Response.ContentType = "text/plain"
        context.Response.AddHeader("content-disposition", "attachment; filename=Dialogs_" & s.Number & ".txt")
        context.Response.Write(pOut)
        context.Response.Flush()
    End Sub

    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class